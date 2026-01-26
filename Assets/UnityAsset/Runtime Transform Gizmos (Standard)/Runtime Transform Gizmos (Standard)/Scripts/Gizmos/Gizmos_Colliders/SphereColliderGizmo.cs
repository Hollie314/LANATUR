using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SphereColliderGizmo (Public Class)
    // Desc: Implements a sphere collider gizmo.
    //-----------------------------------------------------------------------------
    public class SphereColliderGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args
        GizmoCap[]              mCircleCaps         = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // The circle caps which surround the sphere around the X, Y and Z axes
        GizmoCap                mColliderSphere     = new GizmoCap();                                                       // The collider sphere cap. Used to draw the sphere border.
        GizmoCap[]              mRadiusCapsP        = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive radius caps
        GizmoCap[]              mRadiusCapsN        = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative radius caps
        GizmoCap                mCenterRadiusCap    = new GizmoCap();               // Center radius cap that resizes from the center

        float                   mDragStartRadius;                               // Sphere collider radius when the user starts dragging
        Vector3                 mResizePivot;                                   // Pivot point from which resizing occurs
        Vector3                 mResizeAnchorDir;                               // Normalized vector pointing from the pivot to the sphere center

        SphereCollider          mTarget;                                        // The target sphere collider
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the sphere collider gizmo style.
        //-----------------------------------------------------------------------------
        public SphereColliderGizmoStyle style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's sphere collider gizmo
        //       style.
        //-----------------------------------------------------------------------------
        public SphereColliderGizmoStyle activeStyle     { get { return style == null ? RTGizmos.get.skin.sphereColliderGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target sphere collider.
        //-----------------------------------------------------------------------------
        public SphereCollider target { get { return mTarget; } set { mTarget = value; } }
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
            int count = mRadiusCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mRadiusCapsP[i]);
                handles.Add(mRadiusCapsN[i]);
                handles.Add(mCircleCaps[i]);

                // Init
                mRadiusCapsP[i].cullHoveredPixels   = false;
                mRadiusCapsN[i].cullHoveredPixels   = false;
                mCircleCaps[i].hoverable            = false;
                mCircleCaps[i].alignFlatToCamera    = false;
                mCircleCaps[i].capStyle.capType     = EGizmoCapType.WireCircle;
                mCircleCaps[i].zoomScaleMode        = EGizmoHandleZoomScaleMode.One;
                mCircleCaps[i].transformScaleMode   = EGizmoHandleTransformScaleMode.One;

                // Enable culling
                mCircleCaps[i].cullSphereEnabled    = true;
                mRadiusCapsP[i].cullSphereEnabled   = true;
                mRadiusCapsN[i].cullSphereEnabled   = true;
                mRadiusCapsP[i].cullSphereMode      = EGizmoCullSphereMode.Behind;
                mRadiusCapsN[i].cullSphereMode      = EGizmoCullSphereMode.Behind;
                mRadiusCapsP[i].cullSphereTargets   = EGizmoHandleCullSphereTargets.Render;
                mRadiusCapsN[i].cullSphereTargets   = EGizmoHandleCullSphereTargets.Render;
            }

            // Collider sphere
            handles.Add(mColliderSphere);
            mColliderSphere.capStyle.capType    = EGizmoCapType.Sphere;
            mColliderSphere.hoverable           = false;
            mColliderSphere.zoomScaleMode       = EGizmoHandleZoomScaleMode.One;
            mColliderSphere.transformScaleMode  = EGizmoHandleTransformScaleMode.One;

            // Center radius cap
            handles.Add(mCenterRadiusCap);
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
            // Ensure valid data
            mTarget.EnsureValidData();

            // Cache data
            Sphere colliderSphere = new Sphere(mTarget.GetWorldCenter(), mTarget.GetWorldRadius());

            // Update radius caps and circle styles
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            int count = mRadiusCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Radius caps
                mRadiusCapsP[i].capStyle = activeStyle.radiusCapStyle;
                mRadiusCapsN[i].capStyle = activeStyle.radiusCapStyle;

                // Circle caps
                mCircleCaps[i].capStyle.color           = globalStyle.colliderColor;
                mCircleCaps[i].capStyle.circleRadius    = colliderSphere.radius;

                // Set cull sphere
                mCircleCaps[i].cullSphere   = colliderSphere;
                mRadiusCapsP[i].cullSphere  = colliderSphere;
                mRadiusCapsN[i].cullSphere  = colliderSphere;
            }

            // Update transforms
            transform.position  = colliderSphere.center;
            transform.rotation  = mTarget.transform.rotation;
            for (int i = 0; i < count; ++i)
            {
                // Set radius caps' position
                mRadiusCapsP[i].transform.position = transform.position + transform.GetAxis(i) * colliderSphere.radius;
                mRadiusCapsN[i].transform.position = transform.position - transform.GetAxis(i) * colliderSphere.radius;

                // Set circle caps' rotation
                mCircleCaps[i].transform.rotation = QuaternionEx.LookRotationEx(transform.GetAxis(i), Vector3.up);
            }
         
            // Update center radius cap
            mCenterRadiusCap.capStyle = activeStyle.centerRadiusCapStyle;

            // Collider sphere
            mColliderSphere.capStyle.color                 = globalStyle.colliderColor.Alpha(0.0f);
            mColliderSphere.capStyle.sphereBorderVisible   = true;
            mColliderSphere.capStyle.sphereBorderColor     = globalStyle.colliderColor;
            mColliderSphere.capStyle.sphereRadius          = colliderSphere.radius;
            mColliderSphere.transform.position             = colliderSphere.center;

            // Update drag
            mDrag.scaleSensitivity          = 1.0f;
            mDrag.scaleSensitivitySource    = EGizmoDragSettingSource.Custom;
            mDrag.scaleSnap                 = globalStyle.colliderSnap;
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

            // Render collider sphere
            mColliderSphere.Render(mHRenderArgs);

            // Render circles
            int count = mCircleCaps.Length;
            for (int i = 0; i < count; ++i)
                mCircleCaps[i].Render(mHRenderArgs);

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.AddRange(mRadiusCapsP);
            mCapsBuffer.AddRange(mRadiusCapsN);
            mCapsBuffer.Add(mCenterRadiusCap);
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
            
            // Update resize pivot data
            UpdateResizePivotData();

            // Set drag start radius
            mDragStartRadius = mTarget.GetWorldRadius();

            // Resize from center?
            if (dragData.handle == mCenterRadiusCap)
            {
                // Start dragging
                desc.dragType = EGizmoDragType.UniformScale;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;
            }
            else
            {
                // Match drag handle
                int count = mRadiusCapsP.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Match positive radius cap
                    if (dragHandle == mRadiusCapsP[i])
                    {
                        // Start dragging
                        mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.colliderSnap * 2.0f;  // * 2 because we're dragging the diameter
                        desc.dragType   = EGizmoDragType.Scale;
                        desc.axis0      = mTarget.transform.GetAxis(i);
                        desc.axisIndex0 = i;
                        return mDrag.Start(desc, this, transform, camera) ? mDrag : null;            
                    }
                    else
                    // Match negative radius cap
                    if (dragHandle == mRadiusCapsN[i])
                    {
                        // Start dragging
                        mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.colliderSnap * 2.0f;  // * 2 because we're dragging the diameter
                        desc.dragType   = EGizmoDragType.Scale;
                        desc.axis0      = -mTarget.transform.GetAxis(i);
                        desc.axisIndex0 = i;
                        return mDrag.Start(desc, this, transform, camera) ? mDrag : null; 
                    }
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
            // Update collider radius
            RTUndo.get.Record(mTarget);
            float worldRadius = mDragStartRadius + (dragData.handle == mCenterRadiusCap ? mDrag.totalDrag[mDrag.desc.axisIndex0] : mDrag.totalDrag[mDrag.desc.axisIndex0] / 2.0f);

            // Fix float errors when snapping is enabled
            if (mDrag.IsSnapEnabled())
                worldRadius = MathEx.FixFloatError(worldRadius);

            // Set the new world radius
            mTarget.SetWorldRadius(worldRadius);

            // Update center position if we're not resizing from the center
            if (dragData.handle != mCenterRadiusCap)
                mTarget.SetWorldCenter(mResizePivot + mResizeAnchorDir * mTarget.GetWorldRadius());  
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
                // Draw radius label
                string text = $"World radius: {mTarget.GetWorldRadius().ToString("F3")}";
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                            camera, text, EGizmoTextType.DragInfo);
            }
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: UpdateResizePivotData() (Private Function)
        // Desc: Called when a drag operation starts to update the resize pivot data.
        //-----------------------------------------------------------------------------
        void UpdateResizePivotData()
        {
            // Not dragging?
            if (!dragging)
                return;

            // Calculate based on the dragged handle
            var dragHandle = dragData.handle;
            if (dragHandle == mCenterRadiusCap) mResizePivot = mCenterRadiusCap.transform.position;
            else
            {
                // Are we dragging the radius caps?
                int count       = mRadiusCapsP.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Check match and set the pivot to the opposite handle
                    if (mRadiusCapsP[i] == dragHandle)
                    {
                        mResizePivot = mRadiusCapsN[i].transform.position;
                        break;
                    }
                    if (mRadiusCapsN[i] == dragHandle)
                    {
                        mResizePivot = mRadiusCapsP[i].transform.position;
                        break;
                    }
                }
            }

            // Update anchor
            mResizeAnchorDir = (mTarget.GetWorldCenter() - mResizePivot).normalized;         
        }
        #endregion
    }
    #endregion
}
