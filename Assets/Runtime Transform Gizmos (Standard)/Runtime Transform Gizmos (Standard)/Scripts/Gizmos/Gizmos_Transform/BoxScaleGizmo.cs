using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: BoxScaleGizmo (Public Class)
    // Desc: Implements a box scale gizmo.
    //-----------------------------------------------------------------------------
    public class BoxScaleGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args
        GizmoCap[]              mAxisCapsP          = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive axis caps for X, Y, Z
        GizmoCap[]              mAxisCapsN          = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative axis caps for X, Y, Z
        GizmoCap                mUniformCap         = new GizmoCap();               // The uniform scale cap
        GizmoCap                mBox                = new GizmoCap();               // The box handle (non-interactive)
        Vector3                 mDragStartScale;                                    // Drag start target scale
        Vector3                 mDragStartPosition;                                 // Drag start target position
        Vector3                 mScalePivot;                                        // Scale pivot used while scaling
        OBox                    mTargetBox          = OBox.GetInvalid();            // The target box volume
        GameObject              mTarget;                                            // Target object
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the box scale gizmo style.
        //-----------------------------------------------------------------------------
        public BoxScaleGizmoStyle       style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's box scale gizmo style.
        //-----------------------------------------------------------------------------
        public BoxScaleGizmoStyle       activeStyle     { get { return style == null ? RTGizmos.get.skin.boxScaleGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target object affected by the gizmo.
        //-----------------------------------------------------------------------------
        public GameObject               target          { get { return mTarget; } set { mTarget = value; Refresh(); } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Refresh() (Public Function)
        // Desc: Should be called when the client has manually changed the target object's
        //       transform or when changes are made to the target (e.g. deactivated, added or
        //       removed components etc). The function has no effect if the gizmo doesn't
        //       have any targets attached.
        //-----------------------------------------------------------------------------
        public void Refresh()
        {
            // Update target box
            UpdateTargetBox();
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
            // It's easier to always keep the target box updated
            UpdateTargetBox();

            // Are we ready?
            return mTargetBox.isValid && mTargetBox.size.Count0() <= 1;
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
            int count = mAxisCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store and set tags
                handles.Add(mAxisCapsP[i]);
                handles.Add(mAxisCapsN[i]);

                mAxisCapsP[i].tag = (EGizmoHandleTag)((int)EGizmoHandleTag.X + i);
                mAxisCapsN[i].tag = (EGizmoHandleTag)((int)EGizmoHandleTag.X + i);
            }

            // Store uniform scale cap
            handles.Add(mUniformCap);

            // Store box cap
            handles.Add(mBox);

            // Init
            mBox.hoverable                  = false;
            mBox.capStyle.capType           = EGizmoCapType.WireBox;
            mBox.zoomScaleMode              = EGizmoHandleZoomScaleMode.One;
            mBox.transformScaleMode         = EGizmoHandleTransformScaleMode.One;
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
            // Update gizmo transform
            transform.position = mTargetBox.center;
            transform.rotation = mTargetBox.rotation;

            // Loop through each axis cap and update its style
            int count = mAxisCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Cache data
                var color   = RTGizmos.get.skin.globalGizmoStyle.GetAxisColor(i);

                // Set styles and color
                mAxisCapsP[i].capStyle           = activeStyle.GetAxisCapStyle(i, true);
                mAxisCapsP[i].capStyle.color     = color;

                mAxisCapsN[i].capStyle           = activeStyle.GetAxisCapStyle(i, false);
                mAxisCapsN[i].capStyle.color     = color;
            }

            // Loop through each axis cap and update its transform
            count           = mAxisCapsP.Length;
            Vector3 extents = mTargetBox.extents;
            for (int i = 0; i < count; ++i)
            {
                // Cache data
                Vector3 axis     = mTargetBox.rotation * Core.axes[i];
                float extent     = extents[i];
                Vector3 center   = mTargetBox.center;

                // Positive cap
                mAxisCapsP[i].transform.position = center + axis * (extent + activeStyle.axisCapOffset);
                mAxisCapsP[i].transform.rotation = Quaternion.FromToRotation(Vector3.right, axis);

                // Negative cap
                mAxisCapsN[i].transform.position = center - axis * (extent + activeStyle.axisCapOffset);
                mAxisCapsN[i].transform.rotation = Quaternion.FromToRotation(Vector3.right, -axis);
            }

            // Update uniform scale cap
            mUniformCap.capStyle = activeStyle.uniformCapStyle;

            // Update box cap style
            mBox.capStyle.color     = activeStyle.boxColor;
            mBox.capStyle.boxSize   = mTargetBox.size;

            // Hide handles which would otherwise scale a 0 sized volume. 
            Vector3 size = Vector3Ex.Abs(mTargetBox.size);
            if (size.x < 1e-4f)
            {
                mAxisCapsP[0].visible = false;
                mAxisCapsN[0].visible = false;
            }
            else
            {
                mAxisCapsP[0].visible = true;
                mAxisCapsN[0].visible = true;
            }
            if (size.y < 1e-4f)
            {
                mAxisCapsP[1].visible = false;
                mAxisCapsN[1].visible = false;
            }
            else
            {
                mAxisCapsP[1].visible = true;
                mAxisCapsN[1].visible = true;
            }
            if (size.z < 1e-4f)
            {
                mAxisCapsP[2].visible = false;
                mAxisCapsN[2].visible = false;
            }
            else
            {
                mAxisCapsP[2].visible = true;
                mAxisCapsN[2].visible = true;
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

            // Fill cap buffer
            mCapsBuffer.Clear();
            mCapsBuffer.AddRange(mAxisCapsP);
            mCapsBuffer.AddRange(mAxisCapsN);
            mCapsBuffer.Add(mUniformCap);

            // Render box
            mBox.Render(mHRenderArgs);

            // Sort caps
            GizmoHandle.ZSortHandles(mCapsBuffer, mHRenderArgs, mSortedCapsBuffer);
         
            // Render sorted caps
            int count = mSortedCapsBuffer.Count;
            for (int i = 0; i < count; ++i)
                mSortedCapsBuffer[i].Render(mHRenderArgs);

            // Sort dbl-axis sliders and render them
            GizmoHandle.ZSortHandles(internal_planeSliders, mHRenderArgs, mSortedDblAxisSlidersBuffer);
            count = mSortedDblAxisSlidersBuffer.Count;
            for (int i = 0; i < count; ++i)
                mSortedDblAxisSlidersBuffer[i].Render(mHRenderArgs);
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
            GizmoHandle     dragHandle  = dragData.handle;
            GizmoDragDesc   desc        = new GizmoDragDesc();

            // Set drag start data
            mDragStartPosition  = target.transform.position;
            mDragStartScale     = target.transform.lossyScale;

            // Match axis caps
            int count = mAxisCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Match positive caps
                if (dragHandle == mAxisCapsP[i])
                {
                    // Start dragging
                    desc.dragType   = EGizmoDragType.Scale;
                    desc.axis0      = mAxisCapsP[i].transform.position - transform.position;
                    desc.axisIndex0 = i;
                    mScalePivot     = mAxisCapsN[i].transform.position;
                    return mDrag.Start(desc, this, transform, camera) ? mDrag : null;            
                }
                else
                // Match negative sliders and caps
                if (dragHandle == mAxisCapsN[i])
                {
                    // Start dragging
                    desc.dragType   = EGizmoDragType.Scale;
                    desc.axis0      = mAxisCapsN[i].transform.position - transform.position;
                    desc.axisIndex0 = i;
                    mScalePivot     = mAxisCapsP[i].transform.position;
                    return mDrag.Start(desc, this, transform, camera) ? mDrag : null;
                }
            }

            // Match uniform scale cap
            if (dragHandle == mUniformCap)
            {
                desc.dragType   = EGizmoDragType.UniformScale;
                mScalePivot     = transform.position;
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
            // Record object transform change
            RTUndo.get.Record(target.transform);

            // Compute new scale
            Vector3 newScale = Vector3Ex.FixFloatError(mDragStartScale + drag.totalDrag);

            // Compute offset from pivot to position
            Vector3 toPos   = mDragStartPosition - mScalePivot;
            Vector3 s       = Vector3.Scale(newScale, mDragStartScale.SafeInverse());
            toPos           = Vector3.Scale(toPos, s);          // Scale offset based on scale ratio

            target.transform.SetScale(newScale);                // Set scale
            target.transform.position = mScalePivot + toPos;    // Move the object so the pivot remains stationary
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
                // Show drag delta
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_World(transform.position, camera, mDrag.totalDrag.ToString("F3"), EGizmoTextType.DragInfo);
            }          
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: UpdateTargetBox() (Private Function)
        // Desc: Updates the target box. When no target is assigned, the box is marked
        //       as invalid.
        //-----------------------------------------------------------------------------
        void UpdateTargetBox()
        {
            // No target?
            if (!target)
            {
                mTargetBox = OBox.GetInvalid();
                return;
            }

            // Create the bounds query config
            BoundsQueryConfig boundsQConfig = BoundsQueryConfig.defaultConfig;
            boundsQConfig.objectTypes = EGameObjectType.Mesh | EGameObjectType.Sprite;

            // Update box
            mTargetBox = target.CalculateHierarchyWorldOBB(boundsQConfig);
        }
        #endregion
    }
    #endregion
}
