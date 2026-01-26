using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RTGizmosSkin (Public Class)
    // Desc: Represents a gizmos skin which can be used by the gizmos engine when
    //       drawing gizmos. A skin is essentially a collection of style properties
    //       used to draw different kinds of gizmo handles.
    //-----------------------------------------------------------------------------
    [CreateAssetMenu(fileName = "RTGizmosSkin", menuName = "RTG/RTGizmosSkin", order = 1)]
    public class RTGizmosSkin : ScriptableObject
    {
        #region Private Enumerations
        //-----------------------------------------------------------------------------
        // Name: EGizmoType (Private Enum)
        // Desc: Defines different types of gizmos whose styles can be configured in the UI.
        //-----------------------------------------------------------------------------
        enum EGizmoType
        {
            // Basic
            Global = 0,
            View,
            Move,
            Rotate,
            Scale,
            TRS,
            DirectionalLight,
            PointLight,
            SpotLight,
            Terrain,
            BoxCollider,
            SphereCollider,
            CapsuleCollider,
            CharacterController,

            // Standard
            Extrude,
            BoxScale,
            AudioReverbZone,
            AudioSource,
            BoxCollider2D,
            CircleCollider2D,
            CapsuleCollider2D,
            Camera
        }
        #endregion

        #region Private Fields
        #if UNITY_EDITOR
        [SerializeField] EGizmoType         mGizmoType = EGizmoType.Global;     // The currently selected gizmo type
        #endif
        [SerializeField] bool               mInitialized;                       // Has the skin been initialized?

        [SerializeField] GlobalGizmoStyle               mGlobalGizmoStyle               = new GlobalGizmoStyle();               // Global gizmo style
        [SerializeField] ViewGizmoStyle                 mViewGizmoStyle                 = new ViewGizmoStyle();                 // View gizmo style
        [SerializeField] MoveGizmoStyle                 mMoveGizmoStyle                 = new MoveGizmoStyle();                 // Move gizmo style
        [SerializeField] RotateGizmoStyle               mRotateGizmoStyle               = new RotateGizmoStyle();               // Rotate gizmo style
        [SerializeField] ScaleGizmoStyle                mScaleGizmoStyle                = new ScaleGizmoStyle();                // Scale gizmo style
        [SerializeField] TRSGizmoStyle                  mTRSGizmoStyle                  = new TRSGizmoStyle();                  // TRS gizmo style
        [SerializeField] DirectionalLightGizmoStyle     mDirectionalLightGizmoStyle     = new DirectionalLightGizmoStyle();     // Directional light gizmo style
        [SerializeField] PointLightGizmoStyle           mPointLightGizmoStyle           = new PointLightGizmoStyle();           // Point light gizmo style
        [SerializeField] SpotLightGizmoStyle            mSpotLightGizmoStyle            = new SpotLightGizmoStyle();            // Spot light gizmo style
        [SerializeField] TerrainGizmoStyle              mTerrainGizmoStyle              = new TerrainGizmoStyle();              // Terrain gizmo style
        [SerializeField] BoxColliderGizmoStyle          mBoxColliderGizmoStyle          = new BoxColliderGizmoStyle();          // Box collider gizmo style
        [SerializeField] SphereColliderGizmoStyle       mSphereColliderGizmoStyle       = new SphereColliderGizmoStyle();       // Sphere collider gizmo style
        [SerializeField] CapsuleColliderGizmoStyle      mCapsuleColliderGizmoStyle      = new CapsuleColliderGizmoStyle();      // Capsule collider gizmo style
        [SerializeField] CharacterControllerGizmoStyle  mCharacterControllerGizmoStyle  = new CharacterControllerGizmoStyle();  // Character controller gizmo style
        [SerializeField] ExtrudeGizmoStyle              mExtrudeGizmoStyle              = new ExtrudeGizmoStyle();              // Extrude gizmo style
        [SerializeField] BoxScaleGizmoStyle             mBoxScaleGizmoStyle             = new BoxScaleGizmoStyle();             // Box scale gizmo style
        [SerializeField] AudioReverbZoneGizmoStyle      mAudioReverbZoneGizmoStyle      = new AudioReverbZoneGizmoStyle();      // Audio reverb zone gizmo style
        [SerializeField] AudioSourceGizmoStyle          mAudioSourceGizmoStyle          = new AudioSourceGizmoStyle();          // Audio source gizmo style
        [SerializeField] BoxCollider2DGizmoStyle        mBoxCollider2DGizmoStyle        = new BoxCollider2DGizmoStyle();        // Box collider 2D gizmo style
        [SerializeField] CircleCollider2DGizmoStyle     mCircleCollider2DGizmoStyle     = new CircleCollider2DGizmoStyle();     // Circle collider 2D gizmo style
        [SerializeField] CapsuleCollider2DGizmoStyle    mCapsuleCollider2DGizmoStyle    = new CapsuleCollider2DGizmoStyle();    // Capsule collider 2D gizmo style
        [SerializeField] CameraGizmoStyle               mCameraGizmoStyle               = new CameraGizmoStyle();               // Camera gizmo style
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: globalGizmoStyle (Public Property)
        // Desc: Returns the global gizmo style.
        //-----------------------------------------------------------------------------
        public GlobalGizmoStyle     globalGizmoStyle    { get { return mGlobalGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: viewGizmoStyle (Public Property)
        // Desc: Returns the view gizmo style.
        //-----------------------------------------------------------------------------
        public ViewGizmoStyle       viewGizmoStyle      { get { return mViewGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: moveGizmoStyle (Public Property)
        // Desc: Returns the move gizmo style.
        //-----------------------------------------------------------------------------
        public MoveGizmoStyle       moveGizmoStyle      { get { return mMoveGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: rotateGizmoStyle (Public Property)
        // Desc: Returns the rotate gizmo style.
        //-----------------------------------------------------------------------------
        public RotateGizmoStyle     rotateGizmoStyle    { get { return mRotateGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: scaleGizmoStyle (Public Property)
        // Desc: Returns the scale gizmo style.
        //-----------------------------------------------------------------------------
        public ScaleGizmoStyle      scaleGizmoStyle     { get { return mScaleGizmoStyle; } }
        
        //-----------------------------------------------------------------------------
        // Name: trsGizmoStyle (Public Property)
        // Desc: Returns the TRS gizmo style.
        //-----------------------------------------------------------------------------
        public TRSGizmoStyle        trsGizmoStyle       { get { return mTRSGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: directionalLightGizmoStyle (Public Property)
        // Desc: Returns the directional light gizmo style.
        //-----------------------------------------------------------------------------
        public DirectionalLightGizmoStyle   directionalLightGizmoStyle  { get { return mDirectionalLightGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: pointLightGizmoStyle (Public Property)
        // Desc: Returns the point light gizmo style.
        //-----------------------------------------------------------------------------
        public PointLightGizmoStyle         pointLightGizmoStyle        { get { return mPointLightGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: spotLightGizmoStyle (Public Property)
        // Desc: Returns the spot light gizmo style.
        //-----------------------------------------------------------------------------
        public SpotLightGizmoStyle          spotLightGizmoStyle         { get { return mSpotLightGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: terrainGizmoStyle (Public Property)
        // Desc: Returns the terrain gizmo style.
        //-----------------------------------------------------------------------------
        public TerrainGizmoStyle            terrainGizmoStyle           { get { return mTerrainGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: boxColliderGizmoStyle (Public Property)
        // Desc: Returns the box collider gizmo style.
        //-----------------------------------------------------------------------------
        public BoxColliderGizmoStyle        boxColliderGizmoStyle       { get { return mBoxColliderGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: sphereColliderGizmoStyle (Public Property)
        // Desc: Returns the sphere collider gizmo style.
        //-----------------------------------------------------------------------------
        public SphereColliderGizmoStyle     sphereColliderGizmoStyle    { get { return mSphereColliderGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: capsuleColliderGizmoStyle (Public Property)
        // Desc: Returns the capsule collider gizmo style.
        //-----------------------------------------------------------------------------
        public CapsuleColliderGizmoStyle        capsuleColliderGizmoStyle       { get { return mCapsuleColliderGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: characterControllerGizmoStyle (Public Property)
        // Desc: Returns the character controller gizmo style.
        //-----------------------------------------------------------------------------
        public CharacterControllerGizmoStyle    characterControllerGizmoStyle   { get { return mCharacterControllerGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: extrudeGizmoStyle (Public Property)
        // Desc: Returns the extrude gizmo style.
        //-----------------------------------------------------------------------------
        public ExtrudeGizmoStyle                extrudeGizmoStyle               { get { return mExtrudeGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: boxScaleGizmoStyle (Public Property)
        // Desc: Returns the box scale gizmo style.
        //-----------------------------------------------------------------------------
        public BoxScaleGizmoStyle               boxScaleGizmoStyle              { get { return mBoxScaleGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: audioReverbZoneGizmoStyle (Public Property)
        // Desc: Returns the audio reverb zone gizmo style.
        //-----------------------------------------------------------------------------
        public AudioReverbZoneGizmoStyle        audioReverbZoneGizmoStyle       { get { return mAudioReverbZoneGizmoStyle; } }
        
        //-----------------------------------------------------------------------------
        // Name: audioSourceGizmoStyle (Public Property)
        // Desc: Returns the audio source gizmo style.
        //-----------------------------------------------------------------------------
        public AudioSourceGizmoStyle            audioSourceGizmoStyle           { get { return mAudioSourceGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: boxCollider2DGizmoStyle (Public Property)
        // Desc: Returns the box collider 2D gizmo style.
        //-----------------------------------------------------------------------------
        public BoxCollider2DGizmoStyle          boxCollider2DGizmoStyle         { get { return mBoxCollider2DGizmoStyle; } }
        
        //-----------------------------------------------------------------------------
        // Name: circleCollider2DGizmoStyle (Public Property)
        // Desc: Returns the circle collider 2D gizmo style.
        //-----------------------------------------------------------------------------
        public CircleCollider2DGizmoStyle       circleCollider2DGizmoStyle      { get { return mCircleCollider2DGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: capsuleCollider2DGizmoStyle (Public Property)
        // Desc: Returns the capsule collider 2D gizmo style.
        //-----------------------------------------------------------------------------
        public CapsuleCollider2DGizmoStyle      capsuleCollider2DGizmoStyle     { get { return mCapsuleCollider2DGizmoStyle; } }

        //-----------------------------------------------------------------------------
        // Name: cameraGizmoStyle (Public Property)
        // Desc: Returns the camera gizmo style.
        //-----------------------------------------------------------------------------
        public CameraGizmoStyle                 cameraGizmoStyle                { get { return mCameraGizmoStyle; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: UseDefaults() (Public Function)
        // Desc: Use default settings.
        //-----------------------------------------------------------------------------
        public void UseDefaults()
        {
            globalGizmoStyle.UseDefaults();
            viewGizmoStyle.UseDefaults();
            moveGizmoStyle.UseDefaults();
            rotateGizmoStyle.UseDefaults();
            scaleGizmoStyle.UseDefaults();
            trsGizmoStyle.UseDefaults();
            directionalLightGizmoStyle.UseDefaults();
            pointLightGizmoStyle.UseDefaults();
            spotLightGizmoStyle.UseDefaults();
            terrainGizmoStyle.UseDefaults();
            boxColliderGizmoStyle.UseDefaults();
            sphereColliderGizmoStyle.UseDefaults();
            capsuleColliderGizmoStyle.UseDefaults();
            characterControllerGizmoStyle.UseDefaults();
            extrudeGizmoStyle.UseDefaults();
            boxScaleGizmoStyle.UseDefaults();
            audioReverbZoneGizmoStyle.UseDefaults();
            audioSourceGizmoStyle.UseDefaults();
            boxCollider2DGizmoStyle.UseDefaults();
            circleCollider2DGizmoStyle.UseDefaults();
            capsuleCollider2DGizmoStyle.UseDefaults();
            cameraGizmoStyle.UseDefaults();
        }

        #if UNITY_EDITOR
        //-----------------------------------------------------------------------------
        // Name: OnEditorGUI() (Public Function)
        // Desc: Draws the Editor UI which allows the user to change the skin properties.
        //-----------------------------------------------------------------------------
        public void OnEditorGUI()
        {
            // Toolbar button rows
            var buttonRows = new string[][]
            {
                new string[] { "Global", "View", "Move", "Rotate", "Scale", "TRS" },
                new string[] { "Dir. Light", "Point Light", "Spot Light", "Terrain", "Box Collider" },
                new string[] { "Sphere Collider", "Capsule Collider", "Character Controller" },
                new string[] { "Extrude Gizmo", "Box Scale Gizmo", "Audio Reverb Zone" },
                new string[] { "Audio Source", "Box Collider 2D", "Circle Collider 2D" },
                new string[] { "Capsule Collider 2D", "Camera" },
            };

            // Draw multi-row toolbar
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorUI.MultiRowToolbar((int)mGizmoType, buttonRows);
            if (EditorGUI.EndChangeCheck())
            {
                this.OnWillChangeInEditor();
                mGizmoType = (EGizmoType)newIndex;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // Draw selected gizmo style UI
                switch (mGizmoType)
                {
                    case EGizmoType.Global:

                        globalGizmoStyle.DrawEditorGUI(this, "Global Gizmo Style", true);
                        break;

                    case EGizmoType.View:

                        viewGizmoStyle.DrawEditorGUI(this, "View Gizmo Style", true);
                        break;

                    case EGizmoType.Move:

                        moveGizmoStyle.DrawEditorGUI(this, "Move Gizmo Style", true);
                        break;

                    case EGizmoType.Rotate:

                        rotateGizmoStyle.DrawEditorGUI(this, "Rotate Gizmo Style", true);
                        break;

                    case EGizmoType.Scale:

                        scaleGizmoStyle.DrawEditorGUI(this, "Scale Gizmo Style", true);
                        break;

                    case EGizmoType.TRS:

                        trsGizmoStyle.DrawEditorGUI(this, "TRS Gizmo Style", true);
                        break;

                    case EGizmoType.DirectionalLight:

                        directionalLightGizmoStyle.DrawEditorGUI(this, "Directional Light Gizmo Style", true);
                        break;

                    case EGizmoType.PointLight:

                        pointLightGizmoStyle.DrawEditorGUI(this, "Point Light Gizmo Style", true);
                        break;

                    case EGizmoType.SpotLight:

                        spotLightGizmoStyle.DrawEditorGUI(this, "Spot Light Gizmo Style", true);
                        break;

                    case EGizmoType.Terrain:

                        terrainGizmoStyle.DrawEditorGUI(this, "Terrain Gizmo Style", true);
                        break;

                    case EGizmoType.BoxCollider:

                        boxColliderGizmoStyle.DrawEditorGUI(this, "Box Collider Gizmo Style", true);
                        break;

                    case EGizmoType.SphereCollider:

                        sphereColliderGizmoStyle.DrawEditorGUI(this, "Sphere Collider Gizmo Style", true);
                        break;

                    case EGizmoType.CapsuleCollider:

                        capsuleColliderGizmoStyle.DrawEditorGUI(this, "Capsule Collider Gizmo Style", true);
                        break;

                    case EGizmoType.CharacterController:

                        characterControllerGizmoStyle.DrawEditorGUI(this, "Character Controller Gizmo Style", true);
                        break;

                    case EGizmoType.Extrude:

                        extrudeGizmoStyle.DrawEditorGUI(this, "Extrude Gizmo Style", true);
                        break;

                    case EGizmoType.BoxScale:

                        boxScaleGizmoStyle.DrawEditorGUI(this, "Box Scale Gizmo Style", true);
                        break;

                    case EGizmoType.AudioReverbZone:

                        audioReverbZoneGizmoStyle.DrawEditorGUI(this, "Audio Reverb Zone Gizmo Style", true);
                        break;

                    case EGizmoType.AudioSource:

                        audioSourceGizmoStyle.DrawEditorGUI(this, "Audio Source Gizmo Style", this);
                        break;

                    case EGizmoType.BoxCollider2D:

                        boxCollider2DGizmoStyle.DrawEditorGUI(this, "Box Collider 2D Gizmo Style", this);
                        break;

                    case EGizmoType.CircleCollider2D:

                        circleCollider2DGizmoStyle.DrawEditorGUI(this, "Circle Collider 2D Gizmo Style", this);
                        break;

                    case EGizmoType.CapsuleCollider2D:

                        capsuleCollider2DGizmoStyle.DrawEditorGUI(this, "Capsule Collider 2D Gizmo Style", this);
                        break;

                    case EGizmoType.Camera:

                        cameraGizmoStyle.DrawEditorGUI(this, "Camera Gizmo Style", this);
                        break;
                }
            }
        }
        #endif
        #endregion

        #region Private Functions
        #if UNITY_EDITOR
        //-----------------------------------------------------------------------------
        // Name: OnEnable() (Public Function)
        // Desc: Called when the object is enabled.
        //-----------------------------------------------------------------------------
        void OnEnable()
        {
            // Initialize if necessary
            if (!mInitialized)
            {
                UseDefaults();
                mInitialized = true;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: Reset() (Public Function)
        // Desc: Called the the object is reset.
        //-----------------------------------------------------------------------------
        void Reset()
        {
            UseDefaults();
        }
        #endif
        #endregion
    }
    #endregion
}
