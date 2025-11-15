using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class RangeDetector : MonoBehaviour
{
    public List<GameObject> GameObjectsDetected;

    public void ChangePlayerDetected(GameObject newObj, bool add, bool crouchTrigger, bool charCrouched)
    {
        if (add)
        {
            if (GameObjectsDetected.Contains(newObj)) // si le joueur est déjà détecté, sortir
            {
                Debug.Log("Joueur déjà détecté - return");
                return;
            }
            else
            {
                if (!charCrouched) // si le joueur n'est pas crouch, l'ajouter
                {
                    Debug.Log("Joueur debout - ajouté");
                    GameObjectsDetected.Add(newObj);
                    return;
                }
                else
                {
                    if (crouchTrigger) // Si le Trigger est celui de crouch, l'ajouter
                    {
                        Debug.Log("Joueur dans crouch trigger - ajouté");
                        GameObjectsDetected.Add(newObj);
                        return;
                    }
                    else { return; } // Sinon sortir
                }
            }
        }
        else
        {
            if (!GameObjectsDetected.Contains(newObj)) // Si le joueur n'était déjà pas présent, sortir
            {
                Debug.Log("Joueur n'existe pas - return");
                return;
            }
            else
            {
                if(charCrouched) // Si le joueur est crouch, le retirer
                {
                    Debug.Log("Joueur crouch - retiré");
                    GameObjectsDetected.Remove(newObj);
                    return;
                }
                else
                {
                    if (!crouchTrigger) // Si le trigger est celui de base, le retirer
                    {
                        Debug.Log("Joueur sorti de base trigger - retiré");
                        GameObjectsDetected.Remove(newObj);
                        return;
                    }
                    else { return ; } // Sinon sortir
                }
            }
        }
    }
}