using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SpotLightGizmoStyle (Public Class)
    // Desc: Stores style properties for spot light gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class SpotLightGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mRangeCapStyle              = new GizmoCapStyle();              // Range cap style
        [SerializeField] GizmoCapStyle  mInnerSpotCapStyle          = new GizmoCapStyle();              // Cap style for the handles that can be used to change the light's inner spot angle
        [SerializeField] GizmoCapStyle  mOuterSpotCapStyle          = new GizmoCapStyle();              // Cap style for the handles that can be used to change the light's outer spot angle
        
        [SerializeField] float          mSnapRayLength              = defaultSnapRayLength;             // Snap ray length
        
        [SerializeField] bool           mDirectionaLabelVisible     = defaultDirectionLabelVisible;     // Is the direction label visible?
        [SerializeField] bool           mTargetLabelVisible         = defaultTargetLabelVisible;        // Is the look-at target label visible?
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: rangeCapStyle (Public Property)
        // Desc: Returns the range cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle rangeCapStyle      { get { return mRangeCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: innerSpotCapStyle (Public Property)
        // Desc: Returns the inner spot cap style used by the handles that change the
        //       light's inner spot angle.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle innerSpotCapStyle  { get { return mInnerSpotCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: outerSpotCapStyle (Public Property)
        // Desc: Returns the outer spot cap style used by the handles that change the
        //       light's outer spot angle.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle outerSpotCapStyle  { get { return mOuterSpotCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: snapRayLength (Public Property)
        // Desc: Returns or sets the snap ray length. This ray is capped by a handle
        //       that can be used to snap the light's direction to look at the cursor
        //       pick point.
        //-----------------------------------------------------------------------------
        public float        snapRayLength       { get { return mSnapRayLength; } set { mSnapRayLength = Mathf.Max(value, 0.0f); } }

        //-----------------------------------------------------------------------------
        // Name: directionLabelVisible (Public Property)
        // Desc: Returns or sets whether the direction label is visible. This is the
        //       label that shows the light direction vector while dragging the direction
        //       snap handle.
        //-----------------------------------------------------------------------------
        public bool             directionLabelVisible       { get { return mDirectionaLabelVisible; } set { mDirectionaLabelVisible = value; } }

        //-----------------------------------------------------------------------------
        // Name: targetLabelVisible (Public Property)
        // Desc: Returns or sets whether the look-at target label is visible. This is the
        //       label that shows the position of the point the light is looking at while
        //       dragging the direction snap handle.
        //-----------------------------------------------------------------------------
        public bool             targetLabelVisible          { get { return mTargetLabelVisible; } set { mTargetLabelVisible = value; } }
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultRangeCapType             { get { return EGizmoCapType.Quad; } }
        public static EGizmoCapType defaultInnerSpotCapType         { get { return EGizmoCapType.Quad; } }
        public static EGizmoCapType defaultOuterSpotCapType         { get { return EGizmoCapType.Quad; } }

        public static Color         defaultRangeCapColor            { get { return Core.zAxisColor; } }
        public static Color         defaultInnerSpotCapColor        { get { return GlobalGizmoStyle.defaultLightColor; } }
        public static Color         defaultOuterSpotCapColor        { get { return GlobalGizmoStyle.defaultLightColor; } }

        public static float         defaultSnapRayLength            { get { return 7.0f; } }

        public static bool          defaultDirectionLabelVisible    { get { return true; } }
        public static bool          defaultTargetLabelVisible       { get { return true; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public SpotLightGizmoStyle CloneStyle()
        {
            var clone                   = MemberwiseClone() as SpotLightGizmoStyle;
            clone.mRangeCapStyle        = mRangeCapStyle.CloneStyle();
            clone.mInnerSpotCapStyle    = mInnerSpotCapStyle.CloneStyle();
            clone.mOuterSpotCapStyle    = mOuterSpotCapStyle.CloneStyle();
            return clone;
        }
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: OnUseDefaults() (Protected Function)
        // Desc: Called in order to set the style properties to default values.
        //-----------------------------------------------------------------------------
        protected override void OnUseDefaults()
        {
            // Cap styles
            mRangeCapStyle.UseDefaults();
            mRangeCapStyle.capType = defaultRangeCapType;
            mRangeCapStyle.color   = defaultRangeCapColor;

            mInnerSpotCapStyle.UseDefaults();
            mInnerSpotCapStyle.capType  = defaultInnerSpotCapType;
            mInnerSpotCapStyle.color    = defaultInnerSpotCapColor;

            mOuterSpotCapStyle.UseDefaults();
            mOuterSpotCapStyle.capType  = defaultOuterSpotCapType;
            mOuterSpotCapStyle.color    = defaultOuterSpotCapColor;

            // Misc
            snapRayLength = defaultSnapRayLength;

            // Labels
            directionLabelVisible   = defaultDirectionLabelVisible;
            targetLabelVisible      = defaultTargetLabelVisible;
        }

        #if UNITY_EDITOR
        //-----------------------------------------------------------------------------
        // Name: OnEditorGUI() (Protected Function)
        // Desc: Called when the style's GUI must be rendered inside the Unity Editor.
        // Parm: parentObject - The serializable parent object used to record property
        //                      changes.
        //-----------------------------------------------------------------------------
        protected override void OnEditorGUI(UnityEngine.Object parentObject)
        {
            float           newFloat;
            bool            newBool;
            var             content = new GUIContent();

            #region Rays
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Rays");
            {
                // Snap ray length
                content.text    = "Snap ray length";
                content.tooltip = "The length of the snap ray. This ray is capped by a handle " +
                                  "that can be used to snap the light's direction to look at the cursor pick point.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mSnapRayLength);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    snapRayLength = newFloat;
                }
            }
            #endregion

            // Caps
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mRangeCapStyle, "Range Cap", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mInnerSpotCapStyle, "Inner Spot Caps", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mOuterSpotCapStyle, "Outer Spot Caps", string.Empty, true, false, parentObject);

            #region Labels
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Labels");
            {
                // Direction label
                content.text    = "Direction label";
                content.tooltip = "Toggles the visibility of the direction label. This label shows the direction of the light while dragging the snap handle.";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, directionLabelVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    directionLabelVisible = newBool;
                }

                // Target label
                content.text    = "Target label";
                content.tooltip = "Toggles the visibility of the target label. This label shows the position of the point the light is looking at while dragging the snap handle.";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, targetLabelVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    targetLabelVisible = newBool;
                }
            }
            #endregion
        }
        #endif
        #endregion
    }
    #endregion
}
