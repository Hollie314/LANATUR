using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalPart : MonoBehaviour
{
    public static event Action ExitView;
    private bool isTarget = false;
    public Camera_Shot CameraShot { get; set; }

    private void Update()
    {
        if (isTarget)
        {
            if (CheckIfVisible() == false)
            {
                isTarget = false ;
                Debug.Log("target exited view");
                ExitView?.Invoke();
            }
        }

        if (!isTarget && CameraShot != null)
        {
            if (CheckIfVisible())
            {
                if (!CameraShot.animalsOnScreen.Contains(this))
                {
                    CameraShot.animalsOnScreen.Add(this);
                    Debug.Log("added");
                }
            }
            else if (CameraShot.animalsOnScreen.Contains(this))
            {
                CameraShot.animalsOnScreen.Remove(this);
            }
        }
    }

    public void BecomeTarget()
    {
        isTarget = true;
    }

    private bool CheckIfVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (planes.All(plane => plane.GetDistanceToPoint(transform.position) >= 0))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 direction = (transform.position - cameraPos).normalized;

            if (Physics.Raycast(cameraPos, direction, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
