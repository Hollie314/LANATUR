using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxCollider2DGizmoStyle (Public Class)
    // Desc: Stores style properties for 2D box collider gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class BoxCollider2DGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mUniformCapStyle    = new GizmoCapStyle();  // Uniform resize cap style
        [SerializeField] GizmoCapStyle  mResizeCapStyle     = new GizmoCapStyle();  // Resize cap style
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultUnformResizeCapType      { get { return EGizmoCapType.Box; } }
        public static EGizmoCapType defaultResizeCapType            { get { return EGizmoCapType.Quad; } }
        public static Color         defaultUniformResizeCapColor    { get { return GlobalGizmoStyle.defaultColliderColor; } }
        public static Color         defaultResizeCapColor           { get { return GlobalGizmoStyle.defaultColliderColor; } }
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: uniformCapStyle (Public Property)
        // Desc: Returns the uniform resize cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle uniformCapStyle    { get { return mUniformCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: resizeCapStyle (Public Property)
        // Desc: Returns the resize cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle resizeCapStyle     { get { return mResizeCapStyle; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public BoxCollider2DGizmoStyle CloneStyle()
        {
            // Clone
            var clone               = MemberwiseClone() as BoxCollider2DGizmoStyle;
            clone.mResizeCapStyle   = mResizeCapStyle.CloneStyle();
            clone.mUniformCapStyle  = mUniformCapStyle.CloneStyle();

            // Return clone
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
            // Uniform resize cap
            mUniformCapStyle.UseDefaults();
            mUniformCapStyle.capType = defaultUnformResizeCapType;
            mUniformCapStyle.color   = defaultUniformResizeCapColor;

            // Resize cap
            mResizeCapStyle.UseDefaults();
            mResizeCapStyle.capType = defaultResizeCapType;
            mResizeCapStyle.color   = defaultResizeCapColor;
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
            GizmoCapStyle.DrawNonDirectionalCapUI(mResizeCapStyle, "Resize Caps", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mUniformCapStyle, "Uniform Resize Cap", string.Empty, true, true, parentObject);
        }
        #endif
        #endregion
    }
    #endregion
}