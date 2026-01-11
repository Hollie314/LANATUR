#if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
    #if defined (_OBJECT_TRIPLANAR) || defined (_TERRAIN_TRIPLANAR_ONE) || defined (_TERRAIN_TRIPLANAR_ALL)
        #define TRIPLANAR
    #endif
#endif

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

#if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
    #include "InTerra_LayersProperties.hlsl"
#endif

//----- Global Properties -----
float _InTerra_TrackArea;
float3 _InTerra_TrackPosition;
TEXTURE2D(_InTerra_TrackTexture);
float4 _InTerra_TrackTexture_TexelSize;
float _InTerra_TracksLayer;
float _InTerra_TracksFading;
float _InTerra_TracksFadingTime;
float _InTerra_TrackTextureSize;
float _InTerra_TrackLayer;

float _InTerra_GlobalWetness;
float3 _InTerra_GlobalPuddles;
float3 _InTerra_GlobalRaindropRipples;
float4 _InTerra_GlobalRaindropsDistance;

//==========================================================================================
//======================================   FUNCTIONS   =====================================
//==========================================================================================
float2 ObjectFrontUV(float posOffset, half4 splatUV, float offsetZ)
{
    return  float2((posOffset + splatUV.z) / splatUV.x, (offsetZ + splatUV.w) / splatUV.y);
}

float2 ObjectSideUV(float posOffset, half4 splatUV, float offsetX)
{
    return  float2((offsetX + splatUV.z) / splatUV.x, (posOffset + splatUV.w) / splatUV.y);
}

half3 WorldTangent(float3 wTangent, float3 wBTangent, half3 mixedNormal)
{
    mixedNormal.xy = mul(float2x2(wTangent.xz, wBTangent.xz), mixedNormal.xy);
    return  half3(mixedNormal);
}

half2 HeightBlendTwoTextures(float2 splat, float2 heights, half sharpness)
{
    splat *= (1 / (1 * pow(2, heights * (-(sharpness)))) + 1) * 0.5;
    splat /= (splat.r + splat.g);

    return  splat;
}

half3 UnpackNormalGAWithScale(half4 packednormal, float scale)
{
    half3 normal;
    normal.xy = (packednormal.wy * 2 - 1) * scale;
    normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));

    return normal;
}

