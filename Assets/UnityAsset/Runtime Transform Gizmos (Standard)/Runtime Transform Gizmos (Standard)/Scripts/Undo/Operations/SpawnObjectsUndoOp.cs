using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SpawnObjectsUndoOp (Public Class)
    // Desc: Represents an undo operation for object spawning.
    //-----------------------------------------------------------------------------
    public class SpawnObjectsUndoOp : IUndoOperation
    {
        #region Private Fields
        Dictionary<GameObject, bool>    mActiveStateMap     = new Dictionary<GameObject, bool>();   // Stores the original active states of the spawned objects. The Undo/Redo
                                                                                                    // mechanism is implemented by activating and deactivating objects. On Redo,
                                                                                                    // we need to know the original states.
        List<GameObject>                mSpawnedObjects     = new List<GameObject>();               // The spawned objects
        bool                            mDestroyOnFlush;                                            // Controls whether the spawned objects are destroyed during Flush(). Set to true after Undo, false after Redo.
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: SpawnObjectsUndoOp() (Public Constructor)
        // Desc: Creates a spawn objects undo operation.
        // Parm: spawnedObjects - The objects which were spawned.
        //-----------------------------------------------------------------------------
        public SpawnObjectsUndoOp(IList<GameObject> spawnedObjects)
        {
            // Do we have a valid object collection?
            if (spawnedObjects != null)
            {
                // Copy objects and their original active states
                mSpawnedObjects.AddRange(spawnedObjects);
                int count = mSpawnedObjects.Count;
                for (int i = 0; i < count; ++i)
                {
                    GameObject go = mSpawnedObjects[i];
                    if (go && !mActiveStateMap.ContainsKey(go))
                        mActiveStateMap.Add(go, go.activeSelf);
                }
            }
        }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Undo() (Public Function)
        // Desc: Revert the operation's effects.
        //-----------------------------------------------------------------------------
        public void Undo()
        {
            // Deactivate objects
            int count = mSpawnedObjects.Count;
            for (int i = 0; i < count; ++i)
            {
                if (mSpawnedObjects[i])
                    mSpawnedObjects[i].SetActive(false);
            }

            // Destroy objects on flush
            mDestroyOnFlush = true;
        }

        //-----------------------------------------------------------------------------
        // Name: Redo() (Public Function)
        // Desc: Restore the operation's effects.
        //-----------------------------------------------------------------------------
        public void Redo()
        {
            // Activate objects
            int count = mSpawnedObjects.Count;
            for (int i = 0; i < count; ++i)
            {
                // Note: Restore to the original state they had when the operation was created.
                GameObject go = mSpawnedObjects[i];
                if (go) go.SetActive(mActiveStateMap[go]);
            }

            // These objects are alive again, so don't destroy on flush
            mDestroyOnFlush = false;
        }

        //-----------------------------------------------------------------------------
        // Name: Flush() (Public Function)
        // Desc: Called when the operation is about to be removed from the undo stack
        //       because it is no longer needed.
        //-----------------------------------------------------------------------------
        public void Flush()
        {
            // Destroy on flush?
            if (!mDestroyOnFlush)
                return;

            // Destroy objects
            int count = mSpawnedObjects.Count;
            for (int i = 0; i < count; ++i)
            {
                if (mSpawnedObjects[i])
                {
                    // Notify scene
                    RTScene.get.UnregisterObjectHierarchy(mSpawnedObjects[i]);

                    // Destroy
                    #if UNITY_EDITOR
                    GameObject.DestroyImmediate(mSpawnedObjects[i]);
                    #else
                    GameObject.Destroy(mSpawnedObjects[i]);
                    #endif
                }
            }
            mSpawnedObjects.Clear();
        }
        #endregion
    }
    #endregion
}
