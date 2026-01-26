using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: ExtrudeGizmoStyle (Public Class)
    // Desc: Stores style properties for extrude gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class ExtrudeGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle[]            mExtrudeCapStylesP      = new GizmoCapStyle[3] { new GizmoCapStyle(), new GizmoCapStyle(), new GizmoCapStyle() };   // Extrude cap styles for the positive axes
        [SerializeField] GizmoCapStyle[]            mExtrudeCapStylesN      = new GizmoCapStyle[3] { new GizmoCapStyle(), new GizmoCapStyle(), new GizmoCapStyle() };   // Extrude cap styles for the negative axes
        [SerializeField] GizmoPlaneSliderStyle[]    mDblAxisSliderStyles    = new GizmoPlaneSliderStyle[3] { new GizmoPlaneSliderStyle(), new GizmoPlaneSliderStyle(), new GizmoPlaneSliderStyle() };   // Styles for the dbl-axis sliders used to extrude along 2 axes at once
        
        [SerializeField] Color  mBoxColor           = defaultBoxColor;          // Wireframe box color
        [SerializeField] Color  mGhostBoxColor      = defaultGhostBoxColor;     // Wireframe ghost box color
        [SerializeField] float  mExtrudeCapOffset   = defaultExtrudeCapOffset;  // Offset to apply to the extrude caps
        [SerializeField] float  mDblAxisOffset      = defaultDblAxisOffset;     // Offset to apply to the dbl-axis sliders to move them away from the gizmo origin
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: boxColor (Public Property)
        // Desc: Returns or sets the box color.
        //-----------------------------------------------------------------------------
        public Color    boxColor        { get { return mBoxColor; } set { mBoxColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: ghostBoxColor (Public Property)
        // Desc: Returns or sets the ghost box color.
        //-----------------------------------------------------------------------------
        public Color    ghostBoxColor   { get { return mGhostBoxColor; } set { mGhostBoxColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: extrudeCapOffset (Public Property)
        // Desc: Returns or sets the extrude cap offset.
        //-----------------------------------------------------------------------------
        public float    extrudeCapOffset    { get { return mExtrudeCapOffset; } set { mExtrudeCapOffset = value; } }

        //-----------------------------------------------------------------------------
        // Name: dblAxisOffset (Public Property)
        // Desc: Returns or sets the double-axis slider offset from the gizmo origin.
        //-----------------------------------------------------------------------------
        public float    dblAxisOffset       { get { return mDblAxisOffset; } set { mDblAxisOffset = value; } }
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultExtrudeCapType   { get { return EGizmoCapType.Pyramid; } }
        public static Color         defaultBoxColor         { get { return new Color(1.0f, 1.0f, 1.0f, 0.9f); } }
        public static Color         defaultGhostBoxColor    { get { return new Color(0.0f, 1.0f, 0.0f, 0.8f); } }
        public static float         defaultExtrudeCapOffset { get { return 0.7f; } }
        public static float         defaultDblAxisOffset    { get { return 0.0f; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public ExtrudeGizmoStyle CloneStyle()
        {
            // Clone
            var clone = MemberwiseClone() as ExtrudeGizmoStyle;
            
            // Clone styles
            int count = mExtrudeCapStylesP.Length;
            for (int i = 0; i < count; ++i)
            {
                clone.mExtrudeCapStylesP[i] = mExtrudeCapStylesP[i].CloneStyle();
                clone.mExtrudeCapStylesN[i] = mExtrudeCapStylesN[i].CloneStyle();
            }

            count = mDblAxisSliderStyles.Length;
            for (int i = 0; i < count; ++i)
                clone.mDblAxisSliderStyles[i] = mDblAxisSliderStyles[i].CloneStyle();

            // Return clone
            return clone;
        }

        //-----------------------------------------------------------------------------
        // Name: GetExtrudeCapStyle() (Public Function)
        // Desc: Returns the extrude cap style for the specified axis.
        // Parm: axis     - Axis index: (0 = X, 1 = Y, 2 = Z).
        //       positive - True if a positive axis cap should be returned and false
        //                  otherwise.
        // Rtrn: The extrude cap style for the specified axis.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle GetExtrudeCapStyle(int axis, bool positive)
        {
            return positive ? mExtrudeCapStylesP[axis] : mExtrudeCapStylesN[axis];
        }

        //-----------------------------------------------------------------------------
        // Name: GetDblAxisSliderStyle() (Public Function)
        // Desc: Returns the double-axis slider style for the specified plane.
        // Parm: plane - Slider plane.
        // Rtrn: The slider style for the specified plane.
        //-----------------------------------------------------------------------------
        public GizmoPlaneSliderStyle GetDblAxisSliderStyle(EPlane plane)
        {
            return mDblAxisSliderStyles[(int)plane];
        }
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: OnUseDefaults() (Protected Function)
        // Desc: Called in order to set the style properties to default values.
        //-----------------------------------------------------------------------------
        protected override void OnUseDefaults()
        {
            // Extrude caps
            int axisSliderCount = mExtrudeCapStylesP.Length;
            for (int i = 0; i < axisSliderCount; ++i)
            {
                mExtrudeCapStylesP[i].UseDefaults();
                mExtrudeCapStylesN[i].UseDefaults();
                mExtrudeCapStylesP[i].capType = defaultExtrudeCapType;
                mExtrudeCapStylesN[i].capType = defaultExtrudeCapType;
            }
            extrudeCapOffset = defaultExtrudeCapOffset;

            // Dbl-axis sliders
            for (int i = 0; i < mDblAxisSliderStyles.Length; ++i)
                mDblAxisSliderStyles[i].UseDefaults();

            boxColor        = defaultBoxColor;
            ghostBoxColor   = defaultGhostBoxColor;
            dblAxisOffset   = defaultDblAxisOffset;
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
            float   newFloat;
            Color   newColor;
            bool    newBool;
            Vector2 newSize2;
            Vector3 newSize3;

            var content             = new GUIContent();
            int extrudeCapCount     = mExtrudeCapStylesP.Length;
            int dblAxisSliderCount  = mDblAxisSliderStyles.Length;

            // Box color
            EditorGUILayout.Separator();
            content.text    = "Box color";
            content.tooltip = "The box color.";
            EditorGUI.BeginChangeCheck();
            newColor = EditorGUILayout.ColorField(content, boxColor);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                boxColor = newColor;
            }

            // Ghost box color
            content.text    = "Ghost box color";
            content.tooltip = "The color used to render the ghost box during extrusion. "
                            + "The ghost box previews the volume that will be filled with new objects "
                            + "when the user releases the handle.";
            EditorGUI.BeginChangeCheck();
            newColor = EditorGUILayout.ColorField(content, ghostBoxColor);
            if (EditorGUI.EndChangeCheck())
            {
                parentObject.OnWillChangeInEditor();
                ghostBoxColor = newColor;
            }

            #region Extrude Caps
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Extrude Caps");
            {
                // Offset
                content.text    = "Offset";
                content.tooltip = "How much offset to apply to the extrude caps to move them away from the box faces.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, extrudeCapOffset);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    extrudeCapOffset = newFloat;
                }

                // Cap type
                content.text    = "Cap type";
                content.tooltip = "Specifies the type of cap used by the extrusion handles.";
                EditorGUI.BeginChangeCheck();
                var newCapType = (EGizmoCapType)EditorGUILayout.EnumPopup(content, mExtrudeCapStylesP[0].capType, (e) =>
                {
                    EGizmoCapType capType = (EGizmoCapType)e;
                    return
                        capType == EGizmoCapType.Cone ||
                        capType == EGizmoCapType.Pyramid ||
                        capType == EGizmoCapType.WirePyramid ||
                        capType == EGizmoCapType.Box ||
                        capType == EGizmoCapType.WireBox ||
                        capType == EGizmoCapType.Quad ||
                        capType == EGizmoCapType.WireQuad ||
                        capType == EGizmoCapType.Sphere ||
                        capType == EGizmoCapType.Circle ||
                        capType == EGizmoCapType.WireCircle ||
                        capType == EGizmoCapType.Sphere;
                }, false);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    for (int i = 0; i < extrudeCapCount; ++i)
                    {
                        mExtrudeCapStylesP[i].capType = newCapType;
                        mExtrudeCapStylesN[i].capType = newCapType;
                    }
                }
                
                // Check cap type
                if (newCapType == EGizmoCapType.Cone)
                {
                    // Cone radius
                    content.text    = "Radius";
                    content.tooltip = "The cone radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].coneRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].coneRadius = newFloat;
                            mExtrudeCapStylesN[i].coneRadius = newFloat;
                        }
                    }

                    // Cone length
                    content.text    = "Length";
                    content.tooltip = "The cone length.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].coneLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].coneLength = newFloat;
                            mExtrudeCapStylesN[i].coneLength = newFloat;
                        }
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
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].pyramidBaseSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].pyramidBaseSize = newFloat;
                            mExtrudeCapStylesN[i].pyramidBaseSize = newFloat;
                        }
                    }

                    // Length
                    content.text    = "Length";
                    content.tooltip = "The pyramid length (i.e. height).";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].pyramidLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].pyramidLength = newFloat;
                            mExtrudeCapStylesN[i].pyramidLength = newFloat;
                        }
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
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].boxWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        newSize3 = Vector3Ex.FromValue(newFloat);
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].boxSize = newSize3;
                            mExtrudeCapStylesN[i].boxSize = newSize3;
                        }
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
                    newFloat = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].quadWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        newSize2 = Vector2Ex.FromValue(newFloat);
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].quadSize = newSize2;
                            mExtrudeCapStylesN[i].quadSize = newSize2;
                        }
                    }
                }
                else
                if (newCapType == EGizmoCapType.Sphere)
                {
                    // Sphere radius
                    content.text    = "Radius";
                    content.tooltip = "The sphere radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].sphereRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].sphereRadius = newFloat;
                            mExtrudeCapStylesN[i].sphereRadius = newFloat;
                        }
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
                    newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].circleRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < extrudeCapCount; ++i)
                        {
                            mExtrudeCapStylesP[i].circleRadius = newFloat;
                            mExtrudeCapStylesN[i].circleRadius = newFloat;
                        }
                    }

                    // Inset circle?
                    if (newCapType == EGizmoCapType.InsetCircle)
                    {
                        // Circle thickness
                        content.text    = "Thickness";
                        content.tooltip = "The circle thickness.";
                        EditorGUI.BeginChangeCheck();
                        newFloat        = EditorGUILayout.FloatField(content, mExtrudeCapStylesP[0].insetCircleThickness);
                        if (EditorGUI.EndChangeCheck())
                        {
                            parentObject.OnWillChangeInEditor();
                            for (int i = 0; i < extrudeCapCount; ++i)
                            {
                                mExtrudeCapStylesP[i].insetCircleThickness = newFloat;
                                mExtrudeCapStylesN[i].insetCircleThickness = newFloat;
                            }
                        }
                    }
                }
                
                // Positive cap visibility
                EditorGUILayout.Separator();
                EditorUI.PushLabelWidth(10.0f);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < extrudeCapCount; ++i)
                {
                    content.text    = "+" + Core.axisNames[i];
                    content.tooltip = "Is the positive " + Core.axisNames[i] + " cap visible?";
                    EditorGUI.BeginChangeCheck();
                    newBool         = EditorGUILayout.ToggleLeft(content, mExtrudeCapStylesP[i].visible, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mExtrudeCapStylesP[i].visible = newBool;
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Negative cap visibility
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < extrudeCapCount; ++i)
                {
                    content.text    = "-" + Core.axisNames[i];
                    content.tooltip = "Is the negative " + Core.axisNames[i] + " cap visible?";
                    EditorGUI.BeginChangeCheck();
                    newBool         = EditorGUILayout.ToggleLeft(content, mExtrudeCapStylesN[i].visible, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mExtrudeCapStylesN[i].visible = newBool;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.PopLabelWidth();
            }
            #endregion

            #region Double-Axis Sliders
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Dbl-Axis Sliders");
            {               
                // Quad size
                content.text    = "Size";
                content.tooltip = "Plane slider size.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, mDblAxisSliderStyles[0].quadWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    for (int i = 0; i < dblAxisSliderCount; ++i)
                    {
                        mDblAxisSliderStyles[i].quadWidth  = newFloat;
                        mDblAxisSliderStyles[i].quadHeight = newFloat;
                    }
                }

                // Offset
                content.text    = "Offset";
                content.tooltip = "How much offset to apply to the dbl-axis sliders to move them away from the gizmo origin.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, dblAxisOffset);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    dblAxisOffset = newFloat;
                }

                // Border type
                content.text    = "Border type";
                content.tooltip = "Border type.";
                EditorGUI.BeginChangeCheck();
                EGizmoPlaneSliderBorderType newBorderType = (EGizmoPlaneSliderBorderType)EditorGUILayout.EnumPopup(content, mDblAxisSliderStyles[0].borderType);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    for (int i = 0; i < dblAxisSliderCount; ++i)
                        mDblAxisSliderStyles[i].borderType = newBorderType;
                }

                // Check border type
                if (newBorderType == EGizmoPlaneSliderBorderType.Thick ||
                    newBorderType == EGizmoPlaneSliderBorderType.WireThick)
                {
                    // Border width
                    content.text    = "Width";
                    content.tooltip = "The border width.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mDblAxisSliderStyles[0].quadBorderWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < dblAxisSliderCount; ++i)
                            mDblAxisSliderStyles[i].quadBorderWidth = newFloat;
                    }

                    // Border height
                    content.text    = "Height";
                    content.tooltip = "The border height.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mDblAxisSliderStyles[0].quadBorderHeight);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < dblAxisSliderCount; ++i)
                            mDblAxisSliderStyles[i].quadBorderHeight = newFloat;
                    }
                }

                // Plane visibility
                EditorGUILayout.Separator();
                content.text    = "Plane";
                content.tooltip = "Is the slider plane visible?";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, mDblAxisSliderStyles[0].planeVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    for (int i = 0; i < dblAxisSliderCount; ++i)
                        mDblAxisSliderStyles[i].planeVisible = newBool;
                }

                // Border visibility
                content.text    = "Border";
                content.tooltip = "Is the slider border visible?";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, mDblAxisSliderStyles[0].borderVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    for (int i = 0; i < dblAxisSliderCount; ++i)
                        mDblAxisSliderStyles[i].borderVisible = newBool;
                }

                // Dbl-axis visibility
                EditorUI.PushLabelWidth(10.0f);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < dblAxisSliderCount; ++i)
                {
                    content.text    = Core.planeNames[i];
                    content.tooltip = "Is the " + Core.planeNames[i] + " slider visible?";
                    EditorGUI.BeginChangeCheck();
                    newBool         = EditorGUILayout.ToggleLeft(content, mDblAxisSliderStyles[i].visible, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mDblAxisSliderStyles[i].visible = newBool;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.PopLabelWidth();
            }
            #endregion
        }
        #endif
        #endregion
    }
    #endregion
}
