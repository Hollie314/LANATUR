using UnityEngine;
using UnityEditor;

namespace InTerra
{
	public class InTerra_GUI
	{
		static bool disableUpdates = InTerra_Setting.DisableAllAutoUpdates;
		static bool updateDict = InTerra_Setting.DictionaryUpdate;

		//----------------------------- FONT STYLES ----------------------------------
		static GUIStyle styleBold = new GUIStyle(EditorStyles.boldLabel);
		static GUIStyle styleMini = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
		static GUIStyle styBoldCenter = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
		//----------------------------------------------------------------------------


		//=========================================================================
		//-----------------|			MIPMAPS FADING         |-------------------
		//=========================================================================
		public static void MipMapsFading(Material targetMat, string label, MaterialEditor editor, ref bool minMax)
		{
			float mipMapLevel = targetMat.GetFloat("_MipMapLevel");

			EditorGUI.BeginChangeCheck();
			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(label, GUILayout.MinWidth(75));
				EditorGUILayout.LabelField(new GUIContent() { text = "Bias:", tooltip = "Minimal Mip map level where the fading will starts." }, new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight }, GUILayout.MaxWidth(62));
				mipMapLevel = EditorGUILayout.IntField((int)mipMapLevel, GUILayout.MaxWidth(25));
			}

			Vector4 mipMapFade = MinMaxValues(targetMat.GetVector("_MipMapFade"), true, false, ref minMax);

