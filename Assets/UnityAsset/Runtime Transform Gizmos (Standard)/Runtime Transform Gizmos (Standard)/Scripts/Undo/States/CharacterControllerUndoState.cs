using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CharacterControllerUndoState (Public Class)
    // Desc: Implements a 'CharacterController' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class CharacterControllerUndoState : UndoObjectState
    {
        #region Private Fields
        float       mRadius;    // Controller radius
        float       mHeight;    // Controller height
        Vector3     mCenter;    // Controller center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: CharacterControllerUndoState() (Public Constructor)
        // Desc: Creates a 'CharacterControllerUndoState' for the specified character
        //       controller.
        // Parm: controller - Target controller.
        //-----------------------------------------------------------------------------
        public CharacterControllerUndoState(CharacterController controller) : base(controller) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var controller      = target as CharacterController;
            mRadius             = controller.radius;
            mHeight             = controller.height;
            mCenter             = controller.center;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var controller    = target as CharacterController;
            controller.radius = mRadius;
            controller.height = mHeight;
            controller.center = mCenter;
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
            var s = other as CharacterControllerUndoState;
            if (s == null) return false;

            // Check state diff
            if (mRadius     != s.mRadius ||
                mHeight     != s.mHeight ||
                mCenter     != s.mCenter) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
