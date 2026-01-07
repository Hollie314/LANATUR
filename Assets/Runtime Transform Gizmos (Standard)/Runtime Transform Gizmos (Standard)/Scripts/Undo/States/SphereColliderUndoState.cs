using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SphereColliderUndoState (Public Class)
    // Desc: Implements a 'SphereCollider' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class SphereColliderUndoState : UndoObjectState
    {
        #region Private Fields
        float       mRadius;        // Sphere collider radius
        Vector3     mCenter;        // Sphere collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: SphereColliderUndoState() (Public Constructor)
        // Desc: Creates a 'SphereColliderUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public SphereColliderUndoState(SphereCollider collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as SphereCollider;
            mRadius         = collider.radius;
            mCenter         = collider.center;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var collider    = target as SphereCollider;
            collider.radius = mRadius;
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
            var s = other as SphereColliderUndoState;
            if (s == null) return false;

            // Check state diff
            if (mRadius     != s.mRadius ||
                mCenter     != s.mCenter) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
