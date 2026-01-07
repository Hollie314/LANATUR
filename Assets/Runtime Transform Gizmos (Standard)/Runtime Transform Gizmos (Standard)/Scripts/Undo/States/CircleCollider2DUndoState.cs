using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CircleCollider2DUndoState (Public Class)
    // Desc: Implements a 'CircleCollider2D' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class CircleCollider2DUndoState : UndoObjectState
    {
        #region Private Fields
        float   mRadius;        // Circle collider radius
        Vector2 mCenter;        // Circle collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: CircleCollider2DUndoState() (Public Constructor)
        // Desc: Creates a 'CircleCollider2DUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public CircleCollider2DUndoState(CircleCollider2D collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as CircleCollider2D;
            mRadius         = collider.radius;
            mCenter         = collider.offset;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var collider    = target as CircleCollider2D;
            collider.radius = mRadius;
            collider.offset = mCenter;
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
            var s = other as CircleCollider2DUndoState;
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
