using UnityEngine;
using UnityEngine.InputSystem;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: Tutorial_1_Gizmo_Enable_Disable (Public Class)
    // Desc: Tutorial class which implements a simple gizmo manager which allows the
    //       use to enable/disable gizmos with hotkeys. 
    //-----------------------------------------------------------------------------
    public class Tutorial_1_Gizmo_Enable_Disable : MonoBehaviour
    {
        #region Private Fields
        MoveGizmo       mMoveGizmo;         // Move gizmo
        RotateGizmo     mRotateGizmo;       // Rotate gizmo
        ScaleGizmo      mScaleGizmo;        // Scale gizmo
        TRSGizmo        mTRSGizmo;          // TRS gizmo
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
                if (RTInput.get.KeyWentDown(Key.W))         // Enable move gizmo
                {
                    mMoveGizmo.enabled      = true;
                    mRotateGizmo.enabled    = false;
                    mScaleGizmo.enabled     = false;
                    mTRSGizmo.enabled       = false;
                }
                else
                if (RTInput.get.KeyWentDown(Key.E))         // Enable rotate gizmo
                {
                    mMoveGizmo.enabled      = false;
                    mRotateGizmo.enabled    = true;
                    mScaleGizmo.enabled     = false;
                    mTRSGizmo.enabled       = false;
                }
                else
                if (RTInput.get.KeyWentDown(Key.R))         // Enable scale gizmo
                {
                    mMoveGizmo.enabled      = false;
                    mRotateGizmo.enabled    = false;
                    mScaleGizmo.enabled     = true;
                    mTRSGizmo.enabled       = false;
                }
                else
                if (RTInput.get.KeyWentDown(Key.T))         // Enable TRS gizmo
                {
                    mMoveGizmo.enabled      = false;
                    mRotateGizmo.enabled    = false;
                    mScaleGizmo.enabled     = false;
                    mTRSGizmo.enabled       = true;
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

            // In this simple demo, we will have a single target object and all gizmos will control this object
            GameObject targetObject = GameObject.Find("Cube");
            mMoveGizmo.objectTransformGizmo.SetTarget(targetObject);
            mRotateGizmo.objectTransformGizmo.SetTarget(targetObject);
            mScaleGizmo.objectTransformGizmo.SetTarget(targetObject);
            mTRSGizmo.objectTransformGizmo.SetTarget(targetObject);

            // By default we will disable all gizmos, except the move gizmo. We can then toggle gizmos on and off using hotkeys.
            mRotateGizmo.enabled    = false;
            mScaleGizmo.enabled     = false;
            mTRSGizmo.enabled       = false;

            // Let's also create a view gizmo
            RTGizmos.get.CreateViewGizmo(RTCamera.get.settings.targetCamera);
        }
        #endregion
    }
    #endregion
}