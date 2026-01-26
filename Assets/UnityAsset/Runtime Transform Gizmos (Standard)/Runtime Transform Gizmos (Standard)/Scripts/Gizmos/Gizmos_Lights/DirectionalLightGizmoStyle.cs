using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: DirectionalLightGizmoStyle (Public Class)
    // Desc: Stores style properties for directional light gizmos.
    //-----------------------------------------------------------------------------
    [Serializable] public class DirectionalLightGizmoStyle : GizmoStyle
    {
        #region Private Fields
        [SerializeField] GizmoCapStyle  mEmitterCapStyle            = new GizmoCapStyle();              // Emitter cap style
        [SerializeField] int            mRayCount                   = defaultRayCount;                  // Number of rays emanating from the emitter
        [SerializeField] float          mRayLength                  = defaultRayLength;                 // Ray length
        [SerializeField] float          mSnapRayLength              = defaultSnapRayLength;             // Snap ray length

        [SerializeField] float          mRayRotationSpeed           = defaultRayRotationSpeed;          // Ray rotation speed
        [SerializeField] float          mRayLengthPulseSpeed        = defaultRayLengthPulseSpeed;       // Ray length pulsating animation speed
        [SerializeField] float          mRayLengthPulseAmplitude    = defaultRayLengthPulseAmplitude;   // Ray length pulse amplitude. This is the maximum amount that can be added to or subtracted from the ray length.

        [SerializeField] bool           mDirectionaLabelVisible     = defaultDirectionLabelVisible;     // Is the direction label visible?
        [SerializeField] bool           mTargetLabelVisible         = defaultTargetLabelVisible;        // Is the look-at target label visible?
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: emitterCapStyle (Public Property)
        // Desc: Returns the emitter cap style.
        //-----------------------------------------------------------------------------
        public GizmoCapStyle    emitterCapStyle     { get { return mEmitterCapStyle; } }

        //-----------------------------------------------------------------------------
        // Name: rayCount (Public Property)
        // Desc: Returns or sets the ray count. This is the number of rays emanating from
        //       the emitter surface.
        //-----------------------------------------------------------------------------
        public int              rayCount            { get { return mRayCount; } set { mRayCount = Mathf.Clamp(value, 3, 30); } }

        //-----------------------------------------------------------------------------
        // Name: rayLength (Public Property)
        // Desc: Returns or sets the ray length. These are the rays that emanate from
        //       the emitter surface.
        //-----------------------------------------------------------------------------
        public float            rayLength           { get { return mRayLength; } set { mRayLength = Mathf.Max(value, 0.0f); } }

        //-----------------------------------------------------------------------------
        // Name: snapRayLength (Public Property)
        // Desc: Returns or sets the snap ray length. This ray is capped by a handle
        //       that can be used to snap the light's direction to look at the cursor
        //       pick point.
        //-----------------------------------------------------------------------------
        public float            snapRayLength       { get { return mSnapRayLength; } set { mSnapRayLength = Mathf.Max(value, 0.0f); } }      

        //-----------------------------------------------------------------------------
        // Name: rayRotationSpeed (Public Property)
        // Desc: Returns or sets the ray rotation speed.
        //-----------------------------------------------------------------------------
        public float            rayRotationSpeed    { get { return mRayRotationSpeed; } set { mRayRotationSpeed = value; } }
        
        //-----------------------------------------------------------------------------
        // Name: rayLengthPulseSpeed (Public Property)
        // Desc: Returns or sets the ray length pulse speed.
        //-----------------------------------------------------------------------------
        public float            rayLengthPulseSpeed         { get { return mRayLengthPulseSpeed; } set { mRayLengthPulseSpeed = value; } }

        //-----------------------------------------------------------------------------
        // Name: rayLengthPulseAmplitude (Public Property)
        // Desc: Returns or sets the ray length pulse amplitude.
        //-----------------------------------------------------------------------------
        public float            rayLengthPulseAmplitude     { get { return mRayLengthPulseAmplitude; } set { mRayLengthPulseAmplitude = Mathf.Max(value, 0.0f); } }
        
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
        public static float         defaultEmitterRadius            { get { return 1.5f; } }
        public static int           defaultRayCount                 { get { return 10; } }
        public static float         defaultRayLength                { get { return 5.0f; } }
        public static float         defaultSnapRayLength            { get { return 7.0f; } }

        public static float         defaultRayRotationSpeed         { get { return 5.0f; } }
        public static float         defaultRayLengthPulseSpeed      { get { return 2.5f; } }
        public static float         defaultRayLengthPulseAmplitude  { get { return 0.5f; } }

        public static bool          defaultDirectionLabelVisible    { get { return true; } }
        public static bool          defaultTargetLabelVisible       { get { return true; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneStyle() (Public Function)
        // Desc: Clones the style.
        // Rtrn: The cloned style.
        //-----------------------------------------------------------------------------
        public DirectionalLightGizmoStyle CloneStyle()
        {
            var clone               = MemberwiseClone() as DirectionalLightGizmoStyle;
            clone.mEmitterCapStyle  = mEmitterCapStyle.CloneStyle();
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
            mEmitterCapStyle.UseDefaults();
            mEmitterCapStyle.capType        = EGizmoCapType.WireCircle;
            mEmitterCapStyle.circleRadius   = defaultEmitterRadius;

            // Rays
            rayCount                = defaultRayCount;
            rayLength               = defaultRayLength;
            snapRayLength           = defaultSnapRayLength;

            // Animation
            rayRotationSpeed        = defaultRayRotationSpeed;
            rayLengthPulseSpeed     = defaultRayLengthPulseSpeed;
            rayLengthPulseAmplitude = defaultRayLengthPulseAmplitude;

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
            int             newInt;
            bool            newBool;
            var             content = new GUIContent();

            #region Emitter
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Emitter");
            {
                // Radius
                content.text    = "Radius";
                content.tooltip = "Emitter radius.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mEmitterCapStyle.circleRadius);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    mEmitterCapStyle.circleRadius = newFloat;
                }
            }
            #endregion

            #region Rays
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Rays");
            {
                // Ray count
                content.text    = "Count";
                content.tooltip = "The number of rays emanating from the surface.";
                EditorGUI.BeginChangeCheck();
                newInt = EditorGUILayout.IntField(content, mRayCount);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rayCount = newInt;
                }

                // Ray length
                content.text    = "Length";
                content.tooltip = "Ray length.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mRayLength);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rayLength = newFloat;
                }

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

            #region Animation
            EditorGUILayout.Separator();
            EditorUI.SectionTitleLabel("Animation");
            {
                // Ray rotation speed
                content.text    = "Ray rotation speed";
                content.tooltip = "Ray rotation speed in degrees per second. If > 0, the rays will rotate around the light direction vector.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mRayRotationSpeed);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rayRotationSpeed = newFloat;
                }

                // Ray length pulse speed
                content.text    = "Ray length pulse speed";
                content.tooltip = "Ray length pulse speed.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mRayLengthPulseSpeed);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rayLengthPulseSpeed = newFloat;
                }

                // Ray length pulse amplitude
                content.text    = "Ray length pulse amplitude";
                content.tooltip = "Ray length pulse amplitude. This is the maximum amount that can be added to or subtracted from the ray length.";
                EditorGUI.BeginChangeCheck();
                newFloat = EditorGUILayout.FloatField(content, mRayLengthPulseAmplitude);
                if (EditorGUI.EndChangeCheck())
                {
                    parentObject.OnWillChangeInEditor();
                    rayLengthPulseAmplitude = newFloat;
                }
            }
            #endregion

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
