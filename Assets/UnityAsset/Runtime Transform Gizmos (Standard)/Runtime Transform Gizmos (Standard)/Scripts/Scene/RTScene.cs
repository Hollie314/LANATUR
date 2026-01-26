using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace RTGStandard
{
    #region Public Structures
    //-----------------------------------------------------------------------------
    // Name: SceneRayHit (Public Struct)
    // Desc: Stores information for a scene ray hit.
    //-----------------------------------------------------------------------------
    public struct SceneRayHit
    {
        #region Public Fields
        public ObjectRayHit objectHit;  // Object hit
        public RTGridRayHit gridHit;    // Grid hit
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: anyHit (Public Property)
        // Desc: Returns true if there was a hit (object or grid or both) and false
        //       otherwise.
        //-----------------------------------------------------------------------------
        public bool     anyHit          { get { return objectHit.gameObject != null || gridHit.grid != null; } }

        //-----------------------------------------------------------------------------
        // Name: hasObjectHit (Public Property)
        // Desc: Returns true if an object was hit and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     hasObjectHit    { get { return objectHit.gameObject != null; } }

        //-----------------------------------------------------------------------------
        // Name: hasGridHit (Public Property)
        // Desc: Returns true if a grid was hit and false otherwise.
        //-----------------------------------------------------------------------------
        public bool     hasGridHit      { get { return gridHit.grid != null; } }

        //-----------------------------------------------------------------------------
        // Name: closestHit (Public Property)
        // Desc: Returns the closest hit point. If no entity was hit, this returns the
        //       zero vector.
        //-----------------------------------------------------------------------------
        public Vector3  closestHit
        {
            get
            {
                // Valid object hit?
                if (objectHit.gameObject != null)
                {
                    // Valid grid hit? Then return closest hit.
                    if (gridHit.grid != null)
                        return gridHit.t <= objectHit.t ? gridHit.point : objectHit.point;

                    // Return the object hit
                    return objectHit.point;
                }
                else
                // Valid grid hit?
                if (gridHit.grid != null)
                    return gridHit.point;

                // No hit
                return Vector3.zero;
            }
        }
        #endregion
    }
    #endregion

    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RTScene (Public Singleton Class)
    // Desc: Implements scene management and query functionality.
    //-----------------------------------------------------------------------------
    public class RTScene : MonoSingleton<RTScene>
    {
        #region Private Fields     
        SceneObjectTree     mObjectTree             = new SceneObjectTree(); // The object tree which is used to speed up queries such as raycasts and overlap tests

        // Buffers used to avoid memory allocations
        List<GameObject>    mRootBuffer             = new List<GameObject>();
        List<GameObject>    mObjectBuffer           = new List<GameObject>();
        List<RaycastResult> mRaycastResultBuffer    = new List<RaycastResult>();
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: Internal_LoadCurrent() (Public Function)
        // Desc: Called by the system to load the currently active scene.
        //-----------------------------------------------------------------------------
        public void Internal_LoadCurrent()
        {
            // Clear old data
            mObjectTree.Clear();

            // Get all root objects in the active scene
            Scene activeScene       = SceneManager.GetActiveScene();
            mRootBuffer.Capacity    = activeScene.rootCount;
            activeScene.GetRootGameObjects(mRootBuffer);

            // Loop through each root object and register its hierarchy
            int rootCount = mRootBuffer.Count;
            for (int i = 0; i < rootCount; ++i)
                RegisterObjectHierarchy(mRootBuffer[i]);

            // Clear data
            mRootBuffer.Clear();
        }

        //-----------------------------------------------------------------------------
        // Name: Internal_Unload() (Public Function)
        // Desc: Called by the system to unload the scene.
        //-----------------------------------------------------------------------------
        public void Internal_Unload()
        {
            mObjectTree.Clear();
        }

        //-----------------------------------------------------------------------------
        // Name: SetObjectTRS() (Public Function)
        // Desc: Sets an object's TRS data and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       position   - Absolute position.
        //       rotation   - Absolute rotation.
        //       scale      - Absolute scale.
        //-----------------------------------------------------------------------------
        public void SetObjectTRS(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // Set TRS
            Transform t = gameObject.transform;
            t.position = position;
            t.rotation = rotation;
            t.SetScale(scale);

            // Notify
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: SetObjectPosition() (Public Function)
        // Desc: Sets an object's position and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       position   - Absolute position.
        //-----------------------------------------------------------------------------
        public void SetObjectPosition(GameObject gameObject, Vector3 position)
        {
            // Set position and notify
            gameObject.transform.position = position;
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: MoveObject() (Public Function)
        // Desc: Moves an object by the specified offset and calls 'OnObjectTransformChanged'
        //       afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       offset     - Offset to apply to the object's absolute position.
        //-----------------------------------------------------------------------------
        public void MoveObject(GameObject gameObject, Vector3 offset)
        {
            // Move object and notify
            gameObject.transform.position += offset;
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: SetObjectRotaion() (Public Function)
        // Desc: Sets an object's rotation and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       rotation   - Absolute rotation.
        //-----------------------------------------------------------------------------
        public void SetObjectRotaion(GameObject gameObject, Quaternion rotation)
        {
            // Set rotation and notify
            gameObject.transform.rotation = rotation;
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: RotateObject() (Public Function)
        // Desc: Rotates an object and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       rotation   - Rotation to apply to the object's absolute rotation.
        //-----------------------------------------------------------------------------
        public void RotateObject(GameObject gameObject, Quaternion rotation)
        {
            // Rotate object and notify
            gameObject.transform.rotation = rotation * gameObject.transform.rotation;
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: RotateObjectAroundPivot() (Public Function)
        // Desc: Rotates an object and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       rotation   - Rotation to apply to the object's absolute rotation.
        //       pivot      - Rotation pivot.
        //-----------------------------------------------------------------------------
        public void RotateObjectAroundPivot(GameObject gameObject, Quaternion rotation, Vector3 pivot)
        {
            // Rotate object and notify
            gameObject.transform.RotateAroundPivot(rotation, pivot);
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: SetObjectScale() (Public Function)
        // Desc: Sets an object's scale and calls 'OnObjectTransformChanged' afterwards.
        // Parm: gameObject - The game object whose transform is affected.
        //       scale      - Absolute scale.
        //-----------------------------------------------------------------------------
        public void SetObjectScale(GameObject gameObject, Vector3 scale)
        {
            // Set scale and notify
            gameObject.transform.SetScale(scale);
            OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: OnObjectTransformChanged() (Public Function)
        // Desc: Must be called when the transform of an object changes.
        // Parm: gameObject - The game object whose transform changed.
        //-----------------------------------------------------------------------------
        public void OnObjectTransformChanged(GameObject gameObject)
        {
            mObjectTree.OnObjectTransformChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: OnObjectComponentsChanged() (Public Function)
        // Desc: Must be called whenever adding/removing components to/from a game object.
        // Parm: gameObject - The game object whose components changed.
        //-----------------------------------------------------------------------------
        public void OnObjectComponentsChanged(GameObject gameObject)
        {
            // Notify object that its type has changed
            gameObject.OnObjectTypeDirty();

            // Notify scene object tree. The object may need to be removed from the tree or re-integrated.
            mObjectTree.OnObjectComponentsChanged(gameObject);
        }

        //-----------------------------------------------------------------------------
        // Name: RegisterObjectHierarchy() (Public Function)
        // Desc: Must be called whenever a new object hierarchy is created.
        // Parm: root - The root of the created hierarchy.
        //-----------------------------------------------------------------------------
        public void RegisterObjectHierarchy(GameObject root)
        {
            // Get all object in hierarchy
            root.CollectMeAndChildren(true, mObjectBuffer);

            // Loop through each object
            int objectCount = mObjectBuffer.Count;
            for (int i = 0; i < objectCount; ++i)
            {
                // Register this object with the scene tree
                mObjectTree.RegisterObject(mObjectBuffer[i]);
            }
        }

        //-----------------------------------------------------------------------------
        // Name: UnregisterObjectHierarchy() (Public Function)
        // Desc: Must be called whenever a game object is about to be destroyed.
        // Parm: root - The root of the hierarchy which is about to be destroyed.
        //-----------------------------------------------------------------------------
        public void UnregisterObjectHierarchy(GameObject root)
        {
            // Get all objects in the hierarchy
            root.CollectMeAndChildren(true, mObjectBuffer);

            // Loop through each object
            int objectCount = mObjectBuffer.Count;
            for (int i = 0; i < objectCount; ++i)
            {
                // Remove object from the scene tree
                mObjectTree.UnregisterObject(mObjectBuffer[i]);
            }
        }

        //-----------------------------------------------------------------------------
        // Name: BoxCollect() (Public Function)
        // Desc: Collects all scene objects whose bounding volumes intersect or are 
        //       fully contained within the specified oriented box.
        // Parm: box         - The oriented box used for intersection testing.
        //       filter      - Optional object filter. Can be null to include all objects.
        //       gameObjects - Receives the collected objects.
        // Rtrn: True if at least one object was collected; false otherwise.
        //-----------------------------------------------------------------------------
        public bool BoxCollect(OBox box, ObjectFilter filter, List<GameObject> gameObjects)
        {
            return mObjectTree.BoxCollect(box, filter, gameObjects);
        }

        //-----------------------------------------------------------------------------
        // Name: BoxOverlap() (Public Function)
        // Desc: Checks if the specified box overlaps with at least one scene object.
        // Parm: box     - The oriented box used for intersection testing.
        //       filter  - Optional filter used to include/exclude objects. Can be null.
        // Rtrn: True if at least one object is overlapped; false otherwise.
        //-----------------------------------------------------------------------------
        public bool BoxOverlap(OBox box, ObjectFilter filter)
        {
            return mObjectTree.BoxOverlap(box, filter);
        }

        //-----------------------------------------------------------------------------
        // Name: Raycast() (Public Function)
        // Desc: Performs a raycast and returns the information about the closest hit.
        // Parm: ray        - Query ray.
        //       filter     - Object filter. Can be null if no filtering is needed.
        //       sceneHit   - Returns the scene hit information.
        // Rtrn: True if the ray hits a scene entity and false otherwise.
        //-----------------------------------------------------------------------------
        public bool Raycast(Ray ray, ObjectFilter filter, bool raycastGrid, out SceneRayHit sceneHit)
        {
            // Raycast scene entities
            mObjectTree.Raycast(ray, filter, out sceneHit.objectHit);
            if (raycastGrid) RTGrid.get.Raycast(ray, out sceneHit.gridHit);
            else sceneHit.gridHit = new RTGridRayHit();

            // Return result
            return sceneHit.anyHit;
        }

        //-----------------------------------------------------------------------------
        // Name: RaycastUGUI() (Public Function)
        // Desc: Performs a raycast against UGUI elements. This is useful for checking
        //       if the cursor hovers any UGUI elements.
        // Parm: results - Returns the raycast results.
        // Rtrn: True if the cursor hits anything and false otherwise.
        //-----------------------------------------------------------------------------
        public bool RaycastUGUI(List<RaycastResult> results)
        {
            // Clear output
            results.Clear();

            // No event system?
            if (EventSystem.current == null)
                return false;

            // Get the input device's screen coords. If the coords are not available, return false.
            var inputDevice = RTInput.get.pointingInputDevice;
            if (!inputDevice.hasPointer) return false;

            // Construct the pointer event data instance needed for the raycast
            PointerEventData evData = new PointerEventData(EventSystem.current);
            evData.position = inputDevice.position;

            // Raycast all and collect results
            EventSystem.current.RaycastAll(evData, results);
            results.RemoveAll(item => item.gameObject.GetComponent<RectTransform>() == null);

            // Do we have a hit?
            return results.Count != 0;
        }

        //-----------------------------------------------------------------------------
        // Name: IsUGUIHovered() (Public Function)
        // Desc: Checks if the cursor hovers any UGUI elements.
        // Rtrn: True if the cursor hovers any UGUI elements and false otherwise.
        //-----------------------------------------------------------------------------
        public bool IsUGUIHovered()
        {
            return RaycastUGUI(mRaycastResultBuffer);
        }

        //-----------------------------------------------------------------------------
        // Name: ScreenRectCollectInside() (Public Function)
        // Desc: Collects all objects that lie completely within the specified screen
        //       rectangle.
        // Parm: screenRect     - The query rectangle.
        //       camera         - The camera that sees the rectangle.
        //       filter         - Object filter. Can be null if no filtering is needed.
        //       gameObjects    - Returns all objects that lie completely within the rectangle.
        // Rtrn: True if at least one object was collected and false otherwise.
        //-----------------------------------------------------------------------------
        public bool ScreenRectCollectInside(Rect screenRect, Camera camera, ObjectFilter filter, List<GameObject> gameObjects)
        {
            return mObjectTree.ScreenRectCollectInside(screenRect, camera, filter, gameObjects);
        }
        #endregion
    }
    #endregion
}