using UnityEngine;

namespace RTGStandard
{
    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: TerrainHeightsUndoState (Public Class)
    // Desc: Implements an Undo/Redo state which records terrain heights.
    //-----------------------------------------------------------------------------
    public class TerrainHeightsUndoState : UndoObjectState
    {
        #region Private Fields
        TerrainVertexRange  mVertexRange;   // Stores the range of vertices whose heights must be recorded
        float[,]            mHeights;       // The height values
        #endregion

        #region Public Constructors
        //-----------------------------------------------------------------------------
        // Name: TerrainHeightsUndoState() (Public Constructor)
        // Desc: Creates a 'TerrainHeightsUndoState' for the specified terrain.
        // Parm: terrain     - Target terrain.
        //       vertexRange - The range of vertices whose heights must be recorded.
        //-----------------------------------------------------------------------------
        public TerrainHeightsUndoState(Terrain terrain, TerrainVertexRange vertexRange) : base(terrain) 
        {
            mVertexRange = vertexRange;
        }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: CloneState() (Public Function)
        // Desc: Clones the state.
        // Rtrn: The cloned state.
        //-----------------------------------------------------------------------------
        public override UndoObjectState CloneState()
        {
            var clone       = MemberwiseClone() as TerrainHeightsUndoState;
            clone.mHeights  = mHeights.Clone() as float[,];
            return clone;
        }

        //-----------------------------------------------------------------------------
        // Name: Extract() (Public Function)
        // Desc: Extracts the state from its attached object.
        //-----------------------------------------------------------------------------
        public override void Extract()
        {
            // Extract state
            var terrain = target as Terrain;
            if (terrain.terrainData != null)
                mHeights = terrain.terrainData.GetHeights(mVertexRange.minX, mVertexRange.minZ, mVertexRange.width, mVertexRange.depth);
        }

        //-----------------------------------------------------------------------------
        // Name: Apply() (Public Function)
        // Desc: Applies the state to its attached object.
        //-----------------------------------------------------------------------------
        public override void Apply()
        {
            // Apply state
            var terrain = target as Terrain;
            if (terrain.terrainData != null)
                terrain.terrainData.SetHeights(mVertexRange.minX, mVertexRange.minZ, mHeights);
        }

        //-----------------------------------------------------------------------------
        // Name: Diff() (Public Function)
        // Desc: Checks if there is any difference between 'this' state and 'other'. The
        //       function assumes the 2 states have the same target object.
        // Parm: other - The other state.
        // Rtrn: True if the 2 states are different and false otherwise.
        //-----------------------------------------------------------------------------
        public override bool Diff(UndoObjectState other)
        {
            // Check type
            var s = other as TerrainHeightsUndoState;
            if (s == null) return false;

            // Check state diff
            if (mVertexRange.minX != s.mVertexRange.minX ||
                mVertexRange.minZ != s.mVertexRange.minZ ||
                mVertexRange.maxX != s.mVertexRange.maxX ||
                mVertexRange.maxZ != s.mVertexRange.maxZ) return true;

            // Now compare height values
            int height = mHeights.GetLength(0);
            int width  = mHeights.GetLength(1);
            if (width != s.mHeights.GetLength(0) || height != s.mHeights.GetLength(1)) return true;
            for (int r = 0; r < height; ++r)
            {
                for (int c = 0; c < width; ++c)
                {
                    if (mHeights[r, c] != s.mHeights[r, c])
                        return true;
                }
            }

            // No diff
            return false;
        }
        #endregion
    }
    #endregion
}
