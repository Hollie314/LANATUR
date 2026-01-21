using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlickeringLight : MonoBehaviour
{
   public Light Light;

   public float MinTime;
   public float MaxTime;
   public float Timer;
   
   public AudioSource LightAudio;
    void Start()
    {
        Timer = Random.Range(MinTime, MaxTime);
    }

    // Update is called once per frame
    void Update()
    {
        FlickerLight();
    }

    void FlickerLight()
    {
        if (Timer > 0)
            Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            Light.enabled = !Light.enabled;
            Timer = Random.Range(MinTime, MaxTime);
        }
        
        
    }
}
