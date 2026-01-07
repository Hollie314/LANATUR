#ifndef UNIVERSAL_TERRAIN_LIT_INPUT_INCLUDED
#define UNIVERSAL_TERRAIN_LIT_INPUT_INCLUDED


#if defined(_TERRAIN_NORMAL_IN_MASK)
    #undef _NORMALMAP
    #define _NORMALMAP
#endif

 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    half4 _BaseColor;
    half _Cutoff;
CBUFFER_END

#define _Surface 0.0 // Terrain is always opaque
#ifdef _LAYERS_ONE
    #define _LAYER_COUNT 1
#else
    #ifdef _LAYERS_TWO
        #define _LAYER_COUNT 2
    #else
        #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
            #ifdef _LAYERS_EIGHT
                #define _LAYER_COUNT 8
            #endif
            #ifdef _LAYERS_SIXTEEN
                #define _LAYER_COUNT 16
            #endif
        #else
            #define _LAYER_COUNT 4
        #endif
    #endif
#endif


CBUFFER_START(_Terrain)
    float4 _Control_ST;
    float4 _Control_TexelSize;
    half _DiffuseHasAlpha0, _DiffuseHasAlpha1, _DiffuseHasAlpha2, _DiffuseHasAlpha3;
    half _HeightTransition;
    half _NumLayersCount;

    #ifdef UNITY_INSTANCING_ENABLED
        float4 _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
        float4 _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
    #endif
    #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
    #endif

    //InTerra
    half4 _HT_distance, _MipMapFade;
    float _HT_distance_scale, _HT_cover, _MipMapLevel;
    half _Distance_Height_blending, _Distance_HeightTransition, _TriplanarOneToAllSteep, _TriplanarSharpness;
    half4 _TerrainSize;
    float3 _TerrainSizeXZPosY;   
    half _ControlNumber;
    half _ParallaxAffineStepsTerrain;
    float _TerrainColorTintStrenght;
    float _TerrainColorTintMode;
    float4 _TerrainColorTintDistance;
    float _TerrainNormalTintStrenght;
    float4 _TerrainColorTintTexture_ST;
    float4 _TerrainNormalTintTexture_ST;
    float4 _TerrainNormalTintDistance;
    float _HeightmapBlending;
    float _Terrain_Parallax;
    float _Tracks;
    float _GlobalWetness;
    float _PuddleHorizontalWeight;
    float _PuddleWetnessHorizontalWeight;
    float _Gamma;
    float _WorldMapping;

    TEXTURE2D_ARRAY(_SplatArray16);
    TEXTURE2D_ARRAY(_NormalArray16);


    //-----Track Property -----
    float _TrackAO;
    float _TrackEdgeNormals, _TrackEdgeSharpness;
    float _TrackNormalStrenght;
    float _TrackDetailNormalStrenght;
    float _TrackHeightOffset;
    float4 _TrackDetailTexture_ST;
    float4 _Splat0TriplanarBaseMap_ST;
    float _ParallaxTrackAffineSteps;
    float _ParallaxTrackSteps;
    float _TrackHeightTransition;
    float _TrackMultiplyStrenght;

    TEXTURE2D(_TrackDetailTexture);
    TEXTURE2D(_TrackDetailNormalTexture);
    TEXTURE2D(_Splat0TriplanarBaseMap);
    #include "InTerra_LayersProperties.hlsl"

    DECLARE_TERRAIN_LAYER_PROPS(0)
    #ifndef _LAYERS_ONE
        DECLARE_TERRAIN_LAYER_PROPS(1)
        #ifndef _LAYERS_TWO
            DECLARE_TERRAIN_LAYER_PROPS(2)
            DECLARE_TERRAIN_LAYER_PROPS(3)
            #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                DECLARE_TERRAIN_LAYER_PROPS(4)
                DECLARE_TERRAIN_LAYER_PROPS(5)
                DECLARE_TERRAIN_LAYER_PROPS(6)
                DECLARE_TERRAIN_LAYER_PROPS(7)
            #endif
            #ifdef _LAYERS_SIXTEEN
                DECLARE_TERRAIN_LAYER_PROPS(8)
                DECLARE_TERRAIN_LAYER_PROPS(9)
                DECLARE_TERRAIN_LAYER_PROPS(10)
                DECLARE_TERRAIN_LAYER_PROPS(11)

                DECLARE_TERRAIN_LAYER_PROPS(12)
                DECLARE_TERRAIN_LAYER_PROPS(13)
                DECLARE_TERRAIN_LAYER_PROPS(14)
                DECLARE_TERRAIN_LAYER_PROPS(15)
            #endif
        #endif
    #endif


