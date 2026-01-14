using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShoutAtTarget : MonoBehaviour
{
    [SerializeField] private GameObject ShoutSphere;

    [SerializeField] private float shoutFinalRadius;
    private bool isShouting;
    private float ShoutingTime;
    private float timePassed;
    private GameObject invokedSphere;
    
    private void Update()
    {
        if (isShouting)
        {
            timePassed += Time.deltaTime;
            invokedSphere.transform.localScale = Vector3.one * Mathf.Lerp(0, ShoutingTime, timePassed) * shoutFinalRadius;
            Debug.Log($"Sphere Scale: {invokedSphere.transform.localScale}");
            if (timePassed >= ShoutingTime)
            {
                timePassed = 0;
                isShouting = false;
                Destroy(invokedSphere);
                Debug.Log("Sphere Destroyed");
            }
        }
    }

    public void TryShout(float shoutTime, GameObject Iguane)
    {
        invokedSphere = Instantiate(ShoutSphere, Iguane.transform.position, Quaternion.identity);
        isShouting = true;
        ShoutingTime = shoutTime;
    }
    
    /*
    public float shoutForce = 5f;     // Force de recul appliquée au joueur

    public bool TryShout(GameObject target)
    {
        Debug.Log("shouting");
        if (target == null)
            return false;

        // 💥 Cri visuel ou sonore
        Debug.Log($"{name} crie sur {target.name} !");

        // 🔁 Effet sur la cible (recul, stun, etc.)
        CharacterController charController = target.GetComponent<CharacterController>();
        if (charController != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            charController.Move(dir * shoutForce);
            Debug.Log($"force = {dir * shoutForce}");
        }

        return true;
    }
    */
}