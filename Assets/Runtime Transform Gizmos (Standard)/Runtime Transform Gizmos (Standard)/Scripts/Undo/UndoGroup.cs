using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: UndoGroup (Public Class)
    // Desc: Represents an undo group. An undo group is a collection of operations
    //       which should be treated as a unit. All operations belonging to the same
    //       group are undone/redone at the same time.
    //-----------------------------------------------------------------------------
    public class UndoGroup
    {
        #region Private Fields
        Dictionary<object, RecordUndoOp>    mObjectToRecordOpMap    = new Dictionary<object, RecordUndoOp>();   // Maps an object to its record operation. Helps avoid having more than one record op for a single object.
        List<IUndoOperation>                mOperations             = new List<IUndoOperation>();               // Operations belonging to this group
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: operationCount (Public Property)
        // Desc: Returns the number of undo operations stored in this group.
        //-----------------------------------------------------------------------------
        public int operationCount { get { return mOperations.Count; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Undo() (Public Function)
        // Desc: Reverts the effects of all operations in the group.
        //-----------------------------------------------------------------------------
        public void Undo()
        {
            int count = mOperations.Count;
            for (int i = count - 1; i >= 0; --i)
                mOperations[i].Undo();
        }

        //-----------------------------------------------------------------------------
        // Name: Redo() (Public Function)
        // Desc: Restores the effects of all operations in the group.
        //-----------------------------------------------------------------------------
        public void Redo()
        {
            int count = mOperations.Count;
            for (int i = 0; i < count; ++i)
                mOperations[i].Redo();
        }

        //-----------------------------------------------------------------------------
        // Name: Flush() (Public Function)
        // Desc: Called when the group is about to be removed from the undo stack
        //       because it is no longer needed. This has the effect of flushing all 
        //       operations assigned to the group.
        //-----------------------------------------------------------------------------
        public void Flush()
        {
            int count = mOperations.Count;
            for (int i = 0; i < count; ++i)
                mOperations[i].Flush();
        }

        //-----------------------------------------------------------------------------
        // Name: AddOperation() (Public Function)
        // Desc: Adds the specified operation to the group. The function doesn't validate
        //       the operation. It assumes it points to a valid instance and that it hasn't
        //       already been assigned to the group.
        // Parm: op - The operation to add to the group.
        //-----------------------------------------------------------------------------
        public void AddOperation(IUndoOperation op)
        {
            // If this is a record op, check if we already have a record op for this object
            RecordUndoOp recOp = op as RecordUndoOp;
            if (recOp != null)
            {
                // If we are already recording this object, exit
                if (mObjectToRecordOpMap.ContainsKey(recOp.target))
                    return;

                // Store this object for next time
                mObjectToRecordOpMap.Add(recOp.target, recOp);
            }

            // Add operation
            mOperations.Add(op);
        }

        //-----------------------------------------------------------------------------
        // Name: Commit() (Public Function)
        // Desc: Called before the undo group is pushed onto the undo stack in order to
        //       perform a post-process step on the undo operations. For example, record
        //       operations whose before and after states register no diffs, are removed.
        // Rtrn: True if the undo group contains at least one undo operation and false
        //       otherwise.
        //-----------------------------------------------------------------------------
        public bool Commit()
        {
            // Loop through each operation
            for (int i = 0; i < mOperations.Count; )
            {
                // Is this a record op?
                RecordUndoOp recordOp = mOperations[i] as RecordUndoOp;
                if (recordOp != null)
                {
                    // If the 2 states are the same, remove this op and continue
                    if (!recordOp.DiffStates())
                    {
                        mOperations.RemoveAt(i);
                        continue;
                    }
                }

                // Next op
                ++i;
            }

            // Do we have any operations left?
            return mOperations.Count != 0;
        }
        #endregion
    }
    #endregion
}
