using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: Tutorial_5_Multiple_Viewports (Public Class)
    // Desc: Tutorial class which demonstrates support for multiple viewports. Multiple
    //       viewports are supported by default, you don't really need to do anything.
    //       This tutorial is almost the same as 'Tutorial_4_Selection_Manager' with only
    //       a few minor tweaks to allow for viewport selection and highlighting. Search
    //       for the '<!>' sequence of characters to navigate the changes that were made
    //       for this tutorial.
    //-----------------------------------------------------------------------------
    public class Tutorial_5_Multiple_Viewports : MonoBehaviour
    {
        #region Private Enumerations
        //-----------------------------------------------------------------------------
        // Name: EGizmoType (Private Enum)
        // Desc: Defines different gizmo types that can be used to manipulate the selection.
        //       Each member can be used as an index with the 'mGizmos' list.
        //-----------------------------------------------------------------------------
        enum EGizmoType
        {
            Move = 0,
            Rotate,
            Scale,
            TRS,
            BoxScale,
            Extrude
        }
        #endregion

        #region Private Fields
        EGizmoType          mSelectedGizmo  = EGizmoType.Move;          // Keeps track of the current gizmo. This can be changed via hotkeys.
        List<Gizmo>         mGizmos         = new List<Gizmo>();        // Stores all gizmos that can manipulate the selection. Can be indexed with 'EGizmoType'.

        // Keeps track of the currently selected objects. We will also call 'SetTargets' for each gizmo
        // and pass in this list. This way the gizmos will always work with the current selection.
        List<GameObject>    mSelectedObjects = new List<GameObject>();

        // <!> List of all cameras used in this demo
        List<Camera>        mCameras        = new List<Camera>();
        #endregion

        #region Private Properties
        //-----------------------------------------------------------------------------
        // Name: selectedGizmo() (Private Property)
        // Desc: Returns the currently selected gizmo. Shorthand for 'mGizmos[(int)mSelectedGizmo]'.
        //-----------------------------------------------------------------------------
        Gizmo selectedGizmo { get { return mGizmos[(int)mSelectedGizmo]; } }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: Awake() (Private Function)
        // Desc: Called by Unity to allow the object to initialize itself.
        //-----------------------------------------------------------------------------
        void Awake()
        {
            // When the app starts, the plugin needs to initialize itself. So we will
            // have to register a handler for the 'initialized' event. When this event
            // is fired, we know for sure the plugin is initialized and ready to use.
            RTG.get.initialized += OnRTGInit;
        }

        //-----------------------------------------------------------------------------
        // Name: Update() (Private Function)
        // Desc: Called by Unity to allow the object to update itself.
        //-----------------------------------------------------------------------------
        void Update()
        {
            // Change the active gizmo using hotkeys.
            // Note: Since some of these keys can conflict with the camera navigation keys, we will
            //       proceed if the camera is not currently navigating.
            if (RTCamera.get.navigationMode == ECameraNavigationMode.None)
            {
                if (RTInput.get.KeyWentDown(Key.W))     // Enable move gizmo
                    SetSelectedGizmo(EGizmoType.Move);
                else
                if (RTInput.get.KeyWentDown(Key.E))     // Enable rotate gizmo
                    SetSelectedGizmo(EGizmoType.Rotate);
                else
                if (RTInput.get.KeyWentDown(Key.R))     // Enable scale gizmo
                    SetSelectedGizmo(EGizmoType.Scale);
                else
                if (RTInput.get.KeyWentDown(Key.T))     // Enable TRS gizmo
                    SetSelectedGizmo(EGizmoType.TRS);
                else
                if (RTInput.get.KeyWentDown(Key.U))     // Enable box scale gizmo
                    SetSelectedGizmo(EGizmoType.BoxScale);
                else
                if (RTInput.get.KeyWentDown(Key.I))     // Enable extrude gizmo
                    SetSelectedGizmo(EGizmoType.Extrude);
                else
                // Toggle between global and local space
                if (RTInput.get.KeyWentDown(Key.G))
                {
                    // All gizmos use the same space, so we can just get the selected gizmo's transform space
                    EGizmoTransformSpace space = selectedGizmo.objectTransformGizmo.transformSpace;

                    // Toggle space
                    if (space == EGizmoTransformSpace.Global) space = EGizmoTransformSpace.Local;
                    else space = EGizmoTransformSpace.Global;

                    // Set transform space for all gizmos
                    int count = mGizmos.Count;
                    for (int i = 0; i < count; ++i)
                        mGizmos[i].objectTransformGizmo.transformSpace = space;
                }
                else
                // Toggle between relative and absolute move snap
                if (RTInput.get.KeyWentDown(Key.H))
                {
                    EGizmoMoveSnapMode moveSnapMode = RTGizmos.get.moveSnapMode;
                    if (moveSnapMode == EGizmoMoveSnapMode.Relative) RTGizmos.get.moveSnapMode = EGizmoMoveSnapMode.Absolute;
                    else RTGizmos.get.moveSnapMode = EGizmoMoveSnapMode.Relative;
                }
                // Focus the camera on the currently selected objects
                else
                if (RTInput.get.KeyWentDown(Key.F))
                    RTCamera.get.Focus(mSelectedObjects);
                // <!> Camera selection
                else
                if (RTInput.get.KeyWentDown(Key.Digit1))
                    RTCamera.get.settings.targetCamera = mCameras[0];
                else if (RTInput.get.KeyWentDown(Key.Digit2))
                    RTCamera.get.settings.targetCamera = mCameras[1];
                else if (RTInput.get.KeyWentDown(Key.Digit3))
                    RTCamera.get.settings.targetCamera = mCameras[2];
                else if (RTInput.get.KeyWentDown(Key.Digit4))
                    RTCamera.get.settings.targetCamera = mCameras[3];
            }

            // Pick an object with the mouse
            PickObject();

            // Every frame we will check if the selected gizmo should be enabled or not.
            // The gizmo is enabled only when there is at least one selected object available.
            selectedGizmo.enabled = mSelectedObjects.Count != 0;

            // If the active gizmo is a box scale gizmo, set its target. For the
            // box scale gizmo, we need to do this every time the selection changes
            // because it doesn't store a direct reference to the selection collection.
            if (selectedGizmo.enabled && selectedGizmo is BoxScaleGizmo)
                (selectedGizmo as BoxScaleGizmo).target = mSelectedObjects[0];
        }

        //-----------------------------------------------------------------------------
        // Name: OnRTGInit() (Private Function)
        // Desc: Event handler for the 'RTG.initialized' event.
        //-----------------------------------------------------------------------------
        void OnRTGInit()
        {
            // Let's create transform gizmos which allow us to move, rotate and scale objects.
            // Note: The order in which we store these in the list is important. There has to
            //       be a 1-to-1 mapping with the 'EGizmoType' enum.
            mGizmos.Add(RTGizmos.get.CreateObjectMoveGizmo());
            mGizmos.Add(RTGizmos.get.CreateObjectRotateGizmo());
            mGizmos.Add(RTGizmos.get.CreateObjectScaleGizmo());
            mGizmos.Add(RTGizmos.get.CreateObjectTRSGizmo());

            // Create box scale gizmo
            var boxScaleGizmo = RTGizmos.get.CreateGizmo<BoxScaleGizmo>();
            boxScaleGizmo.enabled = false;
            mGizmos.Add(boxScaleGizmo);

            // Create extrude gizmo
            var extrudeGizmo = RTGizmos.get.CreateGizmo<ExtrudeGizmo>();
            extrudeGizmo.SetTargets(mSelectedObjects);
            extrudeGizmo.extrudeSpace = EGizmoExtrudeSpace.Local;
            extrudeGizmo.enabled = false;
            mGizmos.Add(extrudeGizmo);

            // When the extrude gizmo spawns objects, it will check if they overlap other objects in the
            // scene if the 'avoidOverlaps' property is true. We can choose to ignore certain overlaps
            // if this is desirable. For example, when extruding floors, you might want to ignore overlaps
            // with game objects whose name start with "wall_". In this simply demo we just answer 'Yes'.
            extrudeGizmo.ignoreOverlapQuery += (go, answer) => { answer.Yes(); };

            // Call 'SetTargets' to bind the selected objects collection to the gizmos and disable
            // all gizmos by default.
            int count = mGizmos.Count;
            for (int i = 0; i < count; ++i)
            {
                // Skip gizmos which don't have an object transform gizmo behaviour
                if (mGizmos[i].objectTransformGizmo == null)
                    continue;

                // Disable gizmo and set targets
                mGizmos[i].enabled = false;
                mGizmos[i].objectTransformGizmo.SetTargets(mSelectedObjects);
                
                // Set the pivot and transform space
                mGizmos[i].objectTransformGizmo.pivot           = EGizmoPivot.Center;
                mGizmos[i].objectTransformGizmo.transformSpace  = EGizmoTransformSpace.Local;
            }

            // <!> Store cameras
            mCameras.Add(GameObject.Find("Camera_TopLeft").GetCamera());
            mCameras.Add(GameObject.Find("Camera_TopRight").GetCamera());
            mCameras.Add(GameObject.Find("Camera_BottomLeft").GetCamera());
            mCameras.Add(GameObject.Find("Camera_BottomRight").GetCamera());

            // <!> Disable grid rendering for the top right camera and gizmo rendering for the bottom right camera
            RTCamera.get.SetCameraRenderConfig(mCameras[1], new RTCameraRenderConfig { renderFlags = ECameraRenderFlags.All & ~ECameraRenderFlags.SceneGrid });
            RTCamera.get.SetCameraRenderConfig(mCameras[3], new RTCameraRenderConfig { renderFlags = ECameraRenderFlags.All & ~ECameraRenderFlags.Gizmos });

            // <!> Create view gizmos for each camera
            count = mCameras.Count;
            for (int i = 0; i < count; ++i)
                RTGizmos.get.CreateViewGizmo(mCameras[i]);

            // <!> Register the gizmos GUI handler
            RTGizmos.get.onGUI += OnGizmosGUI;
        }

        //-----------------------------------------------------------------------------
        // Name: <!> OnGizmosGUI() (Private Function)
        // Desc: Event handler for the RTGizmos.onGUI' event. We could use 'OnGUI', but
        //       this handler accepts a camera parameter which is the camera that renders
        //       the GUI. This could be useful in some situations.
        // Parm: camera         - The camera that renders the GUI.
        //       guiViewRect    - The camera viewport rectangle in GUI space.
        //-----------------------------------------------------------------------------
        void OnGizmosGUI(Camera camera, Rect guiViewRect)
        {
            // Let's draw the GUI only for the active camera
            if (camera != RTCamera.get.settings.targetCamera)
                return;

            // Start a clipping region so we can move the origin to the view rect top left corner
            GUI.BeginClip(guiViewRect);

            // Present some useful info to the user
            Rect r = new Rect(10.0f, 10.0f, 200.0f, 20.0f);
            GUI.Label(r, "Move Snap Mode: " + RTGizmos.get.moveSnapMode);
            r = r.GUIBelow(r);
            GUI.Label(r, "Transform Space: " + mGizmos[0].objectTransformGizmo.transformSpace);

            // Let's draw a frame around the active camera's viewport
            Color oldColor = GUI.color;
            GUI.color = Color.yellow;
            Texture2D frameTexture = TextureManager.get.gizmoLabelBG;
            const float frameWidth = 2.0f;
            GUI.DrawTexture(new Rect(1.0f, 0.0f, guiViewRect.width, frameWidth), frameTexture);                             // Top 
            GUI.DrawTexture(new Rect(1.0f, guiViewRect.height - frameWidth, guiViewRect.width, frameWidth), frameTexture);  // Bottom
            GUI.DrawTexture(new Rect(1.0f, 0.0f, frameWidth, guiViewRect.height), frameTexture);                            // Left
            GUI.DrawTexture(new Rect(guiViewRect.width - frameWidth, 0.0f, frameWidth, guiViewRect.height), frameTexture);  // Right
            GUI.color = oldColor;

            // End clipping region
            GUI.EndClip();
        }

        //-----------------------------------------------------------------------------
        // Name: SetSelectedGizmo() (Private Function)
        // Desc: Sets the currently selected gizmo. This is the gizmo that is used to
        //       transform the selected objects.
        // Parm: gizmoType - The gizmo type.
        //-----------------------------------------------------------------------------
        void SetSelectedGizmo(EGizmoType gizmoType)
        {
            // Disable the currently selected gizmo and update 'mSelectedGizmo'
            mGizmos[(int)mSelectedGizmo].enabled = false;
            mSelectedGizmo = gizmoType;

            // Make sure this new gizmo is up to date
            RefreshSelectedGizmo();
        }

        //-----------------------------------------------------------------------------
        // Name: PickObject() (Private Function)
        // Desc: Picks a game object with the mouse when the user presses the pick
        //       button (i.e. left mouse button).
        //-----------------------------------------------------------------------------
        void PickObject()
        {
            // When the left mouse button is released, try to pick an object from the scene.
            // Note: We also have to ensure that we don't modify the selection if the mouse
            //       hovers a gizmo's UI (e.g. the view gizmo projection switch label).
            if (RTInput.get.pointingInputDevice.pickButtonWentUp && 
                !RTGizmos.get.IsGizmoGUIHovered())
            {
                // Create a pick ray using the currently active camera
                Ray ray = RTInput.get.pointingInputDevice.GetPickRay(RTCamera.get.settings.targetCamera);

                // We'll use 'RTScene.get.Raycast' to pick objects. This allows us to pick objects without
                // the need of having colliders attached to them. The second parameter is an object filter
                // which allows you to filter objects. By default, the 'Raycast' function picks meshes,
                // sprites and terrains. We will use a filter that picks only 'Mesh' objects in the 'Default'
                // layer.
                ObjectFilter filter = new ObjectFilter { objectTypes = EGameObjectType.Mesh, layerMask = 1 << LayerMask.NameToLayer("Default") };
                if (RTScene.get.Raycast(ray, null, false, out SceneRayHit rayHit))
                {
                    // Cache data for easy access
                    GameObject hitObject = rayHit.objectHit.gameObject;

                    // If we have a hit, 2 things can happen:
                    //  1. If CTRL is pressed, we will append the object to the current selection if not already selected and deselect it if already selected.
                    //  2. If CTRL is NOT pressed, we will clear the selection and select this object only.
                    if (RTInput.get.KeyPressed(Key.LeftCtrl))
                    {
                        if (!mSelectedObjects.Contains(hitObject)) mSelectedObjects.Add(hitObject);
                        else mSelectedObjects.Remove(hitObject);
                    }
                    else
                    {
                        mSelectedObjects.Clear();
                        mSelectedObjects.Add(hitObject);
                    }
                }
                else
                {
                    // When nothing is hit, we just clear the selection
                    mSelectedObjects.Clear();
                }

                // Whenever we modify the target object collection, we need to refresh the selected gizmo
                RefreshSelectedGizmo();
            }
        }

        //-----------------------------------------------------------------------------
        // Name: RefreshSelectedGizmo() (Private Function)
        // Desc: Refreshes the selected gizmo when changes are made to the object selection.
        //-----------------------------------------------------------------------------
        void RefreshSelectedGizmo()
        {
            // Note: The extrude and box scale gizmos don't have an 'objectTransformGizmo'
            //       property. So we have to handle those gizmos carefully.
            if (selectedGizmo.objectTransformGizmo != null)
                selectedGizmo.objectTransformGizmo.Refresh();
            else
            {
                // Try extrude gizmo
                var extrudeGizmo = selectedGizmo as ExtrudeGizmo;
                if (extrudeGizmo != null) extrudeGizmo.Refresh();
                else
                {
                    // Try box scale gizmo
                    var boxScaleGizmo = selectedGizmo as BoxScaleGizmo;
                    if (boxScaleGizmo != null) boxScaleGizmo.Refresh();
                }
            }
        }
        #endregion
    }
    #endregion
}