using UnityEngine;

public class MakeNoise : MonoBehaviour
{
    public void Noise(GameObject thisObject, Vector3 position,float noiseRadius,AudioSource audioSource = null, AudioClip audioClip = null)
    {
        RaycastHit[] allHit;
        allHit = Physics.SphereCastAll(position, noiseRadius, thisObject.transform.forward, noiseRadius, 9);
        foreach (RaycastHit hit in allHit)
        {
            if (hit.collider.GetComponent<AI_Iguane>() != null)
            {
                hit.collider.GetComponent<AI_Iguane>().DetectNoise(thisObject);
                Debug.Log("Someone heard noise");
            }
        }
    }
}
