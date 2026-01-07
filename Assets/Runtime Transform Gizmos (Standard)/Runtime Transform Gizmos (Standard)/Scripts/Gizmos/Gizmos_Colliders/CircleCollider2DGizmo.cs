using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CircleCollider2DGizmo (Public Class)
    // Desc: Implements a 2D circle collider gizmo.
    //-----------------------------------------------------------------------------
    public class CircleCollider2DGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args
        GizmoCap                mCircleCap          = new GizmoCap();               // Circle cap
        GizmoCap[]              mRadiusCapsP        = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };   // Positive radius caps
        GizmoCap[]              mRadiusCapsN        = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };   // Negative radius caps
        GizmoCap                mCenterRadiusCap    = new GizmoCap();               // Center radius cap that resizes from the center

        float                   mDragStartRadius;   // Circle collider radius when the user starts dragging
        Vector3                 mResizePivot;       // Pivot point from which resizing occurs
        Vector3                 mResizeAnchorDir;   // Normalized vector pointing from the pivot to the circle center
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the 2D circle collider gizmo style.
        //-----------------------------------------------------------------------------
        public CircleCollider2DGizmoStyle style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's 2D circle collider
        //       gizmo style.
        //-----------------------------------------------------------------------------
        public CircleCollider2DGizmoStyle activeStyle     { get { return style == null ? RTGizmos.get.skin.circleCollider2DGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target 2D circle collider.
        //-----------------------------------------------------------------------------
        public CircleCollider2D target { get; set; }
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
            // Store handles and initialize them
            int count = mRadiusCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mRadiusCapsP[i]);
                handles.Add(mRadiusCapsN[i]);
            }

            // Circle cap
            handles.Add(mCircleCap);
            mCircleCap.hoverable            = false;
            mCircleCap.alignFlatToCamera    = false;
            mCircleCap.capStyle.capType     = EGizmoCapType.WireCircle;
            mCircleCap.zoomScaleMode        = EGizmoHandleZoomScaleMode.One;
            mCircleCap.transformScaleMode   = EGizmoHandleTransformScaleMode.One;

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
            target.EnsureValidData();

            // Cache data
            Circle colliderCircle = new Circle();
            colliderCircle.Set(target.GetWorldCenter(), target.GetWorldRadius(), target.transform.right, target.transform.forward);

            // Update radius caps and circle styles
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            int count = mRadiusCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Radius caps
                mRadiusCapsP[i].capStyle = activeStyle.radiusCapStyle;
                mRadiusCapsN[i].capStyle = activeStyle.radiusCapStyle;
            }

            // Update circle cap style
            mCircleCap.capStyle.color           = globalStyle.colliderColor;
            mCircleCap.capStyle.circleRadius    = colliderCircle.radius;

            // Update transforms
            transform.position  = colliderCircle.center;
            transform.rotation  = target.transform.rotation;
            for (int i = 0; i < count; ++i)
            {
                // Set radius caps' position
                mRadiusCapsP[i].transform.position = transform.position + transform.GetAxis(i) * colliderCircle.radius;
                mRadiusCapsN[i].transform.position = transform.position - transform.GetAxis(i) * colliderCircle.radius;
            }

            // Update circle cap transform
            mCircleCap.transform.rotation = QuaternionEx.LookRotationEx(transform.GetAxis(2), Vector3.up);
         
            // Update center radius cap
            mCenterRadiusCap.capStyle = activeStyle.centerRadiusCapStyle;

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

            // Render circle cap
            mCircleCap.Render(mHRenderArgs);

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.AddRange(mRadiusCapsP);
            mCapsBuffer.AddRange(mRadiusCapsN);
            mCapsBuffer.Add(mCenterRadiusCap);
            GizmoHandle.ZSortHandles(mCapsBuffer, mHRenderArgs, mSortedCapsBuffer);

            // Render sorted caps
            int count = mSortedCapsBuffer.Count;
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
            mDragStartRadius = target.GetWorldRadius();

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
                        desc.axis0      = target.transform.GetAxis(i);
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
                        desc.axis0      = -target.transform.GetAxis(i);
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
            RTUndo.get.Record(target);
            float worldRadius = mDragStartRadius + (dragData.handle == mCenterRadiusCap ? mDrag.totalDrag[mDrag.desc.axisIndex0] : mDrag.totalDrag[mDrag.desc.axisIndex0] / 2.0f);

            // Fix float errors when snapping is enabled
            if (mDrag.IsSnapEnabled())
                worldRadius = MathEx.FixFloatError(worldRadius);

            // Set the new world radius
            target.SetWorldRadius(worldRadius);

            // Update center position if we're not resizing from the center
            if (dragData.handle != mCenterRadiusCap)
                target.SetWorldCenter(mResizePivot + mResizeAnchorDir * target.GetWorldRadius());  
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
                string text = $"World radius: {target.GetWorldRadius().ToString("F3")}";
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
            mResizeAnchorDir = (target.GetWorldCenter() - mResizePivot).normalized;         
        }
        #endregion
    }
    #endregion
}
