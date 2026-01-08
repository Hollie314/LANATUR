using Sirenix.OdinInspector;
using UnityEngine;

public class AI_Iguane : MonoBehaviour
{
    [Header("Types")]
    // Types
    [SerializeField] private bool is_EatingIguane;
    [SerializeField] private bool is_ShoutingIguane;
    [SerializeField] private bool is_AlphaIguane;
    
    [Header("NoiseRadius")]
    // NoiseRadius
    [SerializeField] private float noiseRadius_Running;
    [SerializeField] private float noiseRadius_Walking;
    [SerializeField] private float noiseRadius_Crouching;
    [SerializeField] private MakeNoise _makeNoise;
    
    // States
    private bool isEating = false;
    private bool isCarryingFood = false;
    private bool isRunning = false;
    private bool isReleasingFood = false;
    private bool isShouting = false;
    
    // Eating Iguane variables
    [ShowIf("is_eatingIguane")]
    
    // Shouting Iguane variables
    [ShowIf("is_ShoutingIguane")]
    [SerializeField] private GameObject IguaneToProtect;
    
    // Alpha Iguane variables
    [ShowIf("is_AlphaIguane")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetectNoise(GameObject detectedObject)
    {
        //vérifier que detectedObject est un type sur lequel l'iguane cris
        if (is_ShoutingIguane)
        {
            Shout();
            return;
        }

        if (is_EatingIguane)
        {
            Run();
            return;
        }
    }

    #region EatingIguane
    private void Run()
    {
        Debug.Log("IguaneRun");
    }
    #endregion

    #region ShoutingIguane
    private void Shout()
    {
        Debug.Log("IguaneShout");
        // SphereCast a une certaine distance
        // Si la cible a un rigidbody, repousser avec les méchaniques Rigidbody
        // Si la cible a un CharacterController, repousser avec les méchaniques CharacterController
        // Sinon ne rien faire
    }

    private void FollowIguane(GameObject iguane)
    {
        
    }
    #endregion
}
