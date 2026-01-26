using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CameraGizmo (Public Class)
    // Desc: Implements a camera gizmo.
    //-----------------------------------------------------------------------------
    public class CameraGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args

        GizmoCap    mNearPlaneQuad      = new GizmoCap();   // Near plane quad cap
        GizmoCap    mFarPlaneQuad       = new GizmoCap();   // Far plane quad cap

        GizmoCap    mNearPlaneCap       = new GizmoCap();   // Near plane cap used to change the camera near plane
        GizmoCap    mFarPlaneCap        = new GizmoCap();   // Far plane cap used to change the camera far plane

        GizmoCap[]  mFarVolumeCapsP     = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };   // Positive X and Y far plane volume caps
        GizmoCap[]  mFarVolumeCapsN     = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };   // Negative X and Y far plane volume caps

        GizmoCap    mViewSnapCap        = new GizmoCap();   // View snap cap

        float       mDragStartPlaneDistance;                // Distance of near or far clipping plane when the user starts dragging
        float       mDragStarArcLength;                     // Used when the user starts dragging the volume handles to change the FOV. When 
                                                            // changing the FOV, we will drag the angle length instead (perspective camera only).
        float       mDragStartHOrthoSize;                   // Used when the user starts dragging the volume handles to change the ortho size
                                                            // and it represents half the camera ortho size (horz or vertical depends on the handles).

        // Buffers used to avoid memory allocations
        List<Segment>   mNearSegmentBuffer = new List<Segment>();
        List<Segment>   mFarSegmentBuffer  = new List<Segment>();
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the camera gizmo style.
        //-----------------------------------------------------------------------------
        public CameraGizmoStyle style            { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's camera gizmo style.
        //-----------------------------------------------------------------------------
        public CameraGizmoStyle activeStyle      { get { return style == null ? RTGizmos.get.skin.cameraGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target camera.
        //-----------------------------------------------------------------------------
        public Camera target { get; set; }
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: IsReady() (Protected Function)
        // Desc: Checks if the gizmo is ready to use.
        // Rtrn: True if the gizmo is ready to use and false otherwise.
        //-----------------------------------------------------------------------------
        protected override bool IsReady()
        {
            return target != null && target.gameObject.activeInHierarchy;
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
            // Store caps
            handles.Add(mNearPlaneCap);
            handles.Add(mFarPlaneCap);
            handles.Add(mViewSnapCap);

            // Store near/far caps
            int count = mFarVolumeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mFarVolumeCapsP[i]);
                handles.Add(mFarVolumeCapsN[i]);
            }

            // Store rectangle caps and init
            handles.Add(mNearPlaneQuad);
            handles.Add(mFarPlaneQuad);

            mNearPlaneQuad.hoverable            = false;
            mNearPlaneQuad.capStyle.capType     = EGizmoCapType.WireQuad;
            mNearPlaneQuad.alignFlatToCamera    = false;
            mNearPlaneQuad.zoomScaleMode        = EGizmoHandleZoomScaleMode.One;
            mNearPlaneQuad.transformScaleMode   = EGizmoHandleTransformScaleMode.One;

            mFarPlaneQuad.hoverable             = false;
            mFarPlaneQuad.capStyle.capType      = EGizmoCapType.WireQuad;
            mFarPlaneQuad.alignFlatToCamera     = false;
            mFarPlaneQuad.zoomScaleMode         = EGizmoHandleZoomScaleMode.One;
            mFarPlaneQuad.transformScaleMode    = EGizmoHandleTransformScaleMode.One;
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
            // Ensure the camera is using valid clip planes
            target.EnsureValidClipPlanes();

            // Update styles
            mNearPlaneCap.capStyle  = activeStyle.planeCapStyle;
            mFarPlaneCap.capStyle   = activeStyle.planeCapStyle;
            mViewSnapCap.capStyle   = activeStyle.viewSnapCapStyle;

            int count = mFarVolumeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                mFarVolumeCapsP[i].capStyle     = activeStyle.volumeCapStyle;
                mFarVolumeCapsN[i].capStyle     = activeStyle.volumeCapStyle;
            }

            // What kind of camera are we?
            if (target.orthographic)
            {
                // Calculate the size of the near and far plane rectangles
                mNearPlaneQuad.capStyle.quadHeight      = target.orthographicSize * 2.0f;
                mNearPlaneQuad.capStyle.quadWidth       = mNearPlaneQuad.capStyle.quadHeight * target.aspect;
                mFarPlaneQuad.capStyle.quadHeight       = target.orthographicSize * 2.0f;
                mFarPlaneQuad.capStyle.quadWidth        = mFarPlaneQuad.capStyle.quadHeight * target.aspect;
            }
            else
            {
                // Calculate data
                float tan = Mathf.Tan(target.fieldOfView * 0.5f * Mathf.Deg2Rad);

                // Calculate the size of the near and far plane rectangles
                mNearPlaneQuad.capStyle.quadHeight      = 2.0f * tan * target.nearClipPlane;
                mNearPlaneQuad.capStyle.quadWidth       = mNearPlaneQuad.capStyle.quadHeight * target.aspect;
                mFarPlaneQuad.capStyle.quadHeight       = 2.0f * tan * target.farClipPlane;
                mFarPlaneQuad.capStyle.quadWidth        = mFarPlaneQuad.capStyle.quadHeight * target.aspect;
            }

            // Update transforms
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            mNearPlaneQuad.transform.position   = target.transform.position + target.transform.forward * target.nearClipPlane;
            mFarPlaneQuad.transform.position    = target.transform.position + target.transform.forward * target.farClipPlane;

            mNearPlaneCap.transform.position    = mNearPlaneQuad.transform.position;
            mFarPlaneCap.transform.position     = mFarPlaneQuad.transform.position;

            // Note: If the view snap cap is being dragged, let the drag op control its position.
            if (dragData.handle != mViewSnapCap)
                mViewSnapCap.transform.position     = mNearPlaneQuad.transform.position + target.transform.forward * activeStyle.viewSnapRayLength;

            count = mFarVolumeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                mFarVolumeCapsP[i].transform.position = mFarPlaneQuad.transform.position + transform.GetAxis(i) * mFarPlaneQuad.capStyle.quadSize[i] * 0.5f;
                mFarVolumeCapsN[i].transform.position = mFarPlaneQuad.transform.position - transform.GetAxis(i) * mFarPlaneQuad.capStyle.quadSize[i] * 0.5f;
            }

            // Update drag
            mDrag.scaleSensitivity          = 1.0f;
            mDrag.scaleSensitivitySource    = EGizmoDragSettingSource.Custom;
            mDrag.scaleSnapStepSource       = EGizmoDragSettingSource.Custom;
            mDrag.ignoreZoomScale           = true;
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

            // Draw the snap ray
            RTGizmos.DrawSegment(mNearPlaneQuad.transform.position, mViewSnapCap.transform.position, activeStyle.viewSnapRayColor);

            // Create 2 quads which represent the near and far planes and extract their edges.
            // We will use the edges to connect the near and far plane corners.
            Quad nearQuad = new Quad();
            nearQuad.Set(mNearPlaneQuad.transform.position, mNearPlaneQuad.capStyle.quadWidth, 
                mNearPlaneQuad.capStyle.quadHeight, mNearPlaneQuad.transform.forward, mNearPlaneQuad.transform.right);
            Quad farQuad = new Quad();
            farQuad.Set(mFarPlaneQuad.transform.position, mFarPlaneQuad.capStyle.quadWidth, 
                mFarPlaneQuad.capStyle.quadHeight, mFarPlaneQuad.transform.forward, mFarPlaneQuad.transform.right);
            nearQuad.CalculateEdges(mNearSegmentBuffer);
            farQuad.CalculateEdges(mFarSegmentBuffer);

            // Connect the near and far plane quad corners
            int count = mNearSegmentBuffer.Count;
            for (int i = 0; i < count; ++i)
                RTGizmos.DrawSegment(mNearSegmentBuffer[i].start, mFarSegmentBuffer[i].start, activeStyle.color);

            // Draw plane quads
            mNearPlaneQuad.Render(mHRenderArgs);
            mFarPlaneQuad.Render(mHRenderArgs);

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.Add(mNearPlaneCap);
            mCapsBuffer.Add(mFarPlaneCap);
            mCapsBuffer.Add(mViewSnapCap);
            mCapsBuffer.AddRange(mFarVolumeCapsP);
            mCapsBuffer.AddRange(mFarVolumeCapsN);
            GizmoHandle.ZSortHandles(mCapsBuffer, mHRenderArgs, mSortedCapsBuffer);

            // Render sorted caps
            count = mSortedCapsBuffer.Count;
            for (int i = 0; i < count; ++i)
                mSortedCapsBuffer[i].Render(mHRenderArgs);
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

            // Match near/far plane caps
            if (dragHandle == mNearPlaneCap || 
                dragHandle == mFarPlaneCap)
            {
                // Set drag start data
                mDragStartPlaneDistance = dragHandle == mNearPlaneCap ? target.nearClipPlane : target.farClipPlane;

                // Start dragging
                mDrag.scaleSnap = activeStyle.planeSnap;
                mDrag.snapMode  = EGizmoDragSnapMode.UseGlobal;
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = target.transform.forward;
                desc.axisIndex0 = 2;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;   
            }
            // Match view snap cap
            else 
            if (dragHandle == mViewSnapCap)
            {
                // Start dragging
                desc.dragType   = EGizmoDragType.SurfaceSnap;
                return mDrag.Start(desc, this, mViewSnapCap.transform, camera) ? mDrag : null;   
            }
            // Volume caps?
            else 
            if (ArrayHasDragHandle(mFarVolumeCapsN) || 
                ArrayHasDragHandle(mFarVolumeCapsP))
            {
                // Perspective or ortho camera?
                if (!target.orthographic)
                {
                    // Set drag start data.
                    // Note: We want to account for the aspect ratio if we're dragging the horizontal handles.
                    float halfFOV = target.fieldOfView / 2.0f;
                    if (dragHandle == mFarVolumeCapsN[0] || dragHandle == mFarVolumeCapsP[0])
                    {
                        float halfWidth = (mFarVolumeCapsN[0].transform.position - mFarVolumeCapsP[0].transform.position).magnitude / 2.0f;
                        halfFOV = Mathf.Atan2(halfWidth, target.farClipPlane) * Mathf.Rad2Deg;
                    }
                    mDragStarArcLength = MathEx.DegreesToArcLength(halfFOV, target.farClipPlane);
                }
                else
                {
                    // Set drag start data.
                    // Note: We want to account for the aspect ratio if we're dragging the horizontal handles.
                    mDragStartHOrthoSize = target.orthographicSize;
                    if (dragHandle == mFarVolumeCapsN[0] || dragHandle == mFarVolumeCapsP[0])
                        mDragStartHOrthoSize *= target.aspect;
                }

                // Start dragging
                mDrag.snapMode  = EGizmoDragSnapMode.Disabled;      // No snapping for the FOV angle
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragHandle.transform.position - (target.transform.position + target.transform.forward * target.farClipPlane); // Vector connecting the point in the middle of the far plane to the drag handle position
                desc.axisIndex0 = 0;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;  
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
            // Cache data
            var dragHandle = dragData.handle;

            // Are we dragging the near or far clipping plane?
            if (dragHandle == mNearPlaneCap)
            {
                RTUndo.get.Record(target);
                target.nearClipPlane = mDragStartPlaneDistance + drag.totalDrag[drag.desc.axisIndex0];

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.nearClipPlane = MathEx.FixFloatError(target.nearClipPlane);
            }
            else if (dragHandle == mFarPlaneCap)
            {
                RTUndo.get.Record(target);
                target.farClipPlane = mDragStartPlaneDistance + drag.totalDrag[drag.desc.axisIndex0];

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.farClipPlane = MathEx.FixFloatError(target.farClipPlane);
            }
            // View snap cap?
            else if (dragHandle == mViewSnapCap)
            {
                RTUndo.get.Record(target.transform);
                target.transform.rotation = QuaternionEx.LookRotationEx((mViewSnapCap.transform.position - target.transform.position), Vector3.up);
            }
            // Field of view/Ortho size?
            else
            if (ArrayHasDragHandle(mFarVolumeCapsN) ||
                ArrayHasDragHandle(mFarVolumeCapsP))
            {
                // Perspective or ortho camera?
                if (!target.orthographic)
                {
                    // Calculate the new arc length
                    float arcLength = mDragStarArcLength + mDrag.totalDrag[mDrag.desc.axisIndex0];

                    // Recalculate the FOV angle
                    float fov = 2.0f * MathEx.ArcLengthToDegrees(arcLength, target.farClipPlane);

                    // If we're dragging the horizontal handles, we need to account for the aspect ratio
                    if (dragData.handle == mFarVolumeCapsP[0] ||
                        dragData.handle == mFarVolumeCapsN[0])
                    {
                        // Unity expects vertical FOV
                        float tan           = Mathf.Tan(fov / 2.0f * Mathf.Deg2Rad);
                        float halfFOV       = Mathf.Atan2(tan, target.aspect);
                        fov                 = 2.0f * halfFOV * Mathf.Rad2Deg;
                    }

                    // Set camera field of view
                    RTUndo.get.Record(target);
                    target.fieldOfView = fov;
                }
                else
                {
                    // Calculate the new ortho size and take aspect ratio into account if necessary
                    float orthoSize = mDragStartHOrthoSize + mDrag.totalDrag[mDrag.desc.axisIndex0];
                    if (dragData.handle == mFarVolumeCapsP[0] ||
                        dragData.handle == mFarVolumeCapsN[0])
                    {
                        // Unity expects a vertical ortho size
                        orthoSize /= target.aspect;
                    }

                    // Set camera ortho size
                    RTUndo.get.Record(target);
                    target.orthographicSize = orthoSize;
                }
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
            if (dragging)
            {
                // Cache data
                var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
                var dragHandle  = dragData.handle;

                // Near clip plane?
                if (dragHandle == mNearPlaneCap)
                    globalStyle.GUILabel_AABBTop(mNearPlaneCap.CalculateAABB(camera), camera, "Near: " + target.nearClipPlane.ToString("F3"), EGizmoTextType.DragInfo);
                // Far clip plane?
                else if (dragHandle == mFarPlaneCap)
                    globalStyle.GUILabel_AABBTop(mFarPlaneCap.CalculateAABB(camera), camera, "Far: " + target.farClipPlane.ToString("F3"), EGizmoTextType.DragInfo);
                // View snap cap?
                else if (dragHandle == mViewSnapCap)
                    globalStyle.GUILabel_AABBTop(mViewSnapCap.CalculateAABB(camera), camera, "Direction: " + target.transform.forward.ToString("F3"), EGizmoTextType.DragInfo);
            }
        }
        #endregion
    }
    #endregion
}

