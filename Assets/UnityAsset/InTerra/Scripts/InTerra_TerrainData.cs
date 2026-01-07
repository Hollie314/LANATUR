using UnityEngine;

namespace InTerra
{
    [AddComponentMenu("/")]
    [ExecuteInEditMode]
    public class InTerra_TerrainData : MonoBehaviour
    {
        [HideInInspector] MaterialPropertyBlock mtb;
        [HideInInspector] public Terrain terrain;
        [HideInInspector] public Material checkMat;
        [SerializeField, HideInInspector] public Texture2DArray normalTextureArray16;
        [SerializeField, HideInInspector] public Texture2DArray splatTextureArray16;

        void OnEnable()
        {
            if (gameObject.TryGetComponent<Terrain>(out terrain))
            {
                DictionaryMaterialTerrain materialTerrain = InTerra_Data.GetSceneData().MaterialTerrain;

                if (materialTerrain != null && materialTerrain.Count > 0)
                {
                    foreach (Material mat in materialTerrain.Keys)
                    {
                        if (materialTerrain[mat] == terrain)
                        {
                            checkMat = mat;
                            break;
                        }
                    }
                }
                if (checkMat == null)
                {
                    if (!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.UpdateTerrainData(InTerra_Setting.DictionaryUpdate);
                }
            }
        }

        void Update()
        {
            if (terrain && terrain.materialTemplate)
            {
                Material terrMat = terrain.materialTemplate;

                if(InTerra_Data.CheckTerrainShader(terrMat) && terrain.terrainData)
                {
                    InTerra_UpdateAndCheck sceneData = InTerra_Data.GetSceneData();
                    sceneData.TracksUpdate = terrMat.GetFloat("_Tracks") == 1;

                    if (InTerra_Data.MaterialPropertyBlockNeedLayersUpdate(terrain))
                    {
                        if (mtb == null) mtb = new MaterialPropertyBlock();
                        terrain.GetSplatMaterialPropertyBlock(mtb);

                        if (terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"))
                        {
                            if (!mtb.GetTexture("_SplatArray16") || !mtb.GetTexture("_NormalArray16") || (checkMat && !checkMat.GetTexture("_SplatArray16")))
                            {

                                if(splatTextureArray16 == null || normalTextureArray16 == null || (checkMat && !checkMat.GetTexture("_SplatArray16") && checkMat.GetFloat("_IsOnTerrain") == 1))
                                {
                                    InTerra_Data.UpdateTerrainData(false);
                                }
                                InTerra_Data.TerrainMaterialPropertyBlockUpdate(terrain, true);
                            }                            
                        }
                        #if !USING_HDRP
                        else
                        {
                            if (!mtb.GetTexture("_Control1") || !mtb.GetTexture("_Splat4"))
                            {
                                InTerra_Data.TerrainMaterialPropertyBlockUpdate(terrain, true);
                            }
                        }
                        #endif
                    }

                    if (checkMat == null) 
                    {
                        return;
                    }
                    else if(!InTerra_Data.CheckObjectShader(checkMat) || checkMat.GetFloat("_IsOnTerrain") == 0)
                    {
                        checkMat = null;
                        if(!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.UpdateTerrainData(InTerra_Setting.DictionaryUpdate);
                    }
                    else if (terrain.terrainData.heightmapTexture.IsCreated() && checkMat.GetTexture("_TerrainHeightmapTexture") == null)
                    {
                        if(!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.UpdateTerrainData(false);
                    }
                }

                #if UNITY_EDITOR												
                    if (!InTerra_Data.CheckTerrainShader(terrMat))
                    {
                        terrain.SetSplatMaterialPropertyBlock(new MaterialPropertyBlock());
                        DestroyImmediate(this);
                    }   
                #endif
            }
            else
            {
                gameObject.TryGetComponent<Terrain>(out terrain);
            }
        }
    }
}
