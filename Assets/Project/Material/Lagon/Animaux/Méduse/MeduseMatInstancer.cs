using UnityEngine;

public class MeduseMatInstancer : MonoBehaviour
{
    [SerializeField] private bool randomOffset;
    
    [SerializeField] private float offset;
    
    [SerializeField] private MeshRenderer meshRenderer;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomOffset)
        {
            MaterialPropertyBlock offsetBlock = new MaterialPropertyBlock();
            offsetBlock.SetFloat("_Time_Offset", Random.Range(0, offset));
            meshRenderer.SetPropertyBlock(offsetBlock);
        }
    }
}
