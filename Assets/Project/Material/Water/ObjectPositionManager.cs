using System;
using JetBrains.Annotations;
using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEditor;


public class ObjectPositionManager : MonoBehaviour
{
    private static readonly int ObjectPos = Shader.PropertyToID("_ObjectPos");
    public Transform objectTransform;
    public Material waterSurfaceMat;

    public float waterDepth;
    public int stepAmount;
    public float maxDensity;
    public float densitySlope;
    
    private GameObject[] _layersArray;
    
    public GameObject layerPrefab;
    private void Awake()
    {
        SetPos(objectTransform, waterSurfaceMat);
    }
    
    // Update is called once per frame
    void Update()
    {
        SetPos(objectTransform, waterSurfaceMat);
    }

    private void SetPos(Transform transformm, Material material)
    {
        material.SetVector(ObjectPos,transformm.position);
    }

    private void UpdateLayers(GameObject[] layersArray, GameObject prefab, float[] distances)
    {
        if (layersArray.Length == 0)
        {
            for (int i = 0; i < distances.Length; i++)
            {
                float d = distances[i];
                //this.transform.position.y - d
                //new Vector3(this.transform.position.x, this.transform.position.y - d, this.transform.position.z)

                GameObject layer = Instantiate(prefab, this.transform);
                layer.transform.position = new Vector3(this.transform.position.x, 
                    this.transform.position.y - d,
                    this.transform.position.z);
                
                

            }
        }
        else
        {
            
        }
    }
}

[CustomEditor(typeof(ObjectPositionManager))]
class ObjectPositionEditor : Editor
{
        public override void OnInspectorGUI()
        {
            ObjectPositionManager objectPositionManager = (ObjectPositionManager)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Calculate Mat"))
            {
                objectPositionManager.SetPos(objectPositionManager.objectTransform, objectPositionManager.waterSurfaceMat);
            }
        }
}

