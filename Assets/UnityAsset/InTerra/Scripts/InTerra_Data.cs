//==========================================================
//-------------|          INTERRA          |---------------
//==========================================================
//-------------|           4.6.2           |--------------- 
//==========================================================
//-------------|   © INEFFABILIS ARCANUM   |---------------
//==========================================================

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEngine.Rendering;
#endif

#if USING_HDRP
	using UnityEngine.Rendering.HighDefinition;
#endif
#if USING_URP
	using UnityEngine.Rendering.Universal;
#endif

namespace InTerra
{
	public static class InTerra_Data
	{
		public const string InTerraVersion = "4.6.2";

		public const string ObjectShaderName = "InTerra/Built-in/Object into Terrain Integration";
		public const string DiffuseObjectShaderName = "InTerra/Built-in/Diffuse/Object into Terrain Integration (Diffuse)";
		public const string URPObjectShaderName = "InTerra/URP/Object into Terrain Integration";
		public const string HDRPObjectShaderName = "InTerra/HDRP/Object into Terrain Integration";
		public const string HDRPObjectTessellationShaderName = "InTerra/HDRP Tessellation/Object into Terrain Integration Tessellation";

		public const string TerrainShaderName = "InTerra/Built-in/Terrain (Standard With Features)";
		public const string DiffuseTerrainShaderName = "InTerra/Built-in/Diffuse/Terrain (Diffuse With Features)";
		public const string URPTerrainShaderName = "InTerra/URP/Terrain (Lit with Features)";
		public const string HDRPTerrainShaderName = "InTerra/HDRP/Terrain (Lit with Features)";
		public const string HDRPTerrainTessellationShaderName = "InTerra/HDRP Tessellation/Terrain (Lit with Features)";

		public const string MeshTerrainShaderName = "InTerra/Built-in/Mesh Terrain (Standard)";
		public const string DiffuseMeshTerrainShaderName = "InTerra/Built-in/Diffuse/Mesh Terrain (Diffuse)";
		public const string URPMeshTerrainShaderName = "InTerra/URP/Mesh Terrain";
		public const string HDRPMeshTerrainShaderName = "InTerra/HDRP/Mesh Terrain";
		public const string HDRPMeshTerrainTessellationShaderName = "InTerra/HDRP Tessellation/Mesh Terrain Tessellation";

		public const string TessellationShaderFolder = "InTerra/HDRP Tessellation";


		static readonly string[] TerrainToObjectsTextures = {
			"_TerrainColorTintTexture",
			"_TerrainNormalTintTexture",
			"_TrackDetailTexture",
			"_TrackDetailNormalTexture"};

		static readonly string[] TerrainToObjectsFloats = {
			"_HeightTransition",
			"_Distance_HeightTransition",
			"_HT_distance_scale",
			"_HT_cover",
			"_TriplanarOneToAllSteep",
			"_TriplanarSharpness",
			"_TerrainColorTintStrenght",
			"_TerrainColorTintMode",
			"_TerrainNormalTintStrenght",
			"_HeightmapBlending",
			"_Terrain_Parallax",
			"_Tracks",
			"_MipMapLevel",
			"_TrackAO",
			"_TrackDetailNormalStrenght",
			"_TrackNormalStrenght",
			"_TrackHeightOffset",
			"_TrackHeightTransition",
			"_ParallaxTrackAffineSteps",
			"_ParallaxTrackSteps",
			"_TrackEdgeNormals",
			"_TrackEdgeSharpness",
			"_PuddleHorizontalWeight",
			"_PuddleWetnessHorizontalWeight",
			"_Gamma",
			"_WorldMapping"};

		static readonly string[] TerrainToObjectsVectors = {
			"_HT_distance",
			"_TerrainColorTintDistance",
			"_TerrainNormalTintDistance",
			"_MipMapFade"};

		static readonly string[] TerrainToObjectsTessellationProperties = {
			"_TessellationFactorMinDistance",
			"_TessellationFactorMaxDistance",
			"_Tessellation_HeightTransition",
			"_TessellationShadowQuality",
			"_TrackTessallationHeightTransition",
			"_TrackTessallationHeightOffset",
			"_TerrainAddTessellationOffset",
			"_TerrainAddTessellationHeight",
			"_TerrainAddTessTextureType"};

		static readonly string[] TerrainToObjectsKeywords = {"_TERRAIN_DISTANCEBLEND"};
		//-----------------------------------------------------------------------------

		//Note: The script "InTerra_UpdateAndCheck" was originally for checking the loss of terrain render texture heightmap and updating materials when the loss happened, there was also data needed per scene, recently the checking was moved to "InTerra_TerrainData" script to improve updating when multiple scenes are loaded. The name of the script would be changed to InTerra_SceneData to better present current use, but changing the name in package may leed to some problems, so it still has its originall name.
		const string UpdaterName = "InTerra_UpdateAndCheck";
		static public GameObject Updater;
		static public InTerra_UpdateAndCheck SceneData;
		static public bool initUpdate;

		static Camera TrackCamera;
		static public string TrackCameraTag = "MainCamera";
		static Vector3 TrackCameraForwardVec;
		static Vector3 TrackCameraPositon;
		static float TracksUpdateTimeCount;
		static bool TracksCameraUpdate;
		public static bool initTrack;

		static MaterialPropertyBlock mtb = new MaterialPropertyBlock();

		#if UNITY_EDITOR
			static string InTerraPath;		

			const int DEFINE_MASKMAP_LineNumber = 1;
			const int DEFINE_NORMAL_MASK_LineNumber = 2;
			const int DEFINE_HEIGTH_ONLY_LineNumber = 3;

			const int RESTRICT_NORMALMAP_LineNumber = 5;
			const int RESTRICT_HEIGHTBLEND_LineNumber = 6;	
			const int RESTRICT_TERR_PARALAX_LineNumber = 7;
			const int RESTRICT_TRACKS_LineNumber = 8;
			const int RESTRICT_OBJ_PARALLAX_LineNumber = 10;
			const int RESTRICT_PUDDLES_LineNumber = 12;
			#if USING_URP
				const int UNITY_VERSION_LineNumber = 13;
			#endif		
			#if !(USING_URP || USING_HDRP)
				const int EIGHT_LAYERS_PASS_LineNumber = 13;
			#endif	
		#endif

		#if (USING_URP || USING_HDRP)
			public static Texture2DArray splatArray16;
		#endif

