using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: AudioSourceUndoState (Public Class)
    // Desc: Implements an 'AudioSource' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class AudioSourceUndoState : UndoObjectState
    {
        #region Private Fields
        float mMinDistance;     // Min distance
        float mMaxDistance;     // Max distance
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: AudioSourceUndoState() (Public Constructor)
        // Desc: Creates an 'AudioSourceUndoState' for the specified audio source.
        // Parm: audioSource - Target audio source.
        //-----------------------------------------------------------------------------
        public AudioSourceUndoState(AudioSource audioSource) : base(audioSource) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var audioSource     = target as AudioSource;
            mMinDistance        = audioSource.minDistance;
            mMaxDistance        = audioSource.maxDistance;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var audioSource          = target as AudioSource;
            audioSource.minDistance  = mMinDistance;
            audioSource.maxDistance  = mMaxDistance;
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
            var s = other as AudioSourceUndoState;
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
