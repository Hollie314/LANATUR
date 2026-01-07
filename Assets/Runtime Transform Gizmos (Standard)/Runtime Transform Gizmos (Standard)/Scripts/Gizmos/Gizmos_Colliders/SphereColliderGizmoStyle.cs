using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SphereColliderGizmoStyle (Public Class)
    // Desc: Stores style properties for sphere collider gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class SphereColliderGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mCenterRadiusCapStyle   = new GizmoCapStyle();  // Center radius cap style which resizes from the center
        [SerializeField] GizmoCapStyle  mRadiusCapStyle         = new GizmoCapStyle();  // Radius cap style. Radius caps are the handles that are used to change the sphere radius.
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: centerRadiusCapStyle (Public Property)
        // Desc: Returns the center radius cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle centerRadiusCapStyle   { get { return mCenterRadiusCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: radiusCapStyle (Public Property)
        // Desc: Returns the radius cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle radiusCapStyle         { get { return mRadiusCapStyle; } }
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultCenterRadiusCapType      { get { return EGizmoCapType.Box; } }
        public static EGizmoCapType defaultRadiusCapType            { get { return EGizmoCapType.Quad; } }

        public static Color         defaultCenterRadiusCapColor     { get { return GlobalGizmoStyle.defaultColliderColor; } }
        public static Color         defaultRadiusCapColor           { get { return GlobalGizmoStyle.defaultColliderColor; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public SphereColliderGizmoStyle CloneStyle()
        {
            var clone                   = MemberwiseClone() as SphereColliderGizmoStyle;
            clone.mCenterRadiusCapStyle = mCenterRadiusCapStyle.CloneStyle();
            clone.mRadiusCapStyle       = mRadiusCapStyle.CloneStyle();
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
            mCenterRadiusCapStyle.UseDefaults();
            mCenterRadiusCapStyle.capType   = defaultCenterRadiusCapType;
            mCenterRadiusCapStyle.color     = defaultCenterRadiusCapColor;

            mRadiusCapStyle.UseDefaults();
            mRadiusCapStyle.capType = defaultRadiusCapType;
            mRadiusCapStyle.color   = defaultRadiusCapColor;
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
            GizmoCapStyle.DrawNonDirectionalCapUI(mRadiusCapStyle, "Radius Caps", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mCenterRadiusCapStyle, "Center Radius Cap", string.Empty, true, true, parentObject);
        }
        #endif
        #endregion
    }
    #endregion
}
