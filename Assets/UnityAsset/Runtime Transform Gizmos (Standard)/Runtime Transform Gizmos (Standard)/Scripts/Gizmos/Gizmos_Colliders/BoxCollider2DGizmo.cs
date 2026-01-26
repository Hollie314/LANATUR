using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxCollider2DGizmo (Public Class)
    // Desc: Implements a 2D box collider gizmo.
    //-----------------------------------------------------------------------------
    public class BoxCollider2DGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag           = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs    = new GizmoHandleRenderArgs();  // Gizmo handle render args

        GizmoCap                mBox            = new GizmoCap();               // The collider box
        GizmoCap                mUniformCap     = new GizmoCap();               // Uniform resize cap

        // Positive and negative axis resize caps
        GizmoCap[]              mAxisResizeCapsP    = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };
        GizmoCap[]              mAxisResizeCapsN    = new GizmoCap[2] { new GizmoCap(), new GizmoCap() };

        Vector3                 mDragStartSize;                                 // The size of the collider the user starts dragging
        Vector3                 mResizePivot;                                   // Pivot point from which resizing occurs
        Vector3                 mResizeAnchorDir;                               // Normalized vector pointing from the pivot to the box center
        EGizmoBoxResizeMode     mResizeMode = EGizmoBoxResizeMode.ByHandle;     // The current resize mode
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the 2D box collider gizmo style.
        //-----------------------------------------------------------------------------
        public BoxCollider2DGizmoStyle    style { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's 2D box collider gizmo style.
        //-----------------------------------------------------------------------------
        public BoxCollider2DGizmoStyle    activeStyle { get { return style == null ? RTGizmos.get.skin.boxCollider2DGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target 2D box collider.
        //-----------------------------------------------------------------------------
        public BoxCollider2D  target { get; set; }      

        //-----------------------------------------------------------------------------
        // Name: resizeMode (Public Property)
        // Desc: Returns or sets the resize mode.
        //-----------------------------------------------------------------------------
        public EGizmoBoxResizeMode  resizeMode
        {
            get { return mResizeMode; }
            set 
            {
                // No-op?
                if (value == mResizeMode)
                    return;

                // Set the resize mode and if we're dragging and we've just switched to uniform resize, make sure
                // the collider has the same size along both axes.
                mResizeMode = value; 
                if (mResizeMode == EGizmoBoxResizeMode.Uniform && dragging && IsAxisResizeHandle(dragData.handle))
                    target.size = target.size.SyncComponents(mDrag.desc.axisIndex0);
            }
        }
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
            // Store axis caps
            int count = mAxisResizeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mAxisResizeCapsP[i]);
                handles.Add(mAxisResizeCapsN[i]);

                // Set tags
                mAxisResizeCapsP[i].tag = (EGizmoHandleTag)i;
                mAxisResizeCapsN[i].tag = (EGizmoHandleTag)i;
            }

            // Store remaining caps
            handles.Add(mBox);
            handles.Add(mUniformCap);

            // Init caps
            mBox.hoverable          = false;
            mBox.zoomScaleMode      = EGizmoHandleZoomScaleMode.One;
            mBox.transformScaleMode = EGizmoHandleTransformScaleMode.One;
            mBox.capStyle.capType   = EGizmoCapType.WireBox;
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
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;

            // Update box style and transform
            Vector3 objectScale         = target.transform.lossyScale;
            mBox.capStyle.color         = globalStyle.colliderColor;
            mBox.transform.rotation     = target.transform.rotation;
            mBox.transform.position     = target.GetWorldCenter();
            transform.position          = mBox.transform.position;
            transform.rotation          = mBox.transform.rotation;
            mBox.capStyle.boxSize       = target.GetWorldSize();

            // Update axis caps
            int count = mAxisResizeCapsP.Length;
            Vector3 halfBoxSize = mBox.capStyle.boxSize / 2.0f;
            Sphere cullSphere = new Sphere(mBox.transform.position, halfBoxSize.magnitude);
            for (int i = 0; i < count; ++i)
            {
                // Style
                mAxisResizeCapsP[i].capStyle = activeStyle.resizeCapStyle;
                mAxisResizeCapsN[i].capStyle = activeStyle.resizeCapStyle;

                // Transform
                mAxisResizeCapsP[i].transform.position = mBox.transform.position + mBox.transform.GetAxis(i) * halfBoxSize[i];
                mAxisResizeCapsN[i].transform.position = mBox.transform.position - mBox.transform.GetAxis(i) * halfBoxSize[i];

                // Set cull sphere
                mAxisResizeCapsP[i].cullSphere = cullSphere;
                mAxisResizeCapsN[i].cullSphere = cullSphere;
            }

            // Update uniform cap
            mUniformCap.capStyle = activeStyle.uniformCapStyle;

            // Update drag
            mDrag.scaleSnap                 = globalStyle.colliderSnap;
            mDrag.scaleSnapStepSource       = EGizmoDragSettingSource.Custom;
            mDrag.scaleSensitivity          = 1.0f;
            mDrag.scaleSensitivitySource    = EGizmoDragSettingSource.Custom;
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
            mHRenderArgs.camera = camera;
            mHRenderArgs.rasterCtx = rasterCtx;

            // Draw box
            mBox.Render(mHRenderArgs);

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.Add(mUniformCap);
            mCapsBuffer.AddRange(mAxisResizeCapsP);
            mCapsBuffer.AddRange(mAxisResizeCapsN);
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
            GizmoHandle dragHandle = dragData.handle;
            GizmoDragDesc desc = new GizmoDragDesc();

            // Update resize pivot data
            UpdateResizePivotData();

            // Store drag start size
            mDragStartSize = target.GetWorldSize();

            // Uniform resize with the uniform cap?
            if (dragData.handle == mUniformCap)
            {
                // Start dragging
                desc.dragType = EGizmoDragType.UniformScale;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;
            }
            else
            {
                // Match resize caps
                int count = mAxisResizeCapsP.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Match?
                    if (dragData.handle == mAxisResizeCapsP[i] ||
                        dragData.handle == mAxisResizeCapsN[i])
                    {
                        // Start dragging
                        desc.dragType   = EGizmoDragType.Scale;
                        desc.axis0      = dragData.handle.transform.position - target.GetWorldCenter();
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
            // Record collider state
            RTUndo.get.Record(target);

            // Uniform resize by handle?
            if (dragData.handle == mUniformCap) target.SetWorldSize(mDragStartSize + mDrag.totalDrag);
            else
            {
                // Resize along a single axis
                int axis            = mDrag.desc.axisIndex0;
                Vector3 worldSize   = mDragStartSize;
                worldSize[axis]     = mDragStartSize[axis] + mDrag.totalDrag[axis];

                // If we're doing a uniform resize, sync other vector components
                if (mResizeMode == EGizmoBoxResizeMode.Uniform)
                    worldSize = worldSize.SyncComponents(mDrag.desc.axisIndex0);

                // Set collider size
                target.SetWorldSize(worldSize);
            }

            // If snapping is enabled, fix float errors
            if (mDrag.IsSnapEnabled())
                target.size = target.size.FixFloatError();

            // Take pivot into account
            if (dragData.handle != mUniformCap)
            {
                int axisIndex       = mDrag.desc.axisIndex0;
                Vector3 boxSize     = target.GetWorldSize();                                        // World box size
                Vector3 boxCenter   = mResizePivot + mResizeAnchorDir * boxSize[axisIndex] / 2.0f;  // Move center away from the pivot by half the box size
                target.SetWorldCenter(boxCenter);                                                   // Set world center
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
                // Draw size label
                string text = $"World size: {target.GetWorldSize().ToString("F3")}";
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
            if (dragHandle == mUniformCap) mResizePivot = mUniformCap.transform.position;
            else
            {
                // Are we dragging the resize caps?
                int count = mAxisResizeCapsP.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Check match and set the pivot to the opposite handle
                    if (mAxisResizeCapsP[i] == dragHandle)
                    {
                        mResizePivot = mAxisResizeCapsN[i].transform.position;
                        break;
                    }
                    if (mAxisResizeCapsN[i] == dragHandle)
                    {
                        mResizePivot = mAxisResizeCapsP[i].transform.position;
                        break;
                    }
                }
            }

            // Update anchor
            mResizeAnchorDir = (target.GetWorldCenter() - mResizePivot).normalized;         
        }

        //-----------------------------------------------------------------------------
        // Name: IsAxisResizeHandle() (Private Function)
        // Desc: Checks if the specified handle is an axis resize handle (i.e. one of the
        //       handles that resizes along a single axis).
        // Parm: handle - Query handle.
        // Rtrn: True if 'handle' is an axis resize handle and false otherwise.
        //-----------------------------------------------------------------------------
        bool IsAxisResizeHandle(GizmoHandle handle)
        {
            // Loop through each axis resize handle
            int count = mAxisResizeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Match?
                if (mAxisResizeCapsP[i] == handle ||
                    mAxisResizeCapsN[i] == handle) return true;
            }

            // No match
            return false;
        }
        #endregion
    }
    #endregion
}
