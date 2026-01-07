using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RecordUndoOp (Public Class)
    // Desc: Represents an undo operation that captures an object's state before
    //       and after a change.
    //-----------------------------------------------------------------------------
    public class RecordUndoOp : IUndoOperation
    {
        #region Private Fields
        UndoObjectState mStateBefore;   // Object state before it was modified
        UndoObjectState mStateAfter;    // Object state after it was modified
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns the record operation target object.
        //-----------------------------------------------------------------------------
        public object target { get { return mStateBefore.target; } }
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: RecordUndoOp() (Public Constructor)
        // Desc: Creates a record undo operation from the specified state.
        // Parm: stateBefore - Object state before it was modified. Must target the same
        //                     object as 'stateAfter'.
        //-----------------------------------------------------------------------------
        public RecordUndoOp(UndoObjectState stateBefore)
        {
            // Validate args
            if (stateBefore == null)
                RTG.Exception(nameof(RecordUndoOp), nameof(RecordUndoOp), "The Undo object state must be valid.");

            // Store state and clone
            mStateBefore    = stateBefore;
            mStateBefore.Extract();
            mStateAfter     = stateBefore.CloneState();
        }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: DiffStates() (Public Function)
        // Desc: Preforms a diff on the before and after states.
        // Rtrn: True if the 2 states are different and false otherwise.
        //-----------------------------------------------------------------------------
        public bool DiffStates()
        {
            // Extract the after state and diff
            mStateAfter.Extract();
            return mStateBefore.Diff(mStateAfter);
        }

        //-----------------------------------------------------------------------------
        // Name: Undo() (Public Function)
        // Desc: Revert the operation's effects.
        //-----------------------------------------------------------------------------
        public void Undo()
        {
            mStateBefore.Apply();
        }

        //-----------------------------------------------------------------------------
        // Name: Redo() (Public Function)
        // Desc: Restore the operation's effects.
        //-----------------------------------------------------------------------------
        public void Redo()
        {
            mStateAfter.Apply();
        }
        #endregion

        #region Public Virtual Functions
        //-----------------------------------------------------------------------------
        // Name: Flush() (Public Virtual Function)
        // Desc: Called when the operation is about to be removed from the undo stack
        //       because it is no longer needed.
        //-----------------------------------------------------------------------------
        public virtual void Flush(){}
        #endregion
    }
    #endregion
}
