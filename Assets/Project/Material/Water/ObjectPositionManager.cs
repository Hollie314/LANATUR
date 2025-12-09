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
    
    public GameObject[] layersArray;
    
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

    public void SetPos(Transform transformm, Material material)
    {
        material.SetVector(ObjectPos,transformm.position);
    }

    public float[] CalcDistances(int steps, float maxDistance)
    {
        float[] distances = new float[steps];
        float interval = maxDistance / (steps + 1);

        for (int i = 1; i <= steps; i++)
        {
            distances[i - 1] = i * interval;
        }

        return distances;
    }
    
    public GameObject[] UpdateLayers(GameObject[] layersArray, GameObject prefab, float[] distances)
    {
        if (layersArray.Length != 0)
        {
            foreach (var previousLayer in layersArray)
            {
                DestroyImmediate(previousLayer);
            }
        }
        
        layersArray = new GameObject[distances.Length];
            
        for (int i = 0; i < distances.Length; i++)
        {
            float d = distances[i];
            //this.transform.position.y - d
            //new Vector3(this.transform.position.x, this.transform.position.y - d, this.transform.position.z)

            GameObject layer = Instantiate(prefab, this.transform);
            layer.transform.position = new Vector3(this.transform.position.x, 
                this.transform.position.y - d,
                this.transform.position.z);
            layersArray.SetValue(layer, i);
        }
        
        
        return layersArray;
    }
}

[CustomEditor(typeof(ObjectPositionManager))]
class ObjectPositionEditor : Editor
{
        public override void OnInspectorGUI()
        {
            ObjectPositionManager oPManager = (ObjectPositionManager)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Update"))
            {
                oPManager.SetPos(oPManager.objectTransform, oPManager.waterSurfaceMat);

                float[] distances = oPManager.CalcDistances(oPManager.stepAmount, oPManager.waterDepth);
                
                oPManager.layersArray = oPManager.UpdateLayers(oPManager.layersArray, oPManager.layerPrefab, distances);
            }
        }
}

