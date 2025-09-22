using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalPart : MonoBehaviour
{
    public GameManager GameManager;
    public static event Action ExitView;
    private bool isTarget = false;


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
