using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: PointLightGizmo (Public Class)
    // Desc: Implements a point light gizmo.
    //-----------------------------------------------------------------------------
    public class PointLightGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag           = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs    = new GizmoHandleRenderArgs();                                          // Gizmo handle render args
        GizmoCap[]              mCircleCaps     = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // The circle caps which surround the sphere around the X, Y and Z axes
        GizmoCap                mLightSphere    = new GizmoCap();                                                       // The light sphere cap. Used to draw the sphere border.
        GizmoCap[]              mRangeCapsP     = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive range caps
        GizmoCap[]              mRangeCapsN     = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative range caps
        
        float                   mDragStartRange;                                // Light range when the drag operation starts
        Light                   mTarget;                                        // The target light
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the point light gizmo style.
        //-----------------------------------------------------------------------------
        public PointLightGizmoStyle style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's point light gizmo style.
        //-----------------------------------------------------------------------------
        public PointLightGizmoStyle activeStyle     { get { return style == null ? RTGizmos.get.skin.pointLightGizmoStyle : style; } }

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
            // Store handles and initialize them
            int count = mRangeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mRangeCapsP[i]);
                handles.Add(mRangeCapsN[i]);
                handles.Add(mCircleCaps[i]);

                // Init
                mRangeCapsP[i].cullHoveredPixels    = false;
                mRangeCapsN[i].cullHoveredPixels    = false;
                mCircleCaps[i].hoverable            = false;
                mCircleCaps[i].alignFlatToCamera    = false;
                mCircleCaps[i].capStyle.capType     = EGizmoCapType.WireCircle;
                mCircleCaps[i].zoomScaleMode        = EGizmoHandleZoomScaleMode.One;
                mCircleCaps[i].transformScaleMode   = EGizmoHandleTransformScaleMode.One;

                // Enable culling
                mCircleCaps[i].cullSphereEnabled    = true;
                mRangeCapsP[i].cullSphereEnabled    = true;
                mRangeCapsN[i].cullSphereEnabled    = true;
                mRangeCapsP[i].cullSphereMode       = EGizmoCullSphereMode.Behind;
                mRangeCapsN[i].cullSphereMode       = EGizmoCullSphereMode.Behind;
                mRangeCapsP[i].cullSphereTargets    = EGizmoHandleCullSphereTargets.Render;
                mRangeCapsN[i].cullSphereTargets    = EGizmoHandleCullSphereTargets.Render;
            }

            // Store light sphere & initialize
            handles.Add(mLightSphere);
            mLightSphere.capStyle.capType       = EGizmoCapType.Sphere;
            mLightSphere.hoverable              = false;
            mLightSphere.zoomScaleMode          = EGizmoHandleZoomScaleMode.One;
            mLightSphere.transformScaleMode     = EGizmoHandleTransformScaleMode.One;
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
            // Cache data
            Sphere lightSphere = new Sphere(mTarget.transform.position, mTarget.range);

            // Update range caps and circles
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            int count = mRangeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Range caps
                mRangeCapsP[i].capStyle = activeStyle.rangeCapStyle;
                mRangeCapsN[i].capStyle = activeStyle.rangeCapStyle;

                // Circle caps
                mCircleCaps[i].capStyle.color           = globalStyle.lightColor;
                mCircleCaps[i].capStyle.circleRadius    = lightSphere.radius;

                // Set cull sphere
                mCircleCaps[i].cullSphere   = lightSphere;
                mRangeCapsP[i].cullSphere  = lightSphere;
                mRangeCapsN[i].cullSphere  = lightSphere;
            }

            // Update transforms
            transform.position = mTarget.transform.position;
            for (int i = 0; i < count; ++i)
            {
                // Set range caps' position
                mRangeCapsP[i].transform.position = transform.position + Core.axes[i] * lightSphere.radius;
                mRangeCapsN[i].transform.position = transform.position - Core.axes[i] * lightSphere.radius;

                // Set circle caps' rotation
                mCircleCaps[i].transform.rotation = QuaternionEx.LookRotationEx(Core.axes[i], Vector3.up);
            }

            // Light sphere
            mLightSphere.capStyle.color                 = globalStyle.lightColor.Alpha(0.0f);
            mLightSphere.capStyle.sphereBorderVisible   = true;
            mLightSphere.capStyle.sphereBorderColor     = globalStyle.lightColor;
            mLightSphere.capStyle.sphereRadius          = lightSphere.radius;
            mLightSphere.transform.position             = lightSphere.center;

            // Update drag
            mDrag.scaleSnap                 = globalStyle.lightRangeSnap;
            mDrag.scaleSnapStepSource       = EGizmoDragSettingSource.Custom;
            mDrag.scaleSensitivity          = 1.0f;
            mDrag.scaleSensitivitySource    = EGizmoDragSettingSource.Custom;
            mDrag.ignoreZoomScale           = true;     // No zoom scale because the light sphere (which is affected by the drag op) doesn't use zoom scale.
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

            // Render light sphere
            mLightSphere.Render(mHRenderArgs);

            // Render circles
            int count = mCircleCaps.Length;
            for (int i = 0; i < count; ++i)
                mCircleCaps[i].Render(mHRenderArgs);

            // Sort range caps
            mCapsBuffer.Clear();
            mCapsBuffer.AddRange(mRangeCapsP);
            mCapsBuffer.AddRange(mRangeCapsN);
            GizmoHandle.ZSortHandles(mCapsBuffer, mHRenderArgs, mSortedCapsBuffer);

            // Render sorted range caps
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

            // Set drag start range
            mDragStartRange = mTarget.range;

            // Match drag handle
            int count = mRangeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Match positive range cap
                if (dragHandle == mRangeCapsP[i])
                {
                    // Start dragging
                    desc.dragType   = EGizmoDragType.Scale;
                    desc.axis0      = Core.axes[i];
                    desc.axisIndex0 = i;
                    return mDrag.Start(desc, this, transform, camera) ? mDrag : null;            
                }
                else
                // Match negative range cap
                if (dragHandle == mRangeCapsN[i])
                {
                    // Start dragging
                    desc.dragType   = EGizmoDragType.Scale;
                    desc.axis0      = -Core.axes[i];
                    desc.axisIndex0 = i;
                    return mDrag.Start(desc, this, transform, camera) ? mDrag : null; 
                }
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
            // Update light range
            RTUndo.get.Record(mTarget);
            mTarget.range = mDragStartRange + mDrag.totalDrag[mDrag.desc.axisIndex0];

            // Fix float errors when snapping is enabled
            if (mDrag.IsSnapEnabled())
                mTarget.range = MathEx.FixFloatError(mTarget.range);
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
                // Draw label at the top of the drag handle. This label shows the light range.
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Range: " + mTarget.range.ToString("F3"), EGizmoTextType.DragInfo);
            }
        }
        #endregion
    }
    #endregion
}
