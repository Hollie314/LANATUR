using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: Tutorial_ObjectTransformGizmoTarget (Public Class)
    // Desc: Implements the 'IRTObjectTransformGizmoTarget' interface. This interface
    //       defines event handlers that are fired when a gizmo transforms a game object.
    //       See the interface comments for detailed behavior. This class allows for fine-
    //       grained customization of how objects respond to transformation actions.
    //       In order to use the interface we have to attach it as a 'MonoBehaviour' to
    //       a game object.
    //-----------------------------------------------------------------------------
    class Tutorial_ObjectTransformGizmoTarget : MonoBehaviour, IRTObjectTransformGizmoTarget
    {
        #region Public Functions
        // Fired when a game object is about to be transformed. This is essentially the gizmo's
        // way of asking for permission to transform the object.
        public bool CanTransform(EGizmoDragChannel channel, ObjectTransformGizmo transformGizmo)
        {
            // In this demo, we will allow the red cube to be moved, the green cube to be rotated and the
            // blue cube to be scaled. We will filter the objects by name here, but you could use any criteria
            // you see fit (e.g. layers, tags or anything else).
            if (gameObject.name == "Cube_Red")      return channel == EGizmoDragChannel.Position;   // The red cube can only change its position
            if (gameObject.name == "Cube_Green")    return channel == EGizmoDragChannel.Rotation;   // The green cube can only change its rotation     
            if (gameObject.name == "Cube_Blue")     return channel == EGizmoDragChannel.Scale;      // The blue cube can only change its scale

            // Any other object can be freely transformed without restriction
            return true;
        }   

        // Fired after the object is transformed. Nothing to do here in this demo.
        public void OnTransformed(EGizmoDragChannel channel, ObjectTransformGizmo transformGizmo)
        {
        }

        // This handler allows us to change the offset that is applied to an object's position when it is being moved
        public Vector3 UpdateMoveOffset(Vector3 moveOffset, ObjectTransformGizmo transformGizmo)
        {
            // If we're moving the red cube, cancel the Y offset. This won't allow it to move vertically.
            if (gameObject.name == "Cube_Red") moveOffset.y = 0.0f;

            // Return the updated move offset
            return moveOffset;
        }
        #endregion
    }

    //-----------------------------------------------------------------------------
    // Name: Tutorial_3_IRTObjectTransformGizmoTarget (Public Class)
    // Desc: Tutorial class which demonstrates the usage of the 'IRTObjectTransformGizmoTarget'
    //       interface which allows objects to be notified about different transform
    //       events.
    //-----------------------------------------------------------------------------
    public class Tutorial_3_IRTObjectTransformGizmoTarget : MonoBehaviour
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
            // Collect gizmo targets and attach the 'Tutorial_ObjectTransformGizmoTarget' Mono
            var targets = new List<GameObject>{ GameObject.Find("Cube_Red"), GameObject.Find("Cube_Green"), GameObject.Find("Cube_Blue") };
            int count = targets.Count;
            for (int i = 0; i < count; ++i)
                targets[i].AddComponent<Tutorial_ObjectTransformGizmoTarget>();

            // Create TRS gizmo
            TRSGizmo trsGizmo = RTGizmos.get.CreateObjectTRSGizmo();
            trsGizmo.objectTransformGizmo.SetTargets(targets);
            trsGizmo.objectTransformGizmo.pivot = EGizmoPivot.Center;   // Let's make it sit at the center

            // Let's also create a view gizmo
            RTGizmos.get.CreateViewGizmo(RTCamera.get.settings.targetCamera);
        }
        #endregion
    }
    #endregion
}