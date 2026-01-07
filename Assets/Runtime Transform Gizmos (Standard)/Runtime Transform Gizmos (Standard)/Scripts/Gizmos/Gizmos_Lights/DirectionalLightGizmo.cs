using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: DirectionalLightGizmo (Public Class)
    // Desc: Implements a directional light gizmo.
    //-----------------------------------------------------------------------------
    public class DirectionalLightGizmo : Gizmo
    {
        #region Private Fields
        GizmoHandleRenderArgs   mHRenderArgs            = new GizmoHandleRenderArgs();  // Gizmo handle render args
        GizmoCap                mEmitterCap             = new GizmoCap();               // The emitter cap where rays emanate from
        GizmoCap                mSnapCap                = new GizmoCap();               // The snap cap used to snap the light direction to the cursor pick point
        GizmoDrag               mDrag                   = new GizmoDrag();              // Drag operation

        float                   mRayRotation            = 0.0f;                         // Animated ray rotation value
        float                   mLengthSineOffset       = 0.0f;                         // Animated ray length sine offset used to animate a sine wave
        float[]                 mRandomLengthSineOffsets;                               // Array that stores random sine offsets for each ray in addition to 'mLengthSineOffset'.
                                                                                        // This allows for different rays to animate differently. Otherwise, they would all be in sync.

        Light                   mTarget;    // The target light

        // Buffers used to avoid memory allocations
        List<Vector3>           mRayOriginBuffer    = new List<Vector3>();
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the directional light gizmo style.
        //-----------------------------------------------------------------------------
        public DirectionalLightGizmoStyle style         { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's directional light
        //       gizmo style.
        //-----------------------------------------------------------------------------
        public DirectionalLightGizmoStyle activeStyle   { get { return style == null ? RTGizmos.get.skin.directionalLightGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target light.
        //-----------------------------------------------------------------------------
        public Light target { get { return mTarget; } set { mTarget = value; } }
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: IsReady() (Protected Function)
        // Desc: Checks if the gizmo is ready to use.
        // Rtrn: True if the gizmo is ready to use and false otherwise.
        //-----------------------------------------------------------------------------
        protected override bool IsReady()
        {
            return mTarget != null && mTarget.gameObject.activeInHierarchy;
        }

        //-----------------------------------------------------------------------------
        // Name: GetActiveGizmoStyle() (Protected Function)
        // Desc: Returns the active gizmo style.
        //-----------------------------------------------------------------------------
        protected override GizmoStyle GetActiveGizmoStyle()
        {
            return activeStyle;
        }

        //-----------------------------------------------------------------------------
        // Name: OnCreateHandles() (Protected Function)
        // Desc: Called during gizmo initialization to allow the gizmo to create all the
        //       handles that it needs. On function return, these handles will be made
        //       children of the gizmo transform.
        // Parm: handles - Returns the list of handles.
        //-----------------------------------------------------------------------------
        protected override void OnCreateHandles(List<GizmoHandle> handles)
        {
            // Store handles
            handles.Add(mEmitterCap);
            handles.Add(mSnapCap);

            // Init handles
            mEmitterCap.hoverable           = false;
            mEmitterCap.alignFlatToCamera   = false;
        }

        //-----------------------------------------------------------------------------
        // Name: OnUpdateHandles() (Protected Function)
        // Desc: Called to allow the gizmo to update its handles. This could mean setting
        //       handle styles, updating their transforms etc. This function is called
        //       once for each camera that renders the gizmo. It is also called once during
        //       the update step to allow for correct interaction between the active camera
        //       and the gizmo handles.
        // Parm: updateReason  - The reason behind the update request.
        //       camera        - The camera that interacts with or renders the gizmo.
        //-----------------------------------------------------------------------------
        protected override void OnUpdateHandles(EGizmoHandleUpdateReason updateReason, Camera camera)
        {
            // Set styles
            var globalStyle             = RTGizmos.get.skin.globalGizmoStyle;
            mEmitterCap.capStyle        = activeStyle.emitterCapStyle;
            mEmitterCap.capStyle.color  = globalStyle.lightColor;
            mSnapCap.capStyle           = globalStyle.lightSnapCapStyle;

            // Update transforms
            transform.position          = mTarget.transform.position;
            transform.rotation          = mTarget.transform.rotation;
            mSnapCap.transform.rotation = Quaternion.identity;

            // Update snap cap transform if we're not dragging. If we're dragging, we let the drag control the
            // position of the cap and snap the light direction vector.
            if (dragData.handle != mSnapCap)
            {
                // Update snap cap transform.
                // Note: Use the emitter cap to finalize the ray length. Using the snap cap doesn't
                //       yield the expected result because the snap cap position depends on the ray
                //       length which in turn is scaled by the zoom scale that depends on the snap
                //       cap position.
                float snapRayLength             = mEmitterCap.FVal(activeStyle.snapRayLength, camera);
                mSnapCap.transform.position     = mTarget.transform.position + mTarget.transform.forward * snapRayLength;
            }
     
            // Animate rays
            if (updateReason == EGizmoHandleUpdateReason.Update)
            {
                // Do we need to rotate the rays?
                if (activeStyle.rayRotationSpeed != 0.0f)
                {
                    mRayRotation += activeStyle.rayRotationSpeed * Time.smoothDeltaTime;
                    mRayRotation %= 360.0f;
                }

                // Animate the ray length parameters
                mLengthSineOffset += activeStyle.rayLengthPulseSpeed * Time.smoothDeltaTime;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnRender() (Protected Function)
        // Desc: Called when the gizmo must be rendered.
        // Parm: camera    - The camera which renders the gizmo.
        //       rasterCtx - The raster graph context.
        //-----------------------------------------------------------------------------
        protected override void OnRender(Camera camera, RasterGraphContext rasterCtx)
        {
            // Fill render args
            mHRenderArgs.camera    = camera;
            mHRenderArgs.rasterCtx = rasterCtx;

            // Draw snap ray
            RTGizmos.DrawSegment(mTarget.transform.position, mSnapCap.transform.position, 
                RTGizmos.get.skin.globalGizmoStyle.lightSnapRayColor);

            // Render caps
            mEmitterCap.Render(mHRenderArgs);
            mSnapCap.Render(mHRenderArgs);

            // Generate the ray origins along the circle circumference around the light's Z direction.
            var circle = new Circle();
            circle.Set( mTarget.transform.position,
                        mEmitterCap.FVal(mEmitterCap.capStyle.circleRadius, camera),
                        mTarget.transform.right, mTarget.transform.forward);
            Geometry.GenerateCirclePoints(circle, activeStyle.rayCount, mRayOriginBuffer);

            // Generate random sine offsets used to animate the ray lengths
            if (mRandomLengthSineOffsets == null || mRandomLengthSineOffsets.Length != mRayOriginBuffer.Count)
            {
                mRandomLengthSineOffsets = new float[mRayOriginBuffer.Count];
                for (int i = 0; i < mRandomLengthSineOffsets.Length; ++i)
                    mRandomLengthSineOffsets[i] = Random.Range(0.0f, 360.0f * Mathf.Deg2Rad);
            }

            // Draw rays
            float lengthAmplitude   = activeStyle.rayLengthPulseAmplitude;
            int count               = mRayOriginBuffer.Count;
            Quaternion rayRotation  = Quaternion.AngleAxis(mRayRotation, mTarget.transform.forward);
            for (int i = 0; i < count; ++i)
            {
                // Rotate ray origin around circle center
                Vector3 toRayorigin     = rayRotation * (mRayOriginBuffer[i] - circle.center);
                mRayOriginBuffer[i]     = circle.center + toRayorigin;

                // Update the ray length by applying animations. If the length is negative, it means the
                // ray is shooting behind. Let's stop this from happening.
                float finalRayLength    = activeStyle.rayLength + lengthAmplitude * Mathf.Sin(mLengthSineOffset + mRandomLengthSineOffsets[i]);
                if (finalRayLength < 0.0f) finalRayLength = 0.0f;
                finalRayLength          = mEmitterCap.FVal(finalRayLength, camera);

                // Draw
                RTGizmos.DrawSegment(mRayOriginBuffer[i], 
                    mRayOriginBuffer[i] + mTarget.transform.forward * finalRayLength, 
                    RTGizmos.get.skin.globalGizmoStyle.lightColor);
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnStartDrag() (Protected Function)
        // Desc: Called to notify the gizmo that a drag operation can start.
        // Parm: camera - The camera that interacts with the gizmo.
        // Rtrn: An instance 'GizmoDrag' if the drag operation can start and null otherwise.
        //-----------------------------------------------------------------------------
        protected override GizmoDrag OnStartDrag(Camera camera)
        {
            // Cache data
            GizmoHandle dragHandle  = dragData.handle;
            GizmoDragDesc desc      = new GizmoDragDesc();

            // Match snap handle
            if (dragHandle == mSnapCap)
            {
                // Start dragging
                desc.dragType   = EGizmoDragType.SurfaceSnap;
                return mDrag.Start(desc, this, mSnapCap.transform, camera) ? mDrag : null; 
            }

            // No drag
            return null;
        }

        //-----------------------------------------------------------------------------
        // Name: OnDrag() (Protected Function)
        // Desc: Called when the gizmo is dragged.
        // Parm: drag - The active drag operation.
        //-----------------------------------------------------------------------------
        protected override void OnDrag(GizmoDrag drag)
        {
            // Snap direction
            if (dragData.handle == mSnapCap)
            {
                RTUndo.get.Record(mTarget.transform);
                mTarget.transform.rotation = QuaternionEx.LookRotationEx((mSnapCap.transform.position - mTarget.transform.position), Vector3.up);
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnGUI() (Protected Function)
        // Desc: Called during the 'OnGUI' event to allow the gizmo to draw GUI elements.
        // Parm: camera - The camera that renders the GUI.
        //-----------------------------------------------------------------------------
        protected override void OnGUI(Camera camera)
        {
            // Are we dragging?
            if (dragData.handle == mSnapCap)
            {
                if (activeStyle.directionLabelVisible)
                {
                    // Draw label at the top of the emitter cap. This label shows the light direction.
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(mEmitterCap.CalculateAABB(camera),
                        camera, "Direction: " + mTarget.transform.forward.ToString("F3"), EGizmoTextType.DragInfo);
                }

                if (activeStyle.targetLabelVisible)
                {
                    // Draw another label at the top of the snap cap. This shows the position of the snap
                    // cap or what the light is looking at.
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(mSnapCap.CalculateAABB(camera),
                        camera, "Target: " + mSnapCap.transform.position.ToString("F3"), EGizmoTextType.DragInfo);
                }
            }
        }
        #endregion

    }
    #endregion
}