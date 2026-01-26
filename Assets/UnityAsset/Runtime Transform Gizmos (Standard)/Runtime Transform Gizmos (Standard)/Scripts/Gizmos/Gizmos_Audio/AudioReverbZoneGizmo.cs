using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: AudioReverbZoneGizmo (Public Class)
    // Desc: Implements an audio reverb zone gizmo.
    //-----------------------------------------------------------------------------
    public class AudioReverbZoneGizmo : Gizmo
    {
        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();                                          // Gizmo handle render args
        
        GizmoCap[]              mMinCircleCaps      = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // The circle caps which make up the min distance sphere
        GizmoCap[]              mMaxCircleCaps      = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // The circle caps which make up the max distance sphere
        
        GizmoCap                mMinSphere          = new GizmoCap();                                                       // The min distance sphere. Used to draw the min sphere border.
        GizmoCap                mMaxSphere          = new GizmoCap();                                                       // The max distance sphere. Used to draw the max sphere border.
        
        GizmoCap[]              mMinDistanceCapsP   = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive min distance caps
        GizmoCap[]              mMinDistanceCapsN   = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative min distance caps
        GizmoCap[]              mMaxDistanceCapsP   = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive max distance caps
        GizmoCap[]              mMaxDistanceCapsN   = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative max distance caps

        float                   mDragStartDistance;     // Distance value (min or max) when the user starts dragging
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the audio reverb zone gizmo style.
        //-----------------------------------------------------------------------------
        public AudioReverbZoneGizmoStyle    style       { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's audio reverb zone
        //       gizmo style.
        //-----------------------------------------------------------------------------
        public AudioReverbZoneGizmoStyle    activeStyle { get { return style == null ? RTGizmos.get.skin.audioReverbZoneGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target audio reverb zone.
        //-----------------------------------------------------------------------------
        public AudioReverbZone              target      { get; set; }      
        #endregion

        #region Protected Functions
        //-----------------------------------------------------------------------------
        // Name: IsReady() (Protected Function)
        // Desc: Checks if the gizmo is ready to use.
        // Rtrn: True if the gizmo is ready to use and false otherwise.
        //-----------------------------------------------------------------------------
        protected override bool IsReady()
        {
            return target && target.gameObject.activeInHierarchy;
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
            int count = mMinDistanceCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store
                handles.Add(mMinDistanceCapsP[i]);
                handles.Add(mMinDistanceCapsN[i]);
                handles.Add(mMaxDistanceCapsP[i]);
                handles.Add(mMaxDistanceCapsN[i]);
                handles.Add(mMinCircleCaps[i]);
                handles.Add(mMaxCircleCaps[i]);

                // Init
                mMinDistanceCapsP[i].cullHoveredPixels      = false;
                mMinDistanceCapsN[i].cullHoveredPixels      = false;
                mMaxDistanceCapsP[i].cullHoveredPixels      = false;
                mMaxDistanceCapsN[i].cullHoveredPixels      = false;

                mMinCircleCaps[i].hoverable                 = false;
                mMinCircleCaps[i].alignFlatToCamera         = false;
                mMinCircleCaps[i].capStyle.capType          = EGizmoCapType.WireCircle;
                mMinCircleCaps[i].zoomScaleMode             = EGizmoHandleZoomScaleMode.One;
                mMinCircleCaps[i].transformScaleMode        = EGizmoHandleTransformScaleMode.One;

                mMaxCircleCaps[i].hoverable                 = false;
                mMaxCircleCaps[i].alignFlatToCamera         = false;
                mMaxCircleCaps[i].capStyle.capType          = EGizmoCapType.WireCircle;
                mMaxCircleCaps[i].zoomScaleMode             = EGizmoHandleZoomScaleMode.One;
                mMaxCircleCaps[i].transformScaleMode        = EGizmoHandleTransformScaleMode.One;

                // Enable culling
                mMinCircleCaps[i].cullSphereEnabled         = true;
                mMaxCircleCaps[i].cullSphereEnabled         = true;

                mMinDistanceCapsP[i].cullSphereEnabled      = true;
                mMinDistanceCapsN[i].cullSphereEnabled      = true;
                mMinDistanceCapsP[i].cullSphereMode         = EGizmoCullSphereMode.Behind;
                mMinDistanceCapsN[i].cullSphereMode         = EGizmoCullSphereMode.Behind;
                mMinDistanceCapsP[i].cullSphereTargets      = EGizmoHandleCullSphereTargets.Render;
                mMinDistanceCapsN[i].cullSphereTargets      = EGizmoHandleCullSphereTargets.Render;

                mMaxDistanceCapsP[i].cullSphereEnabled      = true;
                mMaxDistanceCapsN[i].cullSphereEnabled      = true;
                mMaxDistanceCapsP[i].cullSphereMode         = EGizmoCullSphereMode.Behind;
                mMaxDistanceCapsN[i].cullSphereMode         = EGizmoCullSphereMode.Behind;
                mMaxDistanceCapsP[i].cullSphereTargets      = EGizmoHandleCullSphereTargets.Render;
                mMaxDistanceCapsN[i].cullSphereTargets      = EGizmoHandleCullSphereTargets.Render;
            }

            // Min/max zone spheres
            handles.Add(mMinSphere);
            handles.Add(mMaxSphere);

            mMinSphere.capStyle.capType    = EGizmoCapType.Sphere;
            mMinSphere.hoverable           = false;
            mMinSphere.zoomScaleMode       = EGizmoHandleZoomScaleMode.One;
            mMinSphere.transformScaleMode  = EGizmoHandleTransformScaleMode.One;

            mMaxSphere.capStyle.capType    = EGizmoCapType.Sphere;
            mMaxSphere.hoverable           = false;
            mMaxSphere.zoomScaleMode       = EGizmoHandleZoomScaleMode.One;
            mMaxSphere.transformScaleMode  = EGizmoHandleTransformScaleMode.One;
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
            Sphere minSphere = new Sphere(target.transform.position, target.minDistance);
            Sphere maxSphere = new Sphere(target.transform.position, target.maxDistance);

            // Update caps and circles
            var globalStyle = RTGizmos.get.skin.globalGizmoStyle;
            int count = mMinDistanceCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Distance caps
                mMinDistanceCapsP[i].capStyle = activeStyle.distanceCapStyle;
                mMinDistanceCapsN[i].capStyle = activeStyle.distanceCapStyle;
                mMaxDistanceCapsP[i].capStyle = activeStyle.distanceCapStyle;
                mMaxDistanceCapsN[i].capStyle = activeStyle.distanceCapStyle;

                // Circle caps
                mMinCircleCaps[i].capStyle.color           = activeStyle.color;
                mMinCircleCaps[i].capStyle.circleRadius    = minSphere.radius;
                mMaxCircleCaps[i].capStyle.color           = activeStyle.color;
                mMaxCircleCaps[i].capStyle.circleRadius    = maxSphere.radius;

                // Set cull sphere
                mMinCircleCaps[i].cullSphere        = minSphere;
                mMaxCircleCaps[i].cullSphere        = maxSphere;
                mMinDistanceCapsP[i].cullSphere     = minSphere;
                mMinDistanceCapsN[i].cullSphere     = minSphere;
                mMaxDistanceCapsP[i].cullSphere     = maxSphere;
                mMaxDistanceCapsN[i].cullSphere     = maxSphere;
            }

            // Update transforms
            transform.position = target.transform.position;
            for (int i = 0; i < count; ++i)
            {
                // Set distance caps' position
                mMinDistanceCapsP[i].transform.position = transform.position + Core.axes[i] * minSphere.radius;
                mMinDistanceCapsN[i].transform.position = transform.position - Core.axes[i] * minSphere.radius;
                mMaxDistanceCapsP[i].transform.position = transform.position + Core.axes[i] * maxSphere.radius;
                mMaxDistanceCapsN[i].transform.position = transform.position - Core.axes[i] * maxSphere.radius;

                // Set circle caps' rotation
                mMinCircleCaps[i].transform.rotation = QuaternionEx.LookRotationEx(Core.axes[i], Vector3.up);
                mMaxCircleCaps[i].transform.rotation = mMinCircleCaps[i].transform.rotation;
            }

            // Min zone sphere
            mMinSphere.capStyle.color                   = activeStyle.color.Alpha(0.0f);
            mMinSphere.capStyle.sphereBorderVisible     = true;
            mMinSphere.capStyle.sphereBorderColor       = activeStyle.color;
            mMinSphere.capStyle.sphereRadius            = minSphere.radius;
            mMinSphere.transform.position               = minSphere.center;

            // Max zone sphere
            mMaxSphere.capStyle.color                   = activeStyle.color.Alpha(0.0f);
            mMaxSphere.capStyle.sphereBorderVisible     = true;
            mMaxSphere.capStyle.sphereBorderColor       = activeStyle.color;
            mMaxSphere.capStyle.sphereRadius            = maxSphere.radius;
            mMaxSphere.transform.position               = maxSphere.center;

            // Update drag
            mDrag.scaleSnap                 = globalStyle.audioSnap;
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
            mHRenderArgs.camera    = camera;
            mHRenderArgs.rasterCtx = rasterCtx;

            // Render min/max spheres
            mMinSphere.Render(mHRenderArgs);
            mMaxSphere.Render(mHRenderArgs);

            // Render circles
            int count = mMinCircleCaps.Length;
            for (int i = 0; i < count; ++i)
            {
                mMinCircleCaps[i].Render(mHRenderArgs);
                mMaxCircleCaps[i].Render(mHRenderArgs);
            }

            // Sort caps
            mCapsBuffer.Clear();
            mCapsBuffer.AddRange(mMinDistanceCapsP);
            mCapsBuffer.AddRange(mMinDistanceCapsN);
            mCapsBuffer.AddRange(mMaxDistanceCapsP);
            mCapsBuffer.AddRange(mMaxDistanceCapsN);
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

            // Set drag start data
            mDragStartDistance = (ArrayHasDragHandle(mMinDistanceCapsP) ||
                                  ArrayHasDragHandle(mMinDistanceCapsN)) ? target.minDistance : target.maxDistance;

            // Match drag handle
            int count = mMinDistanceCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Match positive distance cap
                if (dragHandle == mMinDistanceCapsP[i] ||
                    dragHandle == mMaxDistanceCapsP[i])
                {
                    // Start dragging
                    desc.dragType   = EGizmoDragType.Scale;
                    desc.axis0      = Core.axes[i];
                    desc.axisIndex0 = i;
                    return mDrag.Start(desc, this, transform, camera) ? mDrag : null;            
                }
                else
                // Match negative distance cap
                if (dragHandle == mMinDistanceCapsN[i] ||
                    dragHandle == mMaxDistanceCapsN[i])
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
            // Update zone min/max distance
            RTUndo.get.Record(target);
            if (ArrayHasDragHandle(mMinDistanceCapsP) ||
                ArrayHasDragHandle(mMinDistanceCapsN))
            {
                target.minDistance = mDragStartDistance + mDrag.totalDrag[mDrag.desc.axisIndex0];

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.minDistance = MathEx.FixFloatError(target.minDistance);
            }
            else
            {
                target.maxDistance = mDragStartDistance + mDrag.totalDrag[mDrag.desc.axisIndex0];

                // Fix float errors when snapping is enabled
                if (mDrag.IsSnapEnabled())
                    target.maxDistance = MathEx.FixFloatError(target.maxDistance);
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
                // Draw label at the top of the drag handle. This label shows the distance.
                if (ArrayHasDragHandle(mMinDistanceCapsP) ||
                    ArrayHasDragHandle(mMinDistanceCapsN))
                {
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                        camera, "Min distance: " + target.minDistance.ToString("F3"), EGizmoTextType.DragInfo);
                }
                else
                {
                    RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                        camera, "Max distance: " + target.maxDistance.ToString("F3"), EGizmoTextType.DragInfo);
                }
            }
        }
        #endregion
    }
    #endregion
}
