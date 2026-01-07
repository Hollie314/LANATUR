using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: Tutorial_2_Pivots_And_Transform_Spaces (Public Class)
    // Desc: Tutorial class which demonstrates the pivots and transform spaces functionality.
    //       Pivots are mostly useful with the rotate and scale gizmos. A pivot defines
    //       the origin of the transform. A rotate gizmo will rotate around the objects
    //       around the pivot. A scale gizmo will scale the objects from the pivot.
    //       The transform space defines the orientation of the gizmo axes. A move gizmo
    //       that uses the 'Global' space will have its axes aligned with the world axes.
    //       When it uses the 'Local' space, its axes are aligned with the target's axes.
    //-----------------------------------------------------------------------------
    public class Tutorial_2_Pivots_And_Transform_Spaces : MonoBehaviour
    {
        #region Private Fields
        MoveGizmo       mMoveGizmo;         // Move gizmo
        RotateGizmo     mRotateGizmo;       // Rotate gizmo
        ScaleGizmo      mScaleGizmo;        // Scale gizmo
        TRSGizmo        mTRSGizmo;          // TRS gizmo

        // Buffers used to avoid memory allocations
        List<ObjectTransformGizmo> mObjectTransformGizmoBuffer = new List<ObjectTransformGizmo>();
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
            // Change the transform space using hotkeys.
            // Note: Since some of these keys can conflict with the camera navigation keys, we will
            //       proceed if the camera is not currently navigating.
            if (RTCamera.get.navigationMode == ECameraNavigationMode.None)
            {
                // Toggle between global and local space
                if (RTInput.get.KeyWentDown(Key.G))
                {
                    // All gizmos use the same space, so we can just get one of the gizmo's transform space
                    EGizmoTransformSpace space = mMoveGizmo.objectTransformGizmo.transformSpace;

                    // Toggle space
                    if (space == EGizmoTransformSpace.Global) space = EGizmoTransformSpace.Local;
                    else space = EGizmoTransformSpace.Global;

                    // Set transform space for all gizmos. We could just use the member variables here,
                    // but let's do things differently. Say we didn't store the gizmos in member variables,
                    // we can use the 'CollectObjectTransformGizmos' function to get a collection of all
                    // transform gizmos which were created. Then we can loop through each gizmo in this list
                    // and set the transform space.
                    RTGizmos.get.CollectObjectTransformGizmos(mObjectTransformGizmoBuffer);
                    int count = mObjectTransformGizmoBuffer.Count;
                    for (int i = 0; i < count; ++i)
                        mObjectTransformGizmoBuffer[i].transformSpace = space;
                }
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnRTGInit() (Private Function)
        // Desc: Event handler for the 'RTG.initialized' event.
        //-----------------------------------------------------------------------------
        void OnRTGInit()
        {
            // Let's create transform gizmos which allow us to move, rotate and scale objects
            mMoveGizmo   = RTGizmos.get.CreateObjectMoveGizmo();
            mRotateGizmo = RTGizmos.get.CreateObjectRotateGizmo();
            mScaleGizmo  = RTGizmos.get.CreateObjectScaleGizmo();
            mTRSGizmo    = RTGizmos.get.CreateObjectTRSGizmo();

            // We have one object per gizmo, except for the TRS gizmo which uses multiple targets.
            mMoveGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Move"));
            mRotateGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Rotate"));
            mScaleGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Scale"));
            mTRSGizmo.objectTransformGizmo.SetTargets(new List<GameObject> 
            { GameObject.Find("Cube_TRS0"), GameObject.Find("Cube_TRS1"), GameObject.Find("Cube_TRS2"), GameObject.Find("Cube_TRS3") });

            // Let's initialize the pivots. Since the objects in this demo are using Unity's predefined
            // meshes whose pivot is the same as the center of the mesh, the 'Center' and 'Pivot' pivots
            // behave the same. However, in this demo scene we have a blue cube scaled to look like
            // a door. The cube's mesh pivot is at the center, but a door rotates around the hinges. So
            // let's set the pivot for our door object to sit at the left of the object in its local space.
            // Unity's cube meshes have a size of 1, so the left of the cube is <-0.5, 0, 0>.
            ObjectTransformGizmo.SetObjectLocalPivot(GameObject.Find("Cube_Rotate"), new Vector3(-0.5f, 0.0f, 0.0f));

            // Set the pivots for each gizmo
            mMoveGizmo.objectTransformGizmo.pivot   = EGizmoPivot.Pivot;
            mRotateGizmo.objectTransformGizmo.pivot = EGizmoPivot.Pivot;
            mScaleGizmo.objectTransformGizmo.pivot  = EGizmoPivot.Center;

            // The TRS gizmo uses multiple targets. We will set the pivot to 'Center' which will place the
            // gizmo in the middle of the target objects.
            mTRSGizmo.objectTransformGizmo.pivot    = EGizmoPivot.Center;

            // Let's also create a view gizmo
            RTGizmos.get.CreateViewGizmo(RTCamera.get.settings.targetCamera);
        }
        #endregion
    }
    #endregion
}