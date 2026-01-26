using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CapsuleCollider2DUndoState (Public Class)
    // Desc: Implements a 'CapsuleCollider2D' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class CapsuleCollider2DUndoState : UndoObjectState
    {
        #region Private Fields
        Vector2     mSize;          // Capsule collider size
        Vector3     mCenter;        // Capsule collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: CapsuleCollider2DUndoState() (Public Constructor)
        // Desc: Creates a 'CapsuleCollider2DUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public CapsuleCollider2DUndoState(CapsuleCollider2D collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as CapsuleCollider2D;
            mSize           = collider.size;
            mCenter         = collider.offset;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var collider    = target as CapsuleCollider2D;
            collider.size   = mSize;
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
            var s = other as CapsuleCollider2DUndoState;
            if (s == null) return false;

            // Check state diff
            if (mSize   != s.mSize ||
                mCenter != s.mCenter) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
