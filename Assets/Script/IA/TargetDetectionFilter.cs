using UnityEngine;

[RequireComponent(typeof(RangeDetector))]
public class TargetDetectionFilter : MonoBehaviour
{
    public string targetTag = "Player";

    [HideInInspector] public Transform currentTarget;

    private RangeDetector detector;

    private void Awake()
    {
        detector = GetComponent<RangeDetector>();
        detector.OnObjectEnter += OnObjectEnter;
        detector.OnObjectExit += OnObjectExit;
    }

    private void OnObjectEnter(Transform t)
    {
        if (t.CompareTag(targetTag))
            currentTarget = t;
    }

    private void OnObjectExit(Transform t)
    {
        if (currentTarget == t)
            currentTarget = null;
    }

    public bool HasTarget() => currentTarget != null;
}