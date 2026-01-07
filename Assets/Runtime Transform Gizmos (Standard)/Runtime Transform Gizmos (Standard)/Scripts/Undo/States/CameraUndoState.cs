using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CameraUndoState (Public Class)
    // Desc: Implements a 'Camera' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class CameraUndoState : UndoObjectState
    {
        #region Private Fields
        float   mFOV;           // Field of view
        float   mOrthoSize;     // Ortho size
        float   mNearClipPlane; // Near clip plane distance
        float   mFarClipPlane;  // Far clip plane distance
        bool    mOrhto;         // Ortho camera?
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: CameraUndoState() (Public Constructor)
        // Desc: Creates a 'CameraUndoState' for the specified camera.
        // Parm: camera - Target camera.
        //-----------------------------------------------------------------------------
        public CameraUndoState(Camera camera) : base(camera) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var camera      = target as Camera;
            mFOV            = camera.fieldOfView;
            mOrthoSize      = camera.orthographicSize;
            mNearClipPlane  = camera.nearClipPlane;
            mFarClipPlane   = camera.farClipPlane;
            mOrhto          = camera.orthographic;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var camera              = target as Camera;
            camera.fieldOfView      = mFOV;
            camera.orthographicSize = mOrthoSize;
            camera.nearClipPlane    = mNearClipPlane;
            camera.farClipPlane     = mFarClipPlane;
            camera.orthographic     = mOrhto;
        }

        //-----------------------------------------------------------------------------
        // Name: Diff() (Public Function)
        // Desc: Checks if there is any difference between 'this' state and 'other'. The
        //       function assumes the 2 states have the same target object.
        // Parm: other - The other state.
        // Rtrn: True if the 2 states are different and false otherwise.
        //-----------------------------------------------------------------------------
        public override bool Diff(UndoObjectState other)
        {
            // Check type
            var s = other as CameraUndoState;
            if (s == null) return false;

            // Check state diff
            if (mFOV            != s.mFOV           ||
                mOrthoSize      != s.mOrthoSize     ||
                mNearClipPlane  != s.mNearClipPlane ||
                mFarClipPlane   != s.mFarClipPlane  ||
                mOrhto          != s.mOrhto) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
