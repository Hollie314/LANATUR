using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Behavior;
using Action = System.Action;

public class CallEventLamantin : MonoBehaviour
{
    [SerializeField] private StoryEvent storyEvent;
    public static event Action<CallEventLamantin> OnEventCalled;
    
    private float taime = 0f;

    private void Update()
    {
        taime += Time.deltaTime;
        if(taime > 10f)
            CallEvent();
    }
    
    public void CallEvent()
    {
        storyEvent.isEventActive = true;
        OnEventCalled?.Invoke(this);
    }
}
