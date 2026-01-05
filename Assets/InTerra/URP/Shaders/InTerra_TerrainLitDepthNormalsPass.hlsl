#ifndef UNIVERSAL_FORWARD_LIT_DEPTH_NORMALS_PASS_INCLUDED
#define UNIVERSAL_FORWARD_LIT_DEPTH_NORMALS_PASS_INCLUDED

#include "InTerra_TerrainLitPasses.hlsl"

// DepthNormal pass
struct AttributesDepthNormal
{
    float4 positionOS : POSITION;
    half3 normalOS : NORMAL;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VaryingsDepthNormal
{
    float4 uvMainAndLM              : TEXCOORD0; // xy: control, zw: lightmap
    #ifndef TERRAIN_SPLAT_BASEPASS
        float4 uvSplat01                : TEXCOORD1; // xy: splat0, zw: splat1
        float4 uvSplat23                : TEXCOORD2; // xy: splat2, zw: splat3
    #endif

    #if (defined(_NORMALMAPS) || defined(_TERRAIN_NORMAL_IN_MASK))  && !defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
        half4 normal                   : TEXCOORD3;    // xyz: normal, w: viewDir.x
        half4 tangent                  : TEXCOORD4;    // xyz: tangent, w: viewDir.y
        half4 bitangent                : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
        half3 positionWS               : TEXCOORD6;
    #else
        half3 normal                   : TEXCOORD3;
    #endif

    float4 clipPos                  : SV_POSITION;
    UNITY_VERTEX_OUTPUT_STEREO
};

VaryingsDepthNormal DepthNormalOnlyVertex(AttributesDepthNormal v)
{
    VaryingsDepthNormal o = (VaryingsDepthNormal)0;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainInstancing(v.positionOS, v.normalOS, v.texcoord);

    const VertexPositionInputs attributes = GetVertexPositionInputs(v.positionOS.xyz);

    o.uvMainAndLM.xy = v.texcoord;
    o.uvMainAndLM.zw = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
    #ifndef TERRAIN_SPLAT_BASEPASS
        o.uvSplat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
        o.uvSplat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
        #ifndef _LAYERS_TWO
            o.uvSplat23.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
            o.uvSplat23.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
        #endif
    #endif

    #if (defined(_NORMALMAPS) || defined(_TERRAIN_NORMAL_IN_MASK)) && !defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
        half3 viewDirWS = GetWorldSpaceNormalizeViewDir(attributes.positionWS);
        float4 vertexTangent = float4(cross(float3(0, 0, 1), v.normalOS), 1.0);
        VertexNormalInputs normalInput = GetVertexNormalInputs(v.normalOS, vertexTangent);

        o.normal = half4(normalInput.normalWS, viewDirWS.x);
        o.tangent = half4(normalInput.tangentWS, viewDirWS.y);
        o.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);    
        o.positionWS = attributes.positionWS;
    #else
        o.normal = TransformObjectToWorldNormal(v.normalOS);
    #endif

    o.clipPos = attributes.positionCS;

    return o;
}

void SampleNorm(float2 uv[_LAYER_COUNT], half4 blendMask[4], inout float3 mixNormal, half4 mask[_LAYER_COUNT])
{
    half3 normal[_LAYER_COUNT];

    #define norm(i,  blendMask)                                                     \
    UNITY_BRANCH if (blendMask > 0)                                                 \
    {                                                                               \
        normal[i] = SampleNormals(i, uv[i]).xyz;                                    \
        mixNormal += normal[i].xyz * blendMask.x;                                   \
    }                                                                               \
    else                                                                            \
    {                                                                               \
        normal[i] = float3(0, 0, 1);                                                \
    }                                                                               \

    norm(0, blendMask[0].r);
    #ifndef _LAYERS_ONE
        norm(1, blendMask[0].g);
        #ifndef _LAYERS_TWO
            norm(2, blendMask[0].b);
            norm(3, blendMask[0].a);
            #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                norm(4, blendMask[1].r);
                norm(5, blendMask[1].g);
                norm(6, blendMask[1].b);
                norm(7, blendMask[1].a);
                #ifdef _LAYERS_SIXTEEN
                    norm(8, blendMask[2].r);
                    norm(9, blendMask[2].g);
                    norm(10, blendMask[2].b);
                    norm(11, blendMask[2].a);

                    norm(12, blendMask[3].r);
                    norm(13, blendMask[3].g);
                    norm(14, blendMask[3].b);
                    norm(15, blendMask[3].a);
                #endif
            #endif
        #endif
    #endif
    #undef norm   
}


