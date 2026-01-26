using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CameraGizmoStyle (Public Class)
    // Desc: Stores style properties for camera gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class CameraGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] Color          mColor              = defaultColor;             // Gizmo color
        [SerializeField] GizmoCapStyle  mVolumeCapStyle     = new GizmoCapStyle();      // Style used for the handles that change the camera volume
        [SerializeField] GizmoCapStyle  mPlaneCapStyle      = new GizmoCapStyle();      // Style used for the handles that change the camera near/far planes
        [SerializeField] GizmoCapStyle  mViewSnapCapStyle   = new GizmoCapStyle();      // Style used for the handle that snap the camera view vector to a surface pickpoint 
        [SerializeField] Color          mViewSnapRayColor   = defaultViewSnapRayColor;  // Color used to draw the view snap ray   
        [SerializeField] float          mViewSnapRayLength  = defaultViewSnapRayLength; // The length of the view snap ray
        [SerializeField] float          mPlaneSnap          = defaultPlaneSnap;         // The near/far plane snap increment
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: color (Public Property)
        // Desc: Returns or sets the gizmo color.
        //-----------------------------------------------------------------------------
        public Color            color               { get { return mColor; } set { mColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: volumeCapStyle (Public Property)
        // Desc: Returns the volume cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    volumeCapStyle      { get { return mVolumeCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: planeCapStyle (Public Property)
        // Desc: Returns the plane cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    planeCapStyle       { get { return mPlaneCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: viewSnapCapStyle (Public Property)
        // Desc: Returns the view snap cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    viewSnapCapStyle    { get { return mViewSnapCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: viewSnapRayColor (Public Property)
        // Desc: Returns or sets the view snap ray color.
        //-----------------------------------------------------------------------------
        public Color            viewSnapRayColor    { get { return mViewSnapRayColor; } set { mViewSnapRayColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: viewSnapRayLength (Public Property)
        // Desc: Returns or sets the view snap ray length.
        //-----------------------------------------------------------------------------
        public float            viewSnapRayLength   { get { return mViewSnapRayLength; } set { mViewSnapRayLength = Mathf.Max(0.0f, value); } }

        //-----------------------------------------------------------------------------
        // Name: planeSnap (Public Property)
        // Desc: Returns or sets the snap increment used when snapping the near and far
        //       planes.
        //-----------------------------------------------------------------------------
        public float            planeSnap           { get { return mPlaneSnap; } set { mPlaneSnap = Mathf.Max(1e-4f, value); } }
        #endregion

        #region Public Static Properties
        public static Color             defaultColor                { get { return ColorEx.FromRGBABytes(210, 210, 210, 255); } }
        public static Color             defaultVolumeCapColor       { get { return ColorEx.FromRGBABytes(56, 115, 254, 255); } }
        public static Color             defaultPlaneCapColor        { get { return ColorEx.FromRGBABytes(255, 153, 51, 255); } }
        public static Color             defaultViewSnapCapColor     { get { return Color.magenta; } }
        public static Color             defaultViewSnapRayColor     { get { return ColorEx.FromRGBBytes(221, 144, 60); } }
        public static EGizmoCapType     defaultVolumeCapType        { get { return EGizmoCapType.Quad; } }
        public static EGizmoCapType     defaultPlaneCapType         { get { return EGizmoCapType.Circle; } }
        public static EGizmoCapType     defaultViewSnapCapType      { get { return EGizmoCapType.Quad; } }
        public static float             defaultViewSnapRayLength    { get { return 25.0f; } }
        public static float             defaultPlaneSnap            { get { return 0.1f; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public CameraGizmoStyle CloneStyle()
        {
            // Clone
            var clone = MemberwiseClone() as CameraGizmoStyle;

            // Clone styles
            clone.mVolumeCapStyle   = mVolumeCapStyle.CloneStyle();
            clone.mPlaneCapStyle    = mPlaneCapStyle.CloneStyle();
            clone.mViewSnapCapStyle = mViewSnapCapStyle.CloneStyle();

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
            // Volume cap style
            mVolumeCapStyle.UseDefaults();
            mVolumeCapStyle.capType = defaultVolumeCapType;
            mVolumeCapStyle.color   = defaultVolumeCapColor;

            // Plane cap style
            mPlaneCapStyle.UseDefaults();
            mPlaneCapStyle.capType      = defaultPlaneCapType;
            mPlaneCapStyle.color        = defaultPlaneCapColor;

            // View snap cap style
            mViewSnapCapStyle.UseDefaults();
            mViewSnapCapStyle.capType   = defaultViewSnapCapType;
            mViewSnapCapStyle.color     = defaultViewSnapCapColor;

            // Misc
            color               = defaultColor;
            viewSnapRayColor    = defaultViewSnapRayColor;
            viewSnapRayLength   = defaultViewSnapRayLength;
            planeSnap           = defaultPlaneSnap;
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
            Color newColor;
            float newFloat;
            var content = new GUIContent();

            // Caps
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mVolumeCapStyle, "Volume Caps", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mPlaneCapStyle, "Near/Far Plane Caps", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mViewSnapCapStyle, "View Snap Cap", string.Empty, true, false, parentObject);

            // View snap ray color
            content.text    = "View snap ray color";
            content.tooltip = "The color of the view snap ray.";
            EditorGUI.BeginChangeCheck();
            newColor        = EditorGUILayout.ColorField(content, viewSnapRayColor);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                viewSnapRayColor  = newColor;
            }

            // View snap ray length
            content.text    = "View snap ray length";
            content.tooltip = "The length of the view snap ray.";
            EditorGUI.BeginChangeCheck();
            newFloat        = EditorGUILayout.FloatField(content, viewSnapRayLength);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                viewSnapRayLength  = newFloat;
            }

            // Plane snap
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Misc");
            content.text    = "Plane snap";
            content.tooltip = "The snap increment used when snapping the near and far planes.";
            EditorGUI.BeginChangeCheck();
            newFloat        = EditorGUILayout.FloatField(content, planeSnap);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                planeSnap  = newFloat;
            }
        }
        #endif
        #endregion
    }
    #endregion
}