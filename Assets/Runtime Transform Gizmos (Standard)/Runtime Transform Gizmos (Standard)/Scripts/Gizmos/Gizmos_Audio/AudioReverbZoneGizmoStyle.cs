using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: AudioReverbZoneGizmoStyle (Public Class)
    // Desc: Stores style properties for audio reverb zone gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class AudioReverbZoneGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] Color          mColor              = defaultColor;         // Gizmo color
        [SerializeField] GizmoCapStyle  mDistanceCapStyle   = new GizmoCapStyle();  // Distance cap style
        #endregion

        #region Public Static Properties
        public static Color         defaultColor                { get { return ColorEx.FromRGBABytes(145, 167, 193, 255); } }
        public static EGizmoCapType defaultDistanceCapType      { get { return EGizmoCapType.Quad; } }
        public static Color         defaultDistanceCapColor     { get { return ColorEx.FromRGBABytes(56, 115, 254, 255); } }
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: color (Public Property)
        // Desc: Returns or sets the gizmo color.
        //-----------------------------------------------------------------------------
        public Color            color               { get { return mColor; } set { mColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: distanceCapStyle (Public Property)
        // Desc: Returns the cap style for the distance handles.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    distanceCapStyle    { get { return mDistanceCapStyle; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public AudioReverbZoneGizmoStyle CloneStyle()
        {
            // Clone
            var clone               = MemberwiseClone() as AudioReverbZoneGizmoStyle;
            clone.mDistanceCapStyle = mDistanceCapStyle.CloneStyle();

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
            color = defaultColor;
            distanceCapStyle.UseDefaults();
            distanceCapStyle.capType    = defaultDistanceCapType;
            distanceCapStyle.color      = defaultDistanceCapColor;
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
            Color   newColor;
            var     content = new GUIContent();

            // Color
            content.text    = "Color";
            content.tooltip = "Gizmo color.";
            EditorGUI.BeginChangeCheck();
            newColor = EditorGUILayout.ColorField(content, color);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                color = newColor;
            }

            // Caps
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mDistanceCapStyle, "Distance Caps", string.Empty, true, false, parentObject);
        }
        #endif
        #endregion
    }
    #endregion
}

