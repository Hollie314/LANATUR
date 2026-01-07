using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: Tutorial_0_Creating_Gizmos (Public Class)
    // Desc: Tutorial class which demonstrates the creation and initialization of
    //       different kinds of gizmos.
    //-----------------------------------------------------------------------------
    public class Tutorial_0_Creating_Gizmos : MonoBehaviour
    {
        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: Awake() (Private Function)
        // Desc: Called by Unity to allow the object to initialize itself.
        //-----------------------------------------------------------------------------
        void Awake()
        {
            // When the app starts, the plugin needs to initialize itself. So we will
            // have to register a handler for the 'initialized' event. When this event
            // is fired, we know for sure the plugin is initialized and ready to use.
            RTG.get.initialized += OnRTGInit;
        }

        //-----------------------------------------------------------------------------
        // Name: OnRTGInit() (Private Function)
        // Desc: Event handler for the 'RTG.initialized' event.
        //-----------------------------------------------------------------------------
        void OnRTGInit()
        {
            // Let's create transform gizmos which allow us to move, rotate and scale objects
            MoveGizmo   moveGizmo   = RTGizmos.get.CreateObjectMoveGizmo();
            RotateGizmo rotateGizmo = RTGizmos.get.CreateObjectRotateGizmo();
            ScaleGizmo  scaleGizmo  = RTGizmos.get.CreateObjectScaleGizmo();
            TRSGizmo    trsGizmo    = RTGizmos.get.CreateObjectTRSGizmo();

            // Now that we have created the gizmos, we have to assign objects to them. The gizmos
            // that we have created above have a property called 'objectTransformGizmo'. We will
            // use this property's 'SetTarget' function to assign objects to the gizmos.
            moveGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Move"));
            rotateGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Rotate"));
            scaleGizmo.objectTransformGizmo.SetTarget(GameObject.Find("Cube_Scale"));

            // For the TRS gizmo, we will use a different function called 'SetTargets', the plural
            // form of the previous function. This is useful when you have a list of objects that
            // the gizmo can transform. If you are developing a runtime editor, this is most likely
            // what you will end up using. In this little tutorial, we will simply hard-code the list
            // but in a later tutorial, we will build a simple selection manager which allows you to
            // pick objects from the scene and update the list accordingly.
            trsGizmo.objectTransformGizmo.SetTargets(new List<GameObject> 
            { GameObject.Find("Cube_TRS0"), GameObject.Find("Cube_TRS1"), GameObject.Find("Cube_TRS2"), });

            // Let's also set the TRS gizmo's pivot to 'Center' so that it will sit in the middle of the targets.
            trsGizmo.objectTransformGizmo.pivot = EGizmoPivot.Center;

            // Let's also create some light, collider and other types gizmos
            DirectionalLightGizmo       dirLightGizmo               = RTGizmos.get.CreateGizmo<DirectionalLightGizmo>();
            PointLightGizmo             pointLightGizmo             = RTGizmos.get.CreateGizmo<PointLightGizmo>();
            SpotLightGizmo              spotLightGizmo              = RTGizmos.get.CreateGizmo<SpotLightGizmo>();
            BoxColliderGizmo            boxColliderGizmo            = RTGizmos.get.CreateGizmo<BoxColliderGizmo>();
            SphereColliderGizmo         sphereColliderGizmo         = RTGizmos.get.CreateGizmo<SphereColliderGizmo>();
            CapsuleColliderGizmo        capsuleColliderGizmo        = RTGizmos.get.CreateGizmo<CapsuleColliderGizmo>();
            CharacterControllerGizmo    characterControllerGizmo    = RTGizmos.get.CreateGizmo<CharacterControllerGizmo>();
            ExtrudeGizmo                extrudeGizmo                = RTGizmos.get.CreateGizmo<ExtrudeGizmo>();
            BoxScaleGizmo               boxScaleGizmo               = RTGizmos.get.CreateGizmo<BoxScaleGizmo>();
            AudioReverbZoneGizmo        audioReverbZoneGizmo        = RTGizmos.get.CreateGizmo<AudioReverbZoneGizmo>();
            AudioSourceGizmo            audioSourceGizmo            = RTGizmos.get.CreateGizmo<AudioSourceGizmo>();
            BoxCollider2DGizmo          boxCollider2DGizmo          = RTGizmos.get.CreateGizmo<BoxCollider2DGizmo>();
            CircleCollider2DGizmo       circleCollider2DGizmo       = RTGizmos.get.CreateGizmo<CircleCollider2DGizmo>();
            CapsuleCollider2DGizmo      capsuleCollider2DGizmo      = RTGizmos.get.CreateGizmo<CapsuleCollider2DGizmo>();
            CameraGizmo                 cameraGizmo                 = RTGizmos.get.CreateGizmo<CameraGizmo>();

            // We use the 'target' property to assign the target lights and colliders. Other
            // gizmos, such as the ExtrudeGizmo, use the 'SetTargets' function instead.
            dirLightGizmo.target            = GameObject.Find("Directional Light").GetLight();
            pointLightGizmo.target          = GameObject.Find("Point Light").GetLight();
            spotLightGizmo.target           = GameObject.Find("Spot Light").GetLight();
            boxColliderGizmo.target         = GameObject.Find("BoxCollider").GetBoxCollider();
            sphereColliderGizmo.target      = GameObject.Find("SphereCollider").GetSphereCollider();
            capsuleColliderGizmo.target     = GameObject.Find("CapsuleCollider").GetCapsuleCollider();
            characterControllerGizmo.target = GameObject.Find("CharacterController").GetCharacterController();
            extrudeGizmo.SetTargets(new List<GameObject> { GameObject.Find("Cube_Extrude") });
            boxScaleGizmo.target            = GameObject.Find("Cube_BoxScale");
            audioReverbZoneGizmo.target     = GameObject.Find("AudioReverbZone").GetAudioReverbZone();
            audioSourceGizmo.target         = GameObject.Find("AudioSource").GetAudioSource();
            boxCollider2DGizmo.target       = GameObject.Find("BoxCollider2D").GetBoxCollider2D();
            circleCollider2DGizmo.target    = GameObject.Find("CircleCollider2D").GetCircleCollider2D();
            capsuleCollider2DGizmo.target   = GameObject.Find("CapsuleCollider2D").GetCapsuleCollider2D();
            cameraGizmo.target              = GameObject.Find("Camera").GetCamera();

            // Allow the extrude gizmo to take target orientation into account
            extrudeGizmo.extrudeSpace       = EGizmoExtrudeSpace.Local;

            // Finally, let's create a view gizmo (a.k.a scene gizmo)
            RTGizmos.get.CreateViewGizmo(RTCamera.get.settings.targetCamera);
        }
        #endregion
    }
    #endregion
}