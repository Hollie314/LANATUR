using UnityEngine;

public class MakeNoise : MonoBehaviour
{
    [SerializeField] private LayerMask layerHearing;
    
    public void Noise(GameObject thisObject, Vector3 position,float noiseRadius, AudioSource audioSource = null, AudioClip audioClip = null)
    {
        if (audioSource && audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        
        RaycastHit[] allHit;
        allHit = Physics.SphereCastAll(position, noiseRadius, thisObject.transform.forward, noiseRadius, layerHearing);
        Debug.DrawRay(position, transform.forward * noiseRadius, Color.red);
        foreach (RaycastHit hit in allHit)
        {
            Debug.Log($"hit {hit.transform.name}");
            if (hit.collider.GetComponent<AI_Iguane>() != null)
            {
                hit.collider.GetComponent<AI_Iguane>().DetectNoise(thisObject);
                Debug.Log("Someone heard noise");
            }
        }
    }
}
