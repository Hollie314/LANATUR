using UnityEngine;
using UnityEngine.Rendering;

namespace Kimede
{
   [VolumeComponentMenu("Kimede/Volume Mesh Blending")]
   public class VolumeMeshBlending : VolumeComponent
   {
      public BoolParameter enabled = new BoolParameter(false);
      public LayerMaskParameter selectedLayers = new LayerMaskParameter(-1);
      public ClampedIntParameter processingIterations = new ClampedIntParameter(3, 1, 8);
      public ClampedFloatParameter blendingRadius = new ClampedFloatParameter(0.1f, 0.01f, 2f);
      public ClampedIntParameter scalingFactor = new ClampedIntParameter(50, 5, 250);
      public ClampedFloatParameter distanceFade = new ClampedFloatParameter(2f, 0f, 10f);
      public ClampedFloatParameter minimumRange = new ClampedFloatParameter(0.5f, 0f, 10f);
      public ClampedFloatParameter maximumRange = new ClampedFloatParameter(100f, 0f, 1000f);
      public ClampedFloatParameter rangeFalloff = new ClampedFloatParameter(2f, 0f, 10f);
      public ClampedFloatParameter surfaceThreshold = new ClampedFloatParameter(0.5f, 0f, 2f);
      public ClampedFloatParameter ColorIntensity = new ClampedFloatParameter(1f, 0f, 5f);
      public ClampedFloatParameter OpacityLevel = new ClampedFloatParameter(1f, 0f, 5f);
      public ClampedFloatParameter EntityTolerance = new ClampedFloatParameter(0.2f, -1f, 1f);



   }

   public static class BlendDefaultSettings
   {
      public static int ProcessingIterations = 3;
      public static int ScalingFactor = 50;
      public static float BlendingRadius = 0.1f;
      public static float DistanceFade = 0.5f;
      public static float MinimumRange = 1f;
      public static float MaximumRange = 100f;
      public static float RangeFalloff = 3f;
      public static float SurfaceThreshold = 0.5f;
      public static float ColorIntensity = 1f;
      public static float OpacityLevel = 1f;
      public static float EntityTolerance = 0.1f;
      public static bool isChanged = false;

      public static void ResetToDefault(Material material)
      {
         ProcessingIterations = material.GetInt("_ProcessingIterations");
         ScalingFactor = material.GetInt("_ScalingFactor");
         BlendingRadius = material.GetFloat("_BlendingRadius");
         DistanceFade = material.GetFloat("_DistanceFade");
         MinimumRange = material.GetFloat("_MinimumRange");
         MaximumRange = material.GetFloat("_MaximumRange");
         RangeFalloff = material.GetFloat("_RangeFalloff");
         SurfaceThreshold = material.GetFloat("_SurfaceThreshold");
         ColorIntensity = material.GetFloat("_ColorIntensity");
         OpacityLevel = material.GetFloat("_OpacityLevel");
         EntityTolerance = material.GetFloat("_EntityTolerance");
         isChanged = true;
      }

   }

}
