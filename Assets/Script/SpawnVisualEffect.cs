using UnityEngine;
using UnityEngine.VFX;

public class SpawnVisualEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffectfx;

    void InstanciateVisualEffect()
    {
        visualEffectfx.Play();
    }
   
}
