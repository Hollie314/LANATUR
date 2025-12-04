using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Behavior;
using Action = System.Action;

public class CallEventLamantin : MonoBehaviour
{
    [SerializeField] private StoryEvent storyEvent;
    public static event Action<CallEventLamantin> OnEventCalled;
    
    public void CallEvent()
    {
        storyEvent.isEventActive = true;
        OnEventCalled?.Invoke(this);
    }
}
