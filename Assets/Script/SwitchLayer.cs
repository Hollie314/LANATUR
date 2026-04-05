using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLayer : MonoBehaviour
{
  public string newLayerName;
  [SerializeField] private UnityEvent unityEvent;

  void Switch()
  {
    gameObject.layer = LayerMask.NameToLayer(newLayerName);
    unityEvent?.Invoke();   
  }
}