void NormalMapMix(float4 uvMainAndLM, float2 uvSplat[_LAYER_COUNT], float3 positionWS, float3 worldNormal, float3 tangentViewDirTerrain, inout half4 blendMask[4], inout half3 mixedNormal)
{
    #if defined(_NORMALMAPS) && defined(_DEPTH_NORMALS_MAPS)
        half3 mixNormal = half(0.0);
         half3 normal[_LAYER_COUNT];
         float2 uv[_LAYER_COUNT];
         half4 mask[_LAYER_COUNT];

        UvSplat(uv, uvSplat, uvMainAndLM.xy);

        #if (defined(_NORMALMAPS) || defined(_TERRAIN_NORMAL_IN_MASK)) && !defined(ENABLE_TERRAIN_PERPIXEL_NORMAL) && defined(PARALLAX)
        UNITY_BRANCH if (_Terrain_Parallax == 1)
        {        
            float lod = _MipMapLevel + smoothstep(_MipMapFade.x, _MipMapFade.y, (distance(positionWS, _WorldSpaceCameraPos)));
            ParallaxUV(uv, tangentViewDirTerrain, blendMask, 1.0f, lod);
        }
        #endif


        SampleMask(mask, uv, blendMask, 1.0f);

       #if defined(_TERRAIN_BLEND_HEIGHT) && !defined(TERRAIN_SPLAT_ADDPASS)
            #ifdef _TERRAIN_BASEMAP_GEN
            if (_NumLayersCount <= 4)
            #endif
            {
                if (_HeightmapBlending == 1)
                {
                    HeightBlend(mask, blendMask, _HeightTransition);
                }
            }
        #endif 

        SampleNorm(uv, blendMask, mixNormal, mask);  

        #if HAS_HALF
            mixNormal.z += half(0.01);
        #else
            mixNormal.z += 1e-5f;
        #endif
 
        mixedNormal = normalize(mixNormal.xyz);       
    #endif       
        
}

#if !defined(UNITY_2021_2_TO_2022_1)
    void DepthNormalOnlyFragment(
        VaryingsDepthNormal IN
        , out half4 outNormalWS : SV_Target0
    #ifdef _WRITE_RENDERING_LAYERS
        , out float4 outRenderingLayers : SV_Target1
    #endif
    )
#else
    half4 DepthNormalOnlyFragment(VaryingsDepthNormal IN) : SV_TARGET
#endif
{
    #ifdef _ALPHATEST_ON
        ClipHoles(IN.uvMainAndLM.xy);
    #endif

    float2 splatUV = (IN.uvMainAndLM.xy * (_Control_TexelSize.zw - 1.0f) + 0.5f) * _Control_TexelSize.xy;
    half4 blendMask[4];
    blendMask[0] = SAMPLE_TEXTURE2D(_Control, sampler_Control, splatUV);
    blendMask[1] = float4(0.0f, 0.0f, 0.0f, 0.0f); 
    blendMask[2] = float4(0.0f, 0.0f, 0.0f, 0.0f);
    blendMask[3] = float4(0.0f, 0.0f, 0.0f, 0.0f);

    float3 tangentViewDir = float3(0.0h, 0.0h, 1.0h);
    float3 worldPos = float3(0.0h, 0.0h, 0.0h);;

    #if (defined(_NORMALMAPS) || defined(_TERRAIN_NORMAL_IN_MASK)) && !defined(ENABLE_TERRAIN_PERPIXEL_NORMAL) && defined(PARALLAX)
        worldPos = IN.positionWS;
        float3x3 objectToTangent = float3x3((IN.tangent.xyz), (cross(IN.normal.xyz, IN.tangent.xyz)) * -1, IN.normal.xyz);
        tangentViewDir = -normalize(mul(objectToTangent, GetWorldSpaceViewDir(worldPos)));
    #endif
    half3 normalTS = half3(0.0h, 0.0h, 1.0h);

    float2 uvSplat[_LAYER_COUNT];

    uvSplat[0] = IN.uvSplat01.xy;
    uvSplat[1] = IN.uvSplat01.zw;
    #ifndef _LAYERS_TWO
        uvSplat[2] = IN.uvSplat23.xy;
        uvSplat[3] = IN.uvSplat23.zw;
    #endif

    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)  
        float2 tc = _WorldMapping ? (IN.positionWS.xz / _TerrainSizeXZPosY.xy) : IN.uvMainAndLM.xy;
        transformAdditionalTex(uvSplat, tc);

        blendMask[1] = SAMPLE_TEXTURE2D(_Control1, sampler_Control, splatUV);
        #if defined(_LAYERS_SIXTEEN)
            blendMask[2] = SAMPLE_TEXTURE2D(_Control2, sampler_Control, splatUV);
            blendMask[3] = SAMPLE_TEXTURE2D(_Control3, sampler_Control, splatUV);
        #endif
    #endif
    NormalMapMix(IN.uvMainAndLM, uvSplat, worldPos, IN.normal.xyz, tangentViewDir, blendMask, normalTS);

    #if (defined(_NORMALMAPS) || defined(_TERRAIN_NORMAL_IN_MASK)) && !defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
        half3 normalWS = TransformTangentToWorld(normalTS, half3x3(-IN.tangent.xyz, IN.bitangent.xyz, IN.normal.xyz));
    #elif defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
        half3 viewDirWS = IN.viewDir;
        float2 sampleCoords = (IN.uvMainAndLM.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
        half3 normalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
        half3 tangentWS = cross(GetObjectToWorldMatrix()._13_23_33, normalWS);
        half3 normalWS = TransformTangentToWorld(normalTS, half3x3(-tangentWS, cross(normalWS, tangentWS), normalWS));
    #else
        half3 normalWS = IN.normal;
    #endif

    normalWS = NormalizeNormalPerPixel(normalWS);
    
    #if !defined(UNITY_2021_2_TO_2022_1)
        outNormalWS = half4(normalWS, 0.0);
        #ifdef _WRITE_RENDERING_LAYERS
            uint renderingLayers = GetMeshRenderingLayer();

            #if defined(UNITY_6000_2)
                outRenderingLayers = EncodeMeshRenderingLayer();
            #else
                outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
            #endif

        #endif
    #else
        return half4(normalWS, 0.0);
    #endif

}
#endif

