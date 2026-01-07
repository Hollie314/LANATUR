using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: TerrainGizmo (Public Class)
    // Desc: Implements a terrain gizmo that allows the user to modify terrain 
    //       height and manipulate objects interactively.
    //-----------------------------------------------------------------------------
    public class TerrainGizmo : Gizmo
    {
        #region Private Enumerations
        //-----------------------------------------------------------------------------
        // Name: EObjectCollectReason (Private Enum)
        // Desc: Defines different reasons for collecting objects within the influence
        //       radius.
        //-----------------------------------------------------------------------------
        enum EObjectCollectReason
        {
            HorizontalMove = 0,     // Movement across the terrain surface in the XZ plane
            VerticalMove,           // Movement along the Y axis
            Rotate                  // Rotation
        }
        #endregion

        #region Private Fields
        GizmoDrag mDrag                                 = new GizmoDrag();              // Drag operation
        GizmoHandleRenderArgs   mHRenderArgs            = new GizmoHandleRenderArgs();  // Gizmo handle render args

        GizmoCap                mCenterCap              = new GizmoCap();           // The center cap which can be used to snap the gizmo to the terrain surface
        GizmoCap[]              mRadiusCaps             = new GizmoCap[4]           // The radius caps used to change the influence radius
        { new GizmoCap(), new GizmoCap(), new GizmoCap(), new GizmoCap() };
        GizmoLineSlider         mHeightSlider           = new GizmoLineSlider();    // Height slider
        GizmoCap                mHeightCap              = new GizmoCap();           // Height cap
        GizmoCap                mRotationCap            = new GizmoCap();           // Rotation cap used to rotate the objects within radius
        GizmoArc                mRotationArc            = new GizmoArc();           // Rotation arc used when rotating objects

        Terrain                 mTarget;                                            // The target terrain
        List<GizmoHandle>       mSortedHandles          = new List<GizmoHandle>();  // Used to sort handles
        List<GizmoHandle>       mHandleBuffer           = new List<GizmoHandle>();  // Used to avoid memory allocations

        float                   mRadius                 = 4.0f;                     // Current radius of influence
        float                   mDragStartRadius;                                   // The radius of influence on drag start
        Mesh                    mInfluenceCircleMesh;                               // Influence circle mesh
        List<Vector3>           mMeshVertexBuffer       = new List<Vector3>();      // Mesh vertex buffer used to update the mesh vertices

        float[,]                mDragStartHeights;                                  // Terrain heights on drag start
        float[,]                mDragHeights;                                       // The height values that are modified when dragging
        AnimationCurve          mElevationCurve         = new AnimationCurve();     // Controls the falloff of terrain elevation within the radius of influence
        List<GameObject>        mObjectsWithinRadius    = new List<GameObject>();   // Used when collecting objects in the area of influence
        List<GameObject>        mParentsWithinRadius    = new List<GameObject>();   // Same as 'mObjectsWithinRadius' but it stores only the parent objects
        ObjectFilter            mObjectCollectFilter    = new ObjectFilter()        // Object filter used for collecting the objects within radius
        {
            objectTypes = EGameObjectType.Mesh | EGameObjectType.Sprite | 
            EGameObjectType.Light | EGameObjectType.ParticleSystem
        };
        Dictionary<GameObject, float> mObjectOffsetMap  = new Dictionary<GameObject, float>();  // Maps a game object to its distance from the terrain surface. Used when 
                                                                                                // changing the terrain height to allow the objects to move along with the
                                                                                                // terrain.
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: style (Public Property)
        // Desc: Returns or sets the terrain gizmo style.
        //-----------------------------------------------------------------------------
        public TerrainGizmoStyle    style           { get; set; } = null;

        //-----------------------------------------------------------------------------
        // Name: activeStyle (Public Property)
        // Desc: Returns the active style used by the gizmo. This is 'style' if it was set
        //       to a valid style. Otherwise, it is the active skin's terrain gizmo style.
        //-----------------------------------------------------------------------------
        public TerrainGizmoStyle    activeStyle     { get { return style == null ? RTGizmos.get.skin.terrainGizmoStyle : style; } }

        //-----------------------------------------------------------------------------
        // Name: target (Public Property)
        // Desc: Returns or sets the target terrain.
        //-----------------------------------------------------------------------------
        public Terrain              target          { get { return mTarget; } set { if (value == mTarget) return; mTarget = value; } }

        //-----------------------------------------------------------------------------
        // Name: radius (Public Property)
        // Desc: Returns or sets the radius of influence.
        //-----------------------------------------------------------------------------
        public float                radius          { get { return mRadius; } set { mRadius = Mathf.Max(0.0f, value); } }

        //-----------------------------------------------------------------------------
        // Name: elevationCurve (Public Property)
        // Desc: Returns the elevation curve.
        //-----------------------------------------------------------------------------
        public AnimationCurve       elevationCurve  { get { return mElevationCurve; } }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: SnapToPickPoint() (Public Function)
        // Desc: Snaps the gizmo's position to the point where the cursor intersects 
        //       the target terrain. If no valid target exists, the function exits early.
        // Parm: camera - The camera used to compute the picking ray.
        //-----------------------------------------------------------------------------
        public void SnapToPickPoint(Camera camera)
        {
            // No-op?
            if (target == null || 
                RTCamera.get.navigationMode != ECameraNavigationMode.None ||
                RTGizmos.get.hoveredHandle != null || 
                RTGizmos.get.IsGizmoGUIHovered())
                return;

            // Raycast the terrain collider
            TerrainCollider terrainCollider = target.gameObject.GetTerrainCollider();
            if (terrainCollider != null)
            {
                Ray ray = RTInput.get.pointingInputDevice.GetPickRay(camera);
                if (terrainCollider.Raycast(ray, out RaycastHit rayHit, float.MaxValue))
                {
                    // We have a hit. Snap the gizmo position and make the handles visible.
                    transform.position = rayHit.point;
                    SetHandlesVisible(true);
                }
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
            return mTarget != null && mTarget.gameObject.activeInHierarchy && mTarget.enabled;
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

            // Store remaining handles
            handles.Add(mCenterCap);
            handles.Add(mHeightSlider);
            handles.Add(mHeightCap);
            handles.Add(mRotationCap);
            handles.Add(mRotationArc);

            // Init
            mHeightCap.AddRenderHoverConnection(mHeightSlider);
            SetHandlesVisible(false);
            mRotationCap.alignFlatToCamera  = false;
            mRotationArc.tag                = EGizmoHandleTag.RotationArc;
            mCenterCap.hoverPriority        = 1;

            // Simple linear elevation curve initially
            mElevationCurve.AddKey(new Keyframe(0.0f, 0.0f));
            mElevationCurve.AddKey(new Keyframe(1.0f, 1.0f));

            // Set object collect custom filter
            mObjectCollectFilter.customFilter = (go) => { return IsObjectWithinRadius(go); };
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
            // Set styles
            int count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
                mRadiusCaps[i].capStyle = activeStyle.radiusCapStyle;

            mCenterCap.capStyle         = activeStyle.centerCapStyle;
            mHeightSlider.sliderStyle   = activeStyle.heightSliderStyle;
            mHeightCap.capStyle         = activeStyle.heightCapStyle;
            mRotationCap.capStyle       = activeStyle.rotationCapStyle;
            mRotationArc.arcStyle       = RTGizmos.get.skin.globalGizmoStyle.rotationArcStyle;

            // Snap to pick point?
            if (!hovered && RTInput.get.pointingInputDevice.pickButtonWentDown)
                SnapToPickPoint(camera);

            // Update transforms
            mHeightSlider.SetDirection(Vector3.up);
            mHeightCap.transform.rotation       = mHeightSlider.transform.rotation;
            mRotationCap.transform.position     = transform.position;
            mRotationCap.transform.rotation     = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            mRadiusCaps[0].transform.position   = transform.position + Vector3.right * mRadius;
            mRadiusCaps[1].transform.position   = transform.position - Vector3.right * mRadius;
            mRadiusCaps[2].transform.position   = transform.position + Vector3.forward * mRadius;
            mRadiusCaps[3].transform.position   = transform.position - Vector3.forward * mRadius;

            // All radius caps should use an identity rotation and have their positions projected onto the terrain
            count = mRadiusCaps.Length;
            for (int i = 0; i < count; ++i)
            {
                mRadiusCaps[i].transform.rotation = Quaternion.identity;
                mRadiusCaps[i].transform.position = mTarget.ProjectPoint(mRadiusCaps[i].transform.position);
            }

            // Project gizmo position, center cap and height slider onto the terrain
            transform.position                  = mTarget.ProjectPoint(transform.position);
            mCenterCap.transform.position       = mTarget.ProjectPoint(mCenterCap.transform.position);
            mHeightSlider.transform.position    = mTarget.ProjectPoint(mHeightSlider.transform.position);
            mHeightCap.CapSlider(mHeightSlider, camera);

            // Update the influence circle only once
            if (updateReason == EGizmoHandleUpdateReason.Update)
                UpdateInfluenceCircle();

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

            // Draw influence circle
            if (mCenterCap.visible)
                RTGizmos.DrawMesh(mInfluenceCircleMesh, Matrix4x4.identity, activeStyle.radiusCircleColor);

            // Sort handles
            mHandleBuffer.Clear();
            mHandleBuffer.Add(mHeightCap);
            mHandleBuffer.Add(mHeightSlider);
            mHandleBuffer.Add(mRotationCap);
            mHandleBuffer.AddRange(mRadiusCaps);
            GizmoHandle.ZSortHandles(mHandleBuffer, mHRenderArgs, mSortedHandles);

            // Render sorted handles
            int count = mSortedHandles.Count;
            for (int i = 0; i < count; ++i)
                mSortedHandles[i].Render(mHRenderArgs);

            // Draw rotation arc if we're rotating
            mRotationArc.visible = dragData.handle == mRotationCap;
            if (mRotationArc.visible)
            {
                // Set arc details and render
                mRotationArc.transform.position = mRotationCap.transform.position;
                mRotationArc.transform.rotation = mRotationCap.transform.rotation;
                mRotationArc.Set(dragData.dragStartHoverPoint, 
                    mRotationCap.FVal(mRotationCap.capStyle.circleRadius, camera),
                    mDrag.totalDrag[mDrag.desc.axisIndex0]);
                mRotationArc.Render(mHRenderArgs);
            }

            // Draw center cap over everything else
            mCenterCap.Render(mHRenderArgs);
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
            mDragStartRadius = mRadius;

            // Match center cap
            if (dragData.handle == mCenterCap)
            {
                // Collect objects within radius. We will be dragging them along with the gizmo.
                CollectObjectsWithinRadius(EObjectCollectReason.HorizontalMove);

                // Start dragging
                mDrag.surfaceSnapObjectTypes    = EGameObjectType.Terrain;
                mDrag.gridSurfaceSnapEnabled    = false;
                desc.dragType                   = EGizmoDragType.SurfaceSnap;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null; 
            }
            // Match height handles
            else
            if (dragData.handle == mHeightCap ||
                dragData.handle == mHeightSlider)
            {
                // Collect objects within radius. We will be changing their Y position along with the terrain height.
                CollectObjectsWithinRadius(EObjectCollectReason.VerticalMove);

                // Get terrain heights. These will be modified when dragging.
                var vertexRange   = target.CalculateVertexRange(transform.position, mRadius);
                mDragStartHeights = target.terrainData.GetHeights(vertexRange.minX, vertexRange.minZ, 
                    vertexRange.width, vertexRange.depth);
                mDragHeights = mDragStartHeights.Clone() as float[,];

                // Start dragging
                desc.dragType   = EGizmoDragType.Scale;
                desc.axisIndex0 = 0;
                desc.axis0      = Vector3.up;
                mDrag.scaleSnap = activeStyle.heightSnap;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null; 
            }
            // Match radius caps
            else
            if (ArrayHasDragHandle(mRadiusCaps))
            {
                // Start dragging
                desc.dragType   = EGizmoDragType.Scale;
                desc.axis0      = dragData.handle.transform.position - mCenterCap.transform.position;
                desc.axis0.y    = 0.0f;
                desc.axisIndex0 = 0;
                mDrag.scaleSnap = activeStyle.radiusSnap;
                return mDrag.Start(desc, this, transform, camera) ? mDrag : null;        
            }
            // Match rotation cap
            else
            if (dragHandle == mRotationCap)
            {
                // Collect objects within radius so we can rotate them
                CollectObjectsWithinRadius(EObjectCollectReason.Rotate);

                // Start dragging
                desc.dragType       = EGizmoDragType.Rotate;
                desc.axis0          = Vector3.up;
                desc.axisIndex0     = 1;
                desc.rotationCenter = mRotationCap.transform.position;
                return mDrag.Start(desc, this, mRotationCap.transform, camera) ? mDrag : null;
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
            // Dragging center cap?
            if (dragData.handle == mCenterCap)
            {
                // Loop through each object within radius drag it along with the gizmo
                int count = mParentsWithinRadius.Count;
                for (int i = 0; i < count; ++i)
                {
                    // Record object transform
                    RTUndo.get.Record(mParentsWithinRadius[i].transform);

                    // Calculate the object's distance from the terrain for its current position.
                    // We want to maintain the object's distance from the terrain whenever the
                    // object is moved.
                    GameObject gameObject = mParentsWithinRadius[i];
                    Vector3 objectPos = gameObject.transform.position;
                    float d = mTarget.GetDistanceToPoint(objectPos);

                    // Apply the drag and make sure the object keeps its distance from the terrain
                    objectPos += drag.dragDelta;
                    objectPos.y = mTarget.ProjectPoint(objectPos).y + d;    // Project onto the terrain (0 distance) and then add the distance calculated earlier

                    // Set the object's position
                    RTScene.get.SetObjectPosition(gameObject, objectPos);
                }
            }
            else
            // Dragging radius?
            if (ArrayHasDragHandle(mRadiusCaps))
            {
                // Update radius
                mRadius = mDragStartRadius + drag.totalDrag[drag.desc.axisIndex0];
                if (mRadius < 0.0f) mRadius = 0.0f;
            }
            // Dragging height?
            else
            if (dragData.handle == mHeightCap ||
                dragData.handle == mHeightSlider)
            {
                // Map target objects to their distance from the terrain
                int count = mParentsWithinRadius.Count;
                for (int i = 0; i < count; ++i)
                    mObjectOffsetMap.Add(mParentsWithinRadius[i], mTarget.GetDistanceToPoint(mParentsWithinRadius[i].transform.position));

                // Calculate circle vertex range
                var vertexRange = target.CalculateVertexRange(transform.position, mRadius);

                // Calculate terrain quad size
                var terrainData = target.terrainData;
                float quadWidth = terrainData.size.x / terrainData.heightmapResolution;
                float quadDepth = terrainData.size.z / terrainData.heightmapResolution;

                // Loop through each vertex in the area of influence
                Vector3 terrainPos   = target.transform.position;
                Vector3 gizmoPos     = transform.position;
                float   heightOffset = mDrag.totalDrag[mDrag.desc.axisIndex0] / terrainData.heightmapScale.y;   // Offset to add to the height in heightmap space. Will be scaled by elevation curve.
                for (int depth = vertexRange.minZ; depth < vertexRange.maxZ; ++depth)
                {
                    for (int col = vertexRange.minX; col < vertexRange.maxX; ++col)
                    {
                        // Calculate horizontal vertex position
                        float vPosX = terrainPos.x + col   * quadWidth;
                        float vPosZ = terrainPos.z + depth * quadDepth;

                        // Calculate the distance from center
                        float dx = vPosX - gizmoPos.x;
                        float dz = vPosZ - gizmoPos.z;
                        float d = Mathf.Sqrt(dx * dx + dz * dz);
                        if (d <= mRadius)
                        {
                            // Calculate the interpolation factor which interpolates from full elevation to 0 elevation
                            float t = Mathf.Max(1.0f - d / mRadius, 0.0f);
                            t = mElevationCurve.Evaluate(t);

                            // Offset height
                            int x = col   - vertexRange.minX;
                            int z = depth - vertexRange.minZ;
                            mDragHeights[z, x] = mDragStartHeights[z, x] + heightOffset * t;
                            mDragHeights[z, x] = Mathf.Max(mDragHeights[z, x], 0.0f);
                        }
                    }
                }

                // Update heights
                RTUndo.get.RecordTerrainHeights(mTarget, vertexRange);
                terrainData.SetHeightsDelayLOD(vertexRange.minX, vertexRange.minZ, mDragHeights);

                // Update object positions. We want the objects to sit at the same distance
                // from the terrain as before.
                count = mParentsWithinRadius.Count;
                for (int i = 0; i < count; ++i)
                {
                    // Project the object onto the terrain surface
                    Transform objectTransform = mParentsWithinRadius[i].transform;
                    RTUndo.get.Record(objectTransform);
                    objectTransform.position = mTarget.ProjectPoint(objectTransform.position);
                    objectTransform.position += Vector3.up * mObjectOffsetMap[mParentsWithinRadius[i]];
                }

                // Clear object offset map
                mObjectOffsetMap.Clear();
            }
            // Rotation drag?
            else
            if (dragData.handle == mRotationCap)
            {
                // Create rotation
                Quaternion rotation = Quaternion.AngleAxis(mDrag.dragDelta[mDrag.desc.axisIndex0], mDrag.desc.axis0);

                // Rotate objects
                int count = mParentsWithinRadius.Count;
                for (int i = 0; i < count; ++i)
                {
                    // Record transform and rotate
                    Transform objectTransform = mParentsWithinRadius[i].transform;
                    RTUndo.get.Record(objectTransform);
                    objectTransform.rotation = rotation * objectTransform.rotation;
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
            // Have we been changing the terrain height?
            if (dragData.handle == mHeightCap ||
                dragData.handle == mHeightSlider)
            {
                // Sync terrain LOD
                target.terrainData.SyncHeightmap();
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnGUI() (Protected Function)
        // Desc: Called during the 'OnGUI' event to allow the gizmo to draw GUI elements.
        // Parm: camera - The camera that renders the GUI.
        //-----------------------------------------------------------------------------
        protected override void OnGUI(Camera camera)
        {
            // Are we dragging the radius?
            if (ArrayHasDragHandle(mRadiusCaps))
            {
                // Draw radius label
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Radius: " + mRadius.ToString("F3"), EGizmoTextType.DragInfo);
            }
            else
            // Are we dragging the height?
            if (dragData.handle == mHeightCap ||
                dragData.handle == mHeightSlider)
            {
                // Draw label which shows the total offset applied
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, "Offset: " + mDrag.totalDrag[mDrag.desc.axisIndex0].ToString("F3"), EGizmoTextType.DragInfo);
            }
            // Are we rotating?
            if (dragData.handle == mRotationCap)
            {
                // Draw label which shows the total rotation applied
                RTGizmos.get.skin.globalGizmoStyle.GUILabel_AABBTop(dragData.handle.CalculateAABB(camera),
                    camera, mDrag.totalDrag[mDrag.desc.axisIndex0].ToString("F3"), EGizmoTextType.DragInfo);
            }
        }
        #endregion

        #region Private Functions
        //-----------------------------------------------------------------------------
        // Name: CollectObjectsWithinRadius() (Private Function)
        // Desc: Collects the objects that fall within the radius of influence.
        // Parm: collectReason - the reason why objects are being collected.
        // Rtrn: True if at least one object was collected and false otherwise.
        //-----------------------------------------------------------------------------
        bool CollectObjectsWithinRadius(EObjectCollectReason collectReason)
        {
            // Update the object filter based on the collection reason
            switch (collectReason)
            {
                case EObjectCollectReason.HorizontalMove:

                    mObjectCollectFilter.layerMask = activeStyle.hMoveLayerMask;
                    break;

                case EObjectCollectReason.VerticalMove:

                    mObjectCollectFilter.layerMask = activeStyle.vMoveLayerMask;
                    break;

                case EObjectCollectReason.Rotate:

                    mObjectCollectFilter.layerMask = activeStyle.rotationLayerMask;
                    break;
            }

            // Create the collection box and collect
            OBox box = new OBox(transform.position, new Vector3(mRadius, target.terrainData.size.y * 2.0f, mRadius) * 2.0f);
            RTScene.get.BoxCollect(box, mObjectCollectFilter, mObjectsWithinRadius);

            // Collect parents
            GameObjectEx.CollectParents(mObjectsWithinRadius, mParentsWithinRadius);

            // Did we collect anything?
            return mParentsWithinRadius.Count != 0;
        }

        //-----------------------------------------------------------------------------
        // Name: IsObjectWithinRadius() (Private Function)
        // Desc: Checks if the specified object is within the radius of influence.
        // Parm: gameObject - Query object.
        // Rtrn: True of the object is within the radius of influence and false otherwise.
        //-----------------------------------------------------------------------------
        bool IsObjectWithinRadius(GameObject gameObject)
        {
            // Calculate flat object positions
            Vector3 objectPos = gameObject.transform.position;
            Vector3 gizmoPos  = transform.position;
            objectPos.y = gizmoPos.y = 0.0f;

            // Within radius?
            return (objectPos - gizmoPos).magnitude <= mRadius;
        }

        //-----------------------------------------------------------------------------
        // Name: UpdateInfluenceCircle() (Private Function)
        // Desc: Updates the influence circle mesh.
        //-----------------------------------------------------------------------------
        void UpdateInfluenceCircle()
        {
            // Constants
            const int vertexCount = 100;

            // Create the mesh if not already created
            Circle circle = new Circle();
            if (mInfluenceCircleMesh == null)
            {
                // Instantiate the mesh and set the vertex attribute descriptors
                mInfluenceCircleMesh = new Mesh();
                mInfluenceCircleMesh.SetVertexBufferParams(vertexCount,
                    new VertexAttributeDescriptor[]
                    {
                        new VertexAttributeDescriptor
                        {
                            attribute   = VertexAttribute.Position,
                            format      = VertexAttributeFormat.Float32,
                            dimension   = 3,
                            stream      = 0,
                        },
                    });

                // The index data can be generated once and then reused
                var indices = new List<int>();
                circle.Set(transform.position, mRadius, Vector3.right, Vector3.up);
                Geometry.GenerateCirclePoints(circle, vertexCount, mMeshVertexBuffer, indices);
                mInfluenceCircleMesh.SetIndexBufferParams(indices.Count, IndexFormat.UInt32);
                mInfluenceCircleMesh.SetIndexBufferData(indices, 0, 0, indices.Count, MeshUpdateFlags.DontValidateIndices);

                // Create the sub mesh descriptor
                var subMeshDesc             = new SubMeshDescriptor();
                subMeshDesc.baseVertex      = 0;
                subMeshDesc.firstVertex     = 0;
                subMeshDesc.indexCount      = indices.Count;
                subMeshDesc.indexStart      = 0;
                subMeshDesc.topology        = MeshTopology.LineStrip;
                subMeshDesc.vertexCount     = mMeshVertexBuffer.Count;

                // Set the sub mesh descriptor and mark the mesh as dynamic
                mInfluenceCircleMesh.SetSubMesh(0, subMeshDesc, MeshUpdateFlags.DontRecalculateBounds); // Note: No vertex data is available at this point.
                mInfluenceCircleMesh.MarkDynamic();
            }

            // Generate circle vertices
            circle.Set(transform.position, mRadius, Vector3.right, Vector3.up);
            Geometry.GenerateCirclePoints(circle, vertexCount, mMeshVertexBuffer, null);

            // Project vertices onto the terrain
            if (target != null)
            {
                int count = mMeshVertexBuffer.Count;
                for (int i = 0; i < count; ++i)
                    mMeshVertexBuffer[i] = target.ProjectPoint(mMeshVertexBuffer[i]);
            }

            // Update verts
            mInfluenceCircleMesh.SetVertexBufferData(mMeshVertexBuffer, 0, 0, mMeshVertexBuffer.Count, 0, 
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontResetBoneBounds);
        }
        #endregion
    }
    #endregion
}
