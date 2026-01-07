using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: SpotLightGizmo (Public Class)
    // Desc: Implements a spot light gizmo.
    //-----------------------------------------------------------------------------
    public class SpotLightGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args

        // Angle arcs
        GizmoCap                mAzimuthArc         = new GizmoCap();               // Azimuth (i.e. horizontal angle arc)
        GizmoCap                mElevationArc       = new GizmoCap();               // Elevation (i.e. vertical angle arc)

        // Spot circle caps
        GizmoCap                mInnerSpotCircle    = new GizmoCap();               // Inner spot circle
        GizmoCap                mOuterSpotCircle    = new GizmoCap();               // Outer spot circle

        // We'll use 2 caps per spot to change the inner and outer spot angles
        GizmoCap                mInnerSpotCap0      = new GizmoCap();               // First inner spot cap
        GizmoCap                mInnerSpotCap1      = new GizmoCap();               // Second inner spot cap
        GizmoCap                mOuterSpotCap0      = new GizmoCap();               // First outer spot cap
        GizmoCap                mOuterSpotCap1      = new GizmoCap();               // Second outer spot cap
        float                   mDragStarArcLength;                                 // When we drag the spot handles, we will be modifying the arc length and calculate the spot angle from that.
                                                                                    // It produces a much nicer user experience. Also, we want to use the total drag so this is why we have to keep
                                                                                    // track of the arc length at the start of a drag operation.
        float                   mDragStartRange;                                    // Same as 'mDragStarArcLength', but it applies to the light range

        // Range & Snap caps
        GizmoCap                mRangeCap           = new GizmoCap();               // The range cap used to change the light's range
        GizmoCap                mSnapCap            = new GizmoCap();               // The snap cap used to snap the light direction to look at the cursor pick point
        
        Light                   mTarget;                                            // The target light
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the spot light gizmo style.
        //-----------------------------------------------------------------------------
        public SpotLightGizmoStyle style            { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's spot light gizmo style.
        //-----------------------------------------------------------------------------
        public SpotLightGizmoStyle activeStyle      { get { return style == null ? RTGizmos.get.skin.spotLightGizmoStyle : style; } }

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
            handles.Add(mAzimuthArc);
            handles.Add(mElevationArc);
            handles.Add(mInnerSpotCircle);
            handles.Add(mOuterSpotCircle);
            handles.Add(mInnerSpotCap0);
            handles.Add(mInnerSpotCap1);
            handles.Add(mOuterSpotCap0);
            handles.Add(mOuterSpotCap1);
            handles.Add(mRangeCap);
            handles.Add(mSnapCap);

            // Spot circle handles and angle arcs
            mInnerSpotCircle.hoverable          = false;
            mInnerSpotCircle.capStyle.capType   = EGizmoCapType.WireCircle;
            mInnerSpotCircle.alignFlatToCamera  = false;
            mInnerSpotCircle.zoomScaleMode      = EGizmoHandleZoomScaleMode.One;
            mInnerSpotCircle.transformScaleMode = EGizmoHandleTransformScaleMode.One;

            mOuterSpotCircle.hoverable          = false;
            mOuterSpotCircle.capStyle.capType   = EGizmoCapType.WireCircle;
            mOuterSpotCircle.alignFlatToCamera  = false;
            mOuterSpotCircle.zoomScaleMode      = EGizmoHandleZoomScaleMode.One;
            mOuterSpotCircle.transformScaleMode = EGizmoHandleTransformScaleMode.One;

            mAzimuthArc.hoverable               = false;
            mAzimuthArc.alignFlatToCamera       = false;
            mElevationArc.hoverable             = false;
            mElevationArc.alignFlatToCamera     = false;

            mAzimuthArc.capStyle.capType        = EGizmoCapType.WireCircle;
            mElevationArc.capStyle.capType      = EGizmoCapType.WireCircle;

            mAzimuthArc.zoomScaleMode           = EGizmoHandleZoomScaleMode.One;
            mAzimuthArc.transformScaleMode      = EGizmoHandleTransformScaleMode.One;
            mElevationArc.zoomScaleMode         = EGizmoHandleZoomScaleMode.One;
            mElevationArc.transformScaleMode    = EGizmoHandleTransformScaleMode.One;
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
            var globalStyle         = RTGizmos.get.skin.globalGizmoStyle;
            float tanInner          = Mathf.Tan(Mathf.Deg2Rad * mTarget.innerSpotAngle / 2.0f);
            float tanOuter          = Mathf.Tan(Mathf.Deg2Rad * mTarget.spotAngle / 2.0f);
            float innerSpotRadius   = tanInner * mTarget.range;
            float outerSpotRadius   = tanOuter * mTarget.range;

            // Update cap styles
            mRangeCap.capStyle      = activeStyle.rangeCapStyle;
            mSnapCap.capStyle       = globalStyle.lightSnapCapStyle;

            mInnerSpotCap0.capStyle = activeStyle.innerSpotCapStyle;
            mInnerSpotCap1.capStyle = activeStyle.innerSpotCapStyle;

            mOuterSpotCap0.capStyle = activeStyle.outerSpotCapStyle;
            mOuterSpotCap1.capStyle = activeStyle.outerSpotCapStyle;

            mInnerSpotCircle.capStyle.color         = globalStyle.lightColor;
            mInnerSpotCircle.capStyle.circleRadius  = innerSpotRadius;

            mOuterSpotCircle.capStyle.color         = globalStyle.lightColor;
            mOuterSpotCircle.capStyle.circleRadius  = outerSpotRadius;

            mAzimuthArc.capStyle.color      = globalStyle.lightColor;
            mElevationArc.capStyle.color    = globalStyle.lightColor;

            // Update transforms.
            transform.position                  = mTarget.transform.position;
            mRangeCap.transform.position        = mTarget.transform.position + mTarget.transform.forward * mTarget.range;
            if (dragData.handle != mSnapCap)    // When dragging the snap cap, the drag op takes control of the snap cap position
                mSnapCap.transform.position     = mTarget.transform.position + mTarget.transform.forward * activeStyle.snapRayLength;
            mInnerSpotCap0.transform.position   = mRangeCap.transform.position - mTarget.transform.up * tanInner * mTarget.range;
            mInnerSpotCap1.transform.position   = mRangeCap.transform.position + mTarget.transform.up * tanInner * mTarget.range;
            mOuterSpotCap0.transform.position   = mRangeCap.transform.position - mTarget.transform.right * tanOuter * mTarget.range;
            mOuterSpotCap1.transform.position   = mRangeCap.transform.position + mTarget.transform.right * tanOuter * mTarget.range;
            mAzimuthArc.transform.position      = mTarget.transform.position;
            mAzimuthArc.transform.rotation      = mTarget.transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.right);
            mElevationArc.transform.position    = mTarget.transform.position;
            mElevationArc.transform.rotation    = mTarget.transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.up);

            // Set arc circle radius
            mAzimuthArc.capStyle.circleRadius   = mTarget.range;
            mElevationArc.capStyle.circleRadius = mTarget.range;

            // Create an imaginary light sphere (treat the light like point light) and project the cap
            // positions onto this sphere.
            Sphere lightSphere                  = new Sphere(mTarget.transform.position, mTarget.range);
            mRangeCap.transform.position        = lightSphere.ProjectPoint(mRangeCap.transform.position);
            mInnerSpotCap0.transform.position   = lightSphere.ProjectPoint(mInnerSpotCap0.transform.position);
            mInnerSpotCap1.transform.position   = lightSphere.ProjectPoint(mInnerSpotCap1.transform.position);
            mOuterSpotCap0.transform.position   = lightSphere.ProjectPoint(mOuterSpotCap0.transform.position);
            mOuterSpotCap1.transform.position   = lightSphere.ProjectPoint(mOuterSpotCap1.transform.position);

            // Now project the spot circles positions and calculate the radii to make it appear that their
            // vertices have been projected onto the sphere surface.
            Vector3 v = mInnerSpotCap0.transform.position - mTarget.transform.position;
            float l = v.magnitude;
            float z = Vector3.Dot(v, mTarget.transform.forward);
            mInnerSpotCircle.transform.position     = mTarget.transform.position + mTarget.transform.forward * z;
            mInnerSpotCircle.capStyle.circleRadius  = Mathf.Sqrt(l * l - z * z);
            v = mOuterSpotCap0.transform.position - mTarget.transform.position;
            l = v.magnitude;
            z = Vector3.Dot(v, mTarget.transform.forward);
            mOuterSpotCircle.transform.position     = mTarget.transform.position + mTarget.transform.forward * z;
            mOuterSpotCircle.capStyle.circleRadius  = Mathf.Sqrt(l * l - z * z);

            // Set spot circle rotations
            mInnerSpotCircle.transform.rotation = mTarget.transform.rotation;
            mOuterSpotCircle.transform.rotation = mTarget.transform.rotation;

            // All drag caps use an identity rotation
            mRangeCap.transform.rotation        = Quaternion.identity;
            mSnapCap.transform.rotation         = Quaternion.identity;
            mInnerSpotCap0.transform.rotation   = Quaternion.identity;
            mInnerSpotCap1.transform.rotation   = Quaternion.identity;
            mOuterSpotCap0.transform.rotation   = Quaternion.identity;
            mOuterSpotCap1.transform.rotation   = Quaternion.identity;
          
            // Set arc clipping plane. We only want to draw the arcs where they cover the surface of the 
            // outer spot area.
            Plane clipPlane = new Plane(mTarget.transform.forward, mOuterSpotCap0.transform.position);
            mAzimuthArc.SetClipPlane(0, clipPlane);
            mAzimuthArc.SetClipPlaneEnabled(0, true);
            mElevationArc.SetClipPlane(0, clipPlane);
            mElevationArc.SetClipPlaneEnabled(0, true);

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

            // Draw line segment from light position to end of range
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            RTGizmos.DrawSegment(mTarget.transform.position, mRangeCap.transform.position, globalStyle.lightColor);

            // Draw line segment from light position to spot cap positions
            RTGizmos.DrawSegment(mTarget.transform.position, mInnerSpotCap0.transform.position, globalStyle.lightColor);
            RTGizmos.DrawSegment(mTarget.transform.position, mInnerSpotCap1.transform.position, globalStyle.lightColor);
            RTGizmos.DrawSegment(mTarget.transform.position, mOuterSpotCap0.transform.position, globalStyle.lightColor);
            RTGizmos.DrawSegment(mTarget.transform.position, mOuterSpotCap1.transform.position, globalStyle.lightColor);

            // Draw the snap cap ray
            RTGizmos.DrawSegment(mTarget.transform.position, mSnapCap.transform.position, globalStyle.lightSnapRayColor);

            // Render spot circles and angle arcs
            mInnerSpotCircle.Render(mHRenderArgs);
            mOuterSpotCircle.Render(mHRenderArgs);
            mAzimuthArc.Render(mHRenderArgs);
            mElevationArc.Render(mHRenderArgs);

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.Add(mRangeCap);
            mCapsBuffer.Add(mSnapCap);
            mCapsBuffer.Add(mInnerSpotCap0);
            mCapsBuffer.Add(mInnerSpotCap1);
            mCapsBuffer.Add(mOuterSpotCap0);
            mCapsBuffer.Add(mOuterSpotCap1);
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

            // Match snap handle
            if (dragHandle == mSnapCap)
            {
                // Start dragging
                desc.dragType   = EGizmoDragType.SurfaceSnap;
                return mDrag.Start(desc, this, mSnapCap.transform, camera) ? mDrag : null; 
            }
            else
            // Match range handle
            if (dragHandle == mRangeCap)
            {
                // Set drag start range
                mDragStartRange = mTarget.range;

                // Start dragging
                mDrag.scaleSnap = RTGizmos.get.skin.globalGizmoStyle.lightRangeSnap;
                mDrag.snapMode  = EGizmoDragSnapMode.UseGlobal;
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = mTarget.transform.forward;
                desc.axisIndex0 = 2;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;   
            }
            // Match inner & outer spot handles
            else
            if (dragHandle == mInnerSpotCap0 || dragHandle == mInnerSpotCap1 ||
                dragHandle == mOuterSpotCap0 || dragHandle == mOuterSpotCap1)
            {
                // Update the starting arc length
                if (dragHandle == mInnerSpotCap0 ||
                    dragHandle == mInnerSpotCap1) mDragStarArcLength = MathEx.DegreesToArcLength(mTarget.innerSpotAngle / 2.0f, mTarget.range);
                else mDragStarArcLength = MathEx.DegreesToArcLength(mTarget.spotAngle / 2.0f, mTarget.range);

                // Start dragging
                mDrag.snapMode  = EGizmoDragSnapMode.Disabled;      // No snapping for the spot angle
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = new Plane(mTarget.transform.forward, Vector3.zero).ProjectPoint(dragHandle.transform.position - mRangeCap.transform.position).normalized;
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
            // Snap direction?
            if (dragData.handle == mSnapCap)
            {
                RTUndo.get.Record(mTarget.transform);
                mTarget.transform.rotation = QuaternionEx.LookRotationEx((mSnapCap.transform.position - mTarget.transform.position), Vector3.up);
            }
            else
            // Change range?
            if (dragData.handle == mRangeCap)
            { 
                RTUndo.get.Record(mTarget);
                mTarget.range = mDragStartRange + mDrag.totalDrag[mDrag.desc.axisIndex0];

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    mTarget.range = MathEx.FixFloatError(mTarget.range);
            }
            // Change inner spot
            else
            if (dragData.handle == mInnerSpotCap0 || dragData.handle == mInnerSpotCap1)
            {
                // Note: See outer spot drag.
                float arcLength = mDragStarArcLength + mDrag.totalDrag[mDrag.desc.axisIndex0];

                // Recalculate the angle
                RTUndo.get.Record(mTarget);
                mTarget.innerSpotAngle = 2.0f * MathEx.ArcLengthToDegrees(arcLength, mTarget.range);
            }
            // Change outer spot
            else
            if (dragData.handle == mOuterSpotCap0 || dragData.handle == mOuterSpotCap1)
            {
                // Dragging the arc length produces the best results in terms of UX. Other methods tried:
                //  1. drag the angle value directly;
                //  2. drag the length of the opposite side of the RA triangle;
                // The arc length seems to work best and it comes really close to how the spot light gizmo
                // behaves in Unity. Also, we use the total drag distance to prevent interaction when the
                // cursor is too far from the drag handle. This feels more natural when dragging towards
                // 179 degrees and then back towards a smaller angle. Dragging in the other direction
                // will only occur when the cursor returns 'home', in the proximity of the drag handle.
                float arcLength = mDragStarArcLength + mDrag.totalDrag[mDrag.desc.axisIndex0];

                // Recalculate the angle
                RTUndo.get.Record(mTarget);
                mTarget.spotAngle = 2.0f * MathEx.ArcLengthToDegrees(arcLength, mTarget.range);
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnGUI() (Protected Function)
        // Desc: Called during the 'OnGUI' event to allow the gizmo to draw GUI elements.
        // Parm: camera - The camera that renders the GUI.
        //-----------------------------------------------------------------------------
        protected override void OnGUI(Camera camera)
        {
            // Are we dragging the snap cap?
            if (dragData.handle == mSnapCap)
            {
                if (activeStyle.directionLabelVisible)
                {
                    // Draw light direction at light position
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(new Box(mTarget.transform.position, Vector3Ex.FromValue(1e-1f)),
                        camera, "Direction: " + mTarget.transform.forward.ToString("F3"), EGizmoTextType.DragInfo);

                }

                if (activeStyle.targetLabelVisible)
                {
                    // Draw look-at point
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(mSnapCap.CalculateAABB(camera),
                        camera, "Target: " + mSnapCap.transform.position.ToString("F3"), EGizmoTextType.DragInfo);
                }
            }
            // Are we dragging the range cap?
            else
            if (dragData.handle == mRangeCap)
            {
                // Draw range
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Range: " + mTarget.range.ToString("F3"), EGizmoTextType.DragInfo);
            }
            // Are we dragging the inner spot caps?
            else
            if (dragData.handle == mInnerSpotCap0 ||
                dragData.handle == mInnerSpotCap1)
            {
                // Draw inner spot angle
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Inner spot: " + mTarget.innerSpotAngle.ToString("F3"), EGizmoTextType.DragInfo);
            }
            // Are we dragging the outer spot caps?
            else
            if (dragData.handle == mOuterSpotCap0 ||
                dragData.handle == mOuterSpotCap1)
            {
                // Draw outer spot angle
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Outer spot: " + mTarget.spotAngle.ToString("F3"), EGizmoTextType.DragInfo);
            }
        }
        #endregion
    }
    #endregion
}
