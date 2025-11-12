using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

[RequireComponent(typeof(SphereCollider))]
public class ColliderDetector : MonoBehaviour
{
    public RangeDetector RangeDetector;
    public bool isCrouchDetection;

    private bool isCrouched = false;
    private void OnEnable()
    {
        PlayerMotor.Crouched += Crouch;
    }

    private void OnDisable()
    {
        PlayerMotor.Crouched -= Crouch;
    }

    void Crouch(PlayerMotor Crouched)
    {
        isCrouched = !isCrouched;
    }

    private void OnTriggerStay(Collider collision)
    {
        Debug.Log("TriggerStayActive");
        if(!isCrouchDetection && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("BaseDetectionTrigger");
            //vérifier si pas crouch
            if(!isCrouched && !RangeDetector.GameObjectsDetected.Contains(collision.gameObject))
                RangeDetector.ChangeObjectsDetected(collision.gameObject, true, isCrouchDetection, isCrouched);
            else
            {
                Debug.Log("Player is crouched");
                if (RangeDetector.GameObjectsDetected.Contains(collision.gameObject))
                {
                    RangeDetector.ChangeObjectsDetected(collision.gameObject, false, isCrouchDetection, isCrouched);
                    Debug.Log("hidden cause crouched");
                }
            }
            return;
        }
        else if (collision.gameObject.CompareTag("Player") && !RangeDetector.GameObjectsDetected.Contains(collision.gameObject))
        {
            Debug.Log("CrouchDetectionTrigger");
            RangeDetector.ChangeObjectsDetected(collision.gameObject, true, isCrouchDetection, isCrouched);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (RangeDetector.GameObjectsDetected.Contains(collision.gameObject))
        {
            if (isCrouchDetection && collision.gameObject.CompareTag("Player"))
            {
                if(isCrouched)
                    RangeDetector.ChangeObjectsDetected(collision.gameObject, false, isCrouchDetection, isCrouched);
                return;
            }
            RangeDetector.ChangeObjectsDetected(collision.gameObject, false, isCrouchDetection, isCrouched);
        }
    }
}