			if (EditorGUI.EndChangeCheck())
			{
				editor.RegisterPropertyChangeUndo("InTerra Mip Maps Fading");
				targetMat.SetVector("_MipMapFade", mipMapFade);
				targetMat.SetFloat("_MipMapLevel", mipMapLevel);
			}
		}


		//=========================================================================
		//-----------------|			MIN-MAX VALUES         |-------------------
		//=========================================================================
		public static Vector4 MinMaxValues(Vector4 values, bool distanceRange, bool label, ref bool minMax)
		{
			GUILayout.BeginHorizontal();
			if(label)
            {
				EditorGUILayout.LabelField(LabelAndTooltip("Distance:", "The distance where the covering will start and end."), GUILayout.Width(70));
			}
			EditorGUILayout.LabelField(values.x.ToString("0.0"), GUILayout.Width(33));
			EditorGUILayout.MinMaxSlider(ref values.x, ref values.y, values.z, values.w);
			EditorGUILayout.LabelField(values.y.ToString("0.0"), GUILayout.Width(33));
			GUILayout.EndHorizontal();

			if (distanceRange)
			{
				EditorGUI.indentLevel = 1;
				minMax = EditorGUILayout.Foldout(minMax, "Adjust Distance Range", true);
			}
			else
			{
				EditorGUI.indentLevel = 2;
				minMax = EditorGUILayout.Foldout(minMax, "Adjust Range", true);
			}

			EditorGUI.indentLevel = 0;
			if (minMax)
			{
				GUILayout.BeginHorizontal();

				GUIStyle rightAlignment = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
				EditorGUILayout.LabelField("Min:", rightAlignment, GUILayout.Width(45));
				values.z = EditorGUILayout.DelayedFloatField(values.z, GUILayout.MinWidth(50));

				EditorGUILayout.LabelField("Max:", rightAlignment, GUILayout.Width(45));
				values.w = EditorGUILayout.DelayedFloatField(values.w, GUILayout.MinWidth(50));

				GUILayout.EndHorizontal();
			}

			values = MinMaxValuesClamp(values);
			return values;
		}

		public static Vector4 MinMaxValuesClamp(Vector4 values)
		{
			values.x = Mathf.Clamp(values.x, values.z, values.w);
			values.y = Mathf.Clamp(values.y, values.z, values.w);

			values.y = values.x + (float)0.001 >= values.y ? values.y + (float)0.001 : values.y;
			return values;
		}


		//=========================================================================
		//-----------------|		TOGGLE WITH FOLDOUT        |-------------------
		//=========================================================================
		static public void FoldoutToggle(string label, string tooltip, ref bool featureOn, ref bool foldout)
		{
			EditorGUILayout.BeginHorizontal();
			var rect = EditorGUILayout.GetControlRect();

			var foldoutRect = new Rect(rect.x + 15f, rect.y, rect.width, rect.height);
			var toggleRect = new Rect(rect.x + 18f, rect.y, 14f, rect.height);
			var labelRect = new Rect(rect.x + 35f, rect.y, rect.width  , rect.height);


			EditorGUI.BeginChangeCheck();
			featureOn = EditorGUI.Toggle(toggleRect, featureOn, EditorStyles.toggle);

			if (EditorGUI.EndChangeCheck())
			{
				foldout = featureOn;
			}

			foldout = EditorGUI.Foldout(foldoutRect, foldout, string.Empty, true);

			EditorGUI.LabelField(labelRect, LabelAndTooltip(label, tooltip));
			EditorGUILayout.EndHorizontal();
		}

		//=========================================================================
		//-----------------|		CENTERED FOLDOUT        |-------------------
		//=========================================================================
		static public void CenteredFoldout(string label, ref bool foldout)
		{
			EditorGUILayout.BeginHorizontal();

			var rect = EditorGUILayout.GetControlRect();

			var foldoutRect = new Rect(rect.x + 15f, rect.y, rect.width, rect.height);
			var labelRect = new Rect(rect.x, rect.y, rect.width, rect.height);

			foldout = EditorGUI.Foldout(foldoutRect, foldout, string.Empty, true);
			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUI.LabelField(labelRect, label, styBoldCenter);
			EditorStyles.label.fontStyle = FontStyle.Normal;

			EditorGUILayout.EndHorizontal();
		}


		//=========================================================================
		//--------------|			HEIGHTMAP BLENDING         |-------------------
		//=========================================================================
		static public void HeightmapBlending(bool heightBlending, ref bool heightBlendingFoldout, MaterialEditor materialEditor, Material targetMat, Terrain terrain, int numberOfLayers, string label, string tooltip)
		{
			EditorGUI.BeginChangeCheck();

			EditorStyles.label.fontStyle = FontStyle.Bold;
			FoldoutToggle(label, tooltip, ref heightBlending, ref heightBlendingFoldout);
			EditorStyles.label.fontStyle = FontStyle.Normal;

			if (EditorGUI.EndChangeCheck())
			{
				materialEditor.RegisterPropertyChangeUndo("InTerra HeightBlend");
				InTerra_TerrainShaderGUI.SetKeyword("_TERRAIN_BLEND_HEIGHT", heightBlending);
				targetMat.SetFloat("_HeightmapBlending", heightBlending ? 1.0f : 0.0f);
				targetMat.SetFloat("_HeightmapBlendingFoldout", heightBlendingFoldout ? 1.0f : 0.0f);
				if (!disableUpdates) InTerra_Data.UpdateTerrainData(updateDict);
			}

			if (heightBlendingFoldout)
			{
				GUI.enabled = heightBlending;
				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					InTerra_TerrainShaderGUI.PropertyLine("_HeightTransition", "Sharpness", "Sharpness of the textures transitions");

					if (targetMat.IsKeywordEnabled("_TERRAIN_DISTANCEBLEND"))
					{
						InTerra_TerrainShaderGUI.PropertyLine("_Distance_HeightTransition", "Distant Sharpness", "Sharpness of the textures transitions for distant area setted in Hide Tiling.");
					}

					if (targetMat.shader.name.Contains(InTerra_Data.HDRPTerrainTessellationShaderName) || targetMat.shader.name.Contains(InTerra_Data.HDRPMeshTerrainTessellationShaderName))
					{
						InTerra_TerrainShaderGUI.PropertyLine("_Tessellation_HeightTransition", "Tessellation Sharpness", "Sharpness of the textures transitions for Tessellation.");
					}

				}
				GUI.enabled = true;

				if (targetMat.shader.name == InTerra_Data.DiffuseTerrainShaderName ||  targetMat.shader.name == InTerra_Data.DiffuseMeshTerrainShaderName) EditorGUILayout.HelpBox("Heightmap for blending should be included in Diffuse Texture Alpha channel.", MessageType.Info);

				#if !USING_HDRP
					if (heightBlending && terrain != null && terrain.terrainData.alphamapLayers > 4 && numberOfLayers == 1)			
					{
						EditorGUILayout.HelpBox("Heightmap Blending is applied only to one shader pass, which is now four Layers, but you can select \"Eight Layers - Single Pass\" in \"Terrain Layers\" enum.", MessageType.Info);
					}
				#endif
			}

			EditorGUILayout.EndFoldoutHeaderGroup();

		}


		//=============================================================================
		//--------------|         PARALLAX OCCLUSION MAPPING         |-----------------
		//=============================================================================
		static public void ParallaxOcclusionMapping(bool parallax, bool parallaxFoldout, MaterialEditor materialEditor, Material targetMat, TerrainLayer[] terrainLayers, bool meshTerrain, ref bool pomSetting, ref bool mipMinMax)
		{
			EditorGUI.BeginChangeCheck();

			EditorStyles.label.fontStyle = FontStyle.Bold;
			FoldoutToggle("Parallax Occlusion Mapping", "An illusion of 3D effect created by offsetting the texture depending on heightmap.", ref parallax, ref parallaxFoldout);
			EditorStyles.label.fontStyle = FontStyle.Normal;


			if (EditorGUI.EndChangeCheck())
			{
				materialEditor.RegisterPropertyChangeUndo("InTerra Parallax Occlusion Mapping");
				targetMat.SetFloat("_Terrain_Parallax", parallax ? 1.0f : 0.0f);
				targetMat.SetFloat("_Terrain_ParallaxFoldout", parallaxFoldout ? 1.0f : 0.0f);
				if (!disableUpdates) InTerra_Data.UpdateTerrainData(updateDict);
			}

			if (parallaxFoldout)
			{
				GUI.enabled = parallax;
				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
					{
						EditorGUI.BeginChangeCheck();
						float affineSteps = targetMat.GetFloat("_ParallaxAffineStepsTerrain");
						GUILayout.Label(LabelAndTooltip("Affine Steps: ", "The higher number the smoother transition between steps, but also the higher number will increase performance heaviness."));
						affineSteps = EditorGUILayout.IntField((int)affineSteps, GUILayout.MaxWidth(30));
						affineSteps = Mathf.Clamp(affineSteps, 1, 10);

						if (EditorGUI.EndChangeCheck())
						{
							materialEditor.RegisterPropertyChangeUndo("InTerra Parallax Values");
							targetMat.SetFloat("_ParallaxAffineStepsTerrain", affineSteps);
						}
					}
					using (new GUILayout.VerticalScope(EditorStyles.helpBox))
					{
						InTerra_GUI.MipMapsFading(targetMat, "Mip Maps Fading", materialEditor, ref mipMinMax);
					}
					EditorGUILayout.Space();

					EditorGUI.indentLevel = 1;
					pomSetting = EditorGUILayout.Foldout(pomSetting, "Layers Setting", true);
					EditorGUI.indentLevel = 0;
					if (pomSetting && terrainLayers != null)
					{
						using (new GUILayout.VerticalScope(EditorStyles.helpBox))
						{

							using (new GUILayout.HorizontalScope())
							{
								GUILayout.Label(LabelAndTooltip("Height", "The value of the height illusion."), styleMini, GUILayout.MinWidth(40));
								GUILayout.Label(LabelAndTooltip("Steps", "Each step is creating a new layer for offsetting. The more steps, the more precise the parallax effect will be, but also the higher number will increase performance heaviness."), styleMini, GUILayout.MaxWidth(30));
							}
							for (int i = 0; i < terrainLayers.Length; i++)
							{
								TerrainLayer tl = terrainLayers[i];
								if (tl)
								{
									Vector4 amplitude = tl.diffuseRemapMax;
									Vector4 steps = tl.diffuseRemapMin;
									EditorGUI.BeginChangeCheck();
									GUILayout.BeginHorizontal();
									amplitude.w = EditorGUILayout.FloatField((i + 1).ToString() + ". " + tl.name + " :", amplitude.w, GUILayout.MinWidth(60));
									amplitude.w = Mathf.Clamp(amplitude.w, 0, 20);

									float backupDecimal = steps.w % 1;
									steps.w = steps.w - backupDecimal;

									steps.w = EditorGUILayout.IntField((int)steps.w, GUILayout.MaxWidth(25));
									steps.w = Mathf.Clamp(steps.w, 0, 50) + backupDecimal;
									GUILayout.EndHorizontal();
									if (EditorGUI.EndChangeCheck())
									{
										Undo.RecordObject(terrainLayers[i], "InTerra Parallax Values");
										tl.diffuseRemapMax = amplitude;
										tl.diffuseRemapMin = steps;

										if(meshTerrain)
                                        {
											Undo.RecordObject(targetMat, "InTerra Parallax Values");
											InTerra_Data.TerrainLaeyrDataToMaterial(tl, i.ToString(), targetMat);
										}
									}
								}
							}
						}
					}
				}
			}
			GUI.enabled = true;
		}

		//==================================================================
		//-----------------|        TESSELLATION         |------------------
		//==================================================================
		public static void TessellationDistaces(Material targetMat, MaterialEditor editor, ref bool minMax)
		{
			float minDist = targetMat.GetFloat("_TessellationFactorMinDistance");
			float maxDist = targetMat.GetFloat("_TessellationFactorMaxDistance");
			Vector4 mipMapFade = targetMat.GetVector("_MipMapFade");

			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField("Tessellation Factor");

				using (new GUILayout.HorizontalScope())
				{
					minDist = Mathf.Clamp(minDist, mipMapFade.z, mipMapFade.w);
					maxDist = Mathf.Clamp(maxDist, mipMapFade.z, mipMapFade.w);

					EditorGUI.BeginChangeCheck();

					EditorGUILayout.LabelField(minDist.ToString("0.0"), GUILayout.Width(33));
					EditorGUILayout.MinMaxSlider(ref minDist, ref maxDist, mipMapFade.z, mipMapFade.w); //The range is the same as for MipMaps
					EditorGUILayout.LabelField(maxDist.ToString("0.0"), GUILayout.Width(33));

					maxDist = minDist + (float)0.001 >= maxDist ? maxDist + (float)0.001 : maxDist;

					if (EditorGUI.EndChangeCheck())
					{
						editor.RegisterPropertyChangeUndo("Tessellation Factor distance");
						targetMat.SetFloat("_TessellationFactorMinDistance", minDist);
						targetMat.SetFloat("_TessellationFactorMaxDistance", maxDist);

					}
				}
				EditorGUILayout.Space();

				MipMapsFading(targetMat, "Mip Maps", editor, ref minMax);
			}
		}

		static public void Tessellation(MaterialEditor materialEditor, Material targetMat, TerrainLayer[] terrainLayers, ref bool mipMinMax, ref bool tessDistances, ref bool tessSetting)
		{
			bool foldout = targetMat.GetFloat("_TessellationFoldout") == 1;
			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.BeginHorizontal();
				
				var rect = EditorGUILayout.GetControlRect();

				var foldoutRect = new Rect(rect.x + 15f, rect.y, rect.width, rect.height);
				var labelRect = new Rect(rect.x + 20, rect.y, rect.width, rect.height);

				EditorGUI.BeginChangeCheck();

				foldout = EditorGUI.Foldout(foldoutRect, foldout, string.Empty, true);
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorStyles.label.fontSize = 12;
				EditorGUI.LabelField(labelRect, "Tessellation");
				EditorStyles.label.fontSize = 11;
				EditorStyles.label.fontStyle = FontStyle.Normal;


				if (EditorGUI.EndChangeCheck())
				{
					targetMat.SetFloat("_TessellationFoldout", foldout ? 1.0f : 0.0f);
				}
			
				EditorGUILayout.EndHorizontal();

				if(foldout)
				{ 			
					EditorGUILayout.Space();


					InTerra_TerrainShaderGUI.PropertyLine("_TessellationFactor", "Tessellation Factor", "Controls the strength of the tessellation effect. Higher values result in more tessellation. Maximum tessellation factor is 15 on the Xbox One and PS4");
					InTerra_TerrainShaderGUI.PropertyLine("_TessellationBackFaceCullEpsilon", "Triangle Culling Epsilon", "Controls triangle culling. A value of -1.0 disables back face culling for tessellation, higher values produce more aggressive culling and better performance.");
					InTerra_TerrainShaderGUI.PropertyLine("_TessellationFactorTriangleSize", "Triangle Size", "Sets the desired screen space size of triangles (in pixels). Smaller values result in smaller triangle. Set to 0 to disable adaptative factor with screen space size.");
					InTerra_TerrainShaderGUI.TessellationMode tessMode = targetMat.IsKeywordEnabled("_TESSELLATION_PHONG") ? InTerra_TerrainShaderGUI.TessellationMode.Phong : InTerra_TerrainShaderGUI.TessellationMode.None;

					EditorGUI.BeginChangeCheck();

					using (new GUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(LabelAndTooltip("Tessellation Mode", "Specifies the method HDRP uses to tessellate the mesh. None uses only the Displacement Map to tessellate the mesh. Phong tessellation applies additional Phong tessellation interpolation for smoother mesh."), GUILayout.Width(120));
						tessMode = (InTerra_TerrainShaderGUI.TessellationMode)EditorGUILayout.EnumPopup(tessMode);
					}

					if (tessMode == InTerra_TerrainShaderGUI.TessellationMode.Phong)
					{
						InTerra_TerrainShaderGUI.PropertyLine("_TessellationShapeFactor", "Shape Factor", "Controls the strength of Phong tessellation shape (lerp factor).");
					}
					if (EditorGUI.EndChangeCheck())
					{
						materialEditor.RegisterPropertyChangeUndo("InTerra Tessellation Mode");
						InTerra_TerrainShaderGUI.SetKeyword("_TESSELLATION_PHONG", tessMode == InTerra_TerrainShaderGUI.TessellationMode.Phong);
						if (!disableUpdates) InTerra_Data.UpdateTerrainData(updateDict);
					}
					InTerra_TerrainShaderGUI.PropertyLine("_TessellationShadowQuality", "Shadows quality", "Setting of shadows accuracy calculation. Higher value means more precise calculation.");


					EditorGUILayout.Space();

					EditorGUI.indentLevel = 1;
					tessDistances = EditorGUILayout.Foldout(tessDistances, "Fading Distances", true);
					EditorGUI.indentLevel = 0;
					if (tessDistances && terrainLayers != null)
					{
						TessellationDistaces(targetMat, materialEditor, ref mipMinMax);
					}

					EditorGUI.indentLevel = 1;
					tessSetting = EditorGUILayout.Foldout(tessSetting, "Layers Setting", true);
					EditorGUI.indentLevel = 0;

					if (tessSetting && terrainLayers != null)
					{
						float maxTessHeight = 0;
						for (int i = 0; i < terrainLayers.Length; i++)
						{
							TerrainLayer tl = terrainLayers[i];
							if (tl)
							{
								Vector4 remapMax = tl.diffuseRemapMax;
								Vector4 remapMin = tl.diffuseRemapMin;
								float amplitude = remapMax.w;
								float centrer = remapMin.z * 10000;
								float height = (amplitude / 2) + centrer;

								maxTessHeight = maxTessHeight < height ? height : maxTessHeight;

								using (new GUILayout.VerticalScope(EditorStyles.helpBox))
								{
									EditorGUI.BeginChangeCheck();

									using (new GUILayout.HorizontalScope())
									{
										EditorGUILayout.LabelField((i + 1).ToString() + ". " + tl.name, styleBold, GUILayout.MinWidth(100));
									}

									using (new GUILayout.HorizontalScope())
									{
										if (tl && AssetPreview.GetAssetPreview(tl.diffuseTexture))
										{
											GUI.Box(EditorGUILayout.GetControlRect(GUILayout.Width(40), GUILayout.Height(40)), AssetPreview.GetAssetPreview(tl.diffuseTexture));
										}

										using (new GUILayout.VerticalScope())
										{
											amplitude = EditorGUILayout.FloatField("Amplitude :", amplitude);
											centrer = EditorGUILayout.FloatField("Height Offset:", centrer);
										}
									}

									remapMax.w = Mathf.Clamp(amplitude, 0, 100);
									remapMin.z = Mathf.Clamp(centrer, -50, 50) * 0.0001f;

									if (EditorGUI.EndChangeCheck())
									{
										Undo.RecordObject(terrainLayers[i], "InTerra Tessellation Values");
										tl.diffuseRemapMin = remapMin;
										tl.diffuseRemapMax = remapMax;
										if (InTerra_Data.CheckMeshTerrainShader(targetMat))
										{
											InTerra_Data.TerrainLaeyrDataToMaterial(tl, i.ToString(), targetMat);
										}
									}
								}
							}
						}
						targetMat.SetFloat("_TessellationMaxDisplacement", (maxTessHeight + 10.0f) * 0.01f);
					}
					EditorGUI.indentLevel = 0;
				}
			}
			if (foldout)
			{
				EditorGUILayout.Space();
			}
		}


		//=======================================================================
		//--------------|         TRACK MATERIAL EDITOR        |-----------------
		//=======================================================================
		public static void TrackMaterialEditor(Material targetMat, MaterialEditor materialEditor, ref bool minMax)
		{
			InTerra_TracksShaderGUI.TrackType trackType = InTerra_TracksShaderGUI.TrackType.Default;
			using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField("Track Type", GUILayout.MaxWidth(80));
				if (targetMat.IsKeywordEnabled("_FOOTPRINTS"))
				{
					trackType = InTerra_TracksShaderGUI.TrackType.Footprints;
				}
				else if (targetMat.IsKeywordEnabled("_TRACKS"))
				{
					trackType = InTerra_TracksShaderGUI.TrackType.WheelTracks;
				}
				EditorGUI.BeginChangeCheck();
				trackType = (InTerra_TracksShaderGUI.TrackType)EditorGUILayout.EnumPopup(trackType);
				if (EditorGUI.EndChangeCheck())
				{
					materialEditor.RegisterPropertyChangeUndo("InTerra Shader Variant");
					SetKeyword("_TRACKS", trackType == InTerra_TracksShaderGUI.TrackType.WheelTracks);
					SetKeyword("_FOOTPRINTS", trackType == InTerra_TracksShaderGUI.TrackType.Footprints);
				}
			}
			if (trackType != InTerra_TracksShaderGUI.TrackType.Default)
			{
				MaterialProperty heightmap = MaterialEditor.GetMaterialProperty(new Material[] { targetMat }, "_HeightTex");
				using (new GUILayout.HorizontalScope())
				{
					Rect textureRect = EditorGUILayout.GetControlRect(GUILayout.MinWidth(100));
					EditorGUI.BeginChangeCheck();
					materialEditor.TexturePropertyMiniThumbnail(textureRect, heightmap, "Heightmap", "Heightmap of track or footprint");
					if (EditorGUI.EndChangeCheck())
					{
						InTerra_Data.initTrack = false;
					}
					using (new GUILayout.VerticalScope(GUILayout.MinWidth(50)))
					{
						EditorGUI.BeginChangeCheck();
						KeywordToggle("Invert", "_INVERT", "Invert Heightmap texture.");
						KeywordToggle("Rotate", "_ORIENTATION", "Rotate texture by 90 degree");
						if (EditorGUI.EndChangeCheck())
						{
							InTerra_Data.initTrack = false;
						}
					}
				}

				EditorGUI.BeginChangeCheck();
				materialEditor.ShaderProperty(MaterialEditor.GetMaterialProperty(new Material[] { targetMat }, "_TerrainTrackContrast"), LabelAndTooltip("Contrast", "Contrast of Heightmap"));
				materialEditor.ShaderProperty(MaterialEditor.GetMaterialProperty(new Material[] { targetMat }, "_TerrainTrackHeightmapOffset"), LabelAndTooltip("Offset", "Heightmap Offset"));
				if (EditorGUI.EndChangeCheck())
				{
					InTerra_Data.initTrack = false;
				}

				using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					EditorGUI.BeginChangeCheck();
					materialEditor.TextureScaleOffsetProperty(heightmap);

					if (EditorGUI.EndChangeCheck())
					{
						InTerra_Data.initTrack = false;
					}
				}
			}

			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.LabelField("Edge Fading", new GUIStyle(EditorStyles.boldLabel));

				Vector4 edgeFading = targetMat.GetVector("_EdgeFading");

				EditorGUI.BeginChangeCheck();

				edgeFading = MinMaxValues(edgeFading, false, false, ref minMax);
				if (EditorGUI.EndChangeCheck())
				{
					materialEditor.RegisterPropertyChangeUndo("InTerra Track Edge Fading");
					targetMat.SetVector("_EdgeFading", edgeFading);
					InTerra_Data.initTrack = false;
				}

				if (targetMat.IsKeywordEnabled("_TRACKS"))
				{ 
					EditorGUI.BeginChangeCheck();
					materialEditor.ShaderProperty(MaterialEditor.GetMaterialProperty(new Material[] { targetMat }, "_WheelTrackFading"), LabelAndTooltip("Initial Fading In/Out", "The length of fading in and out of the initial track stamp."));
					if (EditorGUI.EndChangeCheck())
					{
						InTerra_Data.initTrack = false;
					}
				}
			}

			void KeywordToggle(string label, string keyword, string tooltip)
			{
				bool toggle = targetMat.IsKeywordEnabled(keyword);
				EditorGUI.BeginChangeCheck();
				toggle = EditorGUILayout.ToggleLeft(LabelAndTooltip(label, tooltip), toggle);
				if (EditorGUI.EndChangeCheck())
				{
					materialEditor.RegisterPropertyChangeUndo("InTerra Track " + label);
					SetKeyword(keyword, toggle);
				}
			}

			void SetKeyword(string name, bool set)
			{
				if (set) targetMat.EnableKeyword(name); else targetMat.DisableKeyword(name);
			}

		}

		static GUIContent LabelAndTooltip(string label, string tooltip)
		{
			return new GUIContent() { text = label, tooltip = tooltip };
		}


		static public void MeshTerrainSplats(MaterialEditor materialEditor, Material targetMat, TerrainLayer[] terrainLayers, MeshRenderer meshTerrain, Texture2D controlMap, string controlMapName, Vector2Int layers, MeshRenderer[] mts)
		{
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(GUILayout.Width(105)))
				{
					EditorGUI.BeginChangeCheck();
					if (controlMapName == "_Control")
					{
						EditorGUILayout.LabelField("Control Map", styleBold, GUILayout.MinWidth(25));
					}
					else if (controlMapName == "_Control1")
					{
						EditorGUILayout.LabelField("Control Map 2", styleBold, GUILayout.MinWidth(25));
					}
					#if (USING_URP || USING_HDRP)
						else if (controlMapName == "_Control2")
						{
							EditorGUILayout.LabelField("Control Map 3", styleBold, GUILayout.MinWidth(25));
						}
						else if (controlMapName == "_Control3")
						{
							EditorGUILayout.LabelField("Control Map 4", styleBold, GUILayout.MinWidth(25));
						}
					#endif
					controlMap = (Texture2D)EditorGUILayout.ObjectField(controlMap, typeof(Texture2D), false, GUILayout.Height(100), GUILayout.Width(100));

					if (EditorGUI.EndChangeCheck())
					{
						materialEditor.RegisterPropertyChangeUndo("InTerra Control Map");

						targetMat.SetTexture(controlMapName, controlMap);
						InTerra_Data.SetMeshTerrainPositionAndSize(targetMat, meshTerrain);
						foreach (var mr in mts)
						{
							if (mr.TryGetComponent<InTerra_MeshTerrainData>(out var m))
							{
								var mtd = mr.GetComponent<InTerra_MeshTerrainData>();
								Undo.RecordObject(mtd, "InTerra Control Map");
								if(controlMapName == "_Control")
                                {
									mtd.ControlMap = controlMap;
								}
								else if (controlMapName == "_Control1")
                                {
									mtd.ControlMap1 = controlMap;
								}
								#if (USING_URP || USING_HDRP)
									else if (controlMapName == "_Control2")
									{
										mtd.ControlMap2 = controlMap;
									}
									else if (controlMapName == "_Control3")
									{
										mtd.ControlMap3 = controlMap;
									}
								#endif
							}
						}
					}
				}

				using (new GUILayout.VerticalScope())
				{
					if (controlMapName == "_Control")
					{
						EditorGUILayout.LabelField("Terrain Layers", GUILayout.MinWidth(35));
					}
					else 
                    {
						EditorGUILayout.LabelField(" ", GUILayout.MinWidth(35));
					}
					string[] splatColorLabel = { "R:", "G:", "B:", "A:" };

					for (int i = layers.x; i < layers.y; i++)
					{
						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField(splatColorLabel[i % 4], GUILayout.Width(15));

							TerrainLayer tl = terrainLayers[i];
							Texture2D layerTexture = null;
							if (tl)
							{
								layerTexture = AssetPreview.GetAssetPreview(tl.diffuseTexture);
							}
							using (new GUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Width(19)))
							{
								if (layerTexture)
								{
									GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(17), GUILayout.Height(17)), layerTexture, ScaleMode.ScaleAndCrop);
								}
								else
								{
									EditorGUILayout.GetControlRect(GUILayout.Width(17), GUILayout.Height(17));
								}
							}

							EditorGUI.BeginChangeCheck();

							tl = (TerrainLayer)EditorGUILayout.ObjectField(tl, typeof(TerrainLayer), false, GUILayout.MinWidth(100), GUILayout.Height(22));

							if (EditorGUI.EndChangeCheck())
							{
								materialEditor.RegisterPropertyChangeUndo("InTerra Terrain Color Tint Texture");
							
								InTerra_Data.TerrainLaeyrDataToMaterial(tl, i.ToString(), meshTerrain.sharedMaterial);
								InTerra_Data.SetMeshTerrainPositionAndSize(targetMat, meshTerrain);


								foreach (var mr in mts)
								{
									if (mr.TryGetComponent<InTerra_MeshTerrainData>(out var m))
									{
										var mtd = mr.GetComponent<InTerra_MeshTerrainData>();
										Undo.RecordObject(mtd, "InTerra Layer Change");
										mtd.TerrainLayers[i] = tl;
										terrainLayers[i] = tl;
									}
								}
								#if (USING_URP || USING_HDRP)
								if (targetMat.IsKeywordEnabled("_LAYERS_SIXTEEN"))
								{
									targetMat.SetTexture("_SplatArray16", InTerra_Data.DiffuseTextureArrays16(terrainLayers));
									targetMat.SetTexture("_SplatArray16", InTerra_Data.NormalTextureArrays16(terrainLayers));
								}
								#endif
							}
						}
					}
				}
			}
		}
	}
}

