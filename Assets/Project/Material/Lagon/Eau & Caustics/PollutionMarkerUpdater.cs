using UnityEngine;
using Sirenix.OdinInspector;

public class PollutionMarkerUpdater : MonoBehaviour
{
    public GameObject[] markers;
    
    [SerializeField] private Material waterMaterial;

    [Button("Update Markers")]
    public void UpdateMarkers()
    {
        for (int i = 0; i < this.markers.Length; i++)
        {
            waterMaterial.SetVector("_Marker_" + (i+1), markers[i].transform.position);
        }
    }
}
