using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RTUndo (Public Singleton Class)
    // Desc: Implements the Undo/Redo system.
    //-----------------------------------------------------------------------------
    public class RTUndo : MonoSingleton<RTUndo>
    {
        #region Public Delegates & Events
        //-----------------------------------------------------------------------------
        // Name: UndoRedoHandler() (Public Delegate)
        // Desc: Handler for the Undo/Redo event fired on Undo/Redo.
        // Parm: redo - True on Redo. False on Undo.
        //-----------------------------------------------------------------------------
        public delegate void    UndoRedoHandler(bool redo);
        public event            UndoRedoHandler undoRedo;
        #endregion

        #region Private Fields
        [SerializeField] int    mStackSize  = 50;                           // Number of undo stack entries
        UndoGroup[]             mStack;                                     // The Undo stack
        int                     mStackTop   = -1;                           // The top of the stack which points at the last group stored on the stack (i.e. mStackTop + 1 = number of groups on the stack).
        int                     mSP         = -1;                           // Stack pointer used to implement the Undo/Redo mechanism. Points to the next group which can be undone.

        UndoGroup               mActiveGroup;                               // The active group that receives undo operations
        ArrayStack<bool>        mEnabledStack = new ArrayStack<bool>();     // Allows nesting of enabled state changes

        bool mInsideUndoHandler;    // Are we inside the Undo handler?
        bool mInsideRedoHandler;    // Are we inside the Redo handler?
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: undoRedoEnabled (Public Property)
        // Desc: Returns whether or not the Undo/Redo functionality is enabled.
        //-----------------------------------------------------------------------------
        public bool undoRedoEnabled     { get { return mEnabledStack.count == 0 || mEnabledStack.Peek(); } }

        //-----------------------------------------------------------------------------
        // Name: stackSize (Public Property)
        // Desc: Returns or sets the undo stack size. Changing the size has the effect
        //       of clearing the undo stack.
        //-----------------------------------------------------------------------------
        public int stackSize            { get { return mStackSize; } set { ClearUndo(); mStackSize = Mathf.Max(1, value); mStack = new UndoGroup[mStackSize]; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Internal_Update() (Public Function)
        // Desc: Called by the system to allow the undo/redo system to update itself.
        //-----------------------------------------------------------------------------
        public void Internal_Update()
        {
            // If any mouse buttons were pressed, create an input bound group if we don't have one already
            if (mActiveGroup == null && RTInput.get.anyMBWentDown)
                mActiveGroup = NewGroup();
        }

        //-----------------------------------------------------------------------------
        // Name: Internal_LateUpdate() (Public Function)
        // Desc: Called by the system to allow the undo/redo system to update itself
        //       during the late update phase.
        //-----------------------------------------------------------------------------
        public void Internal_LateUpdate()
        {
            // Do we have an active group? It is possible to have no group if no mouse buttons
            // were pressed and no operations have been performed. When no mouse buttons are pressed,
            // an undo group is created only when performing an operation.
            if (mActiveGroup != null)
            {
                // If there are any mouse buttons pressed, don't commit because it means that this group is input bound
                if (RTInput.get.anyMBPressed) return;

                // The group is not input bound, commit. If we can't commit, pop the group off the stack.
                if (!mActiveGroup.Commit())
                    --mSP;

                // We no longer have an active group
                mActiveGroup = null;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: ClearUndo() (Public Function)
        // Desc: Clears the undo stack.
        //-----------------------------------------------------------------------------
        public void ClearUndo()
        {
            // Flush all undo groups
            for (int i = mStackTop; i >= 0; --i)
                mStack[i].Flush();

            // Clear stack
            mStackTop   = -1;
            mSP         = -1;
        }

        //-----------------------------------------------------------------------------
        // Name: Undo() (Public Function)
        // Desc: Undo the last group of operations.
        //-----------------------------------------------------------------------------
        public void Undo()
        {
            // No-op?
            if (mInsideUndoHandler ||
                mInsideRedoHandler || 
                mSP < 0) return;

            // Skip empty groups
            while (mSP >= 0 && mStack[mSP].operationCount == 0)
                --mSP;

            // Nothing left?
            if (mSP < 0)
                return;

            // Undo and move the pointer down
            mStack[mSP].Undo();
            --mSP;

            // Fire undo event
            mInsideUndoHandler = true;
            if (undoRedo != null) undoRedo(false);
            mInsideUndoHandler = false;
        }

        //-----------------------------------------------------------------------------
        // Name: Redo() (Public Function)
        // Desc: Redo the last group which was undone.
        //-----------------------------------------------------------------------------
        public void Redo()
        {
            // No-op?
            if (mInsideUndoHandler ||
                mInsideRedoHandler || 
                mSP >= mStackTop) return;

            // Move the pointer up and redo
            ++mSP;
            mStack[mSP].Redo();

            // Fire redo event
            mInsideRedoHandler = true;
            if (undoRedo != null) undoRedo(true);
            mInsideRedoHandler = false;
        }

        //-----------------------------------------------------------------------------
        // Name: Record() (Public Function)
        // Desc: Records the state of the specified Unity Object so that any subsequent
        //       changes may be undone.  The function has no effect if Undo/Redo is disabled.
        // Parm: unityObject - The Unity Object whose state must be recorded.
        //-----------------------------------------------------------------------------
        public void Record(UnityEngine.Object unityObject)
        {
            // No-op?
            if (!undoRedoEnabled)
                return;

            // Do we need to create a new group?
            if (mActiveGroup == null)
                mActiveGroup = NewGroup();

            // Check object type
            if (unityObject is Component)
            {
                // Transform?
                var transform = unityObject as Transform;
                if (transform != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new TransformUndoState(transform)));

                // Light?
                var light = unityObject as Light;
                if (light != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new LightUndoState(light)));

                // Box collider?
                var boxCollider = unityObject as BoxCollider;
                if (boxCollider != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new BoxColliderUndoState(boxCollider)));

                // Sphere collider?
                var sphereCollider = unityObject as SphereCollider;
                if (sphereCollider != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new SphereColliderUndoState(sphereCollider)));

                // Capsule collider?
                var capsuleCollider = unityObject as CapsuleCollider;
                if (capsuleCollider != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new CapsuleColliderUndoState(capsuleCollider)));

                // Character controller?
                var characterController = unityObject as CharacterController;
                if (characterController != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new CharacterControllerUndoState(characterController)));

                // Audio reverb zone?
                var audioReverbZone = unityObject as AudioReverbZone;
                if (audioReverbZone != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new AudioReverbZoneUndoState(audioReverbZone)));

                // Audio source?
                var audioSource = unityObject as AudioSource;
                if (audioSource != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new AudioSourceUndoState(audioSource)));

                // Box collider 2D?
                var boxCollider2D = unityObject as BoxCollider2D;
                if (boxCollider2D != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new BoxCollider2DUndoState(boxCollider2D)));

                // Circle collider 2D?
                var circleCollider2D = unityObject as CircleCollider2D;
                if (circleCollider2D != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new CircleCollider2DUndoState(circleCollider2D)));

                // Capsule collider 2D?
                var capsuleCollider2D = unityObject as CapsuleCollider2D;
                if (capsuleCollider2D != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new CapsuleCollider2DUndoState(capsuleCollider2D)));

                // Camera?
                var camera = unityObject as Camera;
                if (camera != null)
                    mActiveGroup.AddOperation(new RecordUndoOp(new CameraUndoState(camera)));
            }
        }

        //-----------------------------------------------------------------------------
        // Name: RecordTerrainHeights() (Public Function)
        // Desc: Records the heights of the specified terrain so that any subsequent
        //       changes may be undone.  The function has no effect if Undo/Redo is disabled.
        // Parm: terrain     - Terrain whose heights must be recorded.
        //       vertexRange - The range of vertices whose heights should be recorded.
        //-----------------------------------------------------------------------------
        public void RecordTerrainHeights(Terrain terrain, TerrainVertexRange vertexRange)
        {
            // No-op?
            if (!undoRedoEnabled)
                return;

            // Add record op
            mActiveGroup.AddOperation(new RecordUndoOp(new TerrainHeightsUndoState(terrain, vertexRange)));
        }

        //-----------------------------------------------------------------------------
        // Name: RecordObjectsSpawn() (Public Function)
        // Desc: Records a spawn operation for one or more objects, allowing it to be
        //       undone or redone via the Undo system.
        // Parm: spawnedObjects - The spawned objects. If null or empty, the function
        //       exits immediately.
        //-----------------------------------------------------------------------------
        public void RecordObjectsSpawn(IList<GameObject> spawnedObjects)
        {
            // No-op?
            if (!undoRedoEnabled || spawnedObjects == null || spawnedObjects.Count == 0)
                return;

            // Add spawn op
            mActiveGroup.AddOperation(new SpawnObjectsUndoOp(spawnedObjects));
        }

        //-----------------------------------------------------------------------------
        // Name: PushEnabled() (Public Function)
        // Desc: Changes the enabled state of the Undo/Redo functionality to the specified
        //       value. Every call to 'PushEnabled' should be matched by a call to 'PopEnabled'.
        // Parm: enabled - The new enabled state of the Undo/Redo functionality.
        //-----------------------------------------------------------------------------
        public void PushEnabled(bool enabled)
        {
            mEnabledStack.Push(enabled);
        }

        //-----------------------------------------------------------------------------
        // Name: PopEnabled() (Public Function)
        // Desc: Restores the enabled state to what it was before the last call to 
        //       'PushEnabled'. Should be matched with a previous call to 'PushEnabled'.
        //-----------------------------------------------------------------------------
        public void PopEnabled()
        {
            // No-op?
            if (mEnabledStack.count == 0) return;

            // Pop
            mEnabledStack.Pop();
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: Awake() (Private Function)
        // Desc: Called by Unity to allow the object to initialize itself.
        //-----------------------------------------------------------------------------
        void Awake()
        {
            mStack = new UndoGroup[mStackSize];
        }

        //-----------------------------------------------------------------------------
        // Name: NewGroup() (Private Function)
        // Desc: Creates a new undo group and pushes it onto the stack. Any groups which
        //       are invalidated are flushed.
        // Rtrn: The new undo group.
        //-----------------------------------------------------------------------------
        UndoGroup NewGroup()
        {
            // Is the stack pointer right at the top?
            if (mSP == mStackTop)
            {
                // Do we need to make room for this new group?
                if (mStackTop + 1 == mStackSize)
                {
                    // Flush the bottom-most group and move all other groups down
                    mStack[0].Flush();
                    for (int i = 0; i < mStackSize - 1; ++i)
                        mStack[i] = mStack[i + 1];

                    // Store a new group at the top and return it
                    mStack[mStackTop] = new UndoGroup();
                    return mStack[mStackTop];
                }
                else
                {
                    // We still have room, increment top and stack pointer and create a new group
                    mStack[++mStackTop] = new UndoGroup();
                    mSP = mStackTop;

                    // Return group
                    return mStack[mStackTop];
                }
            }
            else
            {
                // If we reach this point, it means that some groups have been undone
                // and the stack pointer lies below the stack top. In this case, we
                // need to flush all groups sitting above the stack pointer.
                ++mSP;
                for (int i = mStackTop; i >= mSP; --i)
                {
                    mStack[i].Flush();
                    mStack[i] = null;
                }

                // Sync stack top
                mStackTop = mSP;

                // Create a new group where the stack pointer is and return it
                mStack[mSP] = new UndoGroup();
                return mStack[mSP];
            }
        }
        #endregion
    }
    #endregion
}
