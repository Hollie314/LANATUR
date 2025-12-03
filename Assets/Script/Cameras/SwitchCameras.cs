using System;
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnimator;
    
    private void OnEnable()
    {
        OpenUI.OnCameraWalk += CameraWalk;
        OpenUI.OnCameraPhoto += CameraPhoto;
    }

    private void OnDisable()
    {
        OpenUI.OnCameraWalk -= CameraWalk;
        OpenUI.OnCameraPhoto -= CameraPhoto;
    }

    private void CameraWalk(OpenUI openUI)
    {
        PlayerAnimator.SetBool("OnCamera", false);
    }
    
    private void CameraPhoto(OpenUI openUI)
    {
        PlayerAnimator.SetBool("OnCamera", true);
    }
}
