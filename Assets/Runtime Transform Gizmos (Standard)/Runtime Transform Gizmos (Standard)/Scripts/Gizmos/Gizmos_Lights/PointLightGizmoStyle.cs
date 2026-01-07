using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: PointLightGizmoStyle (Public Class)
    // Desc: Stores style properties for point light gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class PointLightGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mRangeCapStyle = new GizmoCapStyle();  // Range cap style. Range caps are the handles that can be used to change the light's range.
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: rangeCapStyle (Public Property)
        // Desc: Returns the range cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle rangeCapStyle { get { return mRangeCapStyle; } }
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultRangeCapType    { get { return EGizmoCapType.Quad; } }
        public static Color         defaultRangeCapColor   { get { return GlobalGizmoStyle.defaultLightColor; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public PointLightGizmoStyle CloneStyle()
        {
            var clone               = MemberwiseClone() as PointLightGizmoStyle;
            clone.mRangeCapStyle    = mRangeCapStyle.CloneStyle();
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
            mRangeCapStyle.UseDefaults();
            mRangeCapStyle.capType = defaultRangeCapType;
            mRangeCapStyle.color   = defaultRangeCapColor;
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
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mRangeCapStyle, "Range Caps", string.Empty, true, false, parentObject);
        }
        #endif
        #endregion
    }
    #endregion
}