CBUFFER_END

TEXTURE2D(_Control);    SAMPLER(sampler_Control);
TEXTURE2D(_Control1);
TEXTURE2D(_Control2);
TEXTURE2D(_Control3);

SAMPLER(sampler_Splat0);
SAMPLER(sampler_Mask0);
SAMPLER(sampler_Normal0);

TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex); 

TEXTURE2D(_SpecGlossMap);   SAMPLER(sampler_SpecGlossMap);
TEXTURE2D(_MetallicTex);    SAMPLER(sampler_MetallicTex);
TEXTURE2D(_TriplanarTex);   SAMPLER(sampler_TriplanarTex);
TEXTURE2D(_Triplanar_MetallicAO);   SAMPLER(sampler_Triplanar_MetallicAO);

TEXTURE2D(_TerrainColorTintTexture); 
TEXTURE2D(_TerrainNormalTintTexture); SAMPLER(SamplerState_Linear_Repeat);

#ifdef TERRAIN_SPLAT_BASEPASS
    float _InTerra_GlobalWetness;
#endif

#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
#endif

#if defined(UNITY_INSTANCING_ENABLED) || defined(TRIPLANAR) || defined(_PUDDLES)
    TEXTURE2D(_TerrainHeightmapTexture);
    TEXTURE2D(_TerrainNormalmapTexture);
    SAMPLER(sampler_TerrainNormalmapTexture);
#endif

UNITY_INSTANCING_BUFFER_START(Terrain)
UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
UNITY_INSTANCING_BUFFER_END(Terrain)

#ifdef _ALPHATEST_ON
    TEXTURE2D(_TerrainHolesTexture);
    SAMPLER(sampler_TerrainHolesTexture);

    void ClipHoles(float2 uv)
    {
        float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, sampler_TerrainHolesTexture, uv).r;
        clip(hole == 0.0f ? -1 : 1);
    }
#endif

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;
    specGloss = SAMPLE_TEXTURE2D(_MetallicTex, sampler_MetallicTex, uv);
    specGloss.a = albedoAlpha;
    return specGloss;
}

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
    outSurfaceData = (SurfaceData)0;
    half4 albedoSmoothness = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    outSurfaceData.alpha = 1;

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoSmoothness.a);
    outSurfaceData.albedo = albedoSmoothness.rgb;

    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
    outSurfaceData.occlusion = 1;
    outSurfaceData.emission = 0;
}


void TerrainInstancing(inout float4 positionOS, inout float3 normal, inout float2 uv)
{
#ifdef UNITY_INSTANCING_ENABLED
    float2 patchVertex = positionOS.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z;
    float height = UnpackHeightmap(_TerrainHeightmapTexture.Load(int3(sampleCoords, 0)));

    positionOS.xz = sampleCoords * _TerrainHeightmapScale.xz;
    positionOS.y = height * _TerrainHeightmapScale.y;

#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
    normal = float3(0, 1, 0);
#else
    normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb * 2 - 1;
#endif
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;
    #if defined(TRIPLANAR) || defined(_PUDDLES)
        normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb * 2 - 1;
    #endif
#endif


}

void TerrainInstancing(inout float4 positionOS, inout float3 normal)
{
    float2 uv = { 0, 0 };
    TerrainInstancing(positionOS, normal, uv);
}
#endif
