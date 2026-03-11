using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalPart : MonoBehaviour
{
    [SerializeField] private GameObject OnTargetCuzsor;
    
    public static event Action ExitView;
    private bool isTarget = false;
    [HideInInspector] public Camera_Shot CameraShot { get; set; }

    private void Update()
    {
        if (isTarget)
        {
            if (CheckIfVisible() == false)
            {
                isTarget = false ;
                //OnTargetCuzsor.SetActive(false);
                ExitView?.Invoke();
            }
        }

        if (!isTarget && CameraShot != null)
        {
            if (CheckIfVisible())
            {
                if (!CameraShot.interestPointsVisible.Contains(this))
                {
                    CameraShot.interestPointsVisible.Add(this);
                }
            }
            else if (CameraShot.interestPointsVisible.Contains(this))
            {
                CameraShot.interestPointsVisible.Remove(this);
            }
        }
    }

    public void BecomeTarget()
    {
        isTarget = true;
        //OnTargetCuzsor.SetActive(true);
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
