using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhotoFrame : MonoBehaviour
{
    public Toggle _toggle;
    public PhotoInfos photoInfos;
    
    private bool canBeSelected = false;

    public void OnClicked()
    {
        if(canBeSelected)
            _toggle.isOn = !_toggle.isOn;
    }
}
