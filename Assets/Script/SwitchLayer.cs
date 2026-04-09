using UnityEngine;
using UnityEngine.Events;

public class SwitchLayer : MonoBehaviour
{
    public string newLayerName;
    [SerializeField] private UnityEvent unityEvent;

    public void Switch()  // public pour que le behavior node puisse l'appeler directement
    {
        gameObject.layer = LayerMask.NameToLayer(newLayerName);
        unityEvent?.Invoke();
    }
}