using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: AudioReverbZoneUndoState (Public Class)
    // Desc: Implements an 'AudioReverbZone' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class AudioReverbZoneUndoState : UndoObjectState
    {
        #region Private Fields
        float mMinDistance;     // Min distance
        float mMaxDistance;     // Max distance
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: AudioReverbZoneUndoState() (Public Constructor)
        // Desc: Creates an 'AudioReverbZoneUndoState' for the specified reverb zone.
        // Parm: reverbZone - Target reverb zone.
        //-----------------------------------------------------------------------------
        public AudioReverbZoneUndoState(AudioReverbZone reverbZone) : base(reverbZone) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var reverbZone      = target as AudioReverbZone;
            mMinDistance        = reverbZone.minDistance;
            mMaxDistance        = reverbZone.maxDistance;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var reverbZone          = target as AudioReverbZone;
            reverbZone.minDistance  = mMinDistance;
            reverbZone.maxDistance  = mMaxDistance;
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
            var s = other as AudioReverbZoneUndoState;
            if (s == null) return false;

            // Check state diff
            if (mMinDistance != s.mMinDistance ||
                mMaxDistance != s.mMaxDistance) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
