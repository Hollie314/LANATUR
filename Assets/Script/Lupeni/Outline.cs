using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField]
    private RenderingLayerMask outlineLayer;


    private Renderer[] renderers;
    private uint originalLayer;
    private bool isOutlineActive;


    private void Start()
    {
        renderers = TryGetComponent<Renderer>(out var meshRenderer)
            ? new[] { meshRenderer }
            : GetComponentsInChildren<Renderer>();
        originalLayer = renderers[0].renderingLayerMask;
    }

    public void Activate() => SetOutline(true);

    public void Deactivate() => SetOutline(false);

    private void SetOutline(bool enable)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
            ? originalLayer | outlineLayer
            : originalLayer;
        }
    }
}
