using System;
using UnityEditor.UIElements;
using UnityEngine;

public class SwitchLayer : MonoBehaviour
{
  public string newLayerName;

  void Switch()
  {
    gameObject.layer = LayerMask.NameToLayer(newLayerName);
  }
}
