using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: LightUndoState (Public Class)
    // Desc: Implements a 'Light' Undo/Redo state.
    //-----------------------------------------------------------------------------
    public class LightUndoState : UndoObjectState
    {
        #region Private Fields
        LightType           mLightType;             // Light type
        LightRenderMode     mRenderMode;            // Light render mode
        Color               mColor;                 // Light color
        float               mColorTemp;             // Color temperature
        bool                mUseColorTemp;          // Use color temperature?
        float               mIntensity;             // Light intensity
        float               mIndirectMult;          // Indirect multiplier
        float               mRange;                 // Light range
        float               mInnerSpot;             // Inner spot angle
        float               mOuterSpot;             // Outer spot angle
        int                 mRenderingLayers;       // Rendering layers
        int                 mCullingMask;           // Culling mask
        LightShadows        mShadowType;            // Shadow type
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: LightUndoState() (Public Constructor)
        // Desc: Creates a 'LightUndoState' for the specified light.
        // Parm: light - Target light.
        //-----------------------------------------------------------------------------
        public LightUndoState(Light light) : base(light) {}
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var light           = target as Light;
            mLightType          = light.type;
            mRenderMode         = light.renderMode;
            mColor              = light.color;
            mColorTemp          = light.colorTemperature;
            mUseColorTemp       = light.useColorTemperature;
            mIntensity          = light.intensity;
            mIndirectMult       = light.bounceIntensity;
            mRange              = light.range;
            mInnerSpot          = light.innerSpotAngle;
            mOuterSpot          = light.spotAngle;
            mRenderingLayers    = light.renderingLayerMask;
            mCullingMask        = light.cullingMask;
            mShadowType         = light.shadows;
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var light                   = target as Light;
            light.type                  = mLightType;
            light.renderMode            = mRenderMode;
            light.color                 = mColor;
            light.colorTemperature      = mColorTemp;
            light.useColorTemperature   = mUseColorTemp;
            light.intensity             = mIntensity;
            light.bounceIntensity       = mIndirectMult;
            light.range                 = mRange;
            light.innerSpotAngle        = mInnerSpot;
            light.spotAngle             = mOuterSpot;
            light.renderingLayerMask    = mRenderingLayers;
            light.cullingMask           = mCullingMask;
            light.shadows               = mShadowType;
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
            var s = other as LightUndoState;
            if (s == null) return false;

            // Check state diff
            if (mLightType          != s.mLightType ||
                mRenderMode         != s.mRenderMode ||
                mColor              != s.mColor ||
                mColorTemp          != s.mColorTemp ||
                mUseColorTemp       != s.mUseColorTemp ||
                mIntensity          != s.mIntensity ||
                mIndirectMult       != s.mIndirectMult ||
                mRange              != s.mRange ||
                mInnerSpot          != s.mInnerSpot ||
                mOuterSpot          != s.mOuterSpot ||
                mRenderingLayers    != s.mRenderingLayers ||
                mCullingMask        != s.mCullingMask ||
                mShadowType         != s.mShadowType) return true;

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
