using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CapsuleCollider2DGizmo (Public Class)
    // Desc: Implements a 2D capsule collider gizmo.
    //-----------------------------------------------------------------------------
    public class CapsuleCollider2DGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args

        GizmoCap                mTopHeightCap       = new GizmoCap();               // Height cap that sits at the top of the collider
        GizmoCap                mBottomHeightCap    = new GizmoCap();               // Height cap that sits at the bottom of the collider
        GizmoCap[]              mRadiusCaps                                         // Radius caps
                                                    = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };

        float                   mDragStartHeight;                                   // Collider height when the users starts to drag
        float                   mDragStartPartialHeight;                            // Collider partial height when the user starts to drag
        float                   mDragStartRadius;                                   // Collider radius when the user starts to drag
        Vector3                 mResizePivot;                                       // Pivot point from which resizing occurs
        Vector3                 mResizeAnchorDir;                                   // Normalized vector pointing from the pivot to the capsule center
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the 2D capsule collider gizmo style.
        //-----------------------------------------------------------------------------
        public CapsuleCollider2DGizmoStyle style            { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's 2D capsule collider
        //       gizmo style.
        //-----------------------------------------------------------------------------
        public CapsuleCollider2DGizmoStyle activeStyle      { get { return style == null ? RTGizmos.get.skin.capsuleCollider2DGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target 2D capsule collider.
        //-----------------------------------------------------------------------------
        public CapsuleCollider2D          target            { get; set; }

        //-----------------------------------------------------------------------------
        // Name: resizeMode (Public Property)
        // Desc: Returns or sets the resize mode.
        //-----------------------------------------------------------------------------
        public EGizmoCapsuleResizeMode  resizeMode          { get; set; } = EGizmoCapsuleResizeMode.ByHandle;
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
            // Store radius caps
            int count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
            {
                handles.Add(mRadiusCaps[i]);
            }

            // Store height caps
            handles.Add(mTopHeightCap);
            handles.Add(mBottomHeightCap);
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
            // Always ensure that the collider data is valid
            target.EnsureValidData();

            // Update height cap styles
            mTopHeightCap.capStyle      = activeStyle.heightCapStyle;
            mBottomHeightCap.capStyle   = activeStyle.heightCapStyle;

            // Update radius cap styles
            int count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
                mRadiusCaps[i].capStyle = activeStyle.radiusCapStyle;

            // Update height cap and gizmo transforms
            float worldRadius                   = target.GetWorldRadius();
            float worldHeight                   = target.GetWorldHeight();
            Vector3 heightAxis                  = target.GetWorldHeightAxis();
            Vector3 worldCenter                 = target.GetWorldCenter();
            transform.position                  = worldCenter;
            transform.rotation                  = target.transform.rotation;
            mTopHeightCap.transform.position    = worldCenter + heightAxis * worldHeight / 2.0f;
            mTopHeightCap.transform.rotation    = transform.rotation;
            mBottomHeightCap.transform.position = worldCenter - heightAxis * worldHeight / 2.0f;
            mBottomHeightCap.transform.rotation = transform.rotation;

            // Update radius cap transforms
            int radiusAxisIndex = target.GetRadiusAxisIndex();
            mRadiusCaps[0].transform.position   = worldCenter + worldRadius * transform.GetAxis(radiusAxisIndex);
            mRadiusCaps[1].transform.position   = worldCenter - worldRadius * transform.GetAxis(radiusAxisIndex);

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

            // Draw wire capsule
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            RTGizmos.DrawWireCapsule2D(target.GetWorldCenter(), target.GetWorldRadius(), target.GetWorldHeight(), target.transform.rotation, target.GetHeightAxisIndex(), globalStyle.colliderColor);
            
            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.Add(mTopHeightCap);
            mCapsBuffer.Add(mBottomHeightCap);
            mCapsBuffer.AddRange(mRadiusCaps);
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

            // Set drag start collider data
            mDragStartHeight        = target.GetWorldHeight();
            mDragStartPartialHeight = target.GetPartialWorldHeight();
            mDragStartRadius        = target.GetWorldRadius();

            // Match height caps
            if (mTopHeightCap == dragHandle ||
                mBottomHeightCap == dragHandle)
            {
                // Start dragging
                mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.colliderSnap;
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragData.handle.transform.position - target.GetWorldCenter();
                desc.axisIndex0 = 0;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;  
            }
            else
            {
                // We must be dragging one of the radius caps
                mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.colliderSnap * 2.0f;  // * 2 because we're dragging the diameter
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragData.handle.transform.position - target.GetWorldCenter();
                desc.axisIndex0 = 0;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;  
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnDrag() (Protected Function)
        // Desc: Called when the gizmo is dragged.
        // Parm: drag - The active drag operation.
        //-----------------------------------------------------------------------------
        protected override void OnDrag(GizmoDrag drag)
        {
            // Record collider state
            RTUndo.get.Record(target);

            // Dragging height?
            if (dragData.handle == mTopHeightCap ||
                dragData.handle == mBottomHeightCap)
            {
                // Update collider height.
                // Note: When we drag, we want to change the partial height. The collider's
                //       height property includes 2 times the radius. We want to exclude that
                //       when we drag.
                float worldRadius       = target.GetWorldRadius();
                float partialHeight     = mDragStartPartialHeight + mDrag.totalDrag[mDrag.desc.axisIndex0];
                target.SetPartialWorldHeight(partialHeight);

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.size = new Vector2(target.size.x, MathEx.FixFloatError(target.size.y));

                // Update center position if we're not resizing from the center
                target.EnsureValidData();
                if (resizeMode != EGizmoCapsuleResizeMode.FromCenter)
                    target.SetWorldCenter(mResizePivot + mResizeAnchorDir * target.GetWorldHeight() / 2.0f);
            }
            else
            {
                // We are changing the radius by dragging the diameter
                target.SetWorldRadius(mDragStartRadius + mDrag.totalDrag[mDrag.desc.axisIndex0] / 2.0f);   // Divide by 2 because we're dragging the diameter

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.size = new Vector2(MathEx.FixFloatError(target.size.x), target.size.y);

                // Update center position if we're not resizing from the center
                target.SetWorldHeight(mDragStartHeight);   // When the capsule turns into a sphere, this helps maintain its original height when it turns back into a capsule
                if (resizeMode != EGizmoCapsuleResizeMode.FromCenter)
                    target.SetWorldCenter(mResizePivot + mResizeAnchorDir * target.GetWorldRadius());
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnGUI() (Protected Function)
        // Desc: Called during the 'OnGUI' event to allow the gizmo to draw GUI elements.
        // Parm: camera - The camera that renders the GUI.
        //-----------------------------------------------------------------------------
        protected override void OnGUI(Camera camera)
        {
            if (dragging)
            {
                // Are we dragging the height handles?
                if (dragData.handle == mTopHeightCap ||
                    dragData.handle == mBottomHeightCap)
                {
                    // Draw height label
                    string text = $"World height: {target.GetWorldHeight().ToString("F3")}";
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                                camera, text, EGizmoTextType.DragInfo);
                }
                else
                {
                    // We're dragging the radius handles
                    string text = $"World radius: {target.GetWorldRadius().ToString("F3")}";
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                                camera, text, EGizmoTextType.DragInfo);
                }
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

            // Check height caps
            if (dragData.handle == mTopHeightCap) mResizePivot = mBottomHeightCap.transform.position;
            else if (dragData.handle == mBottomHeightCap) mResizePivot = mTopHeightCap.transform.position;
            else
            {
                // We're dragging the radius caps
                if (dragData.handle == mRadiusCaps[0])      mResizePivot = mRadiusCaps[1].transform.position;
                else if (dragData.handle == mRadiusCaps[1]) mResizePivot = mRadiusCaps[0].transform.position;
                else if (dragData.handle == mRadiusCaps[2]) mResizePivot = mRadiusCaps[3].transform.position;
                else if (dragData.handle == mRadiusCaps[3]) mResizePivot = mRadiusCaps[2].transform.position;
            }

            // Update anchor
            mResizeAnchorDir = (target.GetWorldCenter() - mResizePivot).normalized;
        }
        #endregion
    }
    #endregion
}