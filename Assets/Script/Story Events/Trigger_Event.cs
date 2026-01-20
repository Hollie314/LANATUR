using System;
using UnityEngine;
using UnityEngine.Events;

public class Trigger_Event : MonoBehaviour
{
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter)&& !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerEnter.Invoke();
        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }

    void OntiggerExit(Collider other)
    {
        onTriggerExit.Invoke();
    }
}
