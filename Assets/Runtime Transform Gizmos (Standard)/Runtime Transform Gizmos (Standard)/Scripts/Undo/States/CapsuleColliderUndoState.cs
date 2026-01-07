using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CapsuleColliderUndoState (Public Class)
    // Desc: Implements a 'CapsuleCollider' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class CapsuleColliderUndoState : UndoObjectState
    {
        #region Private Fields
        float       mRadius;        // Capsule collider radius
        float       mHeight;        // Capsule collider height
        Vector3     mCenter;        // Capsule collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: CapsuleColliderUndoState() (Public Constructor)
        // Desc: Creates a 'CapsuleColliderUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public CapsuleColliderUndoState(CapsuleCollider collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as CapsuleCollider;
            mRadius         = collider.radius;
            mHeight         = collider.height;
            mCenter         = collider.center;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var collider    = target as CapsuleCollider;
            collider.radius = mRadius;
            collider.height = mHeight;
            collider.center = mCenter;
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
            var s = other as CapsuleColliderUndoState;
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
