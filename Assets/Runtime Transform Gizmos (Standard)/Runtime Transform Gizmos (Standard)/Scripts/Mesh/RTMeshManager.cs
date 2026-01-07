using UnityEngine;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: RTMeshManager (Public Singleton Class)
    // Desc: Manages 'RTMesh' instances. Clients can map 'Mesh' instances to the
    //       corresponding 'RTMesh' instance.
    //-----------------------------------------------------------------------------
    public class RTMeshManager : Singleton<RTMeshManager>
    {
        #region Private Fields
        Dictionary<Mesh, RTMesh> mMeshMap = new Dictionary<Mesh, RTMesh>(); // Maps 'Mesh' to 'RTMesh'
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: RegisterSceneMeshes() (Public Function)
        // Desc: Registers all readable meshes in the active scene. If 'warmUp' is true,
        //       the function also forces internal BVH trees to be built to avoid runtime
        //       cost during raycasts.
        // Parm: warmUp - If true, forces each mesh's internal acceleration structure
        //                to be built immediately.
        //-----------------------------------------------------------------------------
        public void RegisterSceneMeshes(bool warmUp)
        {
            // Find all MeshFilters in the active scene
            MeshFilter[] meshFilters = GameObjectEx.FindObjectsByType<MeshFilter>();

            // Loop through each filter and register its mesh
            int count = meshFilters.Length;
            for (int i = 0; i < count; ++i)
            {
                // Cache data
                MeshFilter mf = meshFilters[i];
                Mesh mesh     = mf.sharedMesh;
                
                // Skip invalid or missing meshes
                if (mesh == null) continue;

                // Create the RTMesh for this mesh
                RTMesh rtMesh = GetRTMesh(mesh);
                if (rtMesh == null) continue;

                // If warm-up requested, force internal tree construction
                if (warmUp)
                {
                    // This triggers internal AABB tree generation (lazy-initialized normally)
                    rtMesh.WarmUp();
                }
            }
        }

        //-----------------------------------------------------------------------------
        // Name: OnMeshDestroy() (Public Function)
        // Desc: Must be called when the specified mesh is about to be destroyed.
        // Parm: mesh - The mesh which is about to be destroyed.
        //-----------------------------------------------------------------------------
        public void OnMeshDestroy(Mesh mesh)
        {
            // Remove map entry for this mesh
            mMeshMap.Remove(mesh);
        }
        
        //-----------------------------------------------------------------------------
        // Name: OnMeshDataChanged() (Public Function)
        // Desc: Must be called when the following mesh data has changed: vertex positions,
        //       indices, sub-meshes.
        // mesh - mesh whose data has changed.
        //-----------------------------------------------------------------------------
        public void OnMeshDataChanged(Mesh mesh)
        {
            if (mMeshMap.TryGetValue(mesh, out RTMesh rtMesh))
                rtMesh.Internal_OnMeshDataChanged();
        }

        //-----------------------------------------------------------------------------
        // Name: OnSubMeshTopologyChanged() (Public Function)
        // Desc: Must be called when the topology of a sub-mesh has changed.
        // Parm: mesh           - Mesh whose sub-mesh topology has changed.
        //       subMeshInde    - Index of sub-mesh whose topology has changed.
        //-----------------------------------------------------------------------------
        public void OnSubMeshTopologyChanged(Mesh mesh, int subMeshIndex)
        {
            if (mMeshMap.TryGetValue(mesh, out RTMesh rtMesh))
                rtMesh.Internal_OnSubMeshTopologyChanged(subMeshIndex);
        }

        //-----------------------------------------------------------------------------
        // Name: GetRTMesh() (Public Function)
        // Desc: Returns the 'RTMesh' instance associated with the specified 'Mesh'. The
        //       function automatically calls 'RegisterMesh' if no 'RTMesh' exists for
        //       the specified mesh.
        // Parm: mesh - Query 'Mesh'.
        // Rtrn: The 'RTMesh' instance associated with the specified 'Mesh' or null if
        //       something goes wrong.
        //-----------------------------------------------------------------------------
        public RTMesh GetRTMesh(Mesh mesh)
        {
            if (mMeshMap.TryGetValue(mesh, out RTMesh rtMesh)) return rtMesh;
            else return RegisterMesh(mesh);
        }
        #endregion

        #region Private Functions
        
        //-----------------------------------------------------------------------------
        // Name: RegisterMesh() (Private Function)
        // Desc: Registers the specified 'Mesh' instance and returns an 'RTMesh' for this mesh. 
        // Parm: mesh - The 'Mesh' instance to register. Must have valid vertex and index count
        //              and the 'isReadable' property must return true.
        // Rtrn: An instance of' RTMesh' for the specified 'Mesh' or null if something 
        //       goes wrong.
        //-----------------------------------------------------------------------------
        RTMesh RegisterMesh(Mesh mesh)
        {
            // Validate mesh
            if (mesh.vertexCount == 0 || !mesh.isReadable) return null;

            // Must have at least one valid sub-mesh. A valid sub-mesh
            // uses a 'Triangles' topology and its index count is > 0
            // and a multiple of 3.
            bool foundValidSubMesh  = false;
            int subMeshCountr       = mesh.subMeshCount;
            for (int i = 0; i < subMeshCountr; ++i)
            {
                // Validate topology and index count
                uint indexCount = mesh.GetIndexCount(i);
                if (mesh.GetTopology(i) == MeshTopology.Triangles &&
                    indexCount != 0 && indexCount % 3 == 0)
                {
                    foundValidSubMesh = true;
                    break;
                }
            }

            // No valid sub-mesh found?
            if (!foundValidSubMesh) return null;

            // Create the 'RTMesh' instance
            RTMesh rtMesh = new RTMesh(mesh);
            mMeshMap.Add(mesh, rtMesh);

            // Return the 'RTMesh'
            return rtMesh;
        }
        #endregion
    }
    #endregion
}
