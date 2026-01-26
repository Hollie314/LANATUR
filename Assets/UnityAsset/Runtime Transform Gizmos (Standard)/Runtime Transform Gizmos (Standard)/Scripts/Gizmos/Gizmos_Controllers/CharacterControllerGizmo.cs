using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: CharacterControllerGizmo (Public Class)
    // Desc: Implements a character controller gizmo.
    //-----------------------------------------------------------------------------
    public class CharacterControllerGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args

        GizmoCap                mTopHemisphere      = new GizmoCap();               // Used to draw the top hemisphere border
        GizmoCap                mBottomHemisphere   = new GizmoCap();               // Used to draw the bottom hemisphere border

        GizmoCap                mTopHeightCap       = new GizmoCap();               // Height cap that sits at the top of the controller
        GizmoCap                mBottomHeightCap    = new GizmoCap();               // Height cap that sits at the bottom of the controller
        GizmoCap[]              mRadiusCaps                                         // Radius caps
                                                    = new GizmoCap[4] { new GizmoCap(), new GizmoCap(), new GizmoCap(), new GizmoCap() };
        int[]                   mRadiusAxes         = new int[] { 0, 0, 2, 2 };     // Maps to each element in 'mRadiusCaps'. Used to calculate the cap positions.

        float                   mDragStartHeight;                                   // Controller height when the users starts to drag
        float                   mDragStartPartialHeight;                            // Controller partial height when the user starts to drag
        float                   mDragStartRadius;                                   // Controller radius when the user starts to drag
        Vector3                 mResizePivot;                                       // Pivot point from which resizing occurs
        Vector3                 mResizeAnchorDir;                                   // Normalized vector pointing from the pivot to the capsule center

        CharacterController     mTarget;                                            // The target character controller
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the character controller gizmo style.
        //-----------------------------------------------------------------------------
        public CharacterControllerGizmoStyle    style          { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's character controller
        //       gizmo style.
        //-----------------------------------------------------------------------------
        public CharacterControllerGizmoStyle    activeStyle    { get { return style == null ? RTGizmos.get.skin.characterControllerGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target character controller.
        //-----------------------------------------------------------------------------
        public CharacterController              target          { get { return mTarget; } set { mTarget = value; } }

        //-----------------------------------------------------------------------------
        // Name: resizeMode (Public Property)
        // Desc: Returns or sets the resize mode.
        //-----------------------------------------------------------------------------
        public EGizmoCapsuleResizeMode          resizeMode      { get; set; } = EGizmoCapsuleResizeMode.ByHandle;
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
            // Store radius caps
            int count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
            {
                handles.Add(mRadiusCaps[i]);
            }

            // Store height caps
            handles.Add(mTopHeightCap);
            handles.Add(mBottomHeightCap);

            // Store misc handles
            handles.Add(mTopHemisphere);
            handles.Add(mBottomHemisphere);

            // Init
            mTopHemisphere.hoverable                = false;
            mTopHemisphere.zoomScaleMode            = EGizmoHandleZoomScaleMode.One;
            mTopHemisphere.transformScaleMode       = EGizmoHandleTransformScaleMode.One;
            mTopHemisphere.capStyle.capType         = EGizmoCapType.Sphere;

            mBottomHemisphere.hoverable             = false;
            mBottomHemisphere.zoomScaleMode         = EGizmoHandleZoomScaleMode.One;
            mBottomHemisphere.transformScaleMode    = EGizmoHandleTransformScaleMode.One;
            mBottomHemisphere.capStyle.capType      = EGizmoCapType.Sphere;
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
            // Always ensure that the controller data is valid
            mTarget.EnsureValidData();

            // Update height cap styles
            mTopHeightCap.capStyle      = activeStyle.heightCapStyle;
            mBottomHeightCap.capStyle   = activeStyle.heightCapStyle;

            // Update radius cap styles
            int count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
                mRadiusCaps[i].capStyle = activeStyle.radiusCapStyle;

            // Update height cap and gizmo transforms
            float worldRadius                   = mTarget.GetWorldRadius();
            float worldHeight                   = mTarget.GetWorldHeight();
            Vector3 heightAxis                  = mTarget.GetWorldHeightAxis();
            Vector3 worldCenter                 = mTarget.GetWorldCenter();
            transform.position                  = worldCenter;
            transform.rotation                  = Quaternion.identity;  
            mTopHeightCap.transform.position    = worldCenter + heightAxis * worldHeight / 2.0f;
            mTopHeightCap.transform.rotation    = transform.rotation;
            mBottomHeightCap.transform.position = worldCenter - heightAxis * worldHeight / 2.0f;
            mBottomHeightCap.transform.rotation = transform.rotation;

            // Update radius cap transforms
            for (int i = 0; i < 2; ++i)
            {
                mRadiusCaps[i * 2].transform.position       = worldCenter + worldRadius * transform.GetAxis(mRadiusAxes[i * 2]);
                mRadiusCaps[i * 2 + 1].transform.position   = worldCenter - worldRadius * transform.GetAxis(mRadiusAxes[i * 2 + 1]);
            }

            // Update hemispheres
            var globalStyle                             = RTGizmos.get.skin.globalGizmoStyle;
            mTopHemisphere.capStyle.color               = globalStyle.characterControllerColor.Alpha(0.0f);
            mTopHemisphere.transform.position           = mTopHeightCap.transform.position - heightAxis * worldRadius;
            mTopHemisphere.capStyle.sphereBorderVisible = true;
            mTopHemisphere.capStyle.sphereBorderColor   = globalStyle.characterControllerColor;
            mTopHemisphere.capStyle.sphereRadius        = worldRadius;
            mTopHemisphere.SetClipPlane(0, new Plane(heightAxis, mTopHemisphere.transform.position));
            mTopHemisphere.SetClipPlaneEnabled(0, true);

            mBottomHemisphere.capStyle.color               = globalStyle.characterControllerColor.Alpha(0.0f);
            mBottomHemisphere.transform.position           = mBottomHeightCap.transform.position + heightAxis * worldRadius;
            mBottomHemisphere.capStyle.sphereBorderVisible = true;
            mBottomHemisphere.capStyle.sphereBorderColor   = globalStyle.characterControllerColor;
            mBottomHemisphere.capStyle.sphereRadius        = worldRadius;
            mBottomHemisphere.SetClipPlane(0, new Plane(-heightAxis, mBottomHemisphere.transform.position));
            mBottomHemisphere.SetClipPlaneEnabled(0, true);

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
            RTGizmos.DrawWireCapsule(mTarget.GetWorldCenter(), mTarget.GetWorldRadius(), mTarget.GetWorldHeight(), Quaternion.identity, 1, globalStyle.characterControllerColor);
            
            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.Add(mTopHemisphere);
            mCapsBuffer.Add(mBottomHemisphere);
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

            // Set drag start controller data
            mDragStartHeight        = mTarget.GetWorldHeight();
            mDragStartPartialHeight = mTarget.GetPartialWorldHeight();
            mDragStartRadius        = mTarget.GetWorldRadius();

            // Match height caps
            if (mTopHeightCap == dragHandle ||
                mBottomHeightCap == dragHandle)
            {
                // Start dragging
                mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.characterControllerSnap;
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragData.handle.transform.position - mTarget.GetWorldCenter();
                desc.axisIndex0 = 0;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;  
            }
            else
            {
                // We must be dragging one of the radius caps
                mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.characterControllerSnap * 2.0f;  // * 2 because we're dragging the diameter
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragData.handle.transform.position - mTarget.GetWorldCenter();
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
            // Record controller state
            RTUndo.get.Record(mTarget);

            // Dragging height?
            if (dragData.handle == mTopHeightCap ||
                dragData.handle == mBottomHeightCap)
            {
                // Update controller height.
                // Note: When we drag, we want to change the partial height. The controller's
                //       height property includes 2 times the radius. We want to exclude that
                //       when we drag.
                float worldRadius       = mTarget.GetWorldRadius();
                float partialHeight     = mDragStartPartialHeight + mDrag.totalDrag[mDrag.desc.axisIndex0];
                mTarget.SetPartialWorldHeight(partialHeight);

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    mTarget.height = MathEx.FixFloatError(mTarget.height);

                // Update center position if we're not resizing from the center
                mTarget.EnsureValidData();
                if (resizeMode != EGizmoCapsuleResizeMode.FromCenter)
                    mTarget.SetWorldCenter(mResizePivot + mResizeAnchorDir * mTarget.GetWorldHeight() / 2.0f);
            }
            else
            {
                // We are changing the radius by dragging the diameter
                mTarget.SetWorldRadius(mDragStartRadius + mDrag.totalDrag[mDrag.desc.axisIndex0] / 2.0f);   // Divide by 2 because we're dragging the diameter

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    mTarget.radius = MathEx.FixFloatError(mTarget.radius);

                // Update center position if we're not resizing from the center
                mTarget.SetWorldHeight(mDragStartHeight);   // When the capsule turns into a sphere, this helps maintain its original height when it turns back into a capsule
                if (resizeMode != EGizmoCapsuleResizeMode.FromCenter)
                    mTarget.SetWorldCenter(mResizePivot + mResizeAnchorDir * mTarget.GetWorldRadius());
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
                    string text = $"World height: {mTarget.GetWorldHeight().ToString("F3")}";
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                                camera, text, EGizmoTextType.DragInfo);
                }
                else
                {
                    // We're dragging the radius handles
                    string text = $"World radius: {mTarget.GetWorldRadius().ToString("F3")}";
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
            mResizeAnchorDir = (mTarget.GetWorldCenter() - mResizePivot).normalized;
        }
        #endregion
    }
    #endregion
}