		public static void UpdateTerrainData(bool UpdateDictionary)
		{
			Terrain[] terrains = Terrain.activeTerrains;
			List<MeshRenderer> meshTerrainsList = GetSceneData().MeshTerrainsList;
			DictionaryMaterialTerrain materialTerrain = GetSceneData().MaterialTerrain;
			DictionaryMaterialMeshTerrain materialMeshTerrain = GetSceneData().MaterialMeshTerrain;
			Dictionary<Terrain, InTerra_TerrainData> terrainData = TerrainDataDictionary();

			#if UNITY_2023_1_OR_NEWER
				InTerra_MeshTerrainData[] meshTerrains = Object.FindObjectsByType<InTerra_MeshTerrainData>(FindObjectsSortMode.None);
			#else
				InTerra_MeshTerrainData[] meshTerrains = Object.FindObjectsOfType<InTerra_MeshTerrainData>();
			#endif

			//------------------------------------------------------------
			if (terrains.Length == 0 && meshTerrains.Length == 0) return;
			//------------------------------------------------------------

			TerrainMaterialUpdate();

			if (UpdateDictionary || !GetSceneData().FirstInit)
			{
				//--- Dictionary of InTerra materials & sum position of renderers with the material ---
				Dictionary<Material, Vector3> matPos = new Dictionary<Material, Vector3>();

				#if UNITY_2023_1_OR_NEWER
					MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
				#else
					MeshRenderer[] renderers = Object.FindObjectsOfType<MeshRenderer>();
				#endif

				meshTerrainsList.Clear();

				foreach (MeshRenderer rend in renderers)
				{
					if (rend != null && rend.bounds != null)
					{
						foreach (Material mat in rend.sharedMaterials)
						{
							MaterialRenderesSumPossion(mat, rend, ref matPos);

							if (!meshTerrainsList.Contains(rend) && CheckMeshTerrainShader(mat)) 
							{
								meshTerrainsList.Add(rend);
							}
						}
					}
				}
				//-------------------------------------------------------------------------------
				Dictionary<Material, Terrain> tempMatTerDict = CopyMaterialTerrainDictionary(materialTerrain);
				Dictionary<Material, MeshRenderer> tempMatMeshTerDict = CopyMaterialMeshDictionary(materialMeshTerrain);

				materialTerrain.Clear();
				materialMeshTerrain.Clear();


				foreach (Material mat in matPos.Keys)
				{
					Vector2 averagePos = matPos[mat] / matPos[mat].z;
					mat.SetFloat("_IsOnTerrain", 0);

					//========== MATERIAL-TERRAIN DICTIONARY ============
					foreach (Terrain terrain in terrains)
					{
						if (!materialTerrain.ContainsKey(mat) && CheckTerrainShader(terrain.materialTemplate))
						{
							if (mat.GetFloat("_CustomTerrainSelection") > 0 && tempMatTerDict.ContainsKey(mat))
							{
								materialTerrain.Add(mat, tempMatTerDict[mat]);
								mat.SetFloat("_IsOnTerrain", 1);
							}
							else
							{
								if (CheckPosition(terrain, averagePos))
                                {
									materialTerrain.Add(mat, terrain);
									mat.SetFloat("_IsOnTerrain", 1);
								}									
							}
						}
					}

					//=========  MATERIAL-MESH TERRAIN DICTIONARY  ==========
					foreach (MeshRenderer meshTerrain in meshTerrainsList)
					{
						if (!materialMeshTerrain.ContainsKey(mat))
						{
							if (mat.GetFloat("_CustomTerrainSelection") == 1 && mat.HasProperty("_MeshTerrain") 
										&& mat.GetFloat("_MeshTerrain") == 1 && tempMatMeshTerDict.ContainsKey(mat))
							{
								materialMeshTerrain.Add(mat, tempMatMeshTerDict[mat]);
								mat.SetFloat("_IsOnTerrain", 1);
							}
							else if (mat.GetFloat("_CustomTerrainSelection") == 0)
							{
								if (CheckMeshTerrainPosition(meshTerrain, averagePos))
								{
									materialMeshTerrain.Add(mat, meshTerrain);
									mat.SetFloat("_IsOnTerrain", 1);
								}
							}
						}						
					}
				}

				GetSceneData().FirstInit = true;
				#if UNITY_EDITOR
					EditorUtility.SetDirty(GetSceneData());
				#endif
			}

			//================================================================================
			//-------------------|    TERRAIN DATA TO OBJECT MATERIALS    |-------------------
			//================================================================================
			foreach (Material mat in materialTerrain.Keys)
			{
				Terrain terrain = materialTerrain[mat];
				if (terrain != null && terrain.materialTemplate != null && CheckTerrainShader(terrain.materialTemplate) && CheckObjectShader(mat))
				{
					#if (USING_URP || USING_HDRP)
					if (!(mat.IsKeywordEnabled("_LAYERS_ONE") || mat.IsKeywordEnabled("_LAYERS_TWO")))
					{
						
						SetKeyword(mat, "_LAYERS_EIGHT", terrain.materialTemplate.IsKeywordEnabled("_LAYERS_EIGHT"));							
						SetKeyword(mat, "_LAYERS_SIXTEEN", terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"));
						
					}
					#endif

					Vector4 heightScale = new Vector4(terrain.terrainData.heightmapScale.x, 
													  terrain.terrainData.heightmapScale.y / (32766.0f / 65535.0f),
													  terrain.terrainData.heightmapScale.z, 
													  terrain.terrainData.heightmapScale.y);

					SetTerrainDataToMaterial(mat, 
											terrain.terrainData.size, 
											terrain.transform.position, 
											heightScale, 
											terrain.terrainData.heightmapTexture, 
											terrain.terrainData.alphamapTextures, 
											terrain.materialTemplate, 
											terrain.terrainData.terrainLayers);

					terrainData[terrain].checkMat = mat;

					if (terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"))
					{
						mat.SetTexture("_SplatArray16", terrainData[terrain].splatTextureArray16);
						mat.SetTexture("_NormalArray16", terrainData[terrain].normalTextureArray16);
					}
				}
			}

			foreach (Material mat in materialMeshTerrain.Keys)
			{
				InTerra_MeshTerrainData meshTerrainData = materialMeshTerrain[mat] ? materialMeshTerrain[mat].GetComponent<InTerra_MeshTerrainData>() : null;
				if (meshTerrainData != null)
				{
					Material meshTerrainMaterial = materialMeshTerrain[mat].sharedMaterial;
						
					#if (USING_URP || USING_HDRP)
						Texture2D[] controlmaps = new Texture2D[] { meshTerrainData.ControlMap, 
																	meshTerrainData.ControlMap1, 
																	meshTerrainData.ControlMap2, 
																	meshTerrainData.ControlMap3 };
					#else	
						Texture2D[] controlmaps = new Texture2D[] { meshTerrainData.ControlMap, meshTerrainData.ControlMap1 };
					#endif	

					#if (USING_URP || USING_HDRP)							
					if (!(mat.IsKeywordEnabled("_LAYERS_ONE") || mat.IsKeywordEnabled("_LAYERS_TWO")))
					{
						SetKeyword(mat, "_LAYERS_EIGHT", meshTerrainMaterial.IsKeywordEnabled("_LAYERS_EIGHT"));
						SetKeyword(mat, "_LAYERS_SIXTEEN", meshTerrainMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"));

					}
					#endif

					SetTerrainDataToMaterial(mat, 
											meshTerrainMaterial.GetVector("_TerrainSize"), 
											meshTerrainMaterial.GetVector("_TerrainPosition"), 
											meshTerrainMaterial.GetVector("_TerrainHeightmapScale"), 
											meshTerrainMaterial.GetTexture("_TerrainHeightmapTexture"), 
											controlmaps, 
											meshTerrainMaterial, 
											meshTerrainData.TerrainLayers);

					#if (USING_URP || USING_HDRP)	
					if (meshTerrainMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"))
					{
						mat.SetTexture("_SplatArray16", meshTerrainData.splatTextureArray16);
						mat.SetTexture("_NormalArray16", meshTerrainData.normalTextureArray16);
					}
					#endif
				}
			}
		
			#if UNITY_EDITOR
				if (!CheckDefinedKeywords()) WriteDefinedKeywords();
				#if USING_URP
					URPShadersVersionAdjust();
				#endif
			#endif						
		}


		//===============================================================================


		public static bool CheckPosition(Terrain terrain, Vector2 position)
		{
			return terrain != null && terrain.terrainData != null
			&& terrain.GetPosition().x <= position.x && (terrain.GetPosition().x + terrain.terrainData.size.x) > position.x
			&& terrain.GetPosition().z <= position.y && (terrain.GetPosition().z + terrain.terrainData.size.z) > position.y;
		}

		public static bool CheckMeshTerrainPosition(MeshRenderer terrain, Vector2 position)
		{
			return terrain != null
			&& terrain.bounds.min.x <= position.x && (terrain.bounds.min.x + terrain.bounds.size.x) > position.x
			&& terrain.bounds.min.z <= position.y && (terrain.bounds.min.z + terrain.bounds.size.z) > position.y;
		}

		public static bool CheckObjectShader(Material mat)
		{
			return mat && mat.shader && mat.shader.name != null
			&& (mat.shader.name == ObjectShaderName
			 || mat.shader.name == DiffuseObjectShaderName
			 || mat.shader.name == URPObjectShaderName
			 || mat.shader.name == HDRPObjectShaderName
			 || mat.shader.name == HDRPObjectTessellationShaderName);
		}

		public static bool CheckTerrainShader(Material mat)
		{
			return mat && mat.shader && mat.shader.name != null
			   && (mat.shader.name == TerrainShaderName
				|| mat.shader.name == DiffuseTerrainShaderName
				|| mat.shader.name == URPTerrainShaderName
				|| mat.shader.name.Contains(HDRPTerrainShaderName)
				|| mat.shader.name.Contains(HDRPTerrainTessellationShaderName));
		}

		public static bool CheckTerrainShaderContains(Terrain terrain, string name)
		{
			return terrain
				&& terrain.materialTemplate
				&& terrain.materialTemplate.shader
				&& terrain.materialTemplate.shader.name != null
				&& terrain.materialTemplate.shader.name.Contains(name);
		}

		public static bool CheckMeshTerrainShader(Material mat)
		{
			return mat && mat.shader && mat.shader.name != null
			&& (mat.shader.name == MeshTerrainShaderName
			|| mat.shader.name == DiffuseMeshTerrainShaderName
			|| mat.shader.name == URPMeshTerrainShaderName
			|| mat.shader.name.Contains(HDRPMeshTerrainShaderName)
			|| mat.shader.name.Contains(HDRPMeshTerrainTessellationShaderName));
		}

		static void MaterialRenderesSumPossion(Material mat, MeshRenderer rend, ref Dictionary<Material, Vector3> matPos)
		{
			if (CheckObjectShader(mat))
			{
				if (!matPos.ContainsKey(mat))
				{
					matPos.Add(mat, new Vector3(rend.bounds.center.x, rend.bounds.center.z, 1));
				}
				else
				{
					Vector3 sumPos = matPos[mat];
					sumPos.x += rend.bounds.center.x;
					sumPos.y += rend.bounds.center.z;
					sumPos.z += 1;
					matPos[mat] = sumPos;
				}
			}
		}

		static Dictionary<Terrain, InTerra_TerrainData> TerrainDataDictionary()
		{
			Dictionary<Terrain, InTerra_TerrainData> terrainData = new Dictionary<Terrain, InTerra_TerrainData>();
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				if (CheckTerrainShader(terrain.materialTemplate))
				{
					InTerra_TerrainData td;
					if (!terrain.TryGetComponent<InTerra_TerrainData>(out td))
					{
						td = terrain.gameObject.AddComponent<InTerra_TerrainData>();
					}
					terrainData.Add(terrain, td);
				}
			}
			return terrainData;
		}


		public static void SetTerrainDataToMaterial(Material objectMaterial, Vector3 terrainSize, Vector3 terrainPosition, Vector4 heightmapScale, Texture heightmap, Texture2D[] controlMaps, Material terrainMaterial, TerrainLayer[] tl)
		{
			objectMaterial.SetVector("_TerrainSize", terrainSize);
			objectMaterial.SetVector("_TerrainPosition", terrainPosition);
			objectMaterial.SetVector("_TerrainHeightmapScale", heightmapScale);
			objectMaterial.SetTexture("_TerrainHeightmapTexture", heightmap);
			objectMaterial.SetFloat("_NumLayersCount", tl.Count());

			if (CheckTerrainShader(terrainMaterial) || CheckMeshTerrainShader(terrainMaterial))
			{
				TerrainKeywordsToMaterial(terrainMaterial, objectMaterial, TerrainToObjectsKeywords);
				SetTerrainFloatsToMaterial(terrainMaterial, objectMaterial, TerrainToObjectsFloats);				
				SetTerrainTextureToMaterial(terrainMaterial, objectMaterial, TerrainToObjectsTextures);
				SetTerrainVectorsToMaterial(terrainMaterial, objectMaterial, TerrainToObjectsVectors);

				if (objectMaterial.shader.name == HDRPObjectTessellationShaderName && terrainMaterial.shader.name.Contains(TessellationShaderFolder))
				{
					float terrainMaxDisplacement = terrainMaterial.GetFloat("_TessellationMaxDisplacement");
					float objectMaxDisplacement = objectMaterial.GetFloat("_TessellationObjMaxDisplacement");

					objectMaterial.SetFloat("_TessellationMaxDisplacement", terrainMaxDisplacement > objectMaxDisplacement ? terrainMaxDisplacement : objectMaxDisplacement);
					SetTerrainFloatsToMaterial(terrainMaterial, objectMaterial, TerrainToObjectsTessellationProperties);

					string[] addtDisp = { "_TerrainAddTessellationTexture" };
					SetTerrainTextureToMaterial(terrainMaterial, objectMaterial, addtDisp);
				}
			}
			else
			{
				#if (USING_HDRP || USING_URP)
					DisableKeywords(objectMaterial, TerrainToObjectsKeywords);
					if (terrainMaterial.IsKeywordEnabled("_TERRAIN_BLEND_HEIGHT")) objectMaterial.SetFloat("_HeightmapBlending", 1); else objectMaterial.SetFloat("_HeightmapBlending", 0);
					objectMaterial.SetFloat("_HeightTransition", 60 - 60 * terrainMaterial.GetFloat("_HeightTransition"));
				#else
					DisableKeywords(objectMaterial, TerrainToObjectsKeywords);							
				#endif
			}

			//----------- ONE LAYER ------------
			if (objectMaterial.IsKeywordEnabled("_LAYERS_ONE"))
			{
				#if UNITY_EDITOR //The TerrainLayers in Editor are referenced by GUID, in Build by TerrainLayers array index
					TerrainLayer terainLayer = TerrainLayerFromGUID(objectMaterial, "TerrainLayerGUID_1");
					TerrainLaeyrDataToMaterial(terainLayer, "0", objectMaterial);
				#else
					int layerIndex1 = (int)objectMaterial.GetFloat("_LayerIndex1");
					CheckLayerIndex(tl, 0, objectMaterial, ref layerIndex1);
					TerrainLaeyrDataToMaterial(tl[layerIndex1], "0", objectMaterial);	
				#endif
			}

			//----------- TWO LAYERS ------------
			else if (objectMaterial.IsKeywordEnabled("_LAYERS_TWO"))
			{
				#if UNITY_EDITOR
					TerrainLayer terainLayer1 = TerrainLayerFromGUID(objectMaterial, "TerrainLayerGUID_1");
					TerrainLayer terainLayer2 = TerrainLayerFromGUID(objectMaterial, "TerrainLayerGUID_2");
					TerrainLaeyrDataToMaterial(terainLayer1, "0", objectMaterial);
					TerrainLaeyrDataToMaterial(terainLayer2, "1", objectMaterial);
					int layerIndex1 = tl.ToList().IndexOf(terainLayer1);
					int layerIndex2 = tl.ToList().IndexOf(terainLayer2);
				#else
					int layerIndex1 = (int)objectMaterial.GetFloat("_LayerIndex1"); 
					int layerIndex2 = (int)objectMaterial.GetFloat("_LayerIndex2");
					CheckLayerIndex(tl, 0, objectMaterial, ref layerIndex1);
					CheckLayerIndex(tl, 1, objectMaterial, ref layerIndex2);
					TerrainLaeyrDataToMaterial(tl[layerIndex1], "0", objectMaterial);
					TerrainLaeyrDataToMaterial(tl[layerIndex2], "1", objectMaterial);	
				#endif

				objectMaterial.SetFloat("_ControlNumber", layerIndex1 % 4);

				if (controlMaps.Length > layerIndex1 / 4) objectMaterial.SetTexture("_Control", controlMaps[layerIndex1 / 4]);
				if (layerIndex1 > 3 || layerIndex2 > 3) objectMaterial.SetFloat("_HeightmapBlending", 0);
			}

			//----------- ONE PASS ------------
			else
			{
				int passNumber = (int)objectMaterial.GetFloat("_PassNumber");
				int layersNumber = 4;

				if (EightLayersEnabled(objectMaterial))
				{
					passNumber = 0;
					layersNumber = 8;
				}

				#if (USING_URP || USING_HDRP)
				else if(objectMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"))
				{
					passNumber = 0;
					layersNumber = 16;

					if (controlMaps.Length > 2) objectMaterial.SetTexture("_Control2", controlMaps[2]);
					if (controlMaps.Length > 3) objectMaterial.SetTexture("_Control3", controlMaps[3]);
				}
				#endif

				for (int i = 0; (i + (passNumber * layersNumber)) < tl.Length && i < layersNumber; i++)
				{
					TerrainLaeyrDataToMaterial(tl[i + (passNumber * layersNumber)], i.ToString(), objectMaterial);
				}

				if (EightLayersEnabled(objectMaterial) || objectMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"))
				{
					if (controlMaps.Length > 0) objectMaterial.SetTexture("_Control", controlMaps[0]);
					if (controlMaps.Length > 1) objectMaterial.SetTexture("_Control1", controlMaps[1]);
				}
				else
				{
					if (controlMaps.Length > passNumber) objectMaterial.SetTexture("_Control", controlMaps[passNumber]);
				}

				if (passNumber > 0) objectMaterial.SetFloat("_HeightmapBlending", 0);
			}

			if ((objectMaterial.shader.name != DiffuseObjectShaderName) && objectMaterial.GetFloat("_DisableTerrainParallax") == 1)
			{
				objectMaterial.SetFloat("_Terrain_Parallax", 0.0f);							
			}

			if (objectMaterial.GetFloat("_DisableDistanceBlending") == 1)
			{
				objectMaterial.DisableKeyword("_TERRAIN_DISTANCEBLEND");
			}

			if (objectMaterial.shader.name == DiffuseObjectShaderName)
			{
				if (objectMaterial.GetTexture("_BumpMap")) { objectMaterial.EnableKeyword("_OBJECT_NORMALMAP"); } else { objectMaterial.DisableKeyword("_OBJECT_NORMALMAP"); }
			}					
		}

		static Dictionary<Material, Terrain> CopyMaterialTerrainDictionary(Dictionary<Material, Terrain> matTerDict)
		{
			Dictionary<Material, Terrain> tempMatTerDict = new Dictionary<Material, Terrain>();
			foreach (Material mt in matTerDict.Keys)
			{
				if (!tempMatTerDict.ContainsKey(mt))
				{
					tempMatTerDict.Add(mt, matTerDict[mt]);
				}
			}
			return tempMatTerDict;
		}

		static Dictionary<Material, MeshRenderer> CopyMaterialMeshDictionary(Dictionary<Material, MeshRenderer> matMeshDict)
		{
			Dictionary<Material, MeshRenderer> tempMatMeshDict = new Dictionary<Material, MeshRenderer>();

			foreach (Material mt in matMeshDict.Keys)
			{
				if (!tempMatMeshDict.ContainsKey(mt))
				{
					tempMatMeshDict.Add(mt, matMeshDict[mt]);
				}
			}
			return tempMatMeshDict;
		}

		#if UNITY_EDITOR
			public static TerrainLayer TerrainLayerFromGUID(Material mat, string tag)
			{
				return (TerrainLayer)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(mat.GetTag(tag, false)), typeof(TerrainLayer));
			}
			public static TerrainData TerrainDataFromGUID(Material mat, string tag)
			{
				return (TerrainData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(mat.GetTag(tag, false)), typeof(TerrainData));
			}

			public static void TextureAlphaSmoothness(TerrainLayer tl)
			{
				if (tl)
				{
					TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tl.diffuseTexture)) as TextureImporter;
					if (importer && importer.DoesSourceTextureHaveAlpha())
					{
						tl.smoothness = 1;
					}
				}
			}
		#endif

		public static void TerrainLaeyrDataToMaterial(TerrainLayer tl, string n, Material mat)
		{
			#if UNITY_EDITOR
				TextureAlphaSmoothness(tl);
			#endif

			mat.SetTexture("_Splat" + n, tl ? tl.diffuseTexture : null);
			mat.SetTexture("_Normal" + n, tl ? tl.normalMapTexture : null);
			mat.SetTexture("_Mask" + n, tl ? tl.maskMapTexture : null);
			mat.SetVector("_SplatUV" + n, tl ? new Vector4(tl.tileSize.x, tl.tileSize.y, tl.tileOffset.x, tl.tileOffset.y) : new Vector4(1, 1, 0, 0));
			mat.SetVector("_MaskMapRemapScale" + n, tl ? tl.maskMapRemapMax - tl.maskMapRemapMin : new Vector4(1, 1, 1, 1));
			mat.SetVector("_MaskMapRemapOffset" + n, tl ? tl.maskMapRemapMin : new Vector4(0, 0, 0, 0));
			mat.SetVector("_DiffuseRemapScale" + n, tl ? tl.diffuseRemapMax : new Vector4(1, 1, 1, 1));
			mat.SetVector("_DiffuseRemapOffset" + n, tl ? tl.diffuseRemapMin : new Vector4(0, 0, 0, 0));
			mat.SetFloat("_Smoothness" + n, tl ? tl.smoothness : 0.0f);
			mat.SetFloat("_Metallic" + n, tl ? tl.metallic : 0.0f);
			mat.SetFloat("_NormalScale" + n, tl ? tl.normalScale : 0.0f);
			mat.SetColor("_Specular" + n, tl ? tl.specular : new Color(0, 0, 0, 0)); 
			mat.SetFloat("_LayerHasMask" + n, tl ? (float)(tl.maskMapTexture ? 1.0 : 0.0) : (float)0.0);			
		}

		public static void TerrainLaeyrDataToTerrain(Vector3 size, TerrainLayer tl, string n, MaterialPropertyBlock mtb)
		{
			#if UNITY_EDITOR
				TextureAlphaSmoothness(tl);
			#endif
			
			mtb.SetTexture("_Splat" + n, tl && tl.diffuseTexture ? tl.diffuseTexture : Texture2D.whiteTexture);
			mtb.SetTexture("_Normal" + n, tl && tl.normalMapTexture ? tl.normalMapTexture : Texture2D.normalTexture);
			mtb.SetTexture("_Mask" + n, tl && tl.maskMapTexture ? tl.maskMapTexture : Texture2D.linearGrayTexture);
			mtb.SetVector("_Splat" + n + "_ST", tl ? new Vector4(size.x / tl.tileSize.x, size.z / tl.tileSize.y, tl.tileOffset.x, tl.tileOffset.y) : new Vector4(1, 1, 0, 0));
			mtb.SetVector("_MaskMapRemapScale" + n, tl ? tl.maskMapRemapMax - tl.maskMapRemapMin : new Vector4(1, 1, 1, 1));
			mtb.SetVector("_MaskMapRemapOffset" + n, tl ? tl.maskMapRemapMin : new Vector4(0, 0, 0, 0));
			mtb.SetVector("_DiffuseRemapScale" + n, tl ? tl.diffuseRemapMax - tl.diffuseRemapMin : new Vector4(1, 1, 1, 1));
			mtb.SetVector("_DiffuseRemapOffset" + n, tl ? tl.diffuseRemapMin : new Vector4(0, 0, 0, 0));
			mtb.SetFloat("_NormalScale" + n, tl ? tl.normalScale : 1);
			mtb.SetFloat("_Smoothness" + n, tl ? tl.smoothness : 0);
			mtb.SetFloat("_Metallic" + n, tl ? tl.metallic : 0);
			mtb.SetColor("_Specular" + n, tl ? tl.specular : new Color(0, 0, 0, 0));
			mtb.SetFloat("_LayerHasMask" + n, tl ? (float)(tl.maskMapTexture ? 1.0 : 0.0) : (float)0.0);
			mtb.SetVector("_TerrainSizeXZPosY", new Vector3(size.x, size.z, 0.0f));
		}

		public static void CheckLayerIndex(TerrainLayer[] layers, int n, Material mat, ref int layerIndex)
		{
			bool diffuse = mat.shader.name == DiffuseObjectShaderName;
			foreach (TerrainLayer tl in layers)
			{
				bool equal = tl && mat.GetTexture("_Splat" + n.ToString()) == tl.diffuseTexture
				&& mat.GetTexture("_Normal" + n.ToString()) == tl.normalMapTexture
				&& mat.GetVector("_TerrainNormalScale")[n] == tl.normalScale
				&& mat.GetTexture("_Mask" + n.ToString()) == tl.maskMapTexture
				&& mat.GetVector("_SplatUV" + n.ToString()) == new Vector4(tl.tileSize.x, tl.tileSize.y, tl.tileOffset.x, tl.tileOffset.y)
				&& mat.GetVector("_MaskMapRemapScale" + n.ToString()) == tl.maskMapRemapMax - tl.maskMapRemapMin
				&& mat.GetVector("_MaskMapRemapOffset" + n.ToString()) == tl.maskMapRemapMin
				&& mat.GetVector("_DiffuseRemapScale" + n.ToString()) == tl.diffuseRemapMax
				&& mat.GetVector("_DiffuseRemapOffset" + n.ToString()) == tl.diffuseRemapMin;

				bool equalMetallicSmooth = diffuse || tl && mat.GetVector("_TerrainMetallic")[n] == tl.metallic
				&& mat.GetVector("_TerrainSmoothness")[n] == tl.smoothness;

				if (equal && equalMetallicSmooth)
				{
					layerIndex = layers.ToList().IndexOf(tl);
					mat.SetFloat("_LayerIndex" + (n + 1).ToString(), layerIndex);
				}
			}
		}

		static void SetTerrainFloatsToMaterial(Material terrainMaterial, Material objectMaterial, string[] properties)
		{
			foreach (string prop in properties)
			{
				objectMaterial.SetFloat(prop, terrainMaterial.GetFloat(prop));
			}
		}

		static void SetTerrainVectorsToMaterial(Material terrainMaterial, Material objectMaterial, string[] vectors)
		{
			foreach (string vec in vectors)
			{
				objectMaterial.SetVector(vec, terrainMaterial.GetVector(vec));
			}
		}

		static void SetTerrainTextureToMaterial(Material terrainMaterial, Material objectMaterial, string[] textures)
		{
			foreach (string texture in textures)
			{
				objectMaterial.SetTexture(texture, terrainMaterial.GetTexture(texture));
				objectMaterial.SetTextureScale(texture, terrainMaterial.GetTextureScale(texture));
				objectMaterial.SetTextureOffset(texture, terrainMaterial.GetTextureOffset(texture));
			}
		}

		static void TerrainKeywordsToMaterial(Material terrainMaterial, Material objectMaterial, string[] keywords)
		{
			foreach (string keyword in keywords)
			{
				if (terrainMaterial.IsKeywordEnabled(keyword))
				{
					objectMaterial.EnableKeyword(keyword);
				}
				else
				{
					objectMaterial.DisableKeyword(keyword);
				}
			}
		}

		static void DisableKeywords(Material mat, string[] keywords)
		{
			foreach (string keyword in keywords)
			{
				mat.DisableKeyword(keyword);
			}
		}
		
		public static InTerra_UpdateAndCheck GetSceneData()
		{
			if (SceneData == null)
			{
				if (!Updater)
				{
					if (!GameObject.Find(UpdaterName))
					{
						Updater = new GameObject(UpdaterName);
						Updater.AddComponent<InTerra_UpdateAndCheck>();

						Updater.hideFlags = HideFlags.HideInInspector;
						Updater.hideFlags = HideFlags.HideInHierarchy;						
					}
					else
					{
						Updater = GameObject.Find(UpdaterName);
					}
				}
				SceneData = Updater.GetComponent<InTerra_UpdateAndCheck>();
			}

			//Note: The object is hidden to prevent accidentall deleting. If you want the object to be visible in hierarchy you can uncomment the folowing line.
			//Updater.hideFlags = HideFlags.None;

			return (SceneData);
		}

		public static InTerra_GlobalData GetGlobalData()
		{
			if (GetSceneData().GlobalData == null)
			{
				InTerra_GlobalData globalData = Resources.Load<InTerra_GlobalData>("InTerra_GlobalData");
				#if UNITY_EDITOR
				if (globalData == null)
				{
					if (!Directory.Exists(Path.Combine(GetInTerraPath(), "Resources")))
					{
						AssetDatabase.CreateFolder(GetInTerraPath(), "Resources");
					}

					globalData = ScriptableObject.CreateInstance<InTerra_GlobalData>();
					AssetDatabase.CreateAsset(globalData, GetInTerraPath() + "/Resources/InTerra_GlobalData.asset");
					AssetDatabase.SaveAssets();
				}
				#endif				 

				GetSceneData().GlobalData = globalData;
			}
			return GetSceneData().GlobalData;
		}

		public static void TracksUpdate()
		{
			if (Updater != null)
			{
				if (TrackCamera == null)
				{
					if (!Updater.TryGetComponent<Camera>(out Camera c))
                    {
						TrackCamera = Updater.AddComponent<Camera>();
						InTerra_TracksCameraSettings.SetTrackCamera(Updater.GetComponent<Camera>());
					}
					else
                    {
						TrackCamera = Updater.GetComponent<Camera>();
					}					
				}
				else
				{
					TracksUpdateTimeCount += Time.deltaTime;

					if (!TracksCameraUpdate)
					{
						EnableTracksCamera(false);
					}
					else
                    {
						TracksCameraUpdate = false;
						TracksUpdateTimeCount = 0;
					}
					
					if ((TracksUpdateTimeCount >= GetGlobalData().trackUpdateTime) || !initTrack)
					{
						#if USING_HDRP || USING_URP
							TrackCamera.enabled = true;
						#endif

						initTrack = true;
						TracksCameraUpdate = true;
						EnableTracksCamera(true);

						if (GetSceneData().TrackTexture == null) CreateTrackRenderTexture();

						#if UNITY_EDITOR
							if (GetSceneData().TrackTexture.height != GetGlobalData().trackTextureSize) CreateTrackRenderTexture();
							if (TrackCamera.cullingMask != 1 << GetGlobalData().trackLayer) TrackCamera.cullingMask = 1 << GetGlobalData().trackLayer;

							var view = SceneView.lastActiveSceneView;
							Camera activeCamera = null; 

					 		if (view != null && EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.ToString() == " (UnityEditor.SceneView)" && UnityEditorInternal.InternalEditorUtility.isApplicationActive)
							{

								activeCamera = view.camera;
							}
							else if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.ToString() == " (UnityEditor.GameView)" )
							{
                                Camera[] cameras = Camera.allCameras;
                                activeCamera = cameras.FirstOrDefault(c => c.enabled && c.CompareTag(TrackCameraTag)) 
                                            ?? cameras.FirstOrDefault(c => c.enabled);
							}

							if (activeCamera != null)
                            {
                                TrackCameraForwardVec = activeCamera.transform.forward;
                                TrackCameraPositon = activeCamera.transform.position;
                            }
                            else
                            {
                                TrackCameraForwardVec = TrackCameraForwardVec != Vector3.zero ? TrackCameraForwardVec : Vector3.forward;
                                TrackCameraPositon = TrackCameraPositon != Vector3.zero ? TrackCameraPositon : Vector3.zero;
                            }
						#else
							Camera activeCamera = Camera.allCameras.FirstOrDefault(c => c.enabled && c.CompareTag(TrackCameraTag)) 
                                               ?? Camera.allCameras.FirstOrDefault(c => c.enabled);
                            if (activeCamera != null)
                            {
                                TrackCameraForwardVec = activeCamera.transform.forward;
                                TrackCameraPositon = activeCamera.transform.position;
                            }
                            else
                            {
                                // Fallback: Use last known values or default
                                TrackCameraForwardVec = TrackCameraForwardVec != Vector3.zero ? TrackCameraForwardVec : Vector3.forward;
                                TrackCameraPositon = TrackCameraPositon != Vector3.zero ? TrackCameraPositon : Vector3.zero;
                            }
						#endif

						float trackArea = GetGlobalData().trackArea;
						
						Vector3 tcPos = TrackCameraPositon + TrackCameraForwardVec * Mathf.Round((trackArea * 0.5f));
						float roundIndex = trackArea / (GetGlobalData().trackTextureSize * 0.2f);
						tcPos.x = Mathf.Round(tcPos.x / roundIndex) * roundIndex;
						tcPos.y = Mathf.Round(tcPos.y / roundIndex) * roundIndex;
						tcPos.z = Mathf.Round(tcPos.z / roundIndex) * roundIndex;

						Updater.transform.position = new Vector3(tcPos.x, TracksStampYPosition() - 100.0f, tcPos.z);

						Shader.SetGlobalVector("_InTerra_TrackPosition", tcPos);

						Shader.SetGlobalFloat("_InTerra_TrackArea", trackArea);
						Shader.SetGlobalTexture("_InTerra_TrackTexture", GetSceneData().TrackTexture);

						TrackCamera.targetTexture = GetSceneData().TrackTexture;
						TrackCamera.orthographicSize = trackArea * 0.5f;
					}
				}
			}
		}

		public static float TracksStampYPosition()
		{
			float positionY = -100.0f;

			if (Terrain.activeTerrain != null)
			{
				positionY = Terrain.activeTerrain.GetPosition().y - 10.0f;
			}
			else if (GetSceneData().MeshTerrainsList.Count > 0)
			{
				positionY = GetSceneData().MeshTerrainsList[0].bounds.min.y - 10.0f;
			}

			return positionY - GetSceneData().TrackDepthIndex;
		}

		private static void EnableTracksCamera(bool enable)
        {
			#if USING_HDRP
			if (TrackCamera.TryGetComponent<HDAdditionalCameraData>(out var cam))
			{
				cam.fullscreenPassthrough = !enable;
			}
			#elif USING_URP
			if (TrackCamera.TryGetComponent<UniversalAdditionalCameraData>(out var cam))
			{
				cam.renderType = enable ? CameraRenderType.Base : CameraRenderType.Overlay;
			}
			#else
				TrackCamera.enabled = enable;
			#endif
        }

		static public void CreateTrackRenderTexture()
		{
			if(GetSceneData().TrackTexture != null)
            {
				GetSceneData().TrackTexture.Release();
			}

			int tracksTexSize = GetGlobalData().trackTextureSize;
			GetSceneData().TrackTexture = new RenderTexture(tracksTexSize, tracksTexSize, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear) { name = "TrackTexture", enableRandomWrite = true};

			GetSceneData().TrackTexture.Create();
		}

		public static bool TracksFadingEnabled()
		{
			return GetSceneData().TracksFading;
		}

		public static void SetTracksFading(bool enable)
		{
			GetSceneData().TracksFading = enable;
		}

		public static void SetTracksFadingTime(float time)
		{
			GetSceneData().TracksFadingTime = time;
		}

		public static bool EightLayersEnabled(Material mat)
		{
			#if !(USING_URP || USING_HDRP)
				return GetGlobalData().eightLayersPass;
			#else
				return mat.IsKeywordEnabled("_LAYERS_EIGHT");
			#endif
		}


	#if UNITY_EDITOR
		static public void WriteDefinedKeywords()
		{
			if (File.Exists(GetKeywordsPath()))
			{ 
				string[] lines = File.ReadAllLines(GetKeywordsPath());
				InTerra_GlobalData data = GetGlobalData();

				lines[DEFINE_MASKMAP_LineNumber] = DefineKeywordLine(data.maskMapMode == 1, "_TERRAIN_MASK_MAPS");
				lines[DEFINE_NORMAL_MASK_LineNumber] = DefineKeywordLine(data.maskMapMode == 2, "_TERRAIN_NORMAL_IN_MASK");
				lines[DEFINE_HEIGTH_ONLY_LineNumber] = DefineKeywordLine(data.maskMapMode == 3, "_TERRAIN_MASK_HEIGHTMAP_ONLY"); 

				lines[RESTRICT_NORMALMAP_LineNumber] = DefineKeywordLine(!data.disableNormalmap, "_NORMALMAPS");
				lines[RESTRICT_HEIGHTBLEND_LineNumber] = DefineKeywordLine(!data.disableHeightmapBlending, "_TERRAIN_BLEND_HEIGHT");
				lines[RESTRICT_TERR_PARALAX_LineNumber] = DefineKeywordLine(!data.disableTerrainParallax, "_TERRAIN_PARALLAX");
				lines[RESTRICT_TRACKS_LineNumber] = DefineKeywordLine(!data.disableTracks, "_TRACKS");
				lines[RESTRICT_OBJ_PARALLAX_LineNumber] = DefineKeywordLine(!data.disableObjectParallax, "_OBJECT_PARALLAX");
				lines[RESTRICT_PUDDLES_LineNumber] = DefineKeywordLine(!data.disablePuddles, "_PUDDLES");
				#if !(USING_URP || USING_HDRP)
					lines[EIGHT_LAYERS_PASS_LineNumber] = DefineKeywordLine(data.eightLayersPass, "_LAYERS_EIGHT");
				#endif
				File.WriteAllLines(GetKeywordsPath(), lines);

				EditorApplication.delayCall += () =>
				{
					File.WriteAllLines(GetKeywordsPath(), lines);
					EditorApplication.delayCall += () =>
					{
						AssetImporter ai = AssetImporter.GetAtPath(GetKeywordsPath());
						ai.SaveAndReimport();
					};
				};
			}
		}

		static public bool CheckDefinedKeywords()
		{
			InTerra_GlobalData data = GetGlobalData();

			return
				#if !(USING_URP || USING_HDRP)
					DefinedKeyword(EIGHT_LAYERS_PASS_LineNumber) == (data.eightLayersPass) &&
				#endif
				DefinedKeyword(DEFINE_MASKMAP_LineNumber) == (data.maskMapMode == 1) &&
				DefinedKeyword(DEFINE_NORMAL_MASK_LineNumber) == (data.maskMapMode == 2) &&
				DefinedKeyword(DEFINE_HEIGTH_ONLY_LineNumber) == (data.maskMapMode == 3) &&
				DefinedKeyword(RESTRICT_NORMALMAP_LineNumber) != data.disableNormalmap &&
				DefinedKeyword(RESTRICT_HEIGHTBLEND_LineNumber) != data.disableHeightmapBlending &&
				DefinedKeyword(RESTRICT_TERR_PARALAX_LineNumber) != data.disableTerrainParallax &&
				DefinedKeyword(RESTRICT_TRACKS_LineNumber) != data.disableTracks &&
				DefinedKeyword(RESTRICT_OBJ_PARALLAX_LineNumber) != data.disableObjectParallax &&
				DefinedKeyword(RESTRICT_PUDDLES_LineNumber) != data.disablePuddles;
		}

		#if USING_URP
			static public void URPShadersVersionAdjust()
			{
				if (File.Exists(GetKeywordsPath()))
				{
					
					string[] globalLines = File.ReadAllLines(GetKeywordsPath());
					#if   UNITY_6000_2_OR_NEWER
						globalLines[UNITY_VERSION_LineNumber] = "#define UNITY_6000_2";
					#elif UNITY_6000_1_OR_NEWER
						globalLines[UNITY_VERSION_LineNumber] = "#define UNITY_6000_1";
					#elif UNITY_6000_0_OR_NEWER
						globalLines[UNITY_VERSION_LineNumber] = "#define UNITY_6000_0";
					#elif UNITY_2022_2_OR_NEWER
						globalLines[UNITY_VERSION_LineNumber] = "#define UNITY_2022_2_TO_2022_3";
					#else 
						globalLines[UNITY_VERSION_LineNumber] = "#define UNITY_2021_2_TO_2022_1";
					#endif

					if(File.ReadAllLines(GetKeywordsPath())[UNITY_VERSION_LineNumber] != globalLines[UNITY_VERSION_LineNumber])
					{
						EditorApplication.delayCall += () =>
						{
							File.WriteAllLines(GetKeywordsPath(), globalLines);
							EditorApplication.delayCall += () =>
							{
								AssetImporter ai = AssetImporter.GetAtPath(GetKeywordsPath());
								ai.SaveAndReimport();
							};
						};
					}
				}
			}
		#endif

		public static string GetInTerraPath()
		{			
			if(string.IsNullOrEmpty(InTerraPath) || !Directory.Exists(InTerraPath))
            {
				string[] guids = AssetDatabase.FindAssets($"t:script InTerra_Data");
				if (guids == null || guids.Length == 0) return null;
				string relativePath = AssetDatabase.GUIDToAssetPath(guids[0]);
				InTerraPath = Path.GetDirectoryName(Path.GetDirectoryName(relativePath));
			}
			return InTerraPath;
		}

		public static string GetKeywordsPath()
		{
			#if USING_HDRP
				string hdrpGlobal = Path.Combine(GetInTerraPath(), "HDRP", "Shaders", "InTerra_HDRP_DefinedGlobalKeywords.hlsl");
				if (File.Exists(hdrpGlobal))
				{ return hdrpGlobal; }
				else
				{	//There was a typo in folder name in some versions, but updating package will not update the old name of folder, therfore there is a folowing alternative path.
					return Path.Combine(GetInTerraPath(), "HRDP", "Shaders", "InTerra_HDRP_DefinedGlobalKeywords.hlsl"); 				
				}							
			#elif USING_URP
				return Path.Combine(GetInTerraPath(), "URP", "Shaders", "InTerra_URP_DefinedGlobalKeywords.hlsl");
			#else
				return Path.Combine(GetInTerraPath(), "Built-in", "Shaders", "InTerra_DefinedGlobalKeywords.cginc");
			#endif
		}


		static string DefineKeywordLine(bool set, string keyword)
		{			
			return (set ? "#define " : "#undef ") + keyword;
		}

		public static bool DefinedKeyword(int line)
		{
			if (!File.Exists(GetKeywordsPath())) return false;
			string[] definedKeywordsLines = File.ReadAllLines(GetKeywordsPath());
			bool defined;

			if (definedKeywordsLines[line].Contains("define"))
			{
				defined = true;
			}
			else
			{
				defined = false;
			}
			return defined;
		}
		#endif

		#if (USING_URP || USING_HDRP)
		public static bool DiffuseTextureArrayCheckFormat(TerrainLayer[] tl)
		{
			bool okFormat = true;

			if (tl[0] && tl[0].diffuseTexture)
			{
				for (int i = 1; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].diffuseTexture != null)
					{
						okFormat = tl[0].diffuseTexture.format == tl[i].diffuseTexture.format ? okFormat : false;
					}
				}
			}
			else
			{
				okFormat = false;
			}

