using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxCollider2DUndoState (Public Class)
    // Desc: Implements a 'BoxCollider2D' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class BoxCollider2DUndoState : UndoObjectState
    {
        #region Private Fields
        Vector2 mSize;      // Box collider size
        Vector2 mCenter;    // Box collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: BoxCollider2DUndoState() (Public Constructor)
        // Desc: Creates a 'BoxCollider2DUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public BoxCollider2DUndoState(BoxCollider2D collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as BoxCollider2D;
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
            var collider    = target as BoxCollider2D;
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
            var s = other as BoxCollider2DUndoState;
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
