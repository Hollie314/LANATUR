using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxColliderUndoState (Public Class)
    // Desc: Implements a 'BoxCollider' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class BoxColliderUndoState : UndoObjectState
    {
        #region Private Fields
        Vector3 mSize;      // Box collider size
        Vector3 mCenter;    // Box collider center
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: BoxColliderUndoState() (Public Constructor)
        // Desc: Creates a 'BoxColliderUndoState' for the specified collider.
        // Parm: collider - Target collider.
        //-----------------------------------------------------------------------------
        public BoxColliderUndoState(BoxCollider collider) : base(collider) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var collider    = target as BoxCollider;
            mSize           = collider.size;
            mCenter         = collider.center;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var collider    = target as BoxCollider;
            collider.size   = mSize;
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
            var s = other as BoxColliderUndoState;
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
