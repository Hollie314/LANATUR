using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace RTGStandard
{
    #region Public Structures
    //-----------------------------------------------------------------------------
    // Name: MaterialRenderStates (Public Struct)
    // Desc: Provides storage for a combination of material render states.
    //-----------------------------------------------------------------------------
    public struct MaterialRenderStates
    {
        public CompareFunction  zTest;      // Z test function
        public bool             zWrite;     // Z write toggle
        public CullMode         cullMode;   // Cull mode
    }
    #endregion

    #region Public Classes
    //-----------------------------------------------------------------------------
    // Name: MaterialManager (Public Singleton Class)
    // Desc: Manages a collection of materials used throughout the plugin and
    //       implements relevant utility functions.
    //-----------------------------------------------------------------------------
    public class MaterialManager : Singleton<MaterialManager>
    {
        #region Private Fields
        Dictionary<MaterialRenderStates, Material>
                 mRTGizmoHandleMaterialMap = new Dictionary<MaterialRenderStates, Material>();    // Maps gizmo handle material states to a gizmo handle material

        Material mRTGrid;       // Grid
        Material mRTCameraBG;   // Camera background
        Material mRTUnlit;      // Unlit
        Material mRT2D;         // 2D rendering
        #endregion

        #region Public Properties
        //-----------------------------------------------------------------------------
        // Name: rtGrid (Public Property)
        // Desc: Returns the material which renders the 'RTGrid'.
        //-----------------------------------------------------------------------------
        public Material rtGrid
        {
            get
            {
                if (mRTGrid == null) mRTGrid = new Material(ShaderManager.get.rtGrid);
                return mRTGrid;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rtCameraBG (Public Property)
        // Desc: Returns the material which renders the 'RTCamera' background.
        //-----------------------------------------------------------------------------
        public Material rtCameraBG
        {
            get
            {
                if (mRTCameraBG == null) mRTCameraBG = new Material(ShaderManager.get.rtCameraBG);
                return mRTCameraBG;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rtUnlit (Public Property)
        // Desc: Returns the material that renders unlit geometry.
        //-----------------------------------------------------------------------------
        public Material rtUnlit
        {
            get
            {
                if (mRTUnlit == null) mRTUnlit = new Material(ShaderManager.get.rtUnlit);
                return mRTUnlit;
            }
        }

        //-----------------------------------------------------------------------------
        // Name: rt2D (Public Property)
        // Desc: Returns the material that renders 2D geometry.
        //-----------------------------------------------------------------------------
        public Material rt2D
        {
            get
            {
                if (mRT2D == null)
                {
                    mRT2D = new Material(ShaderManager.get.rt2D);
                    mRT2D.DisableKeyword("RT_SAMPLE_TEXTURE");
                }
                return mRT2D;
            }
        }
        #endregion

        #region Public Functions
        //-----------------------------------------------------------------------------
        // Name: GetRTGizmoHandleMaterial() (Public Function)
        // Desc: Returns the gizmo handle material that uses the specified render states.
        // Parm: renderStates - Render state combo.
        // Rtrn: The material that uses the specified render state combo.
        //-----------------------------------------------------------------------------
        public Material GetRTGizmoHandleMaterial(MaterialRenderStates renderStates)
        {
            // Do we have an entry for this state combo?
            Material mtrl = null;
            if (mRTGizmoHandleMaterialMap.TryGetValue(renderStates, out mtrl))
                return mtrl;

            // Create a new entry
            mtrl = new Material(ShaderManager.get.rtGizmoHandle);
            mtrl.SetCullMode(renderStates.cullMode);
            mtrl.SetZTest(renderStates.zTest);
            mtrl.SetZWriteEnabled(renderStates.zWrite);

            // Store entry and return it
            mRTGizmoHandleMaterialMap.Add(renderStates, mtrl);
            return mtrl;
        }
        #endregion
    }
    #endregion
}
