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
        
        Debug.Log($"{this.gameObject.name} is target:  {isTarget}");
    }

    public void BecomeTarget()
    {
        isTarget = true;
    }

    private bool CheckIfVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Debug.Log($"{this.gameObject.name} is visible part 0");
        if (planes.All(plane => plane.GetDistanceToPoint(transform.position) >= 0))
        {
            Debug.Log($"{this.gameObject.name} is visible part 1");
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 direction = (transform.position - cameraPos).normalized;

            if (Physics.Raycast(cameraPos, direction, out RaycastHit hit))
            {
                Debug.Log($"{this.gameObject.name} is visible part 2");
                return true;
            }
        }
        return false;
    }
}
