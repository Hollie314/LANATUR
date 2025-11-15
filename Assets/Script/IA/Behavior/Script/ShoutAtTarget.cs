using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShoutAtTarget : MonoBehaviour
{
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
}