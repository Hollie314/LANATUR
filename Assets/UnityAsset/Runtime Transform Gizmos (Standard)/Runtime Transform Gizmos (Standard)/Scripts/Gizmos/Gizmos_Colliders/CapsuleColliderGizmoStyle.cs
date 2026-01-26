using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CapsuleColliderGizmoStyle (Public Class)
    // Desc: Stores style properties for capsule collider gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class CapsuleColliderGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mRadiusCapStyle     = new GizmoCapStyle();  // Cap style for the radius handles
        [SerializeField] GizmoCapStyle  mHeightCapStyle     = new GizmoCapStyle();  // Cap style for the height handles
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: radiusCapStyle (Public Property)
        // Desc: Returns the radius cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle radiusCapStyle     { get { return mRadiusCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: heightCapStyle (Public Property)
        // Desc: Returns the height cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle heightCapStyle     { get { return mHeightCapStyle; } }
        #endregion
        
        #region Public Static Properties
        public static EGizmoCapType defaultRadiusCapType    { get { return EGizmoCapType.Quad; } }
        public static EGizmoCapType defaultHeightCapType    { get { return EGizmoCapType.Quad; } }

        public static Color         defaultRadiusCapColor   { get { return GlobalGizmoStyle.defaultColliderColor; } }
        public static Color         defaultHeightCapColor   { get { return GlobalGizmoStyle.defaultColliderColor; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public CapsuleColliderGizmoStyle CloneStyle()
        {
            var clone               = MemberwiseClone() as CapsuleColliderGizmoStyle;
            clone.mRadiusCapStyle   = mRadiusCapStyle.CloneStyle();
            clone.mHeightCapStyle   = mHeightCapStyle.CloneStyle();
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
            mRadiusCapStyle.UseDefaults();
            mRadiusCapStyle.capType = defaultRadiusCapType;
            mRadiusCapStyle.color   = defaultRadiusCapColor;

            mHeightCapStyle.UseDefaults();
            mHeightCapStyle.capType   = defaultHeightCapType;
            mHeightCapStyle.color     = defaultHeightCapColor;
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
            GizmoCapStyle.DrawNonDirectionalCapUI(mHeightCapStyle, "Height Cap", string.Empty, true, false, parentObject);
        }
        #endif
        #endregion
    }
    #endregion
}
