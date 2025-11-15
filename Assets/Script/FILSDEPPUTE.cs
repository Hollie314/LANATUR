using UnityEngine;

public class FILSDEPPUTE : MonoBehaviour
{
    public LayerMask SearchedLayer;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer == 6) 
        {
            Debug.Log("rightLayer");
        }
        return;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("sorti");
        }
        return;
    }
}
