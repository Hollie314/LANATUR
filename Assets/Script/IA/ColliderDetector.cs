using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

[RequireComponent(typeof(SphereCollider))]
public class ColliderDetector : MonoBehaviour
{
    public RangeDetector RangeDetector;
    public bool isCrouchDetection;
    public bool isBaseDetection;
    public bool isBaieDetection;

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
        if(isBaseDetection)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if(isCrouched)
                {
                    RangeDetector.ChangePlayerDetected(collision.gameObject, false, false, isCrouched);
                }
                else
                {
                    RangeDetector.ChangePlayerDetected(collision.gameObject, true, false, isCrouched);
                }
            }
            return;
        }
        else if (isCrouchDetection)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                RangeDetector.ChangePlayerDetected(collision.gameObject, true, true, isCrouched);
            }
            return;
        }
        else if (isBaieDetection)
        {
            if (collision.gameObject.layer == 6 && !collision.gameObject.CompareTag("Player") && !RangeDetector.GameObjectsDetected.Contains(collision.gameObject))
            {
                RangeDetector.GameObjectsDetected.Add(collision.gameObject);
            }
            return;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (isBaseDetection)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                RangeDetector.ChangePlayerDetected(collision.gameObject, false, false, isCrouched);
            }
        }
        else if (isCrouchDetection)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                RangeDetector.ChangePlayerDetected(collision.gameObject, false, true, isCrouched);
            }
        }
        else if (isBaieDetection)
        {
            if (collision.gameObject.layer == 6)
            {
                RangeDetector.GameObjectsDetected.Remove(collision.gameObject);
            }
        }
    }
}
