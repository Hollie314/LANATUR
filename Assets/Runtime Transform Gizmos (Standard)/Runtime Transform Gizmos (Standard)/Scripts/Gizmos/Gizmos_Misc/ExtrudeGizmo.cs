using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace RTGStandard
{
    #region Public Enumerations
    //-----------------------------------------------------------------------------
    // Name: EGizmoExtrudeSpace (Public Enum)
    // Desc: Defines the space in which the extrude gizmo operates.
    //-----------------------------------------------------------------------------
    public enum EGizmoExtrudeSpace
    {
        Global = 0, // The gizmo aligns to world axes
        Local       // The gizmo aligns to the target object's local axes
    }
    #endregion

    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: ExtrudeGizmo (Public Class)
    // Desc: Implements an extrude gizmo.
    //-----------------------------------------------------------------------------
    public class ExtrudeGizmo : Gizmo
    {
        #region Public Delegates & Events
        //-----------------------------------------------------------------------------
        // Name: IgnoreOverlapHandler() (Public Delegate)
        // Desc: Handler for the event fired to determine whether an overlapped object
        //       should be ignored during the spawn phase.
        // Parm: gameObject - The intersecting 'GameObject'.
        //       answer     - Listeners can use this to answer 'yes' or 'no'.
        //-----------------------------------------------------------------------------
        public delegate void    IgnoreOverlapHandler(GameObject gameObject, BinaryAnswer answer);
        public event            IgnoreOverlapHandler ignoreOverlapQuery;
        #endregion

        #region Private Fields
        GizmoDrag               mDrag               = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs        = new GizmoHandleRenderArgs();  // Gizmo handle render args
        GizmoCap[]              mExtrudeCapsP       = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Positive extrude caps for X, Y, Z
        GizmoCap[]              mExtrudeCapsN       = new GizmoCap[3] { new GizmoCap(), new GizmoCap(), new GizmoCap() };   // Negative extrude caps for X, Y, Z
        GizmoPlaneSlider[]      mDblAxisSliders     = new GizmoPlaneSlider[3]{ new GizmoPlaneSlider(), new GizmoPlaneSlider(), new GizmoPlaneSlider() };    // Double-axis sliders to allow extrusion along 2 axes at once
        GizmoCap                mBox                = new GizmoCap();               // The box handle (non-interactive)
        GizmoCap                mGhostBoxCap        = new GizmoCap();               // The ghost box cap (non-interactive). Shows the extrusion volume.

        ObjectFilter            mOverlapFiler       = new ObjectFilter
        { objectTypes = EGameObjectType.Mesh | EGameObjectType.Sprite };            // The overlap filter used when checking for overlaps
        IList<GameObject>       mTargets;                                           // Points to the target objects
        List<GameObject>        mTargetParents      = new List<GameObject>();       // The target parents the gizmo operates on
        OBox                    mTargetBox          = OBox.GetInvalid();            // The target box volume
        EGizmoExtrudeSpace      mExtrudeSpace       = EGizmoExtrudeSpace.Global;    // Extrude space

        Dictionary<Transform, Vector3>  mTargetAnchorMap        = new();                // Maps each target to its offset from the box center at drag start
        Vector3                         mDragStartCenter;                               // The target box center on drag start
        BinaryAnswer                    mIgnoreOverlapAnswer    = new BinaryAnswer();   // Used when asking if an overlap should be ignored

        // Used to avoid memory allocations    
        List<GameObject>    mSpawnedObjectsBuffer   = new List<GameObject>();      
        List<GameObject>    mObjectBuffer           = new List<GameObject>();
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the extrude gizmo style.
        //-----------------------------------------------------------------------------
        public ExtrudeGizmoStyle    style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's extrude gizmo style.
        //-----------------------------------------------------------------------------
        public ExtrudeGizmoStyle    activeStyle     { get { return style == null ? RTGizmos.get.skin.extrudeGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: extrudeSpace (Public Property)
        // Desc: Returns or sets the extrude space (world or local).
        //-----------------------------------------------------------------------------
        public EGizmoExtrudeSpace   extrudeSpace    { get { return mExtrudeSpace; } set { if (value != mExtrudeSpace) { mExtrudeSpace = value; UpdateTargetBox(); } } }

        //-----------------------------------------------------------------------------
        // Name: avoidOverlaps (Public Property)
        // Desc: Returns or sets whether to skip extrusion in locations that would
        //       overlap existing scene objects.
        //-----------------------------------------------------------------------------
        public bool                 avoidOverlaps   { get; set; } = true;

        //-----------------------------------------------------------------------------
        // Name: overlapPadding (Public Property)
        // Desc: Returns or sets the padding applied to the overlap test box during
        //       extrusion. Negative values will shrink the box, positive values inflate it.
        //-----------------------------------------------------------------------------
        public Vector3              overlapPadding  { get; set; } = Vector3Ex.FromValue(-1e-1f);

        //-----------------------------------------------------------------------------
        // Name: targetCount (Public Property)
        // Desc: Returns the number of target objects.
        //-----------------------------------------------------------------------------
        public int                  targetCount     { get { return mTargets != null ? mTargets.Count : 0; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: SetTargets() (Public Function)
        // Desc: Sets the target game objects that the gizmo operates on. Only the root
        //       parents are stored internally, allowing the gizmo to treat entire
        //       hierarchies as a single target entity.
        // Parm: targets - The list of target game objects.
        //-----------------------------------------------------------------------------
        public void SetTargets(IList<GameObject> targets)
        {
            // Validate call
            if (targets == null)
            {
                mTargets = null;
                mTargetParents.Clear();
                mTargetBox = OBox.GetInvalid();
                return;
            }

            // Store targets
            mTargets = targets;

            // Collect root parents only and update the target box
            GameObjectEx.CollectParents(targets, mTargetParents);
            UpdateTargetBox();
        }

        //-----------------------------------------------------------------------------
        // Name: Refresh() (Public Function)
        // Desc: Should be called during any one of the following scenarios:
        //          a) the client has manually changed the target objects' transform.
        //          b) the client has added or removed objects to the target collection. 
        //          c) the client has made changes to the target objects (e.g. activated or
        //             deactivated them, added or removed components etc).
        //       The function exits immediately if no targets are assigned.
        //-----------------------------------------------------------------------------
        public void Refresh()
        {
            // Validate call
            if (targetCount == 0)
                return;

            // Collect parents
            GameObjectEx.CollectParents(mTargets, mTargetParents);

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
            // Store extrude caps
            int count = mExtrudeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Store and set tags
                handles.Add(mExtrudeCapsP[i]);
                handles.Add(mExtrudeCapsN[i]);

                mExtrudeCapsP[i].tag = (EGizmoHandleTag)((int)EGizmoHandleTag.X + i);
                mExtrudeCapsN[i].tag = (EGizmoHandleTag)((int)EGizmoHandleTag.X + i);
            }

            // Store box caps
            handles.Add(mBox);
            handles.Add(mGhostBoxCap);

            // Store dbl-axis sliders
            count = mDblAxisSliders.Length;
            for (int i = 0; i < count; ++i)
            {
                handles.Add(mDblAxisSliders[i]);
                mDblAxisSliders[i].tag = (EGizmoHandleTag)((int)EGizmoHandleTag.XY + i);

                // In order for these sliders to sit nicely in a corner, they need to use
                // a common zoom scale value. Otherwise, they will be out of sync.
                mDblAxisSliders[i].zoomScaleMode    = EGizmoHandleZoomScaleMode.FromGizmo;
                mDblAxisSliders[i].zoomScaleGizmo   = this;
            }

            // Init
            mBox.hoverable                  = false;
            mGhostBoxCap.hoverable          = false;
            mBox.capStyle.capType           = EGizmoCapType.WireBox;
            mBox.zoomScaleMode              = EGizmoHandleZoomScaleMode.One;
            mBox.transformScaleMode         = EGizmoHandleTransformScaleMode.One;
            mGhostBoxCap.capStyle.capType   = EGizmoCapType.WireBox;
            mGhostBoxCap.zoomScaleMode      = EGizmoHandleZoomScaleMode.One;
            mGhostBoxCap.transformScaleMode = EGizmoHandleTransformScaleMode.One;
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

            // Loop through each extrude cap and update its style
            int count = mExtrudeCapsP.Length;
            for (int i = 0; i < count; ++i)
            {
                // Cache data
                var color   = RTGizmos.get.skin.globalGizmoStyle.GetAxisColor(i);

                // Set styles and color
                mExtrudeCapsP[i].capStyle           = activeStyle.GetExtrudeCapStyle(i, true);
                mExtrudeCapsP[i].capStyle.color     = color;

                mExtrudeCapsN[i].capStyle           = activeStyle.GetExtrudeCapStyle(i, false);
                mExtrudeCapsN[i].capStyle.color     = color;
            }

            // Loop through each extrude cap and update its transform
            count           = mExtrudeCapsP.Length;
            Vector3 extents = mTargetBox.extents;
            for (int i = 0; i < count; ++i)
            {
                // Cache data
                Vector3 axis     = mTargetBox.rotation * Core.axes[i];
                float extent     = extents[i];
                Vector3 center   = mTargetBox.center;

                // Positive cap
                mExtrudeCapsP[i].transform.position = center + axis * (extent + activeStyle.extrudeCapOffset);
                mExtrudeCapsP[i].transform.rotation = Quaternion.FromToRotation(Vector3.right, axis);

                // Negative cap
                mExtrudeCapsN[i].transform.position = center - axis * (extent + activeStyle.extrudeCapOffset);
                mExtrudeCapsN[i].transform.rotation = Quaternion.FromToRotation(Vector3.right, -axis);
            }

            // Update dbl-axis sliders
            var globalStyle                 = RTGizmos.get.skin.globalGizmoStyle;
            var dblAxisStyle                = activeStyle.GetDblAxisSliderStyle(EPlane.XY);
            dblAxisStyle.color              = globalStyle.GetAxisColor(2).Alpha(globalStyle.planeSliderAlpha);
            dblAxisStyle.borderColor        = globalStyle.GetAxisColor(2);
            mDblAxisSliders[(int)EPlane.XY].sliderStyle = dblAxisStyle;

            dblAxisStyle                    = activeStyle.GetDblAxisSliderStyle(EPlane.YZ);
            dblAxisStyle.color              = globalStyle.GetAxisColor(0).Alpha(globalStyle.planeSliderAlpha);
            dblAxisStyle.borderColor        = globalStyle.GetAxisColor(0);
            mDblAxisSliders[(int)EPlane.YZ].sliderStyle = dblAxisStyle;

            dblAxisStyle                    = activeStyle.GetDblAxisSliderStyle(EPlane.ZX);
            dblAxisStyle.color              = globalStyle.GetAxisColor(1).Alpha(globalStyle.planeSliderAlpha);
            dblAxisStyle.borderColor        = globalStyle.GetAxisColor(1);
            mDblAxisSliders[(int)EPlane.ZX].sliderStyle = dblAxisStyle;

            // Make dbl-axis planes sit between the slider pairs.
            // Note: The way we specify the slider indices affects the way in which the drag operation is setup inside 'OnStartDrag'.
            mDblAxisSliders[(int)EPlane.XY].SitRightAngle(transform.position, transform.right,      transform.up,       activeStyle.dblAxisOffset, camera, true);
            mDblAxisSliders[(int)EPlane.YZ].SitRightAngle(transform.position, transform.up,         transform.forward,  activeStyle.dblAxisOffset, camera, true);
            mDblAxisSliders[(int)EPlane.ZX].SitRightAngle(transform.position, transform.forward,    transform.right,    activeStyle.dblAxisOffset, camera, true);

            // Update box cap style
            mBox.capStyle.color     = activeStyle.boxColor;
            mBox.capStyle.boxSize   = mTargetBox.size;

            // Update ghost box cap style
            mGhostBoxCap.capStyle.color     = activeStyle.ghostBoxColor;

            // Hide handles which would otherwise extrude a 0 sized volume. 
            Vector3 size = Vector3Ex.Abs(mTargetBox.size);
            if (size.x < 1e-4f)
            {
                mExtrudeCapsP[0].visible = false;
                mExtrudeCapsN[0].visible = false;
            }
            else
            {
                mExtrudeCapsP[0].visible = true;
                mExtrudeCapsN[0].visible = true;
            }
            if (size.y < 1e-4f)
            {
                mExtrudeCapsP[1].visible = false;
                mExtrudeCapsN[1].visible = false;
            }
            else
            {
                mExtrudeCapsP[1].visible = true;
                mExtrudeCapsN[1].visible = true;
            }
            if (size.z < 1e-4f)
            {
                mExtrudeCapsP[2].visible = false;
                mExtrudeCapsN[2].visible = false;
            }
            else
            {
                mExtrudeCapsP[2].visible = true;
                mExtrudeCapsN[2].visible = true;
            }

            mDblAxisSliders[(int)EPlane.XY].visible = (mExtrudeCapsP[0].visible && mExtrudeCapsP[1].visible);
            mDblAxisSliders[(int)EPlane.YZ].visible = (mExtrudeCapsP[1].visible && mExtrudeCapsP[2].visible);
            mDblAxisSliders[(int)EPlane.ZX].visible = (mExtrudeCapsP[2].visible && mExtrudeCapsP[0].visible);
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
            mCapsBuffer.AddRange(mExtrudeCapsP);
            mCapsBuffer.AddRange(mExtrudeCapsN);

            // Render box
            mBox.Render(mHRenderArgs);

            // Render ghost box if we're dragging
            if (dragging)
            {
                // Cache data
                int a0              = mDrag.desc.axisIndex0;
                int a1              = mDrag.desc.axisIndex1;
                Vector3 centerDir   = mTargetBox.center - mDragStartCenter;

                // Calculate ghost box size along the first axis
                Vector3 ghostSize   = mTargetBox.size;
                ghostSize[a0]       = Vector3Ex.AbsDot(centerDir, mDrag.desc.axis0) + mTargetBox.size[a0];

                // If we're doing a dbl-axis drag, calculate the box size on the second axis also
                bool dblAxisDrag    = ArrayHasDragHandle(mDblAxisSliders);
                if (dblAxisDrag)
                    ghostSize[a1]   = Vector3Ex.AbsDot(centerDir, mDrag.desc.axis1) + mTargetBox.size[a1];

                // Set the new box size
                mGhostBoxCap.capStyle.boxSize = ghostSize;

                // Is the ghost box valid?
                if (ghostSize.Count0() <= 1)
                {
                    // Calculate box position and render box
                    mGhostBoxCap.transform.position = (mTargetBox.center + mDragStartCenter) / 2.0f;
                    mGhostBoxCap.Render(mHRenderArgs);
                }
            }

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
            GizmoHandle dragHandle  = dragData.handle;
            GizmoDragDesc desc      = new GizmoDragDesc();

            // Update drag
            mDrag.moveGridSnapDescSource    = EGizmoDragSettingSource.Custom;
            var gridSnapDesc                = new GridSnapDesc();
            gridSnapDesc.cellSize           = mTargetBox.size;
            gridSnapDesc.right              = transform.right;
            gridSnapDesc.up                 = transform.up;
            gridSnapDesc.forward            = transform.forward;
            gridSnapDesc.origin             = transform.position;
            mDrag.moveGridSnapDesc          = gridSnapDesc;
            mDrag.snapMode                  = EGizmoDragSnapMode.Enabled;

            // Set drag start data
            mDragStartCenter = mTargetBox.center;

            // Cache each target's anchor offset relative to the box center
            mTargetAnchorMap.Clear();
            int count = mTargetParents.Count;
            for (int i = 0; i < count; ++i)
            {
                Transform transform = mTargetParents[i].transform;
                mTargetAnchorMap[transform] = transform.position - mDragStartCenter;
            }

            // Are we dragging the extrude caps?
            if (ArrayHasDragHandle(mExtrudeCapsP) || 
                ArrayHasDragHandle(mExtrudeCapsN))
            {
                // Match caps
                count = mExtrudeCapsP.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Match extrude caps
                    if (dragHandle == mExtrudeCapsP[i])
                    {
                        // Start dragging
                        desc.dragType   = EGizmoDragType.Move;
                        desc.axis0      = mExtrudeCapsP[i].transform.position - mTargetBox.center;
                        desc.axisIndex0 = i;
                        return mDrag.Start(desc, this, transform, camera) ? mDrag : null;            
                    }
                    else
                    if (dragHandle == mExtrudeCapsN[i])
                    {
                        // Start dragging
                        desc.dragType   = EGizmoDragType.Move;
                        desc.axis0      = mExtrudeCapsN[i].transform.position - mTargetBox.center;
                        desc.axisIndex0 = i;
                        return mDrag.Start(desc, this, transform, camera) ? mDrag : null;
                    }
                }
            }
            else
            // Are we dragging the dbl-axis sliders?
            if (ArrayHasDragHandle(mDblAxisSliders))
            {
                // Match slider
                count = mDblAxisSliders.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Match slider
                    if (dragHandle == mDblAxisSliders[i])
                    {
                        // Start dragging
                        desc.dragType   = EGizmoDragType.DblMove;
                        desc.axis0      = mDblAxisSliders[i].slideDirection0;
                        desc.axis1      = mDblAxisSliders[i].slideDirection1;
                        desc.axisIndex0 = i;
                        desc.axisIndex1 = (i + 1) % 3;
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
            // Did we actually drag?
            if (drag.dragDelta.magnitude != 0.0f)
            {
                // Update the box position as we're dragging the gizmo
                mTargetBox.center = transform.position;

                // Offset targets
                int count = mTargetParents.Count;
                for (int i = 0; i < count; ++i)
                {
                    RTUndo.get.Record(mTargetParents[i].transform);
                    mTargetParents[i].transform.position += drag.dragDelta;
                    RTScene.get.OnObjectTransformChanged(mTargetParents[i]);
                }
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnDragEnd() (Protected Function)
        // Desc: Called when the gizmo's drag operation ends.
        // Parm: drag - The drag operation that ended.
        //-----------------------------------------------------------------------------
        protected override void OnDragEnd(GizmoDrag drag)
        {
            // Cache data.
            // Note: We will set 'axis1' to zero if we're not dbl-axis dragging. This
            //       allows us to treat the spawn area as a grid even if we are doing
            //       a single-axis drag.
            bool    dblAxisDrag     = ArrayHasDragHandle(mDblAxisSliders); 
            Vector3 axis0           = drag.desc.axis0;
            Vector3 axis1           = dblAxisDrag ? drag.desc.axis1 : Vector3.zero;
            Vector3 centerDir       = mTargetBox.center - mDragStartCenter;
            float   dot0            = Vector3.Dot(centerDir, axis0);
            float   dot1            = Vector3.Dot(centerDir, axis1);
            var     dragDesc        = drag.desc;

            // Reverse axes if we've been dragging in the other direction
            if (dot0 < 0.0f) axis0 = -axis0;
            if (dot1 < 0.0f) axis1 = -axis1;

            // Calculate spawn grid size
            Vector2Int gridSize     = CalculateSpawnGridSize();
            int     columnCount     = gridSize.x;
            int     rowCount        = gridSize.y;

            // Clear object spawn buffer
            mSpawnedObjectsBuffer.Clear();

            // Are we avoiding overlaps?
            if (avoidOverlaps)
            {              
                // Setup object filter
                GameObjectEx.CollectAllObjects(mTargetParents, false, mObjectBuffer);
                mOverlapFiler.SetIgnoredObjects(mObjectBuffer);
                mOverlapFiler.customFilter = (GameObject go) =>
                {
                    // Ask for permission to overlap with this object
                    mIgnoreOverlapAnswer.Clear();
                    if (ignoreOverlapQuery != null) ignoreOverlapQuery(go, mIgnoreOverlapAnswer);
                    if (mIgnoreOverlapAnswer.noCount != 0) return false;

                    // If the object is part of the spawned objects list, ignore
                    return !mSpawnedObjectsBuffer.Contains(go); 
                };
            }

            // Establish bounds query config for overlap detection
            BoundsQueryConfig overlapBoundsQCOnfig = BoundsQueryConfig.defaultConfig;
            overlapBoundsQCOnfig.objectTypes = EGameObjectType.Mesh | EGameObjectType.Sprite;

            // Loop through each row (this is 1 if we're doing single axis drag)
            for (int r = 0; r < rowCount; ++r)
            {
                // Calculate group center for this row
                Vector3 groupCenter_Row = mDragStartCenter + axis1 * r * mTargetBox.size[dragDesc.axisIndex1];

                // Loop through each column
                for (int c = 0; c < columnCount; ++c)
                {
                    // Ignore last row & col to avoid overlap with the original targets
                    if (dblAxisDrag && c == (columnCount - 1) && r == (rowCount - 1))
                        continue;

                    // Calculate group center for this column
                    Vector3 groupCenter_Col = groupCenter_Row + axis0 * c * mTargetBox.size[dragDesc.axisIndex0];

                    // Loop through each target inside the target map and instantiate it in the right position
                    foreach (var pair in mTargetAnchorMap)
                    {
                        // Get offset to box center
                        Transform transform = pair.Key;
                        Vector3 anchor      = pair.Value;

                        // Instantiate a copy of the original target's parent at the computed position
                        Vector3 spawnPosition = groupCenter_Col + anchor;
                        GameObject clone = GameObject.Instantiate(pair.Key.gameObject, spawnPosition, transform.rotation);
                        clone.transform.parent = transform.parent;
                        clone.transform.SetScale(transform.lossyScale);

                        // Check for overlaps if necessary
                        if (avoidOverlaps)
                        {
                            // If the spawned objects intersects other objects in the scene, destroy it
                            OBox worldOBB       = clone.CalculateHierarchyWorldOBB(overlapBoundsQCOnfig);
                            Vector3 overlapSize = Vector3.Max(worldOBB.size + overlapPadding, Vector3.zero);    // Use 'Max()' manually. 'OBox.size' uses 'Abs()'.
                            worldOBB.size       = overlapSize;
                            if (RTScene.get.BoxOverlap(worldOBB, mOverlapFiler))
                            {
                                GameObject.DestroyImmediate(clone);
                                continue;
                            }
                        }

                        // Notify scene
                        RTScene.get.RegisterObjectHierarchy(clone);

                        // Add this hierarchy to the spawned object list
                        clone.CollectMeAndChildren(false, mSpawnedObjectsBuffer, true);
                    }
                }
            }

            // Record object spawn
            RTUndo.get.RecordObjectsSpawn(mSpawnedObjectsBuffer);
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
                // Identify the axes that are involved in the drag
                bool dblAxisDrag  = ArrayHasDragHandle(mDblAxisSliders);
                string axis0Label = Core.axisNames[mDrag.desc.axisIndex0];
                string axis1Label = dblAxisDrag ? Core.axisNames[mDrag.desc.axisIndex1] : string.Empty;

                // Create the label
                Vector2Int  gridSize    = CalculateSpawnGridSize();
                string      label       = dblAxisDrag ? $"{axis0Label} x {axis1Label}: {gridSize.x} x {gridSize.y}" : $"{axis0Label}: {gridSize.x}";

                // Show the spawn grid size
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_World(mTargetBox.center, camera, label, EGizmoTextType.DragInfo);
            }          
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: UpdateTargetBox() (Private Function)
        // Desc: Updates the target box based on the current extrude space. When no targets
        //       are assigned, the box is marked as invalid.
        //-----------------------------------------------------------------------------
        void UpdateTargetBox()
        {
            // No targets?
            if (targetCount == 0)
            {
                mTargetBox = OBox.GetInvalid();
                return;
            }
  
            // Create the bounds query config
            BoundsQueryConfig boundsQConfig = BoundsQueryConfig.defaultConfig;
            boundsQConfig.objectTypes = EGameObjectType.Mesh | EGameObjectType.Sprite;

            // Update based on extrude space
            if (extrudeSpace == EGizmoExtrudeSpace.Global)
                mTargetBox = new OBox(GameObjectEx.CalculateHierarchiesWorldAABB(mTargetParents, boundsQConfig));
            else
                mTargetBox = GameObjectEx.CalculateHierarchiesWorldOBB(mTargetParents, boundsQConfig);
        }

        //-----------------------------------------------------------------------------
        // Name: CalculateSpawnGridSize() (Private Function)
        // Desc: Calculates and returns the spawn grid size — the number of spawn
        //       target groups along each axis.
        // Rtrn: The spawn grid size. 'x' represents the number of groups along the
        //       first drag axis; 'y' along the second. This will always be at least
        //       one during a valid drag, even for single-axis drags. If the user is
        //       not dragging, or the target box hasn't moved (e.g. drag started but
        //       the mouse hasn't moved enough), the returned value is <0, 0>.
        //-----------------------------------------------------------------------------
        Vector2Int CalculateSpawnGridSize()
        {
            // Are we dragging?
            if (!dragging)
                return Vector2Int.zero;

            // If we didn't move at all, return 0
            if (Vector3.Distance(mDragStartCenter, mTargetBox.center) < 1e-5f)
                return Vector2Int.zero;

            // Cache data
            bool    dblAxisDrag     = ArrayHasDragHandle(mDblAxisSliders); 
            Vector3 axis0           = mDrag.desc.axis0;
            Vector3 axis1           = dblAxisDrag ? mDrag.desc.axis1 : Vector3.zero;
            Vector3 centerDir       = mTargetBox.center - mDragStartCenter;
            float   dot0            = Vector3.Dot(centerDir, axis0);
            float   dot1            = Vector3.Dot(centerDir, axis1);
            var     dragDesc        = mDrag.desc;

            // Calculate the number of target groups along each drag axis. This is the
            // distance between the extrude origin and the target box center divided by
            // the size of the box along the drag axis.
            int     columnCount     = Mathf.RoundToInt(MathEx.FastAbs(dot0) / mTargetBox.size[dragDesc.axisIndex0]);
            int     rowCount        = dblAxisDrag ? Mathf.RoundToInt(MathEx.FastAbs(dot1) / mTargetBox.size[dragDesc.axisIndex1]) : 1;

            // If we're double dragging, we need +1 on both axes, but we will ensure to
            // skip the last row & column to avoid overlap with the original targets.
            if (dblAxisDrag)
            {
                ++columnCount;
                ++rowCount;
            }

            // Return result
            return new Vector2Int(columnCount, rowCount);
        }
        #endregion
    }
    #endregion
}
