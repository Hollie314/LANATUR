using System.Collections.Generic;
using UnityEngine;
using System;

namespace InTerra
{
	[Serializable] public class DictionaryMaterialTerrain : SerializableDictionary<Material, Terrain> { }
	[Serializable] public class DictionaryMaterialMeshTerrain : SerializableDictionary<Material, MeshRenderer> { }

	[AddComponentMenu("/")]
	public class InTerra_UpdateAndCheck : MonoBehaviour
	{
		[SerializeField, HideInInspector] public bool FirstInit;
		[SerializeField, HideInInspector] public DictionaryMaterialTerrain MaterialTerrain = new DictionaryMaterialTerrain();
		[SerializeField, HideInInspector] public DictionaryMaterialMeshTerrain MaterialMeshTerrain = new DictionaryMaterialMeshTerrain();

		[SerializeField, HideInInspector] public List<MeshRenderer> MeshTerrainsList = new List<MeshRenderer>();

		[NonSerialized, HideInInspector] public bool TracksUpdate;
		[SerializeField, HideInInspector] public bool TracksEnabled;
		[SerializeField, HideInInspector] public bool TracksFading;
		[SerializeField, HideInInspector] public float TracksFadingTime = 30.0f;
		[HideInInspector] public RenderTexture TrackTexture;
		[HideInInspector] public float TrackDepthIndex;

		[SerializeField, HideInInspector] public bool SetSceneWetnessValues;
		[SerializeField, HideInInspector] public float GlobalWetness = 0;
		[SerializeField, HideInInspector] public Vector2 GlobalPuddles = new Vector2(-0.05f, 0.0f);
		[SerializeField, HideInInspector] public Vector3 GlobalRaindropRipples = new Vector3(0.0f, 1.0f, 1.0f);
		[SerializeField, HideInInspector] public Vector4 GlobalRaindropDistance = new Vector4(0.0f, 30.0f, 1.0f, 50.0f);

		[SerializeField, HideInInspector] public InTerra_GlobalData GlobalData;

		#if (USING_URP || USING_HDRP)
			[SerializeField, HideInInspector] public bool TextureArrayCheck;
		#endif

		void Start()
		{
			if (!InTerra_Setting.DisableAllAutoUpdates && !InTerra_Data.initUpdate)
			{
				InTerra_Data.UpdateTerrainData(InTerra_Setting.DictionaryUpdate);
				InTerra_Data.initUpdate = true;
			}
		}

		void Update()
		{
			if (TracksEnabled && TracksUpdate) InTerra_Data.TracksUpdate();
		}
	}
}
