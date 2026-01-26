using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxScaleGizmoStyle (Public Class)
    // Desc: Stores style properties for box scale gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class BoxScaleGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle[]        mAxisCapStylesP         = new GizmoCapStyle[3] { new GizmoCapStyle(), new GizmoCapStyle(), new GizmoCapStyle() };   // Axis cap styles for the positive axes
        [SerializeField] GizmoCapStyle[]        mAxisCapStylesN         = new GizmoCapStyle[3] { new GizmoCapStyle(), new GizmoCapStyle(), new GizmoCapStyle() };   // Axis cap styles for the negative axes
        [SerializeField] GizmoCapStyle          mUniformCapStyle        = new GizmoCapStyle();      // Uniform cap style

        [SerializeField] Color  mBoxColor       = defaultBoxColor;          // Wireframe box color
        [SerializeField] float  mAxisCapOffset  = defaultAxisCapOffset;     // Offset to apply to the axis caps
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: uniformCapStyle (Public Property)
        // Desc: Returns the uniform cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    uniformCapStyle { get { return mUniformCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: boxColor (Public Property)
        // Desc: Returns or sets the box color.
        //-----------------------------------------------------------------------------
        public Color    boxColor        { get { return mBoxColor; } set { mBoxColor = value; } }

        //-----------------------------------------------------------------------------
        // Name: axisCapOffset (Public Property)
        // Desc: Returns or sets the axis cap offset.
        //-----------------------------------------------------------------------------
        public float    axisCapOffset   { get { return mAxisCapOffset; } set { mAxisCapOffset = value; } }
        #endregion

        #region Public Static Properties
        public static EGizmoCapType defaultAxisCapType      { get { return EGizmoCapType.Box; } }
        public static EGizmoCapType defaultUniformCapType   { get { return EGizmoCapType.Box; } }
        public static Color         defaultBoxColor         { get { return new Color(1.0f, 1.0f, 1.0f, 0.9f); } }
        public static float         defaultAxisCapOffset    { get { return 0.0f; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public BoxScaleGizmoStyle CloneStyle()
        {
            // Clone
            var clone = MemberwiseClone() as BoxScaleGizmoStyle;
            
            // Clone styles
            int count = mAxisCapStylesP.Length;
            for (int i = 0; i < count; ++i)
            {
                clone.mAxisCapStylesP[i] = mAxisCapStylesP[i].CloneStyle();
                clone.mAxisCapStylesN[i] = mAxisCapStylesN[i].CloneStyle();
            }
            clone.mUniformCapStyle = mUniformCapStyle.CloneStyle();

            // Return clone
            return clone;
        }

        //-----------------------------------------------------------------------------
        // Name: GetAxisCapStyle() (Public Function)
        // Desc: Returns the axis cap style for the specified axis.
        // Parm: axis     - Axis index: (0 = X, 1 = Y, 2 = Z).
        //       positive - True if a positive axis cap should be returned and false
        //                  otherwise.
        // Rtrn: The axis cap style for the specified axis.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle GetAxisCapStyle(int axis, bool positive)
        {
            return positive ? mAxisCapStylesP[axis] : mAxisCapStylesN[axis];
        }
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: OnUseDefaults() (Protected Function)
        // Desc: Called in order to set the style properties to default values.
        //-----------------------------------------------------------------------------
        protected override void OnUseDefaults()
        {
            // Axis caps
            int axisSliderCount = mAxisCapStylesP.Length;
            for (int i = 0; i < axisSliderCount; ++i)
            {
                mAxisCapStylesP[i].UseDefaults();
                mAxisCapStylesN[i].UseDefaults();
                mAxisCapStylesP[i].capType = defaultAxisCapType;
                mAxisCapStylesN[i].capType = defaultAxisCapType;
            }

            // Uniform scale cap
            uniformCapStyle.UseDefaults();
            uniformCapStyle.capType = defaultUniformCapType;

            // Misc
            axisCapOffset   = defaultAxisCapOffset;
            boxColor        = defaultBoxColor;
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
            Color           newColor;
            bool            newBool;
            Vector2         newSize2;
            Vector3         newSize3;
            EGizmoCapType   newCapType;

            var content         = new GUIContent();
            int axisCapCount    = mAxisCapStylesP.Length;

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

            #region Axis Caps
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Axis Caps");
            {
                // Offset
                content.text    = "Offset";
                content.tooltip = "How much offset to apply to the axis caps to move them away from the box faces.";
                EditorGUI.BeginChangeCheck();
                newFloat        = EditorGUILayout.FloatField(content, axisCapOffset);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    axisCapOffset = newFloat;
                }

                // Cap type
                content.text    = "Cap type";
                content.tooltip = "Specifies the type of cap used by the axis handles.";
                EditorGUI.BeginChangeCheck();
                newCapType      = (EGizmoCapType)EditorGUILayout.EnumPopup(content, mAxisCapStylesP[0].capType, (e) =>
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
                    for (int i = 0; i < axisCapCount; ++i)
                    {
                        mAxisCapStylesP[i].capType = newCapType;
                        mAxisCapStylesN[i].capType = newCapType;
                    }
                }
                
                // Check cap type
                if (newCapType == EGizmoCapType.Cone)
                {
                    // Cone radius
                    content.text    = "Radius";
                    content.tooltip = "The cone radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].coneRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].coneRadius = newFloat;
                            mAxisCapStylesN[i].coneRadius = newFloat;
                        }
                    }

                    // Cone length
                    content.text    = "Length";
                    content.tooltip = "The cone length.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].coneLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].coneLength = newFloat;
                            mAxisCapStylesN[i].coneLength = newFloat;
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
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].pyramidBaseSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].pyramidBaseSize = newFloat;
                            mAxisCapStylesN[i].pyramidBaseSize = newFloat;
                        }
                    }

                    // Length
                    content.text    = "Length";
                    content.tooltip = "The pyramid length (i.e. height).";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].pyramidLength);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].pyramidLength = newFloat;
                            mAxisCapStylesN[i].pyramidLength = newFloat;
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
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].boxWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        newSize3 = Vector3Ex.FromValue(newFloat);
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].boxSize = newSize3;
                            mAxisCapStylesN[i].boxSize = newSize3;
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
                    newFloat = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].quadWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        newSize2 = Vector2Ex.FromValue(newFloat);
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].quadSize = newSize2;
                            mAxisCapStylesN[i].quadSize = newSize2;
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
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].sphereRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].sphereRadius = newFloat;
                            mAxisCapStylesN[i].sphereRadius = newFloat;
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
                    newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].circleRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        for (int i = 0; i < axisCapCount; ++i)
                        {
                            mAxisCapStylesP[i].circleRadius = newFloat;
                            mAxisCapStylesN[i].circleRadius = newFloat;
                        }
                    }

                    // Inset circle?
                    if (newCapType == EGizmoCapType.InsetCircle)
                    {
                        // Circle thickness
                        content.text    = "Thickness";
                        content.tooltip = "The circle thickness.";
                        EditorGUI.BeginChangeCheck();
                        newFloat        = EditorGUILayout.FloatField(content, mAxisCapStylesP[0].insetCircleThickness);
                        if (EditorGUI.EndChangeCheck())
                        {
                            parentObject.OnWillChangeInEditor();
                            for (int i = 0; i < axisCapCount; ++i)
                            {
                                mAxisCapStylesP[i].insetCircleThickness = newFloat;
                                mAxisCapStylesN[i].insetCircleThickness = newFloat;
                            }
                        }
                    }
                }
                
                // Positive cap visibility
                EditorGUILayout.Separator();
                EditorUI.PushLabelWidth(10.0f);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < axisCapCount; ++i)
                {
                    content.text    = "+" + Core.axisNames[i];
                    content.tooltip = "Is the positive " + Core.axisNames[i] + " cap visible?";
                    EditorGUI.BeginChangeCheck();
                    newBool         = EditorGUILayout.ToggleLeft(content, mAxisCapStylesP[i].visible, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mAxisCapStylesP[i].visible = newBool;
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Negative cap visibility
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < axisCapCount; ++i)
                {
                    content.text    = "-" + Core.axisNames[i];
                    content.tooltip = "Is the negative " + Core.axisNames[i] + " cap visible?";
                    EditorGUI.BeginChangeCheck();
                    newBool         = EditorGUILayout.ToggleLeft(content, mAxisCapStylesN[i].visible, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mAxisCapStylesN[i].visible = newBool;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.PopLabelWidth();
            }
            #endregion

            #region Uniform Scale Cap
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Uniform Scale Cap");
            {
                // Cap type
                content.text    = "Cap type";
                content.tooltip = "Uniform scale cap type.";
                EditorGUI.BeginChangeCheck();
                newCapType = (EGizmoCapType)EditorGUILayout.EnumPopup(content, mUniformCapStyle.capType, (e) => 
                {
                    EGizmoCapType capType = (EGizmoCapType)e;
                    return  capType == EGizmoCapType.Box || capType == EGizmoCapType.WireBox || capType == EGizmoCapType.Sphere ||
                            capType == EGizmoCapType.Quad || capType == EGizmoCapType.WireQuad ||
                            capType == EGizmoCapType.Circle || capType == EGizmoCapType.WireCircle || capType == EGizmoCapType.InsetCircle;
                }, false);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mUniformCapStyle.capType = newCapType;
                }

                // Check cap type
                if (newCapType == EGizmoCapType.Box ||
                    newCapType == EGizmoCapType.WireBox)
                {
                    // Box size
                    content.text    = "Size";
                    content.tooltip = "The box size.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mUniformCapStyle.boxWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mUniformCapStyle.boxSize = Vector3Ex.FromValue(newFloat);
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
                    newFloat        = EditorGUILayout.FloatField(content, mUniformCapStyle.quadWidth);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mUniformCapStyle.quadSize = Vector2Ex.FromValue(newFloat);
                    }
                }
                else
                if (newCapType == EGizmoCapType.Sphere)
                {
                    // Sphere radius
                    content.text    = "Radius";
                    content.tooltip = "The sphere radius.";
                    EditorGUI.BeginChangeCheck();
                    newFloat        = EditorGUILayout.FloatField(content, mUniformCapStyle.sphereRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mUniformCapStyle.sphereRadius = newFloat;
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
                    newFloat        = EditorGUILayout.FloatField(content, mUniformCapStyle.circleRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        parentObject.OnWillChangeInEditor();
                        mUniformCapStyle.circleRadius = newFloat;
                    }

                    // Inset circle?
                    if (newCapType == EGizmoCapType.InsetCircle)
                    {
                        // Circle thickness
                        content.text    = "Thickness";
                        content.tooltip = "The circle thickness.";
                        EditorGUI.BeginChangeCheck();
                        newFloat        = EditorGUILayout.FloatField(content, mUniformCapStyle.insetCircleThickness);
                        if (EditorGUI.EndChangeCheck())
                        {
                            parentObject.OnWillChangeInEditor();
                            mUniformCapStyle.insetCircleThickness = newFloat;
                        }
                    }
                }

                // Cap color
                content.text    = "Color";
                content.tooltip = "Uniform scale cap color.";
                EditorGUI.BeginChangeCheck();
                newColor        = EditorGUILayout.ColorField(content, mUniformCapStyle.color);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mUniformCapStyle.color = newColor;
                }

                // Cap visibility
                EditorGUILayout.Separator();
                content.text    = "Visible";
                content.tooltip = "Is the uniform scale cap visible?";
                EditorGUI.BeginChangeCheck();
                newBool = EditorGUILayout.ToggleLeft(content, mUniformCapStyle.visible);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mUniformCapStyle.visible = newBool;
                }
            }
            #endregion
        }
        #endif
        #endregion
    }
    #endregion
}