			return okFormat;
		}

		public static bool DiffuseTextureArrayCheckSize(TerrainLayer[] tl)
		{
			bool okSize = true;

			if (tl[0] && tl[0].diffuseTexture)
			{
				for (int i = 1; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].diffuseTexture != null)
					{
						okSize = tl[0].diffuseTexture.width == tl[i].diffuseTexture.width ? okSize : false;
						okSize = tl[0].diffuseTexture.height == tl[i].diffuseTexture.height ? okSize : false;
					}
				}
			}
			else
			{
				okSize = false;
			}

			return okSize;
		}

		public static bool NormalMapTextureArrayCheckFormat(TerrainLayer[] tl)
		{
			bool okFormat = true;

			if (tl[0] && tl[0].normalMapTexture)
			{
				for (int i = 1; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].normalMapTexture != null)
					{
						okFormat = tl[0].normalMapTexture.format == tl[i].normalMapTexture.format ? okFormat : false;
					}
				}
			}
			else
			{
				okFormat = false;
			}

			return okFormat;
		}

		public static bool NormalMapTextureArrayCheckSize(TerrainLayer[] tl)
		{
			bool okSize = true;

			if (tl[0] && tl[0].normalMapTexture)
			{
				for (int i = 1; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].normalMapTexture != null)
					{
						okSize = tl[0].normalMapTexture.width == tl[i].normalMapTexture.width ? okSize : false;
						okSize = tl[0].normalMapTexture.height == tl[i].normalMapTexture.height ? okSize : false;
					}
				}
			}
			else
            {
				okSize = false;
			}

			return okSize;
		}

		public static Texture2DArray DiffuseTextureArrays16(TerrainLayer[] tl)
		{
			Texture2DArray splatArray = null;

			if (tl[0] && DiffuseTextureArrayCheckSize(tl) && DiffuseTextureArrayCheckFormat(tl))
			{
				int dWidth = tl[0].diffuseTexture.width;
				int dHeight = tl[0].diffuseTexture.height;
				splatArray = new Texture2DArray(dWidth, dHeight, 16, tl[0].diffuseTexture.format, true);

				for (int i = 0; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].diffuseTexture != null)
					{
						for (int mip = 0; mip < tl[i].diffuseTexture.mipmapCount; ++mip)
						{
							Graphics.CopyTexture(tl[i].diffuseTexture, 0, mip, splatArray, i, mip);
						}
					}
				}
			}
			else
			{
				splatArray = new Texture2DArray(1, 1, 16, TextureFormat.RGBA32, true);
			}
			return splatArray;
		}

		public static Texture2DArray NormalTextureArrays16(TerrainLayer[] tl)
		{
			Texture2DArray normalArray = null;

			if (tl[0] && NormalMapTextureArrayCheckSize(tl) && NormalMapTextureArrayCheckFormat(tl))
			{
				int nWidth = tl[0].normalMapTexture.width;
				int nHeight = tl[0].normalMapTexture.height;
				normalArray = new Texture2DArray(nWidth, nHeight, 16, tl[0].normalMapTexture.format, true, true);

				for (int i = 0; i < tl.Length && i < 16; i++)
				{
					if (tl[i] && tl[i].normalMapTexture != null)
					{
						for (int mip = 0; mip < tl[i].normalMapTexture.mipmapCount; ++mip)
						{
							Graphics.CopyTexture(tl[i].normalMapTexture, 0, mip, normalArray, i, mip);
						}
					}
				}
			}
			else
			{
				normalArray = new Texture2DArray(1, 1, 16, TextureFormat.RGBA32, true, true);
			}

			return normalArray;
		}
		#endif

		public static void TerrainMaterialUpdate()
		{
			Terrain[] terrains = Terrain.activeTerrains;
			bool tracksEnabled = false;
			#if (USING_URP || USING_HDRP)
				bool textureArrayCheck = false;
			#endif
 
			foreach (Terrain terrain in terrains)
			{
				if (terrain && terrain.terrainData && terrain.materialTemplate && CheckTerrainShader(terrain.materialTemplate))
				{
					Material terMat = terrain.materialTemplate;

					if(terMat.GetFloat("_Tracks") == 1)
                    {
						tracksEnabled = true;
						#if UNITY_EDITOR
							terMat.SetFloat("_Gamma", (PlayerSettings.colorSpace == ColorSpace.Gamma ? 1.0f : 0.0f));
						#endif
                    }

					#if (USING_URP || USING_HDRP)
						if (terMat.IsKeywordEnabled("_LAYERS_SIXTEEN"))
						{
							textureArrayCheck = true;
						}
						terMat.renderQueue = 2225;
					#endif

					Texture normalTintTexture = terMat.GetTexture("_TerrainNormalTintTexture");
					if (normalTintTexture == null) terMat.SetFloat("_TerrainNormalTintStrenght", 0.0f);

					Texture ColorTintTexture = terMat.GetTexture("_TerrainColorTintTexture");
					if (ColorTintTexture == null) terMat.SetFloat("_TerrainColorTintStrenght", 0.0f);

					foreach(var tl in terrain.terrainData.terrainLayers)
                    {
						RemapMinCorection(tl);
					}

					if (terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"))
					{
						InTerra_TerrainData terrainData = null;

						if (!terrain.gameObject.TryGetComponent<InTerra_TerrainData>(out terrainData))
						{
							terrainData = terrain.gameObject.AddComponent<InTerra_TerrainData>();
						}
						#if (USING_URP || USING_HDRP)
							terrainData.splatTextureArray16 = DiffuseTextureArrays16(terrain.terrainData.terrainLayers);
							terrainData.normalTextureArray16 = NormalTextureArrays16(terrain.terrainData.terrainLayers);
						#endif
					}

					TerrainMaterialPropertyBlockUpdate(terrain, MaterialPropertyBlockNeedLayersUpdate(terrain));
				}
			}


			if (!tracksEnabled && Updater.TryGetComponent<Camera>(out Camera cam) && cam.enabled)
			{
				cam.enabled = false;
			}

			foreach (var meshTerrain in GetSceneData().MeshTerrainsList)
			{
				if (meshTerrain != null && CheckMeshTerrainShader(meshTerrain.sharedMaterial) && meshTerrain.bounds != null && meshTerrain.transform)
				{
					meshTerrain.sharedMaterial.SetVector("_TerrainSizeXZPosY", new Vector3(meshTerrain.bounds.size.x, meshTerrain.bounds.size.z, meshTerrain.transform.position.y));

					if(meshTerrain.sharedMaterial.GetFloat("_Tracks") == 1)
					{
						tracksEnabled = true;
					}
					#if (USING_URP || USING_HDRP)
						if (meshTerrain.sharedMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"))
						{
							textureArrayCheck = true;
						}
					#endif

					GameObject meshTerGO = meshTerrain.gameObject;
					InTerra_MeshTerrainData meshTerrainData = null;

					if (!meshTerGO.TryGetComponent<InTerra_MeshTerrainData>(out meshTerrainData))
					{
						meshTerrainData = meshTerGO.AddComponent<InTerra_MeshTerrainData>();
					}
					
					if (meshTerrainData != null)
					{
						meshTerrain.sharedMaterial.SetFloat("_NumLayersCount", meshTerrainData.TerrainLayers.Count());
						TerrainLayer[] mtLayers = meshTerrainData.TerrainLayers;
						SetMeshTerrainPositionAndSize(meshTerrain.sharedMaterial, meshTerrain);

						int layers = 8;
						#if (USING_URP || USING_HDRP)
						if (meshTerrain.sharedMaterial.IsKeywordEnabled("_LAYERS_SIXTEEN"))
						{
							meshTerrainData.splatTextureArray16 = DiffuseTextureArrays16(meshTerrainData.TerrainLayers);
							meshTerrainData.normalTextureArray16 = NormalTextureArrays16(meshTerrainData.TerrainLayers);

							meshTerrain.sharedMaterial.SetTexture("_SplatArray16", meshTerrainData.splatTextureArray16);
							meshTerrain.sharedMaterial.SetTexture("_NormalArray16", meshTerrainData.normalTextureArray16);

							layers = 16;
						}
						#endif
								
						for (int i = 0; i < mtLayers.Length && i < layers; i++)
						{
							TerrainLayer tl = meshTerrainData.TerrainLayers[i];
							if(tl)
                            {
								RemapMinCorection(tl);
								TerrainLaeyrDataToMaterial(tl, i.ToString(), meshTerrain.sharedMaterial);
							}
						}
					}
				}
			}
		

			if(GetSceneData().TracksEnabled != tracksEnabled)
            {
				GetSceneData().TracksEnabled = tracksEnabled;
				#if UNITY_EDITOR
					EditorUtility.SetDirty(GetSceneData());
				#endif
			}

			#if (USING_URP || USING_HDRP)
			if(GetSceneData().TextureArrayCheck != textureArrayCheck)
            {
				GetSceneData().TextureArrayCheck = textureArrayCheck;
				#if UNITY_EDITOR
					EditorUtility.SetDirty(GetSceneData());
				#endif
			}
			#endif

			if (GetSceneData().SetSceneWetnessValues)
			{
				Shader.SetGlobalFloat("_InTerra_GlobalWetness", GetSceneData().GlobalWetness);
				Shader.SetGlobalVector("_InTerra_GlobalPuddles", GetSceneData().GlobalPuddles);
				Shader.SetGlobalVector("_InTerra_GlobalRaindropRipples", GetSceneData().GlobalRaindropRipples);
				Shader.SetGlobalVector("_InTerra_GlobalRaindropsDistance", GetSceneData().GlobalRaindropDistance);
			}

			static void RemapMinCorection(TerrainLayer tl)
            {
				//Note: The diffuseRemapMin values are used differently than originall purpouse, InTerra started just as Buit-in asset where the values are not used by Unity shader at all, so it wasn't matter what number is there, but in URP and HDRP the values are affecting the tint and therefore was scaled to prevent unwanted tint when swithcing to default shader.
				if (tl)
				{
					float heightOffset = Mathf.Abs(tl.diffuseRemapMin.z) >= 0.005 ?
										tl.diffuseRemapMin.z / 100.0f : tl.diffuseRemapMin.z;

					float distBlendScale = Mathf.Abs(tl.diffuseRemapMin.x) >= 0.0011 ?
										tl.diffuseRemapMin.x / 1000.0f : tl.diffuseRemapMin.x;


					tl.diffuseRemapMin = new Vector4(distBlendScale,
													tl.diffuseRemapMin.y,
													heightOffset,
													tl.diffuseRemapMin.w);
				}
			}
		}

		static public bool MaterialPropertyBlockNeedLayersUpdate(Terrain terrain)
		{
			return (EightLayersEnabled(terrain.materialTemplate)					
					&& terrain.terrainData.terrainLayers.Length > 4) 
					|| terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN");
		}


		public static void TerrainMaterialPropertyBlockUpdate(Terrain terrain, bool updateLayers)
		{
			mtb.Clear();
			terrain.SetSplatMaterialPropertyBlock(mtb);

			mtb.SetVector("_TerrainSizeXZPosY", new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.z, terrain.transform.position.y));
			mtb.SetFloat("_NumLayersCount", terrain.terrainData.alphamapLayers);
			
			#if !USING_HDRP
			if (updateLayers)
            {
				for (int i = 0; i < terrain.terrainData.alphamapLayers && i < 16; i++)
				{
					TerrainLaeyrDataToTerrain(terrain.terrainData.size, terrain.terrainData.terrainLayers[i], i.ToString(), mtb);
				}

				mtb.SetTexture("_Control", terrain.terrainData.alphamapTextureCount > 0 ? terrain.terrainData.alphamapTextures[0] : Texture2D.redTexture);
				mtb.SetTexture("_Control1", terrain.terrainData.alphamapTextureCount > 1 ? terrain.terrainData.alphamapTextures[1] : Texture2D.blackTexture);
				if(terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"))
                {
					mtb.SetTexture("_Control2", terrain.terrainData.alphamapTextureCount > 2 ? terrain.terrainData.alphamapTextures[2] : Texture2D.redTexture);
					mtb.SetTexture("_Control3", terrain.terrainData.alphamapTextureCount > 3 ? terrain.terrainData.alphamapTextures[3] : Texture2D.blackTexture);

					InTerra_TerrainData terrainData = null;

					if (!terrain.gameObject.TryGetComponent<InTerra_TerrainData>(out terrainData))
					{
						terrainData = terrain.gameObject.AddComponent<InTerra_TerrainData>();
					}

					if (terrainData.splatTextureArray16) mtb.SetTexture("_SplatArray16", terrainData.splatTextureArray16);
					if (terrainData.normalTextureArray16) mtb.SetTexture("_NormalArray16", terrainData.normalTextureArray16); 
				}
			}
			#else
			if(terrain.materialTemplate.IsKeywordEnabled("_LAYERS_SIXTEEN"))
            {
				InTerra_TerrainData terrainData = null;

				if (!terrain.gameObject.TryGetComponent<InTerra_TerrainData>(out terrainData))
				{
					terrainData = terrain.gameObject.AddComponent<InTerra_TerrainData>();
				}

				mtb.SetTexture("_SplatArray16", terrainData.splatTextureArray16);
				mtb.SetTexture("_NormalArray16", terrainData.normalTextureArray16);
			}
			#endif

			terrain.SetSplatMaterialPropertyBlock(mtb);
			terrain.terrainData.SetBaseMapDirty();
		}

		public static void SetMeshTerrainPositionAndSize(Material mat, MeshRenderer meshTerrain)
		{
			List<MeshRenderer> meshTerrainsList = GetSceneData().MeshTerrainsList;
			meshTerrain.TryGetComponent<InTerra_MeshTerrainData>(out var mtd);

			if (meshTerrainsList.Count > 1)
			{
				Vector3 positonMin = meshTerrain.bounds.min;
				Vector3 positonMax = meshTerrain.bounds.max;

				if (mtd && mat.GetFloat("_HeightmapBase") == 0)
                {
					positonMin.y = meshTerrain.transform.position.y;
				}

				foreach (var mt in meshTerrainsList)
				{
					if(mt && mt.sharedMaterial == mat)
					{ 
						positonMin.x = mt.bounds.min.x < positonMin.x ? mt.bounds.min.x : positonMin.x;						
						positonMin.z = mt.bounds.min.z < positonMin.z ? mt.bounds.min.z : positonMin.z;

						switch (mat.GetFloat("_HeightmapBase"))
						{
							case 0:
								positonMin.y = mt.transform.position.y < positonMin.y ? mt.transform.position.y : positonMin.y;
								break;
							case 1:
								positonMin.y = mt.bounds.min.y < positonMin.y ? mt.bounds.min.y : positonMin.y;
								break;
							case 2:
								positonMin.y = mat.GetFloat("_HeightmapBaseCustom");
								break;
						}
						 
						positonMax.x = mt.bounds.max.x > positonMax.x ? mt.bounds.max.x : positonMax.x;
						positonMax.y = mt.bounds.max.y > positonMax.y ? mt.bounds.max.y : positonMax.y;
						positonMax.z = mt.bounds.max.z > positonMax.z ? mt.bounds.max.z : positonMax.z;					
					}
				}

				mat.SetVector("_TerrainSize", new Vector3(positonMax.x - positonMin.x, positonMax.y - positonMin.y, positonMax.z - positonMin.z));
				mat.SetVector("_TerrainPosition", positonMin);
			}
			else
			{
				mat.SetVector("_TerrainSize", meshTerrain.bounds.size);

				Vector3 positonMin = meshTerrain.bounds.min;

				switch (mat.GetFloat("_HeightmapBase"))
				{
					case 0:
						positonMin.y = meshTerrain.transform.position.y;
						break;
					case 1:
						positonMin.y = meshTerrain.bounds.min.y;
						break;
					case 2:
						positonMin.y = mat.GetFloat("_HeightmapBaseCustom");
						break;
				}
				
				mat.SetVector("_TerrainPosition", positonMin);
			}
		}

		static void SetKeyword(Material mat, string name, bool set)
		{
			if (set) mat.EnableKeyword(name); else mat.DisableKeyword(name);
		}

		#if UNITY_EDITOR
			#if UNITY_2020_1_OR_NEWER
				public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
				{
					var main = EditorGUIUtility.GetMainWindowPosition();
					var pos = aWin.position;
					float w = (main.width - pos.width) * 0.5f;
					float h = (main.height - pos.height) * 0.5f;
					pos.x = main.x + w;
					pos.y = main.y + h;
					aWin.position = pos;
				}
			#else
				public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
				{
					var result = new List<System.Type>();
					var assemblies = aAppDomain.GetAssemblies();
					foreach (var assembly in assemblies)
					{
						var types = assembly.GetTypes();
						foreach (var type in types)
						{
							if (type.IsSubclassOf(aType))
								result.Add(type);
						}
					}
					return result.ToArray();
				}

				public static Rect GetEditorMainWindowPos()
				{
					var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
					if (containerWinType == null)
						throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
					var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					if (showModeField == null || positionProperty == null)
						throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
					var windows = Resources.FindObjectsOfTypeAll(containerWinType);
					foreach (var win in windows)
					{
						var showmode = (int)showModeField.GetValue(win);
						if (showmode == 4) // main window
						{
							var pos = (Rect)positionProperty.GetValue(win, null);
							return pos;
						}
					}
					throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
				}

				public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
				{
					var main = GetEditorMainWindowPos();
					var pos = aWin.position;
					float w = (main.width - pos.width) * 0.5f;
					float h = (main.height - pos.height) * 0.5f;
					pos.x = main.x + w;
					pos.y = main.y + h;
					aWin.position = pos;
				}
			#endif
		#endif
	}

	//The Serialized Dictionary is based on christophfranke123 code from this page https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html
	[System.Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> keys = new List<TKey>();

		[SerializeField]
		private List<TValue> values = new List<TValue>();

		// save the dictionary to lists
		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();
			foreach (KeyValuePair<TKey, TValue> pair in this)
			{
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		// load dictionary from lists
		public void OnAfterDeserialize()
		{
			if (this != null)
            {
				this.Clear();
				if (keys.Count != values.Count)
					throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

				for (int i = 0; i < keys.Count; i++)
					this.Add(keys[i], values[i]);
			}	
		}
	}
}
