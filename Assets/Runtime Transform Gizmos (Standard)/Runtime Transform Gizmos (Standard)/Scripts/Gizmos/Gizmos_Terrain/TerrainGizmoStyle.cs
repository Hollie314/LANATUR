using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: TerrainGizmoStyle (Public Class)
    // Desc: Stores style properties for terrain gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class TerrainGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] Color                  mRadiusCircleColor      = defaultRadiusCircleColor;     // The color of the radius indicator circle projected onto the terrain
        [SerializeField] GizmoLineSliderStyle   mHeightSliderStyle      = new GizmoLineSliderStyle();   // The style of the slider used to change the terrain height
        [SerializeField] GizmoCapStyle          mHeightCapStyle         = new GizmoCapStyle();          // The style of the height slider cap
        [SerializeField] GizmoCapStyle          mCenterCapStyle         = new GizmoCapStyle();          // The style of the cap which sits in the center of the circle
        [SerializeField] GizmoCapStyle          mRadiusCapStyle         = new GizmoCapStyle();          // The style of the radius caps
        [SerializeField] GizmoCapStyle          mRotationCapStyle       = new GizmoCapStyle();          // The style of the handle used to rotate the objects within the radius of influence

        [SerializeField] float                  mRadiusSnap             = defaultRadiusSnap;            // Radius increment used when snapping is enabled
        [SerializeField] float                  mHeightSnap             = defaultHeightSnap;            // height increment used when snapping is enabled

        [SerializeField] int                    mHMoveLayerMask         = defaultHMoveLayerMask;        // Which object layers can be moved horizontally with the gizmo
        [SerializeField] int                    mVMoveLayerMask         = defaultVMoveLayerMask;        // Which object layers can be moved vertically with the gizmo
        [SerializeField] int                    mRotationLayerMask      = defaultRotationLayerMask;     // Which object layers can be rotated with the gizmo
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: radiusCircleColor (Public Property)
        // Desc: Returns or sets the radius circle color.
        //-----------------------------------------------------------------------------
        public Color                radiusCircleColor       { get { return mRadiusCircleColor; } set { mRadiusCircleColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: heightSliderStyle (Public Property)
        // Desc: Returns the height slider style.
        //-----------------------------------------------------------------------------
        public GizmoLineSliderStyle heightSliderStyle       { get { return mHeightSliderStyle; } }

        //-----------------------------------------------------------------------------
        // Name: heightCapStyle (Public Property)
        // Desc: Returns the height cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle        heightCapStyle          { get { return mHeightCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: centerCapStyle (Public Property)
        // Desc: Returns the center cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle        centerCapStyle          { get { return mCenterCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: radiusCapStyle (Public Property)
        // Desc: Returns the radius cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle        radiusCapStyle          { get { return mRadiusCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: rotationCapStyle (Public Property)
        // Desc: Returns the rotation cap style. This is the handle that is used to rotate
        //       the objects within the radius of influence.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle        rotationCapStyle        { get { return mRotationCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: radiusSnap (Public Property)
        // Desc: Returns or sets the radius increment used when snapping is enabled.
        //-----------------------------------------------------------------------------
        public float                radiusSnap              { get { return mRadiusSnap; } set { mRadiusSnap = Mathf.Max(value, 1e-4f); } }

        //-----------------------------------------------------------------------------
        // Name: heightSnap (Public Property)
        // Desc: Returns or sets the height increment used when snapping is enabled.
        //-----------------------------------------------------------------------------
        public float                heightSnap              { get { return mHeightSnap; } set { mHeightSnap = Mathf.Max(value, 1e-4f); } }
        
        //-----------------------------------------------------------------------------
        // Name: hMoveLayerMask (Public Property)
        // Desc: Returns or sets the horizontal movement layer mask. This controls which
        //       object layers can be moved horizontally with the gizmo. 
        //-----------------------------------------------------------------------------
        public int                  hMoveLayerMask          { get { return mHMoveLayerMask; } set { mHMoveLayerMask = value; } }

        //-----------------------------------------------------------------------------
        // Name: vMoveLayerMask (Public Property)
        // Desc: Returns or sets the vertical movement layer mask. This controls which
        //       object layers can be moved vertically with the gizmo. 
        //-----------------------------------------------------------------------------
        public int                  vMoveLayerMask          { get { return mVMoveLayerMask; } set { mVMoveLayerMask = value; } }

        //-----------------------------------------------------------------------------
        // Name: rotationLayerMask (Public Property)
        // Desc: Returns or sets the rotation layer mask. This controls which object layers
        //       can be rotated with the gizmo. 
        //-----------------------------------------------------------------------------
        public int                  rotationLayerMask       { get { return mRotationLayerMask; } set { mRotationLayerMask = value; } }
        #endregion

        #region Public Static Properties
        public static Color             defaultRadiusCircleColor    { get { return Color.white; } }
        public static Color             defaultHeightHandleColor    { get { return Color.red; } }
        public static Color             defaultCenterCapColor       { get { return Color.green; } }
        public static Color             defaultRadiusCapColor       { get { return Color.green; } }
        public static Color             defaultRotationCapColor     { get { return Color.green; } }

        public static EGizmoCapType     defaultHeightCapType        { get { return EGizmoCapType.Cone; } }
        public static EGizmoCapType     defaultCenterCapType        { get { return EGizmoCapType.Box; } }
        public static EGizmoCapType     defaultRadiusCapType        { get { return EGizmoCapType.Quad; } }

        public static float             defaultRotationCapRadius    { get { return 2.5f; } }

        public static float             defaultRadiusSnap           { get { return 0.1f; } }
        public static float             defaultHeightSnap           { get { return 0.1f; } }

        public static int               defaultHMoveLayerMask       { get { return ~0; } }
        public static int               defaultVMoveLayerMask       { get { return ~0; } }
        public static int               defaultRotationLayerMask    { get { return ~0; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public TerrainGizmoStyle CloneStyle()
        {
            var clone                   = MemberwiseClone() as TerrainGizmoStyle;
            clone.mHeightSliderStyle    = mHeightSliderStyle.CloneStyle();
            clone.mHeightCapStyle       = mHeightCapStyle.CloneStyle();
            clone.mCenterCapStyle       = mCenterCapStyle.CloneStyle();
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
            // Styles
            radiusCircleColor = defaultRadiusCircleColor;

            heightSliderStyle.UseDefaults();
            heightSliderStyle.color = defaultHeightHandleColor;

            heightCapStyle.UseDefaults();
            heightCapStyle.color    = defaultHeightHandleColor;
            heightCapStyle.capType  = defaultHeightCapType;

            centerCapStyle.UseDefaults();
            centerCapStyle.color    = defaultCenterCapColor;
            centerCapStyle.capType  = defaultCenterCapType;

            radiusCapStyle.UseDefaults();
            radiusCapStyle.color    = defaultRadiusCapColor;
            radiusCapStyle.capType  = defaultRadiusCapType;

            rotationCapStyle.UseDefaults();
            rotationCapStyle.color          = defaultRotationCapColor;
            rotationCapStyle.circleRadius   = defaultRotationCapRadius;
            rotationCapStyle.torusRadius    = defaultRotationCapRadius;
            rotationCapStyle.cylinderRadius = defaultRotationCapRadius;
            rotationCapStyle.capType        = EGizmoCapType.WireCircle;

            // Snapping
            radiusSnap      = defaultRadiusSnap;
            heightSnap      = defaultHeightSnap;

            // Objects
            hMoveLayerMask      = defaultHMoveLayerMask;
            vMoveLayerMask      = defaultVMoveLayerMask;
            rotationLayerMask   = defaultRotationLayerMask;
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
            EGizmoCapType   newCapType;
            float           newFloat;
            Color           newColor;
            bool            newBool;
            int             newInt;
            var             content = new GUIContent();

            // Circle radius color
            EditorGUILayout.Separator();
            content.text    = "Radius circle color";
            content.tooltip = "The color of the radius circle.";
            EditorGUI.BeginChangeCheck();
            newColor        = EditorGUILayout.ColorField(content, mRadiusCircleColor);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                mRadiusCircleColor = newColor;
            }

            // Height handles color
            content.text    = "Height handles color";
            content.tooltip = "The color used to draw the height handles.";
            EditorGUI.BeginChangeCheck();
            newColor        = EditorGUILayout.ColorField(content, mHeightCapStyle.color);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                mHeightCapStyle.color       = newColor;
                mHeightSliderStyle.color    = newColor;
            }

            // Rotation cap color
            content.text    = "Rotation cap color";
            content.tooltip = "The color of the rotation handle that is used to rotate the objects within the radius of influence.";
            EditorGUI.BeginChangeCheck();
            newColor        = EditorGUILayout.ColorField(content, mRotationCapStyle.color);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                mRotationCapStyle.color = newColor;
            }

            #region Height Slider
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Height Slider");
            {
                // Length
                content.text    = "Length";
                content.tooltip = "Slider length.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, mHeightSliderStyle.length);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightSliderStyle.length = newFloat;
                }

                // Start cutoff
                content.text    = "Start cutoff";
                content.tooltip = "How much to cut off from the start of the slider.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, mHeightSliderStyle.startCutoff);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightSliderStyle.startCutoff = newFloat;
                }

                // End cutoff
                content.text    = "End cutoff";
                content.tooltip = "How much to cut off from the end of the slider.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, mHeightSliderStyle.endCutoff);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightSliderStyle.endCutoff = newFloat;
                }

                // Slider type
                content.text    = "Slider type";
                content.tooltip = "Slider type.";
                EditorGUI.BeginChangeCheck();
                EGizmoLineSliderType newLineType = (EGizmoLineSliderType)EditorGUILayout.EnumPopup(content, mHeightSliderStyle.sliderType);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightSliderStyle.sliderType = newLineType;
                }

                // Thickness
                if (newLineType != EGizmoLineSliderType.Thin)
                {
                    content.text    = "Thickness";
                    content.tooltip = "Slider thickness when the slider type is not 'Thin'.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightSliderStyle.thickness);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightSliderStyle.thickness = newFloat;
                    }
                }

                // Visibility
                EditorGUILayout.Separator();
                content.text    = "Visible";
                content.tooltip = "Is the height slider visible?";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, mHeightSliderStyle.visible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightSliderStyle.visible = newBool;
                }
            }
            #endregion

            #region Height Cap
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Height Cap");
            {
                // Cap type
                content.text    = "Cap type";
                content.tooltip = "Height cap type.";
                EditorGUI.BeginChangeCheck();
                newCapType = (EGizmoCapType)EditorGUILayout.EnumPopup(content, mHeightCapStyle.capType, (e) => 
                {
                    EGizmoCapType capType = (EGizmoCapType)e;
                    return capType != EGizmoCapType.Torus && capType != EGizmoCapType.InsetCylinder;
                }, false);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightCapStyle.capType = newCapType;
                }

                // Check cap type
                if (newCapType == EGizmoCapType.Cone)
                {
                    // Cone radius
                    content.text    = "Radius";
                    content.tooltip = "The cone radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.coneRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.coneRadius = newFloat;
                    }

                    // Cone length
                    content.text    = "Length";
                    content.tooltip = "The cone length.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.coneLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.coneLength = newFloat;
                    }
                }
                else
                if (newCapType == EGizmoCapType.Pyramid || 
                    newCapType == EGizmoCapType.WirePyramid)
                {
                    // Base size
                    content.text    = "Base size";
                    content.tooltip = "The pyramid base size.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.pyramidBaseSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.pyramidBaseSize = newFloat;
                    }

                    // Length
                    content.text    = "Length";
                    content.tooltip = "The pyramid length (i.e. height).";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.pyramidLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.pyramidLength = newFloat;
                    }
                }
                else
                if (newCapType == EGizmoCapType.Box ||
                    newCapType == EGizmoCapType.WireBox)
                {
                    // Box size
                    content.text    = "Size";
                    content.tooltip = "The box size.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.boxWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.boxSize = Vector3Ex.FromValue(newFloat);
                    }
                }
                else
                if (newCapType == EGizmoCapType.Quad ||
                    newCapType == EGizmoCapType.WireQuad)
                {
                    // Quad size
                    content.text    = "Size";
                    content.tooltip = "The quad size.";
                    EditorGUI.BeginChangeCheck();
                    newFloat = EditorGUILayout.FloatField(content, mHeightCapStyle.quadWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.quadSize = Vector2Ex.FromValue(newFloat);
                    }
                }
                else
                if (newCapType == EGizmoCapType.Sphere)
                {
                    // Sphere radius
                    content.text    = "Radius";
                    content.tooltip = "The sphere radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.sphereRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.sphereRadius = newFloat;
                    }
                }
                else
                if (newCapType == EGizmoCapType.Circle || 
                    newCapType == EGizmoCapType.WireCircle ||
                    newCapType == EGizmoCapType.InsetCircle)
                {
                    // Circle radius
                    content.text    = "Radius";
                    content.tooltip = "The circle radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.circleRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mHeightCapStyle.circleRadius = newFloat;
                    }

                    // Inset circle?
                    if (newCapType == EGizmoCapType.InsetCircle)
                    {
                        // Circle thickness
                        content.text    = "Thickness";
                        content.tooltip = "The circle thickness.";
                        EditorGUI.BeginChangeCheck();
                        newFloat        = EditorGUILayout.FloatField(content, mHeightCapStyle.insetCircleThickness);
                        if (EditorGUI.EndChangeCheck())
                        {
                            parentObject.OnWillChangeInEditor();
                            mHeightCapStyle.insetCircleThickness = newFloat;
                        }
                    }
                }

                // Visibility
                EditorGUILayout.Separator();
                content.text    = "Visible";
                content.tooltip = "Is the height cap visible?";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, mHeightCapStyle.visible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mHeightCapStyle.visible = newBool;
                }
            }
            #endregion

            // Caps
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mCenterCapStyle, "Center Cap", string.Empty, true, false, parentObject);
            EditorGUILayout.Separator();
            GizmoCapStyle.DrawNonDirectionalCapUI(mRadiusCapStyle, "Radius Caps", string.Empty, true, false, parentObject);

            // Rotation cap
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Rotation Cap");
            {
                // Radius
                content.text    = "Radius";
                content.tooltip = "The circle radius.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, mRotationCapStyle.circleRadius);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mRotationCapStyle.circleRadius      = newFloat;
                    mRotationCapStyle.torusRadius       = newFloat;
                    mRotationCapStyle.cylinderRadius    = newFloat;
                }
            }

            // Snapping
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Snapping");
            {
                // Radius snap
                content.text    = "Radius snap";
                content.tooltip = "Radius increment used when snapping is enabled.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, radiusSnap);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    radiusSnap = newFloat;
                }

                // Height snap
                content.text    = "Height snap";
                content.tooltip = "Height increment used when snapping is enabled.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, heightSnap);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    heightSnap = newFloat;
                }
            }

            // Objects
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Objects");
            {
                // Horizontal move layers
                content.text    = "Horizontal move layers";
                content.tooltip = "Controls which object layers can be moved horizontally with the gizmo.";
                EditorGUI.BeginChangeCheck();
                newInt          = EditorUI.LayerMaskField(content, hMoveLayerMask);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    hMoveLayerMask = newInt;
                }

                // Vertical move layers
                content.text    = "Vertical move layers";
                content.tooltip = "Controls which object layers can be moved vertically with the gizmo.";
                EditorGUI.BeginChangeCheck();
                newInt          = EditorUI.LayerMaskField(content, vMoveLayerMask);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    vMoveLayerMask = newInt;
                }

                // Rotation layers
                content.text    = "Rotation layers";
                content.tooltip = "Controls which object layers can be rotated with the gizmo.";
                EditorGUI.BeginChangeCheck();
                newInt          = EditorUI.LayerMaskField(content, rotationLayerMask);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rotationLayerMask = newInt;
                }
            }
        }
        #endif
        #endregion
    }
    #endregion
}