float random(in float2 st)
{
    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

float2 random2(float2 st) {
    st = float2(dot(st, float2(127.1, 311.7)),
        dot(st, float2(269.5, 183.3)));
    return frac(sin(st) * 43758.5453123);
}

#if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
    #define DiffuseRemap(i) float4(_DiffuseRemapScale##i.xyzw)
#else
    #define DiffuseRemap(i) float4(_DiffuseRemapScale##i.xyzw + _DiffuseRemapOffset##i.xyzw)

    void uvArray(in out float2 uv[_LAYER_COUNT], float4 inUv[_LAYER_COUNT/2])
    {
        uv[0] = inUv[0].xy;
        uv[1] = inUv[0].zw;
        #ifndef _LAYERS_TWO
            uv[2] = inUv[1].xy;
            uv[3] = inUv[1].zw;
            #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)  
                    uv[4] = inUv[2].xy;
                    uv[5] = inUv[2].zw;
                    uv[6] = inUv[3].xy;
                    uv[7] = inUv[3].zw;
                #endif
                #ifdef _LAYERS_SIXTEEN
                    uv[8] = inUv[4].xy;
                    uv[9] = inUv[4].zw;
                    uv[10] = inUv[5].xy;
                    uv[11] = inUv[5].zw;

                    uv[12] = inUv[6].xy;
                    uv[13] = inUv[6].zw;
                    uv[14] = inUv[7].xy;
                    uv[15] = inUv[7].zw;
                #endif
            #endif
        #endif
    }

    void transformAdditionalTex(in out float2 uv[_LAYER_COUNT], float2 tc)
    {
        #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
            uv[4] = TRANSFORM_TEX(tc, _Splat4);
            uv[5] = TRANSFORM_TEX(tc, _Splat5);
            uv[6] = TRANSFORM_TEX(tc, _Splat6);
            uv[7] = TRANSFORM_TEX(tc, _Splat7);

            #ifdef _LAYERS_SIXTEEN
                uv[8] = TRANSFORM_TEX(tc, _Splat8);
                uv[9] = TRANSFORM_TEX(tc, _Splat9);
                uv[10] = TRANSFORM_TEX(tc, _Splat10);
                uv[11] = TRANSFORM_TEX(tc, _Splat11);

                uv[12] = TRANSFORM_TEX(tc, _Splat12);
                uv[13] = TRANSFORM_TEX(tc, _Splat13);
                uv[14] = TRANSFORM_TEX(tc, _Splat14);
                uv[15] = TRANSFORM_TEX(tc, _Splat15);
            #endif
        #endif
    }
#endif

#if defined(INTERRA_MESH_TERRAIN)
    float2 TerrainFrontUV(float3 wPos, half4 splatUV, float2 tc, float3 flip)
    {
        return  float2(tc.x * -flip.z, (wPos.y - _TerrainSizeXZPosY.z) * (splatUV.y / _TerrainSizeXZPosY.y) + splatUV.w);
    }

    float2 TerrainSideUV(float3 wPos, half4 splatUV, float2 tc, float3 flip)
    {
        return  float2(tc.y * flip.x, (wPos.y - _TerrainSizeXZPosY.z) * (splatUV.x / _TerrainSizeXZPosY.x) + splatUV.z);
    }
#endif

void TriplanarOneToAllSteep(in out half4 blendMask[4], float weightY, in out half splatWeight)
{
    if (_TriplanarOneToAllSteep == 1)
    {
        #if !defined(TERRAIN_SPLAT_ADDPASS) 
            blendMask[0] = float4(saturate(blendMask[0].r + weightY), saturate((blendMask[0].gba) - weightY));
            blendMask[1] = float4(saturate((blendMask[1].rgba) - weightY));
            splatWeight = saturate(splatWeight + weightY);

            blendMask[0] = float4(saturate(blendMask[0].r + weightY), saturate(blendMask[0].gba - weightY));
            splatWeight = saturate(splatWeight + weightY);
        #else
            blendMask[0] = float4(saturate(blendMask[0].rgba - weightY));
            splatWeight = saturate(splatWeight - weightY);
        #endif
    }
}

half3 TriplanarNormal(half3 normal, half3 tangent, half3 bTangent, half3 normal_front, half3 normal_side, float3 weights, half3 flipUV)
{
    #ifdef INTERRA_OBJECT
        normal_front.y *= -flipUV.z;
        normal_front.xy = mul(float2x2(tangent.xy, bTangent.xy), normal_front.xy);

        normal_side.x *= -flipUV.x;
        normal_side.xy = mul(float2x2(tangent.yz, bTangent.yz), normal_side.xy);
    #else
        normal_side.xy = normal_side.yx; //this is needed because the uv was rotated
        normal_front.xy *= -flipUV.z;       
        normal_side.x *= -flipUV.x;
        normal_side.y *= flipUV.x;
    #endif

    return half3 (normal+ normal_front + normal_side);
}

float3 ObjectTriplanarNormal(float3 normal, float3 tangent, float3 bitangent, float3 normal_front, float3 normal_side, float3 triplanarWeights, half3 flipUV)
{
    normal_side.x *= flipUV.x;
    normal_side.xy = mul(float2x2(tangent.zy, bitangent.zy), normal_side.xy);

    normal_front.x *= -flipUV.z;
    normal_front.xy = mul(float2x2(tangent.xy, bitangent.xy), normal_front.xy);

    normal = WorldTangent(tangent, bitangent, normal);

    return half3 (normal * triplanarWeights.y + normal_front * triplanarWeights.z + normal_side * triplanarWeights.x);
}


#if defined (PARALLAX)

    #define MipMapLod(i, lod) float(_MipMapLevel + (lod * log2(max(_Mask##i##_TexelSize.z, _Mask##i##_TexelSize.w)) + 1))

    float GetParallaxHeight(texture2D maskT, sampler maskS, float2 uv, float lod, float2 offset, int invert)
    {
        return abs(SAMPLE_TEXTURE2D_LOD(maskT, maskS, float2(uv + offset), lod).b -invert);
    }

    //this function is based on Parallax Occlusion Mapping from Shader Graph URP
    float2 ParallaxOffset(texture2D maskT, sampler maskS, int numSteps, float amplitude, float2 uv, float3 tangentViewDir, float affineSteps, float lod, int invert)
    {    
        float2 offset = 0;

        if (numSteps > 0)
        {
            float3 viewDir = float3(tangentViewDir.xy * amplitude * -0.01, tangentViewDir.z);
            float stepSize = (1.0 / numSteps);

            float2 texOffsetPerStep = stepSize * viewDir.xy;
                

            // Do a first step before the loop to init all value correctly
            float2 texOffsetCurrent = float2(0.0, 0.0); 
            float prevHeight = GetParallaxHeight(maskT, maskS, uv, lod, texOffsetCurrent, invert);
            texOffsetCurrent += texOffsetPerStep;
            float currHeight = GetParallaxHeight(maskT, maskS, uv, lod, texOffsetCurrent, invert);
            float rayHeight = 1.0 - stepSize; // Start at top less one sample

            for (int stepIndex = 0; stepIndex < numSteps; ++stepIndex)
            {
                // Have we found a height below our ray height ? then we have an intersection
                if (currHeight > rayHeight)
                    break; // end the loop

                prevHeight = currHeight;
                rayHeight -= stepSize;
                texOffsetCurrent += texOffsetPerStep;

                currHeight = GetParallaxHeight(maskT, maskS, uv, lod, texOffsetCurrent, invert);
            }

            if (affineSteps <= 1)
            {
                float delta0 = currHeight - rayHeight;
                float delta1 = (rayHeight + stepSize) - prevHeight;
                float ratio = delta0 / (delta0 + delta1);
                offset = texOffsetCurrent - ratio * texOffsetPerStep;

                currHeight = GetParallaxHeight(maskT, maskS, uv, lod, texOffsetCurrent, invert);
            }
            else
            {
                float pt0 = rayHeight + stepSize;
                float pt1 = rayHeight;
                float delta0 = pt0 - prevHeight;
                float delta1 = pt1 - currHeight;

                float delta;

                // Secant method to affine the search
                // Ref: Faster Relief Mapping Using the Secant Method - Eric Risser
                for (int i = 0; i < affineSteps; ++i)
                {
                    // intersectionHeight is the height [0..1] for the intersection between view ray and heightfield line
                    float intersectionHeight = (pt0 * delta1 - pt1 * delta0) / (delta1 - delta0);
                    // Retrieve offset require to find this intersectionHeight
                    offset = (1 - intersectionHeight) * texOffsetPerStep * numSteps;

                    currHeight = GetParallaxHeight(maskT, maskS, uv, lod, offset, invert);

                    delta = intersectionHeight - currHeight;

                    if (abs(delta) <= 0.01)
                        break;

                    // intersectionHeight < currHeight => new lower bounds
                    if (delta < 0.0)
                    {
                        delta1 = delta;
                        pt1 = intersectionHeight;
                    }
                    else
                    {
                        delta0 = delta;
                        pt0 = intersectionHeight;
                    }
                }
            }
        }  
        return offset;
    }

    #if !defined(_TERRAIN_BASEMAP_GEN)
        void ParallaxUV(inout float2 uv[_LAYER_COUNT], float3 tangentViewDir, half4 blendMask[4],float weight, float lod)
        {
            #define uvParallax(i, blendMask)                                \
            UNITY_BRANCH if (blendMask * weight  > 0.01f)                   \
            {                                                               \
                uv[i] += ParallaxOffset(_Mask##i, SamplerState_Linear_Repeat, _DiffuseRemapOffset##i.w,  DiffuseRemap(i).w, uv[i], tangentViewDir, _ParallaxAffineStepsTerrain,  MipMapLod(i, lod), 0);\
            }                                                               \

            UNITY_BRANCH if (_Terrain_Parallax == 1)
            {
                uvParallax(0, blendMask[0].r);
                #ifndef _LAYERS_ONE
                    uvParallax(1, blendMask[0].g);
                    #ifndef _LAYERS_TWO
                        uvParallax(2, blendMask[0].b);
                        uvParallax(3, blendMask[0].a);                   
                        #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                            uvParallax(4, blendMask[1].r);
                            uvParallax(5, blendMask[1].g);
                            uvParallax(6, blendMask[1].b);
                            uvParallax(7, blendMask[1].a);
                        #endif
                        #ifdef _LAYERS_SIXTEEN
                            uvParallax(8, blendMask[2].r);
                            uvParallax(9, blendMask[2].g);
                            uvParallax(10, blendMask[2].b);
                            uvParallax(11, blendMask[2].a);

                            uvParallax(12, blendMask[3].r);
                            uvParallax(13, blendMask[3].g);
                            uvParallax(14, blendMask[3].b);
                            uvParallax(15, blendMask[3].a);
                        #endif
                    #endif
                #endif
            }
        }
    #endif
#endif


    float3 UnpackNormals(float4 packednormal, float normalScale)
    {    
        #ifdef UNITY_NO_DXT5nm
            return UnpackNormalRGB(packednormal, normalScale);
        #else
            return UnpackNormalAG(packednormal, normalScale);
        #endif
    }

    float3 UnpackNormalGAWithScale(float4 packednormal, float scale, half hasMask)
    {
        UNITY_BRANCH if (hasMask > 0)
        {
            return UnpackNormalAG(packednormal, scale);
        }
        else
        {
            return float3(0, 0, 1);
        }
    }
 
    float3 BlendNormals(float3 n1, float3 n2)
    {
        #ifdef INTERRA_OBJECT
            float3 t = n1.xyz + float3(0.0, 0.0, 1.0);
            float3 u = n2.xyz * float3(-1.0, -1.0, 1.0);
            float3 r = (t / t.z) * dot(t, u) - u;
            return r;
        #else
            return (float3(n1.xy + n2.xy, n1.z));
        #endif
    }


    #if defined(_NORMALMAPS) && !defined(_TERRAIN_NORMAL_IN_MASK) 
        #if defined(_LAYERS_SIXTEEN)           
            #define SampleNormals(i, uv) (UnpackNormals(SAMPLE_TEXTURE2D_ARRAY(_NormalArray16, sampler_Splat0, uv, i), _NormalScale##i).xyz)
            #define SampleNormalsGrad(i, uv, ddx, ddy) (UnpackNormals(SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalArray16, sampler_Splat0, uv, i, ddx, ddy), _NormalScale##i).xyz)
        #else
            #define SampleNormals(i, uv) (UnpackNormals(SAMPLE_TEXTURE2D(_Normal##i, sampler_Splat0, uv), _NormalScale##i).xyz)
            #define SampleNormalsGrad(i, uv, ddx, ddy) (UnpackNormals(SAMPLE_TEXTURE2D_GRAD(_Normal##i, sampler_Splat0, uv, ddx, ddy), _NormalScale##i).xyz)
        #endif
        
    #elif defined(_TERRAIN_NORMAL_IN_MASK) 
        #define SampleNormals(i, uv) float3(UnpackNormalGAWithScale(mask[i], _NormalScale##i, _LayerHasMask##i).xyz)
        #define SampleNormalsGrad(i, uv, ddx, ddy) float3(UnpackNormalGAWithScale(mask[i], _NormalScale##i, _LayerHasMask##i).xyz)
    #else
        #define SampleNormals(i, uv) float3(0, 0, 1)
        #define SampleNormalsGrad(i, uv, ddx, ddy) float3(0, 0, 1)
    #endif

    float3 SmoothMaskOrAlbedo(half mask, half albedo, float hasMask, float smoothness)
    {
        UNITY_BRANCH if (hasMask > 0)
        {                                                                               
            albedo = mask;
        }
        else                                                                      
        {                                                                           
            albedo *= smoothness;
        }
        return albedo;
    }

    #ifdef _TERRAIN_MASK_MAPS
        #define Smoothness(i, albedo) SmoothMaskOrAlbedo(mask[i].a, albedo.a, _LayerHasMask##i, _Smoothness##i)
    #else
        #define Smoothness(i, albedo) albedo.a *= _Smoothness##i
    #endif

    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
        #ifdef INTERRA_OBJECT
            #define UV(i) (posOffset.xz + _SplatUV##i.zw  + _SteepDistortion) / _SplatUV##i.xy;
        #else
            #define UV(i) (posOffset.xz + _SplatUV##i.zw) / _SplatUV##i.xy;
        #endif
        #ifdef PARALLAX                                                      
            #define fUV(i) ObjectFrontUV(posOffset.x, _SplatUV##i, offsetZ + (_DiffuseRemapScale##i.w * 0.004 * _SplatUV##i.x) * -flip.z);
            #define sUV(i) ObjectSideUV(posOffset.z, _SplatUV##i, offsetX + (_DiffuseRemapScale##i.w * 0.004 * _SplatUV##i.y) * -flip.x);
        #else   
            #if defined(TESSELLATION_ON)
                #define fUV(i) ObjectFrontUV(posOffset.x, _SplatUV##i, offsetZ + (-_DiffuseRemapOffset##i.y * 0.005 - _TerrainTessOffset) * -flip.z);
                #define sUV(i) ObjectSideUV(posOffset.z, _SplatUV##i, offsetX + (-_DiffuseRemapOffset##i.y * 0.005 - _TerrainTessOffset) * -flip.x);
            #else
                #define fUV(i) ObjectFrontUV(posOffset.x, _SplatUV##i, offsetZ);
                #define sUV(i) ObjectSideUV(posOffset.z, _SplatUV##i, offsetX);    
            #endif
        #endif
    #else
        #define UV(i) uv[i];
        #define fUV(i) TerrainFrontUV(worldPos, _Splat##i##_ST, uvSplat[i], flip);  
        #define sUV(i) TerrainSideUV(worldPos, _Splat##i##_ST, uvSplat[i], flip);

    #endif

    float4 RemapMasks(half4 mask, float4 remapScale, float4 remapOffset)
    {
        #ifdef _TERRAIN_NORMAL_IN_MASK
            mask.rb * remapScale.gb + remapOffset.gb;
            return mask;
        #else
            return mask * remapScale + remapOffset;
        #endif

    }

    #ifdef TERRAIN_MASK

        #if defined(INTERRA_OBJECT)
            #define Mask(i, uv) SAMPLE_TEXTURE2D(_Mask##i, sampler_Splat0, uv)
            #define MaskGrad(i, uv, ddx, ddy) SAMPLE_TEXTURE2D_GRAD(_Mask##i, sampler_Splat0, uv, ddx, ddy)
        #else
                #define Mask(i, uv) SAMPLE_TEXTURE2D(_Mask##i, sampler_Mask0, uv)
            #define MaskGrad(i, uv, ddx, ddy) SAMPLE_TEXTURE2D_GRAD(_Mask##i, sampler_Mask0, uv, ddx, ddy)
        #endif

        #ifdef _TERRAIN_NORMAL_IN_MASK
            #define RemapMask(i, mask) (mask * float4(_MaskMapRemapScale##i.g, 1, _MaskMapRemapScale##i.b, 1)  \
                                        + float4(_MaskMapRemapOffset##i.g, 0, _MaskMapRemapOffset##i.b, 0))
        #else
            #define RemapMask(i, mask) (mask * _MaskMapRemapScale##i + _MaskMapRemapOffset##i)
        #endif
    #else
        #define Mask(i, uv) float4(_Metallic##i, 1, 0.5, 0)
        #define MaskGrad(i, uv, ddx, ddy) float4(_Metallic##i, 1, 0.5, 0)
        #define RemapMask(i, mask) mask
    #endif

    void SampleMask(out half4 mask[_LAYER_COUNT], float2 uv[_LAYER_COUNT], half4 blendMask[4], float weight)
    {
    #define SampleMasks(i, blendMask)                                       \
        UNITY_BRANCH if (blendMask * weight > 1e-5f && _LayerHasMask##i > 0)\
        {                                                                   \
            mask[i] = RemapMask(i,  Mask(i, uv[i]));                        \
        }                                                                   \
        else                                                                \
        {                                                                   \
            mask[i] = float4(_Metallic##i, 1, 0.5, 0);                      \
        }                                                                   \

        SampleMasks(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
            SampleMasks(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                SampleMasks(2, blendMask[0].b);
                SampleMasks(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        SampleMasks(4, blendMask[1].r);
                        SampleMasks(5, blendMask[1].g);
                        SampleMasks(6, blendMask[1].b);
                        SampleMasks(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        SampleMasks(8, blendMask[2].r);
                        SampleMasks(9, blendMask[2].g);
                        SampleMasks(10, blendMask[2].b);
                        SampleMasks(11, blendMask[2].a);

                        SampleMasks(12, blendMask[3].r);
                        SampleMasks(13, blendMask[3].g);
                        SampleMasks(14, blendMask[3].b);
                        SampleMasks(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif
        #undef SampleMasks
    }

    void SampleDistantMask(half4 mask[_LAYER_COUNT], out half4 distMask[_LAYER_COUNT], float2 uvR[_LAYER_COUNT], float2 uvG[_LAYER_COUNT], float2 uv[_LAYER_COUNT], half4 blendMask[4], int mode[_LAYER_COUNT], inout float2 stochasticMask[_LAYER_COUNT], float weight)
    {
           
        float2 sMask;
        float2 dx;
        float2 dy;

        #define SampleDistMask(i, blendMask)                        \
        distMask[i] = float4(_Metallic##i, 1.0f, 0.5f, 0.0f);       \
        sMask = stochasticMask[i];                                  \
        UNITY_BRANCH if (blendMask * weight > 0.0f && _LayerHasMask##i > 0 && mode[i] != 3) \
        {                                                                                   \
            dx = ddx(uv[i]);                                                                \
            dy = ddy(uv[i]);                                                                \
            distMask[i] = sMask.r * RemapMask(i, MaskGrad(i, uvR[i], dx, dy));              \
            UNITY_BRANCH if (mode[i] > 0)                                                   \
            {                                                                               \
                distMask[i] +=  sMask.g * RemapMask(i, MaskGrad(i, uvG[i], dx, dy));        \
            }                                                                               \
        }                                                                                   \
        else                                                        \
        {                                                           \
            distMask[i] = mask[i];                                  \
        }                                                           \

        SampleDistMask(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
            SampleDistMask(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                SampleDistMask(2, blendMask[0].b);
                SampleDistMask(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        SampleDistMask(4, blendMask[1].r);
                        SampleDistMask(5, blendMask[1].g);
                        SampleDistMask(6, blendMask[1].b);
                        SampleDistMask(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        SampleDistMask(8, blendMask[2].r);
                        SampleDistMask(9, blendMask[2].g);
                        SampleDistMask(10, blendMask[2].b);
                        SampleDistMask(11, blendMask[2].a);

                        SampleDistMask(12, blendMask[3].r);
                        SampleDistMask(13, blendMask[3].g);
                        SampleDistMask(14, blendMask[3].b);
                        SampleDistMask(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif

    #undef SampleMaskStoch
    }

    #if defined(_LAYERS_SIXTEEN) && (defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN))     
         #define SampleSplats(i, uv) SAMPLE_TEXTURE2D_ARRAY(_SplatArray16, sampler_Splat0, uv, i)
         #define SampleSplatsGrad(i, uv, ddx, ddy) SAMPLE_TEXTURE2D_ARRAY_GRAD(_SplatArray16, sampler_Splat0, uv, i, ddx, ddy)
    #else
        #define SampleSplats(i, uv) SAMPLE_TEXTURE2D(_Splat##i, sampler_Splat0, uv);
        #define SampleSplatsGrad(i, uv, ddx, ddy) SAMPLE_TEXTURE2D_GRAD(_Splat##i, sampler_Splat0, uv, ddx, ddy)
    #endif

    void SampleSplat(float2 uv[_LAYER_COUNT], half4 blendMask[4], float weight, inout half4 mask[_LAYER_COUNT], out float4 mixAlbedo, out float3 mixNormal)
    {
        float4 albedo[_LAYER_COUNT];
        float3 normal[_LAYER_COUNT];

        mixAlbedo = 0;
        mixNormal = 0;

        #define Samples(i,  blendMask)  blendMask *= weight;                            \
        UNITY_BRANCH if (blendMask > 1e-5f)                                             \
        {                                                                               \
            albedo[i] = SampleSplats(i, uv[i]);                                         \
            albedo[i].rgb *= DiffuseRemap(i).xyz;                                       \
            albedo[i].a = Smoothness(i, albedo[i]).x;                                   \
            normal[i] = SampleNormals(i, uv[i]).xyz;                                    \
            mixAlbedo += albedo[i].rgba * blendMask.x;                                  \
            mixNormal += normal[i].xyz * blendMask.x;                                   \
        }                                                                               \
        else                                                                            \
        {                                                                               \
            albedo[i] = float4(0, 0, 0, 0);                                             \
            normal[i] = float3(0, 0, 1);                                                \
        }                                                                               \


        Samples(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
            Samples(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                Samples(2, blendMask[0].b);
                Samples(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        Samples(4, blendMask[1].r);
                        Samples(5, blendMask[1].g);
                        Samples(6, blendMask[1].b);
                        Samples(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        Samples( 8, blendMask[2].r);
                        Samples( 9, blendMask[2].g);
                        Samples(10, blendMask[2].b);
                        Samples(11, blendMask[2].a);

                        Samples(12, blendMask[3].r);
                        Samples(13, blendMask[3].g);
                        Samples(14, blendMask[3].b);
                        Samples(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif
        #undef Samples       
    }

    void SampleDistantSplat(float2 uvR[_LAYER_COUNT], float2 uvG[_LAYER_COUNT], float2 uv[_LAYER_COUNT], half4 blendMask[4], float2 stochasticMask[_LAYER_COUNT], float weight, inout half4 mask[_LAYER_COUNT], int mode[_LAYER_COUNT], out float4 mixAlbedo, out float3 mixNormal, in out half coverMask)
    { 
        float3 normal[_LAYER_COUNT]; 
        half4 albedo[_LAYER_COUNT];

        half4 albedoR;
        half4 albedoG;

        mixAlbedo = float4(0, 0, 0, 0);
        mixNormal = float3(0, 0, 0);
        float2 sMask = float2(1, 0);

        float2 dx;
        float2 dy;

        #define SampleDistSplat(i,  blendMask)   blendMask *= weight;           \
        normal[i] = float3(0, 0, 1);                                            \
        sMask =   stochasticMask[i];                                            \
        UNITY_BRANCH if (blendMask > 1e-5f && mode[i] != 3 )                    \
        {                                                                       \
            dx = ddx(uv[i]);                                                    \
            dy = ddy(uv[i]);                                                    \
            albedoR = SampleSplatsGrad(i, uvR[i], dx, dy);                      \
            albedoR.rgb *=  DiffuseRemap(i).xyz;                                \
            albedoR.a = Smoothness(i, albedoR).x;                               \
            albedoR *= sMask.r;                                                 \
            albedo[i] = albedoR;                                                \
            normal[i] = SampleNormalsGrad(i, uvR[i], dx, dy).xyz * sMask.r;     \
                                                                                \
            UNITY_BRANCH if (mode[i] > 0)                                       \
            {                                                                   \
                albedoG = SampleSplatsGrad(i, uvG[i], dx, dy);                  \
                albedoG.rgb *=  DiffuseRemap(i).xyz;                            \
                albedoG.a = Smoothness(i, albedoG).x;                           \
                albedoG *= sMask.g;                                             \
                albedo[i] = albedoR + albedoG;                                  \
                normal[i] += SampleNormalsGrad(i, uvG[i], dx, dy) * sMask.g;    \
            }                                                                   \
            coverMask += blendMask;                                             \
            mixAlbedo += albedo[i].rgba * blendMask;                            \
            mixNormal += normal[i].xyz * blendMask;                             \
        }                                                                       \

        mixNormal.z += +1e-5f;

        SampleDistSplat(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
        SampleDistSplat(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                SampleDistSplat(2, blendMask[0].b);
                SampleDistSplat(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        SampleDistSplat(4, blendMask[1].r);
                        SampleDistSplat(5, blendMask[1].g);
                        SampleDistSplat(6, blendMask[1].b);
                        SampleDistSplat(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        SampleDistSplat( 8, blendMask[2].r);
                        SampleDistSplat( 9, blendMask[2].g);
                        SampleDistSplat(10, blendMask[2].b);
                        SampleDistSplat(11, blendMask[2].a);

                        SampleDistSplat(12, blendMask[3].r);
                        SampleDistSplat(13, blendMask[3].g);
                        SampleDistSplat(14, blendMask[3].b);
                        SampleDistSplat(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif
        #undef SampleDistSplat                          
    }

    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN)  
        #ifndef TRIPLANAR
            void UvSplat(out float2 uvSplat[_LAYER_COUNT], float3 posOffset)
        #else
            void UvSplat(out float2 uvSplat[_LAYER_COUNT], out float2 uvFront[_LAYER_COUNT], out float2 uvSide[_LAYER_COUNT], float3 posOffset, float offsetZ, float offsetX, float3 flip)
        #endif
    #else
        #ifndef TRIPLANAR
            void UvSplat(out float2 uvSplat[_LAYER_COUNT], float2 uv[_LAYER_COUNT], float2 splatBaseUV)
        #else
            void UvSplat(out float2 uvSplat[_LAYER_COUNT], out float2 uvFront[_LAYER_COUNT], out float2 uvSide[_LAYER_COUNT], float3 worldPos, float2 uv[_LAYER_COUNT], float2 splatBaseUV, float3 flip)
        #endif
    #endif
    {
        #ifndef TRIPLANAR
            #define SplatUV(i);             \
            uvSplat[i] = UV(i);       
        #else
            #define SplatUV(i)              \
            uvSplat[i] = UV(i);             \
            uvFront[i] = fUV(i);            \
            uvSide[i] = sUV(i);             \

        #endif

        SplatUV(0);
        #ifndef _LAYERS_ONE
            SplatUV(1);
            #ifndef _LAYERS_TWO
                SplatUV(2);
                SplatUV(3);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        SplatUV(4);
                        SplatUV(5);
                        SplatUV(6);
                        SplatUV(7);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        SplatUV(8);
                        SplatUV(9);
                        SplatUV(10);
                        SplatUV(11);

                        SplatUV(12);
                        SplatUV(13);
                        SplatUV(14);
                        SplatUV(15);
                    #endif
                #endif
            #endif    
        #endif
    }

    void ScaleUV(out float2 scaleUV[_LAYER_COUNT], float2 uvSplat[_LAYER_COUNT], int mode[_LAYER_COUNT])
    {
        #define scale(i) scaleUV[i] = mode[i] == 2 ? uvSplat[i] : uvSplat[i]    \
                      * ((_DiffuseRemapOffset##i.x * 1000) + 1) * _HT_distance_scale;    \
        
        scale(0);
        #ifndef _LAYERS_ONE
            scale(1);
            #ifndef _LAYERS_TWO
                scale(2);
                scale(3);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        scale(4);
                        scale(5);
                        scale(6);
                        scale(7);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        scale(8);
                        scale(9);
                        scale(10);
                        scale(11);

                        scale(12);
                        scale(13);
                        scale(14);
                        scale(15);
                    #endif
                #endif
            #endif    
        #endif
    }
     
    void CoverMaskAndMode(half4 blendMask[4], out float coverMask, out int mode[_LAYER_COUNT])
    {
        coverMask = 0;

        #define CoverAndMode(i, blendMask)                  \
        mode[i] =  _DiffuseRemapOffset##i.y * 1000.0f;      \
        UNITY_BRANCH if (blendMask > 1e-5f && mode[i] != 3) \
        {coverMask += blendMask;}    

        coverMask = saturate(coverMask);
        
        CoverAndMode(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
            CoverAndMode(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                CoverAndMode(2, blendMask[0].b);
                CoverAndMode(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        CoverAndMode(4, blendMask[1].r);
                        CoverAndMode(5, blendMask[1].g);
                        CoverAndMode(6, blendMask[1].b);
                        CoverAndMode(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        CoverAndMode(8, blendMask[2].r);
                        CoverAndMode(9, blendMask[2].g);
                        CoverAndMode(10, blendMask[2].b);
                        CoverAndMode(11, blendMask[2].a);

                        CoverAndMode(12, blendMask[3].r);
                        CoverAndMode(13, blendMask[3].g);
                        CoverAndMode(14, blendMask[3].b);
                        CoverAndMode(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif
    }

    void StochasticMaskAndUV(int mode[_LAYER_COUNT], float2 uvSplat[_LAYER_COUNT], out float2 StochasticMask[_LAYER_COUNT], out float2 uvR[_LAYER_COUNT], out float2 uvG[_LAYER_COUNT])
    {
        float2 sMask;

        float maskR, maskG;
        float2 uvFracR, uvFracG;
               
        #define uvStoch(i)                                          \
        uvR[i] = uvSplat[i];                                        \
        sMask = mode[i] == 3 ? float2(0, 0) : float2(1, 0);         \
        uvFracR = frac(uvSplat[i]);                                 \
        maskR = min(min(uvFracR.x, 1.0 - uvFracR.x),                \
                    min(uvFracR.y, 1.0 - uvFracR.y));               \
        uvFracG = frac(uvSplat[i] + 0.5);                           \
        maskG = min(min(uvFracG.x, 1.0 - uvFracG.x),                \
                    min(uvFracG.y, 1.0 - uvFracG.y));               \
        UNITY_BRANCH if (mode[i] > 0 && mode[i] <= 2)               \
        {                                                           \
            uvR[i] = uvSplat[i] + random2(floor(uvSplat[i]));       \
            uvG[i] = uvSplat[i] + random2(floor(uvSplat[i] + 0.5)); \
            sMask = StochasticMask[i];                              \
            sMask = float2(maskR + 3.0, maskG + 3.0);               \
            sMask = pow(sMask, float2(65, 65));                     \
            sMask = sMask / (sMask.r + sMask.g);                    \
        }                                                           \
        StochasticMask[i] = sMask;                                              

        uvStoch(0);
        #ifndef _LAYERS_ONE
            uvStoch(1);
            #ifndef _LAYERS_TWO
                uvStoch(2);
                uvStoch(3);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        uvStoch(4);
                        uvStoch(5);
                        uvStoch(6);
                        uvStoch(7);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        uvStoch(8);
                        uvStoch(9);
                        uvStoch(10);
                        uvStoch(11);

                        uvStoch(12);
                        uvStoch(13);
                        uvStoch(14);
                        uvStoch(15);
                    #endif
                #endif
            #endif    
        #endif
    }


    void MaskWeight(inout half4 mask[_LAYER_COUNT], half4 mask_front[_LAYER_COUNT], half4 mask_side[_LAYER_COUNT], half4 blendMask[4], inout float3 triplanarWeights)
    {
        float splatWeight[_LAYER_COUNT];
        float3 heights = 0;

        splatWeight[0] = blendMask[0].x;
        #ifndef _LAYERS_ONE
            splatWeight[1] = blendMask[0].y;
            #ifndef _LAYERS_TWO
                splatWeight[2] = blendMask[0].z;
                splatWeight[3] = blendMask[0].w;
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        splatWeight[4] = blendMask[1].r;
                        splatWeight[5] = blendMask[1].g;
                        splatWeight[6] = blendMask[1].b;
                        splatWeight[7] = blendMask[1].a;
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        splatWeight[8] = blendMask[2].r;
                        splatWeight[9] = blendMask[2].g;
                        splatWeight[10] = blendMask[2].b;
                        splatWeight[11] = blendMask[2].a;

                        splatWeight[12] = blendMask[3].r;
                        splatWeight[13] = blendMask[3].g;
                        splatWeight[14] = blendMask[3].b;
                        splatWeight[15] = blendMask[3].a;
                    #endif
                #endif
            #endif
        #endif

        #if !defined(_TERRAIN_BASEMAP_GEN)
            for (int i = 0; i < _LAYER_COUNT; ++i)
            {
                mask[i] = (mask[i] * triplanarWeights.y) + (mask_front[i] * triplanarWeights.z) + (mask_side[i] * triplanarWeights.x);
            }
        #endif
    }

    half4 MaskSplatWeight(half4 mask[_LAYER_COUNT],  half4 blendMask[4], out half4 mixedMask)
    {
        float splatWeight[_LAYER_COUNT];
        mixedMask = 0;

        #ifdef _TERRAIN_NORMAL_IN_MASK
            #define MixdMask(i,  blendMask) float4(_Metallic##i, mask[i].r, mask[i].b, 0.0f) * blendMask
        #elif defined(_TERRAIN_MASK_HEIGHTMAP_ONLY)
            #define MixdMask(i,  blendMask) float4(_Metallic##i, 1.0f, mask[i].b, 0.0f) * blendMask
        #else
            #define MixdMask(i,  blendMask) mask[i] * blendMask
        #endif


        #define MixdMasks(i,  blendMask)                        \
        UNITY_BRANCH if (blendMask > 0)                         \
        {                                                       \
            mixedMask += MixdMask(i, blendMask);                \
        }

        MixdMasks(0, blendMask[0].r);
        #ifndef _LAYERS_ONE
            MixdMasks(1, blendMask[0].g);
            #ifndef _LAYERS_TWO
                MixdMasks(2, blendMask[0].b);
                MixdMasks(3, blendMask[0].a);
                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        MixdMasks(4, blendMask[1].r);
                        MixdMasks(5, blendMask[1].g);
                        MixdMasks(6, blendMask[1].b);
                        MixdMasks(7, blendMask[1].a);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        MixdMasks( 8, blendMask[2].r);
                        MixdMasks( 9, blendMask[2].g);
                        MixdMasks(10, blendMask[2].b);
                        MixdMasks(11, blendMask[2].a);

                        MixdMasks(12, blendMask[3].r);
                        MixdMasks(13, blendMask[3].g);
                        MixdMasks(14, blendMask[3].b);
                        MixdMasks(15, blendMask[3].a);
                    #endif
                #endif
            #endif
        #endif

        return mixedMask;
    }

    float HeightSum(half4 mask[_LAYER_COUNT], half4 blendMask[4])
    {   
        #ifdef _LAYERS_ONE
            return float(mask[0].b);
        #else
            float heightSum = dot(blendMask[0].rg, float2(mask[0].b, mask[1].b));
            #ifndef _LAYERS_TWO
                heightSum += dot(blendMask[0].ba, float2(mask[2].b, mask[3].b));
                #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                    heightSum += dot(blendMask[1], float4(mask[4].b, mask[5].b, mask[6].b, mask[7].b));
                    #ifdef _LAYERS_SIXTEEN
                        heightSum += dot(blendMask[2], float4(mask[8].b, mask[9].b, mask[10].b, mask[11].b));
                        heightSum += dot(blendMask[3], float4(mask[12].b, mask[13].b, mask[14].b, mask[15].b));
                    #endif
                #endif
            #endif
            return heightSum;
        #endif
    }

    #ifdef _TERRAIN_BLEND_HEIGHT
        void HeightBlend(half4 mask[_LAYER_COUNT], inout half4 blendMask[4], float sharpness)
        {
            #ifdef _LAYERS_TWO
                float2 height = float2(mask[0].b, mask[1].b);

                blendMask[0].rg *= (1 / (pow(2, (height + blendMask[0].rg) * (-(sharpness)))) + 1) * 0.5;
                blendMask[0].rg /= (blendMask[0].r + blendMask[0].g);
            #else
                float4 height = float4 (mask[0].b, mask[1].b, mask[2].b, mask[3].b);
                blendMask[0].rgba *= (1 / (pow(2, (height + blendMask[0].rgba) * (-(sharpness)))) + 1) * 0.5;
                float heightSum = blendMask[0].r + blendMask[0].g + blendMask[0].b + blendMask[0].a;

                #if !(defined(_TERRAIN_BASEMAP_GEN) || defined(TERRAIN_SPLAT_ADDPASS))
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        float4 height1 = float4 (mask[4].b, mask[5].b, mask[6].b, mask[7].b);
                        blendMask[1].rgba *= (1 / (pow(2, (height1 + blendMask[1].rgba) * (-(sharpness)))) + 1) * 0.5;
                        heightSum += blendMask[1].r + blendMask[1].g + blendMask[1].b + blendMask[1].a;
                    
                        #ifdef _LAYERS_SIXTEEN
                            float4 height2 = float4 (mask[8].b, mask[9].b, mask[10].b, mask[11].b);
                            blendMask[2].rgba *= (1 / (pow(2, (height2 + blendMask[2].rgba) * (-(sharpness)))) + 1) * 0.5;
                            heightSum += blendMask[2].r + blendMask[2].g + blendMask[2].b + blendMask[2].a;
                        

                            float4 height3 = float4 (mask[12].b, mask[13].b, mask[14].b, mask[15].b);
                            blendMask[3].rgba *= (1 / (pow(2, (height3 + blendMask[3].rgba) * (-(sharpness)))) + 1) * 0.5;
                            heightSum += blendMask[3].r + blendMask[3].g + blendMask[3].b + blendMask[3].a;

                            blendMask[2].rgba /= heightSum;
                            blendMask[3].rgba /= heightSum; 
                        #endif
                        blendMask[1].rgba /= heightSum;
                    #endif
                #endif

                blendMask[0].rgba /= heightSum;
            #endif
        }
    #endif

    #if !defined(_TERRAIN_BASEMAP_GEN)
    float4 TrackSplatValues(half4 blendMask[4], float4 trackSplats[_LAYER_COUNT])
    {   
       #ifdef _LAYERS_ONE
            return trackSplats[0];
        #else
            float4 color = (blendMask[0].r * trackSplats[0])
                         + (blendMask[0].g * trackSplats[1]);
            #ifndef _LAYERS_TWO
                    color += (blendMask[0].b * trackSplats[2])
                           + (blendMask[0].a * trackSplats[3]);

                #if !defined(TERRAIN_SPLAT_ADDPASS)
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        color += (blendMask[1].r * trackSplats[4])
                               + (blendMask[1].g * trackSplats[5])
                               + (blendMask[1].b * trackSplats[6])
                               + (blendMask[1].a * trackSplats[7]);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        color += (blendMask[2].r * trackSplats[8])
                               + (blendMask[2].g * trackSplats[9])
                               + (blendMask[2].b * trackSplats[10])
                               + (blendMask[2].a * trackSplats[11])
                               + (blendMask[3].r * trackSplats[12])
                               + (blendMask[3].g * trackSplats[13])
                               + (blendMask[3].b * trackSplats[14])
                               + (blendMask[3].a * trackSplats[15]);
                    #endif
                #endif
            #endif
            return color;
        #endif
    }


    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
        #define SpecularValueR(i) _Specular##i.r;
        #define SpecularValueG(i) _Specular##i.g;
        #define SpecularValueB(i) _Specular##i.b;
    #else
        #define SpecularValueR(i) _Gamma ? _Specular##i.r : pow(abs(_Specular##i.r),1/2.2f);
        #define SpecularValueG(i) _Gamma ? _Specular##i.g : pow(abs(_Specular##i.g),1/2.2f);
        #define SpecularValueB(i) _Gamma ? _Specular##i.b : pow(abs(_Specular##i.b),1/2.2f);
    #endif


    void UnpackTrackSplatValues(out float4 trackSplats[_LAYER_COUNT])
    {
        float value;
        int precision = 1024;
       
        #define trackSplat(i)  value =  SpecularValueR(i);                  \
        trackSplats[i].z  = value % precision;                              \
                            value = floor(value / precision);               \
        trackSplats[i].x  = value;                                          \
        trackSplats[i] /= (precision - 1);                                  \
                                                                            \
        trackSplats[i].y = (_DiffuseRemapOffset##i.w * 10.0f) % 1 ;         \
        trackSplats[i].w = floor((_DiffuseRemapOffset##i.w % 1 ) * 10.0f);  \
        
        trackSplat(0);
        #ifndef _LAYERS_ONE
            trackSplat(1);
            #ifndef _LAYERS_TWO
                trackSplat(2);
                trackSplat(3);
                #if !defined(TERRAIN_SPLAT_ADDPASS)
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        trackSplat(4);
                        trackSplat(5);
                        trackSplat(6);
                        trackSplat(7);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        trackSplat(8);
                        trackSplat(9);
                        trackSplat(10);
                        trackSplat(11);

                        trackSplat(12);
                        trackSplat(13);
                        trackSplat(14);
                        trackSplat(15);
                    #endif
                #endif
            #endif    
        #endif           
    }

    void UnpackTrackSplatColor(out float4 trackSplatsColor[_LAYER_COUNT])
    {
        float color;
        float value;
        int precision = 1024;

        #define trackSplatColor(i)  color = SpecularValueG(i)       \
                                                                    \
        trackSplatsColor[i].y = color % precision;                  \
        color = floor(color / precision);                           \
        trackSplatsColor[i].x = color;                              \
        value = SpecularValueB(i);                                  \
        trackSplatsColor[i].w  =   value % precision;               \
        value = floor(value / precision);                           \
        trackSplatsColor[i].z = value % precision;                  \
        trackSplatsColor[i] /= (precision - 1);                     \
        
        trackSplatColor(0);
        #ifndef _LAYERS_ONE
            trackSplatColor(1);
            #ifndef _LAYERS_TWO
                trackSplatColor(2);
                trackSplatColor(3);
                #if !defined(TERRAIN_SPLAT_ADDPASS)
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        trackSplatColor(4);
                        trackSplatColor(5);
                        trackSplatColor(6);
                        trackSplatColor(7);
                    #endif
                    #ifdef _LAYERS_SIXTEEN
                        trackSplatColor(8);
                        trackSplatColor(9);
                        trackSplatColor(10);
                        trackSplatColor(11);

                        trackSplatColor(12);
                        trackSplatColor(13);
                        trackSplatColor(14);
                        trackSplatColor(15);
                    #endif
                #endif
            #endif    
        #endif
    }

    void SampleSplatTOL(in out half4 mixedAlbedo, in out half3 mixedNormal, float2 uv[_LAYER_COUNT], half4 blendMask[4], float weight, half4 mask[_LAYER_COUNT])
    {
        float4 albedo[1];
        float3 normal[1];

            albedo[0] = float4(0, 0, 0, 0);
            normal[0] = float3(0, 0, 1);

            blendMask[0].r *= weight;

        #ifndef TERRAIN_SPLAT_ADDPASS
            UNITY_BRANCH if (blendMask[0].r > 1e-5f)
            {          
                albedo[0] = SAMPLE_TEXTURE2D(_Splat0, sampler_Splat0, uv[0]);
                albedo[0].rgb *= DiffuseRemap(0).rgb;
                albedo[0].a = Smoothness(0, albedo[0]).x;
                normal[0] = SampleNormals(0, uv[0]);
            }

            mixedAlbedo = (albedo[0] * blendMask[0].r);
            mixedNormal = (normal[0] * blendMask[0].r);
        #else
                mixedAlbedo = 0.0f;
                mixedNormal = 0.0f;
        #endif
    }

    void SampleDistantSplatTOL(in out half4 mixedAlbedo, in out half3 mixedNormal, float2 uvR[_LAYER_COUNT], float2 uvG[_LAYER_COUNT], half4 blendMask[4], float2 stochasticMask[_LAYER_COUNT], float weight, half4 mask[_LAYER_COUNT], int mode[_LAYER_COUNT], in out half coverMask)
    {
        float4 albedo[1];
        float3 normal[1];

            albedo[0] = float4(0, 0, 0, 0);
            normal[0] = float3(0, 0, 1);

            float4 albedoR;
            float4 albedoG;

            float2 sMask = stochasticMask[0];
            blendMask[0].r *= weight;

        #ifndef TERRAIN_SPLAT_ADDPASS
            UNITY_BRANCH if (blendMask[0].r > 1e-5f && mode[0] != 3)
            {
                albedoR = SAMPLE_TEXTURE2D(_Splat0, sampler_Splat0, uvR[0]);
                albedoR.rgb *= DiffuseRemap(0).xyz;
                albedoR.a = Smoothness(0, albedoR).x;
                albedoR *= sMask.r;
                albedo[0] = albedoR;
                normal[0] = sMask.r * SampleNormals(0, uvR[0]);

                if (mode[0] > 0)
                {
                    albedoG = SAMPLE_TEXTURE2D(_Splat0, sampler_Splat0, uvG[0]);
                    albedoG.rgb *= DiffuseRemap(0).xyz;
                    albedoG.a = Smoothness(0, albedoG).x;
                    albedoG *= sMask.g;
                    albedo[0] = albedoR + albedoG;
                    normal[0] += sMask.g * SampleNormals(0, uvG[0]);
                }

                coverMask += blendMask[0].r;
            }

            mixedAlbedo = (albedo[0] * blendMask[0].r);
            mixedNormal = (normal[0]* blendMask[0].r);
        #else
                mixedAlbedo = 0.0f;
                mixedNormal = 0.0f;
        #endif
    }

    void SampleMaskTOL(out half4 mask[_LAYER_COUNT], half4 noTriplanarMask[_LAYER_COUNT], float2 uv[_LAYER_COUNT], float weight)
    {
        #ifndef TERRAIN_SPLAT_ADDPASS
        mask[0] = float4(_Metallic0, 1, 0.5, 0);

        UNITY_BRANCH if (weight > 1e-5f && _LayerHasMask0 > 0)
        {
            mask[0] = RemapMask(0, Mask(0, uv[0]));
        }

        #else
            mask[0] = noTriplanarMask[0];
        #endif                     
        mask[1] = noTriplanarMask[1];
        #ifndef _LAYERS_TWO
            mask[2] = noTriplanarMask[2];            
            mask[3] = noTriplanarMask[3];
            #if !defined(TERRAIN_SPLAT_ADDPASS)
                #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                    mask[4] = noTriplanarMask[4];
                    mask[5] = noTriplanarMask[5];
                    mask[6] = noTriplanarMask[6];
                    mask[7] = noTriplanarMask[7];
                #endif
                #ifdef _LAYERS_SIXTEEN
                    mask[8] = noTriplanarMask[8];
                    mask[9] = noTriplanarMask[9];
                    mask[10] = noTriplanarMask[10];
                    mask[11] = noTriplanarMask[11];

                    mask[12] = noTriplanarMask[12];
                    mask[13] = noTriplanarMask[13];
                    mask[14] = noTriplanarMask[14];
                    mask[15] = noTriplanarMask[15];
                #endif
            #endif
        #endif
    }

    void SampleDistantMaskTOL(out half4 mask[_LAYER_COUNT], half4 noTriplanarMask[_LAYER_COUNT], float2 uvR[_LAYER_COUNT], float2 uvG[_LAYER_COUNT], float weight, int mode[_LAYER_COUNT], float2 stochasticMask[_LAYER_COUNT])
    {

        #ifndef TERRAIN_SPLAT_ADDPASS
        mask[0] = float4(_Metallic0, 1.0f, 0.5f, 0.0f);

        UNITY_BRANCH if (weight > 1e-5f && mode[0] != 3 && _LayerHasMask0 > 0)
        {
            mask[0] = stochasticMask[0].r * RemapMask(0, Mask(0, uvR[0]));
            if (mode[0] > 0)
            {
                mask[0] += stochasticMask[0].g * RemapMask(0, Mask(0, uvG[0]));
            }
        }

        #else
            mask[0] = noTriplanarMask[0];
        #endif                     
        mask[1] = noTriplanarMask[1];
        #ifndef _LAYERS_TWO
            mask[2] = noTriplanarMask[2];            
            mask[3] = noTriplanarMask[3];
            #if !defined(TERRAIN_SPLAT_ADDPASS)
                #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                    mask[4] = noTriplanarMask[4];
                    mask[5] = noTriplanarMask[5];
                    mask[6] = noTriplanarMask[6];
                    mask[7] = noTriplanarMask[7];
                #endif
                #ifdef _LAYERS_SIXTEEN
                    mask[8] = noTriplanarMask[8];
                    mask[9] = noTriplanarMask[9];
                    mask[10] = noTriplanarMask[10];
                    mask[11] = noTriplanarMask[11];

                    mask[12] = noTriplanarMask[12];
                    mask[13] = noTriplanarMask[13];
                    mask[14] = noTriplanarMask[14];
                    mask[15] = noTriplanarMask[15];
                #endif
            #endif
        #endif
    }
           
    #define TAU 6.283185307

    float2 rotate_2d(float2 p_input, float p_theta)
    {
        float2x2 l_rot_matrix = float2x2(cos(p_theta), -sin(p_theta),
            sin(p_theta), cos(p_theta));
        return mul(l_rot_matrix, p_input);
    }

    float3 RainRipples(float2 uv, float scale, float rotation)
    {
        float3 normal = float3(0, 0, 1);
        uv.xy = rotate_2d(uv.xy, rotation);
        float2 sUV = (uv.xy * scale + scale * 0.1f);
        sUV.x += step(1.0f, (sUV.y % 2.0)) * 0.5f;

        float2 center = float2(0.5f, 0.5f);
        float size = 0.25f;

        float2 tile = floor(sUV);
        float2 fract = frac(sUV);

        float2 offset = (center - fract.xy) * size;
        float2 cUV = (fract - 0.5) / size + offset;

        float2 polarUV = float2(length(cUV), atan2(cUV.y, cUV.x));
        float time = _Time.x * scale * 3.0f;
        float radius = frac(time * 0.9f + (random(floor(sUV) + 5000.0).x)) * 1.25f;

        if (radius < 0.8f)
        {
            float thickness = min((size * 0.5f + 0.25f) * 0.4f, radius);

            float start = radius + thickness;
            float end = max(0., radius - thickness);
            if (radius > 0.15)
            {
                thickness *= (0.9 - (radius * 0.9f));
            }

            float radius2 = radius - thickness * 1.25f;
            float cAngle = smoothstep(start, end, polarUV.x) * PI;

            float start2 = radius2 + thickness;
            float end2 = max(0., radius2 - thickness);
            float cAngle2 = smoothstep(start2, end2, polarUV.x) * PI;

            float rippleMask = smoothstep(thickness, -0.1f, abs(polarUV.x - radius));
            float rippleMask2 = smoothstep(thickness, -0.1f, abs(polarUV.x - radius2));

            float decayMask = min(1., max(0., 1. - polarUV.x));
            cAngle = lerp(cAngle, PI * 0.5f, (1.0f - rippleMask * decayMask));
            cAngle2 = lerp(cAngle2, PI * 0.5f, (1.0f - rippleMask2 * decayMask));

            float c = lerp(cos(cAngle), cos(cAngle2), 0.5f);
            normal = float3(c * sin(polarUV.y), c * cos(polarUV.y), sin(cAngle));

            float opacity = _InTerra_GlobalRaindropRipples.y;
            normal.xy = rotate_2d(normal.xy, rotation) * (opacity - (radius * opacity));
        }
        return normal;
    }
#endif
