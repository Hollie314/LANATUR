using UnityEditor;
using UnityEngine;
using System.IO;

namespace InTerra
{
	public class InTerra_UpdateAndCheckInEditor : UnityEditor.AssetModificationProcessor
	{
		static bool GlobalKeywordsCheck;

		[InitializeOnLoadMethod]
		static void InTerra_InitializeTerrainDataLoading()
		{
			if (!InTerra_Setting.DisableAllAutoUpdates) EditorApplication.update += EditorUpdate;
			Undo.undoRedoPerformed += UndoChecks;
			UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnEditorSceneManagerSceneOpened;
			EditorApplication.delayCall += () =>
			{
				CheckTerrain();
			};
		}

		static void EditorUpdate()
		{
			if (!EditorApplication.isPlaying || EditorApplication.isPaused)
			{
				if (InTerra_Data.SceneData && InTerra_Data.SceneData.TracksEnabled) InTerra_Data.TracksUpdate();
			}
			if (!GlobalKeywordsCheck)
			{
				if (!InTerra_Data.CheckDefinedKeywords()) InTerra_Data.WriteDefinedKeywords();						
				GlobalKeywordsCheck = true;
				#if USING_URP
					InTerra_Data.URPShadersVersionAdjust();
				#endif
			}
		}

		static void UndoChecks()
		{
			if (!InTerra_Data.CheckDefinedKeywords()) InTerra_Data.WriteDefinedKeywords();
			InTerra_Data.TerrainMaterialUpdate();
			InTerra_TerrainShaderGUI.restrictInit = false;			
		}

		static void OnEditorSceneManagerSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
		{
			CheckTerrain();
		}

		static void CheckTerrain()
		{
			bool usingInTerra = false;
			foreach (var terrain in Terrain.activeTerrains)
			{
				if (InTerra_Data.CheckTerrainShader(terrain.materialTemplate))
				{
					GameObject terGO = terrain.gameObject;
					if (!terGO.TryGetComponent<InTerra_TerrainData>(out var td))
					{
						terGO.AddComponent<InTerra_TerrainData>();
					}
					usingInTerra = true;
				}
			}
			
			#if UNITY_2023_1_OR_NEWER
				InTerra_MeshTerrainData[] meshTerrains = Object.FindObjectsByType<InTerra_MeshTerrainData>(FindObjectsSortMode.None);
			#else
				InTerra_MeshTerrainData[] meshTerrains = Object.FindObjectsOfType<InTerra_MeshTerrainData>();
			#endif
			
			usingInTerra = meshTerrains.Length > 0 ? true : usingInTerra;

			if (!InTerra_Setting.DisableAllAutoUpdates && usingInTerra)
			{
				if (!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.UpdateTerrainData(false);
			}
						

			#if USING_HDRP
				Material terrainDemoMat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(InTerra_Data.GetInTerraPath(), "HDRP", "Demo Scene", "Materials", "InTerra_Terrain_HDRP_Demo.mat"));
			

				if (terrainDemoMat && !terrainDemoMat.shader.isSupported)
				{
					#if UNITY_2022_2_OR_NEWER	
						terrainDemoMat.shader = Shader.Find("InTerra/HDRP Tessellation/Terrain (Lit with Features) 2022.2");               
					#endif

					#if UNITY_2023_1_OR_NEWER
						terrainDemoMat.shader = Shader.Find("InTerra/HDRP Tessellation/Terrain (Lit with Features) 2023.1 or Heigher"); 
					#endif
				}				
			#endif
		}
	}
}