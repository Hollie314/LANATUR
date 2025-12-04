using System;
using JetBrains.Annotations;
using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEditor;


public class ObjectPositionManager : MonoBehaviour
{
    public float a = 0;
    private static readonly int ObjectPos = Shader.PropertyToID("_ObjectPos");
    public Transform objectTransform;
    public Material waterSurfaceMat;

    private void Awake()
    {
        SetPos(objectTransform, waterSurfaceMat);
    }
    
    // Update is called once per frame
    void Update()
    {
        SetPos(objectTransform, waterSurfaceMat);
    }

    public void SetPos(Transform transform, Material material)
    {
        material.SetVector(ObjectPos,transform.position);
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

