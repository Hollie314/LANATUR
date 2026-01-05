using UnityEngine;

namespace InTerra
{
    [AddComponentMenu("/")]
    [ExecuteInEditMode]
    public class InTerra_MeshTerrainData : MonoBehaviour
    {
        [SerializeField, HideInInspector] public Texture2D ControlMap;
        [SerializeField, HideInInspector] public Texture2D ControlMap1;
        [SerializeField, HideInInspector] public Texture2D HeightMap;
        #if (USING_URP || USING_HDRP)
            [SerializeField, HideInInspector] public Texture2D ControlMap2;
            [SerializeField, HideInInspector] public Texture2D ControlMap3;
            [SerializeField, HideInInspector] public TerrainLayer[] TerrainLayers = new TerrainLayer[16];

            [SerializeField, HideInInspector] public Texture2DArray normalTextureArray16;
            [SerializeField, HideInInspector] public Texture2DArray splatTextureArray16;

		#else
            [SerializeField, HideInInspector] public TerrainLayer[] TerrainLayers = new TerrainLayer[8];

        #endif

        private void Update()
        {
            #if (USING_URP || USING_HDRP)
                Material mat = GetComponent<Renderer>().sharedMaterial;
                if (mat && mat.IsKeywordEnabled("_LAYERS_SIXTEEN") && !mat.GetTexture("_SplatArray16"))
                {
                    if (!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.UpdateTerrainData(InTerra_Setting.DictionaryUpdate);
                }
            #endif
        }
    }
}
