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

