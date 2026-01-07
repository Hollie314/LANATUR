#ifdef INTERRA_OBJECT
    void ObjectIntegration_float(float heightOffset, float3 tangentViewDirTerrain, float3 worldViewDir, float3 worldNormal, float3 worldTangent, float3 worldBitangent, float2 detailUV, float3 worldPos, float4 terrainNormals, float4 objectAlbedo, float3 objectNormal, float4 objectMask, float3 objectEmission, out float3 albedo, out float3 mixedNormal, out float smoothness, out float metallic, out float occlusion, out float3 emission)
#else
    #ifdef INTERRA_MESH_TERRAIN
        #ifndef TESSELLATION_ON
            void SplatmapMix_float(float3 tangentViewDirTerrain,  float3 worldNormal, float3 worldPos,float3 worldTangent, float3 worldBitangent, out half3 mixedAlbedo, out half3 mixedNormal, out half smoothness, out half metallic, out half occlusion)
        #else
            void SplatmapMix_float(float3 worldNormal, float3 worldPos, float3 worldTangent, float3 worldBitangent, out half3 mixedAlbedo, out half3 mixedNormal, out half smoothness, out half metallic, out half occlusion)
        #endif
    #else
        #ifndef TESSELLATION_ON
            #include "InTerra_Functions.hlsl"
        #endif
        void SplatmapMix(float2 splatBaseUV, float3 worldNormal, float3 tangentViewDirTerrain, float3 worldPos, out float3 mixedAlbedo, out float smoothness, out float metallic, out float occlusion, inout float3 mixedNormal)
    #endif
#endif
{ 
    float4 mixedDiffuse;
    mixedNormal = float3(0, 0, 1);
    #include "InTerra_SplatMapControl.hlsl"
    
    #ifdef INTERRA_OBJECT	
        if (intersection < 1e-5f)
        {
            blendMask[0] = 0.0f;
            blendMask[1] = 0.0f;
            blendMask[2] = 0.0f;
            blendMask[3] = 0.0f;
            #if defined(_TERRAIN_DISTANCEBLEND)
                dBlendMask[0] = 0.0f;
                dBlendMask[1] = 0.0f;
                dBlendMask[2] = 0.0f;
                dBlendMask[3] = 0.0f;
            #endif 
        }       
    #endif

    float  hTransition = _HeightTransition;
    #if defined(_TERRAIN_BLEND_HEIGHT) && !defined(_LAYERS_ONE) && !defined(TERRAIN_SPLAT_ADDPASS)         
        if (_HeightmapBlending == 1)
        {           
            #ifdef _TERRAIN_DISTANCEBLEND
                hTransition = lerp(_HeightTransition, _Distance_HeightTransition, distBlend);
            #endif
        }
    #endif
      
    //====================================================================================
    //-----------------------------------  MASK MAPS  ------------------------------------
    //====================================================================================  
    #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)        
        #ifdef _TERRAIN_TRIPLANAR_ONE
            SampleMask(mask, uvSplat, blendMask, triplanarWeights.y + (1 - blendMask[0].r));
            SampleMaskTOL(mask_front, mask, uvSplat_front, triplanarWeights.z);
            SampleMaskTOL(mask_side, mask, uvSplat_side, triplanarWeights.x);
        #else
            SampleMask(mask, uvSplat, blendMask, triplanarWeights.y);
            SampleMask(mask_front, uvSplat_front, blendMask, triplanarWeights.z);
            SampleMask(mask_side, uvSplat_side, blendMask, triplanarWeights.x);
        #endif 
        MaskWeight(mask, mask_front, mask_side, blendMask, triplanarWeights, hTransition);
    #else
        SampleMask(mask, uvSplat, blendMask, 1.0f);
    #endif
    #ifdef _TERRAIN_DISTANCEBLEND		
        #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)          
            #ifdef _TERRAIN_TRIPLANAR_ONE 
                float weightTop = saturate(dTriplanarWeights.y + (1 - dBlendMask[0].r));
                float weightFront = dBlendMask[0].r * dTriplanarWeights.z;
                float weightSide = dBlendMask[0].r * dTriplanarWeights.x;

                SampleDistantMask(mask, dMask, uvStochR, uvStochG, uvScaled, dBlendMask, dMode, stochMask, weightTop);
                SampleDistantMaskTOL(dMask_front, dMask, uvStochR_front, uvStochG_front, uvScaled_front, weightFront, dMode, stochMask_front);
                SampleDistantMaskTOL(dMask_side, dMask, uvStochR_side, uvStochG_side, uvScaled_side, weightSide, dMode, stochMask_side);
            #else
                SampleDistantMask(mask, dMask, uvStochR, uvStochG, uvScaled, dBlendMask, dMode, stochMask, triplanarWeights.y);
                SampleDistantMask(mask_front, dMask_front, uvStochR_front, uvStochG_front, uvScaled_front, dBlendMask, dMode, stochMask_front, triplanarWeights.z);
                SampleDistantMask(mask_side, dMask_side, uvStochR_side, uvStochG_side, uvScaled_side, dBlendMask, dMode, stochMask_side, triplanarWeights.x);
            #endif
            MaskWeight(dMask, dMask_front, dMask_side, dBlendMask, dTriplanarWeights, _Distance_HeightTransition);
        #else
            SampleDistantMask(mask, dMask, uvStochR, uvStochG, uvScaled, dBlendMask, dMode, stochMask, 1.0f);
        #endif       
    #endif    
   
    //========================================================================================
    //------------------------------ HEIGHT MAP SPLAT BLENDINGS ------------------------------
    //========================================================================================
    #if defined(_TERRAIN_BLEND_HEIGHT) && !defined(_LAYERS_ONE) && !defined(TERRAIN_SPLAT_ADDPASS)        
        if (_HeightmapBlending == 1)
        {
            blendMask = origBlendMask;
            HeightBlend(mask, blendMask, hTransition);         
            #ifdef _TERRAIN_DISTANCEBLEND
                dBlendMask = dOrigBlendMask;
                HeightBlend(dMask, dBlendMask, _Distance_HeightTransition);
            #endif            
        }
    #endif
        
    MaskSplatWeight(mask, blendMask, mixedMask);
    #ifdef _TERRAIN_DISTANCEBLEND       
        MaskSplatWeight(dMask, dBlendMask, dMixedMask);
        dMixedMask = lerp(mixedMask, dMixedMask, _HT_cover * coverMask);
        mixedMask = lerp(mixedMask, dMixedMask, distBlend);
    #endif

    occlusion = mixedMask.g;
    metallic = mixedMask.r;
    heightSum = mixedMask.b;
    
    #if defined(_TERRAIN_BLEND_HEIGHT) && !defined(_LAYERS_ONE) && !defined(TERRAIN_SPLAT_ADDPASS)
        if (_HeightmapBlending == 1)
        {   
            #ifdef _TERRAIN_DISTANCEBLEND
                if (sampleDistMask)
                {
                    dBlendMask[0] = 0;
                    #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                        dBlendMask[1] = 0;
                        #ifdef _LAYERS_SIXTEEN
                            dBlendMask[2] = 0;
                            dBlendMask[3] = 0;
                        #endif
                    #endif
                }
                if(sampleDistMaskFullCover)
                {
                   blendMask[0] = 0;
                   #ifdef _LAYERS_EIGHT
                    blendMask[1] = 0;
                   #endif
                    #ifdef _LAYERS_SIXTEEN
                        blendMask[2] = 0;
                        blendMask[3] = 0;
                    #endif
                }
            #endif
        }
    #endif

    //-------------------- HEIGHTMAP OBJECT INTERSECTION ----------------
    #ifdef INTERRA_OBJECT
        if (_BaseTexturesTriplanar)
        {
            float4 objectMaskSide = SAMPLE_TEXTURE2D(_MaskMap, sampler_BaseColorMap, objectSideUV) * baseTriplanarWeights.x;
            float4 objectMaskFront = SAMPLE_TEXTURE2D(_MaskMap, sampler_BaseColorMap, objectFrontUV) * baseTriplanarWeights.z;
            objectMask = objectMask * baseTriplanarWeights.y + objectMaskSide + objectMaskFront;
        }

        objectMask.rgba = objectMask.rgba * _MaskMapRemapScale.rgba + _MaskMapRemapOffset.rgba;
        half height = objectMask.b;

        half terrainHeith = 0.5f;
        if (_HeightmapBlending == 1)
        {
            terrainHeith = lerp(heightSum, 1, intersection);
        }

        float2 heightIntersect = (1 / (pow(2, float2(((1 - intersection) * height), (intersection * terrainHeith)) * (-(_Sharpness)))) + 1) * 0.5;
        heightIntersect /= (heightIntersect.r + heightIntersect.g);

        heightSum = (heightSum * heightIntersect.g) + (height * heightIntersect.r);

        if (heightIntersect.g  < 1e-3f)
        {
            blendMask[0] = 0.0f;
            blendMask[1] = 0.0f;
            blendMask[2] = 0.0f;
            blendMask[3] = 0.0f;
            #if defined(_TERRAIN_DISTANCEBLEND)
                dBlendMask[0] = 0.0f;
                dBlendMask[1] = 0.0f;
                dBlendMask[2] = 0.0f;
                dBlendMask[3] = 0.0f;
            #endif 
        }
    #endif
      

    //=======================================================================
    //--------------------  PUDDLES & RAINDROPS NORMALS  --------------------
    //=======================================================================
    #if defined(_PUDDLES) && !defined(_TERRAIN_BASEMAP_GEN)
        float3 ripNormal = float3(0, 0, 1);
        float raindropSize = 1.0f / _InTerra_GlobalRaindropRipples.z * 0.1f;
        float2 puddlesHeight = float2(0.0f, 1.0f);

        float3 puddleWeight = pow(abs(worldNormal.rgb), 3.0f);
        puddleWeight = puddleWeight / (puddleWeight.x + puddleWeight.y + puddleWeight.z);

        puddlesHeight = (1 / (pow(2, float2(_InTerra_GlobalPuddles.x, heightSum) * (-(100)))) + 1) * 0.5;
        puddlesHeight /= (puddlesHeight.r + puddlesHeight.g);
        float horizontalWeight = smoothstep(0.9f, 1.0f, saturate(puddleWeight.y - ((puddleWeight.x + puddleWeight.z) * pow(2.0f, _PuddleHorizontalWeight * 10.0f))));
        puddlesHeight.x *= horizontalWeight;        
        
        if (_InTerra_GlobalRaindropRipples.x > 0.0f)
        { 
            if (puddlesHeight.x > 0.8f && _InTerra_GlobalRaindropsDistance.y > 0.0f)
            {
                float raindropsDistance = smoothstep(_InTerra_GlobalRaindropsDistance.x, _InTerra_GlobalRaindropsDistance.y, distanceFromCamera);
                float rainIndex =  1 / (min(5.0f, _InTerra_GlobalRaindropRipples.x) * 4.0f);

                for (float i = 5.0; i > 4.0; i -= rainIndex)
                {
                    ripNormal = BlendNormal(ripNormal, RainRipples(worldPos.zx * raindropSize * i + i * 0.25f ,  i, i * 0.25f));
                }
                ripNormal = lerp(ripNormal, float3(0, 0, 1), raindropsDistance);
            }

            float2  uvRefractOffset = ripNormal.xy * 0.01;
            for (int i = 0; i < _LAYER_COUNT; ++i)
            {
                uvSplat[i] += uvRefractOffset;
            }

            #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN)
                ripNormal = WorldTangent(worldTangent, worldBitangent, ripNormal);
            #endif 
        }
    #endif
        
    //========================================================================================
    //-------------------------------  ALBEDO, SMOOTHNESS & NORMAL ---------------------------
    //========================================================================================
    #if !(defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN))
        float3 worldTangent;
        float3 worldBitangent;
    #endif 

    #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)
        float4 frontDiffuse;
        float3 frontNormal;
        float4 sideDiffuse;
        float3 sideNormal;

        #ifdef _TERRAIN_TRIPLANAR_ONE
            float tolWeightY = saturate(triplanarWeights.y + (1 - blendMask[0].r));

            SampleSplat(uvSplat, blendMask, tolWeightY, mask, mixedDiffuse, mixedNormal);
            SampleSplatTOL(frontDiffuse, frontNormal, uvSplat_front, blendMask, triplanarWeights.z, mask);
            SampleSplatTOL(sideDiffuse, sideNormal, uvSplat_side, blendMask, triplanarWeights.x, mask);
        #else
            SampleSplat(uvSplat, blendMask, triplanarWeights.y, mask, mixedDiffuse, mixedNormal);
            SampleSplat(uvSplat_front, blendMask, triplanarWeights.z, mask, frontDiffuse, frontNormal);
            SampleSplat(uvSplat_side, blendMask, triplanarWeights.x, mask, sideDiffuse, sideNormal);
        #endif 
    #else
        SampleSplat(uvSplat, blendMask, 1.0f, mask, mixedDiffuse, mixedNormal);
    #endif

    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN)
        mixedNormal = WorldTangent(worldTangent, worldBitangent, mixedNormal);
    #endif 

    #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)
        mixedDiffuse = mixedDiffuse + frontDiffuse + sideDiffuse;
        mixedNormal = TriplanarNormal(mixedNormal, worldTangent, worldBitangent, frontNormal, sideNormal, triplanarWeights, flipUV);
    #endif

    #ifdef _TERRAIN_DISTANCEBLEND     
        float4 distantDiffuse;   
        float3 distantNormal;
        coverMask = 0;

        #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)
            float4 dFrontDiffuse;
            float3 dFontNormal;
            float4 dSideDiffuse;
            float3 dSideNormal;

            #ifdef _TERRAIN_TRIPLANAR_ONE
                tolWeightY = saturate(dTriplanarWeights.y + (1 - dBlendMask[0].r));

                SampleDistantSplat(uvStochR, uvStochG, uvScaled, dBlendMask, stochMask, tolWeightY,
                                    dMask, dMode, distantDiffuse, distantNormal, coverMask);

                SampleDistantSplatTOL(dFrontDiffuse, dFontNormal, uvStochR_front, uvStochG_front, uvScaled_front,
                                        dBlendMask, stochMask_front, dTriplanarWeights.z, dMask, dMode, coverMask);

                SampleDistantSplatTOL(dSideDiffuse, dSideNormal, uvStochR_side, uvStochG_side, uvScaled_side,
                                    dBlendMask, stochMask_side, dTriplanarWeights.x, dMask, dMode, coverMask) ;
            #else
                SampleDistantSplat(uvStochR, uvStochG, uvScaled, dBlendMask, stochMask, dTriplanarWeights.y,
                                    dMask, dMode, distantDiffuse, distantNormal, coverMask);

                SampleDistantSplat(uvStochR_front, uvStochG_front, uvScaled_front, dBlendMask, stochMask_front,
                                    dTriplanarWeights.z, dMask, dMode, dFrontDiffuse, dFontNormal, coverMask);

                SampleDistantSplat(uvStochR_side, uvStochG_side, uvScaled_side, dBlendMask, stochMask_side,
                                    dTriplanarWeights.x, dMask, dMode, dSideDiffuse, dSideNormal, coverMask);

            #endif
        #else
            SampleDistantSplat(uvStochR, uvStochG, uvScaled, dBlendMask, stochMask, 1.0f, dMask, dMode, distantDiffuse, distantNormal, coverMask);
        #endif
                    
        #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN)
            distantNormal = WorldTangent(worldTangent, worldBitangent, distantNormal);
        #endif

        #if defined(TRIPLANAR) && !defined(_TERRAIN_BASEMAP_GEN)
            distantDiffuse = distantDiffuse + dFrontDiffuse + dSideDiffuse;
            distantNormal = TriplanarNormal(distantNormal, worldTangent, worldBitangent, dFontNormal, dSideNormal, dTriplanarWeights, flipUV);
        #endif

        distantDiffuse = lerp(mixedDiffuse, distantDiffuse, _HT_cover * coverMask);
        distantNormal = lerp(mixedNormal, distantNormal, _HT_cover * coverMask);
        #ifdef _TERRAIN_BASEMAP_GEN            
            mixedDiffuse = distantDiffuse;
        #else
            mixedDiffuse = lerp(mixedDiffuse, distantDiffuse, distBlend);
            mixedNormal = lerp(mixedNormal, distantNormal, distBlend);
        #endif        
    #endif

    float3 tint = SAMPLE_TEXTURE2D(_TerrainColorTintTexture, SamplerState_Linear_Repeat, tintUV).rgb;
    #ifndef _TERRAIN_BASEMAP_GEN 
        float tintDist = smoothstep(_TerrainColorTintDistance.x, _TerrainColorTintDistance.y, distanceFromCamera);
    #else
        float tintDist = 1;
    #endif
    if (_TerrainColorTintMode == 0)
    {
        tint *= mixedDiffuse.rgb;
    }

    #if !defined(TRIPLANAR_TINT) 
        mixedDiffuse.rgb = lerp(mixedDiffuse.rgb, tint, _TerrainColorTintStrenght * tintDist).rgb;
    #endif

    float normalDist = smoothstep(_TerrainNormalTintDistance.x, _TerrainNormalTintDistance.y, distanceFromCamera);
    float3 normalTint = UnpackNormals(SAMPLE_TEXTURE2D(_TerrainNormalTintTexture, sampler_Splat0, normalTintUV), 1);
    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
        normalTint = WorldTangent(worldTangent, worldBitangent, normalTint);
    #endif   
    mixedNormal = lerp(mixedNormal, BlendNormals(mixedNormal, normalTint), _TerrainNormalTintStrenght * normalDist).rgb;

    //========================================================================================
    //---------------------------------------  TRACKS   --------------------------------------
    //========================================================================================
    #if defined(_TRACKS) && !defined(_TERRAIN_BASEMAP_GEN)
        if (_Tracks == 1)
        {
        UnpackTrackSplatValues(trackSplats);
        UnpackTrackSplatColor(trackSplatsColor);

        float4 trackColor = TrackSplatValues(tBlendMask, trackSplatsColor);
        float4 trackValues = TrackSplatValues(tBlendMask, trackSplats);

        #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
            float2 terrainSize = _TerrainSize.xz;
        #else
            float2 terrainSize = _TerrainSizeXZPosY.xy;
        #endif
        
        float2 trackDetailUV = (float2(splatBaseUV.x, -splatBaseUV.y) * _TrackDetailTexture_ST.xy  * terrainSize + _TrackDetailTexture_ST.zw);

        #if defined(PARALLAX) && defined(_TERRAIN_PARALLAX)
        if (_Terrain_Parallax == 1)
        {
            float2 trackParallaxOffset = ParallaxOffset(_InTerra_TrackTexture, SamplerState_Linear_Repeat, _ParallaxTrackSteps,  -trackValues.y, trackUV, float3( -tangentViewDirTerrain.x, tangentViewDirTerrain.y, -tangentViewDirTerrain.z), _ParallaxTrackAffineSteps, _MipMapLevel + (lod * (log2(max(_InTerra_TrackTexture_TexelSize.z, _InTerra_TrackTexture_TexelSize.w)) + 1)), 1 );

              trackUV += trackParallaxOffset;
              trackDetailUV += (trackParallaxOffset ) * (_TrackDetailTexture_ST.xy * _InTerra_TrackArea);
        }
        #endif
        float4 trackDetail = SAMPLE_TEXTURE2D(_TrackDetailTexture, SamplerState_Linear_Repeat, trackDetailUV);
        trackDepth = SAMPLE_TEXTURE2D_LOD(_InTerra_TrackTexture, SamplerState_Linear_Repeat, trackUV, 0);
 
        float normalsOffset = _InTerra_TrackTexture_TexelSize.x;
        float texelArea = _InTerra_TrackTexture_TexelSize.x * 100 * _InTerra_TrackArea;
        float normalStrenghts = _TrackNormalStrenght / texelArea;
        float normalEdgeStrenghts = _TrackEdgeNormals / texelArea;

        float4 heights[4];
        heights[0] = (SAMPLE_TEXTURE2D_LOD(_InTerra_TrackTexture, SamplerState_Linear_Repeat, trackUV + float2(0.0f, normalsOffset), 0.0f));
        heights[1] = (SAMPLE_TEXTURE2D_LOD(_InTerra_TrackTexture, SamplerState_Linear_Repeat, trackUV + float2(normalsOffset, 0.0f), 0.0f));
        heights[2] = (SAMPLE_TEXTURE2D_LOD(_InTerra_TrackTexture, SamplerState_Linear_Repeat, trackUV + float2(-normalsOffset, 0.0f), 0.0f));
        heights[3] = (SAMPLE_TEXTURE2D_LOD(_InTerra_TrackTexture, SamplerState_Linear_Repeat, trackUV + float2(0.0f, -normalsOffset), 0.0f));

        for (int i = 0; i < 4; ++i)
        {
            heights[i] *= float4(1.0f, 1.0f, normalStrenghts * 2, normalEdgeStrenghts * 2);
        }

        float3 edgeNormals = (float3(float2(heights[2].a - heights[1].a, heights[0].a - heights[3].a), 1.0f));
        float3 trackNormal =  float3(float2(heights[2].b - heights[1].b, heights[0].b - heights[3].b), 1.0f);        

        float4 trackDetailNormal = SAMPLE_TEXTURE2D(_TrackDetailNormalTexture, SamplerState_Linear_Repeat, trackDetailUV);
        trackDetailNormal.xyz = UnpackNormalScale(float4(trackDetailNormal.x, trackDetailNormal.y, 0, 1-trackDetailNormal.w), _TrackDetailNormalStrenght);
        trackDetailNormal.z += 1e-5f;

        float heightSum = HeightSum(mask, blendMask);
        float trackHeightMap = saturate(trackDepth.b + _TrackHeightOffset);
        float2 trackIntersect = float2(trackDepth.b, 1 - trackDepth.b);

        trackIntersect *= (1 / (pow(2, float2(trackHeightMap, heightSum) * (-(_TrackHeightTransition)))) + 1) * 0.5;
        trackIntersect /= (trackIntersect.r + trackIntersect.g);

        trackNormal = (lerp(trackNormal, normalize(lerp(trackDetailNormal.xyz, trackNormal, 0.5f)), trackValues.a)) * trackIntersect.r;
        float trackEdge = saturate(pow(abs(trackDepth.a), _TrackEdgeSharpness));
  
        float track = trackIntersect.r * trackDist;
        float colorOpacity = saturate(track * trackColor.a);
        float normalOpacity = saturate(trackValues.z * (trackEdge + track)) * trackDist;

        #if defined(_NORMALMAPS)
            trackNormal = normalize(lerp(edgeNormals, trackNormal, trackDepth.b));
            trackNormal.z += 1e-5f;
            trackColor = lerp(trackColor, (trackColor * trackDetail), trackValues.a);

            #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
                trackNormal.xy *= -1;
                edgeNormals.xy *= -1;
                trackNormal = WorldTangent(worldTangent, worldBitangent, trackNormal);
            #endif
        
            mixedNormal = lerp(mixedNormal, trackNormal, normalOpacity);
            mixedNormal.z += +1e-5f;
        #endif
        mixedDiffuse.rgb = lerp(mixedDiffuse.rgb, trackColor.rgb, colorOpacity);
        mixedDiffuse.a = lerp(mixedDiffuse.a, trackValues.x, track);
        occlusion = lerp(occlusion, _TrackAO, track);
        }
    #endif

    mixedDiffuse.a = lerp(mixedDiffuse.a, 1.0f, _InTerra_GlobalWetness);
    mixedNormal.xy = _InTerra_GlobalWetness > 0.4 ? mixedNormal.xy * (1 - min(0.7f, (_InTerra_GlobalWetness * 2.25f - 1))) : mixedNormal.xy;
    mixedNormal.z += +1e-5f;

    mixedDiffuse = mixedDiffuse + SAMPLE_TEXTURE2D_LOD(_Splat0, sampler_Splat0, float2(0,0), 10) * 0.000001f;


    #ifdef INTERRA_MESH_TERRAIN
        if (_CheckHeight)
        {
            mixedDiffuse.rgb *= float3(1.0f, 0.80f, 0.60f);
        }
    #endif
 
    //=======================================================================================
    //==============================|   OBJECT INTEGRATION   |===============================
    //=======================================================================================
    #ifdef INTERRA_OBJECT
        if (_BaseTexturesTriplanar)
        {
            float3 objectNormalSide = UnpackNormals(SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseColorMap, objectSideUV), _NormalScale);
            float3 objectNormalFront = UnpackNormals(SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseColorMap, objectFrontUV), _NormalScale);
            objectNormal = ObjectTriplanarNormal(objectNormal, worldTangent, worldBitangent, objectNormalFront, objectNormalSide, baseTriplanarWeights, flipUV);
        }

        objectAlbedo.a = _HasMask == 1 ? objectMask.a : _Smoothness;
        objectAlbedo.a = _GlobalWetnessDisabled ? objectAlbedo.a : lerp(objectAlbedo.a, 1.0f, _InTerra_GlobalWetness);
        float objectMetallic = _HasMask == 1 ? objectMask.r : _Metallic;
        float objectAo = _HasMask == 1 ? objectMask.g : _Ao;

        float3 detail = float3(0, 0, 0);
        float2 detailFrontUV = float2(0, 0);
        float2 detailSideUV = float2(0, 0);

        UNITY_BRANCH if (_Detail > 0)
        {
            UNITY_BRANCH if (_BaseTexturesTriplanar)
            {
                detailFrontUV = float2(worldPos.x * -flipUV.z, worldPos.y) * _DetailMap_ST.xy + _DetailMap_ST.zw;
                detailSideUV = float2(worldPos.z * flipUV.x, worldPos.y) * _DetailMap_ST.xy + _DetailMap_ST.zw;
            }
            UNITY_BRANCH if (_HasDetailAlbedo > 0)
            {
                float3 detail = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, detailUV).rgb;
                
                UNITY_BRANCH if (_BaseTexturesTriplanar)
                {
                    float3 detailSide = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, detailSideUV).rgb * baseTriplanarWeights.x;
                    float3 detailFront = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, detailFrontUV).rgb * baseTriplanarWeights.z;
                    detail = detail * baseTriplanarWeights.y + detailSide + detailFront;
                }
                objectAlbedo.rgb = lerp(objectAlbedo.rgb, half(2.0) * detail, _DetailStrenght).rgb;
            }

            float3 detailNormal = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailUV), _DetailNormalMapScale);
            UNITY_BRANCH if (_BaseTexturesTriplanar)
            {
                float3 detailNormalSide = UnpackNormals(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailSideUV), _DetailNormalMapScale);
                float3 detailNormalFront = UnpackNormals(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailFrontUV), _DetailNormalMapScale);
                detailNormal = ObjectTriplanarNormal(detailNormal, worldTangent, worldBitangent, detailNormalFront, detailNormalSide, baseTriplanarWeights, flipUV);
            }
            objectNormal = (lerp(objectNormal, BlendNormalRNM(objectNormal, detailNormal), _DetailStrenght));
        }

        mixedDiffuse = lerp(mixedDiffuse, objectAlbedo, heightIntersect.r);

        float3 terrainNormal = (mixedNormal.z * terrainNormals.xyz) + 1e-5f;
        terrainNormal.xy = mixedNormal.xy + terrainNormal.xy;
        mixedNormal = lerp(mixedNormal, terrainNormal, intersectNormal);
        mixedNormal = lerp(mixedNormal, objectNormal, heightIntersect.r);

        metallic = lerp(metallic, objectMetallic, heightIntersect.r);
        occlusion = lerp(occlusion, objectAo, heightIntersect.r);
        albedo = mixedDiffuse.rgb;
        emission = 0;
        
        UNITY_BRANCH if (_EmissionEnabled > 0)
        {
            UNITY_BRANCH if (_BaseTexturesTriplanar)
            {
                float2 emissionFrontUV = float2(worldPos.x * -flipUV.z, worldPos.y) * _EmissiveColorMap_ST.xy + _EmissiveColorMap_ST.zw;
                float2 emissionSideUV = float2(worldPos.z * flipUV.x, worldPos.y) * _EmissiveColorMap_ST.xy + _EmissiveColorMap_ST.zw;

                float3 emissionSide = SAMPLE_TEXTURE2D(_EmissiveColorMap, sampler_EmissiveColorMap, emissionSideUV).rgb * _EmissiveColor.rgb * baseTriplanarWeights.x;
                float3 emissionFront = SAMPLE_TEXTURE2D(_EmissiveColorMap, sampler_EmissiveColorMap, emissionFrontUV).rgb * _EmissiveColor.rgb * baseTriplanarWeights.z;
                objectEmission = (objectEmission.rgb * baseTriplanarWeights.y) + emissionSide + emissionFront;
            }

            emission = lerp(0, objectEmission, heightIntersect.r);
        }
    #else
        mixedAlbedo = mixedDiffuse.rgb;
    #endif
      
    float lightSlopeWeight = 0;
    float wettnessAroundPuddle = 0;
    float wetHorizontal = 0;

    //===============|   PUDDLES & RAINDROPS  FINAL MIX |======================
    #if defined(_PUDDLES) && !defined(_TERRAIN_BASEMAP_GEN)
        if (_InTerra_GlobalPuddles.y > 0.0f)
        {            
            lightSlopeWeight = smoothstep(0.0f, 1.0f, saturate(puddleWeight.y - ((puddleWeight.x + puddleWeight.z) * pow(2.0f, _PuddleWetnessHorizontalWeight * 10.0f))));
            wettnessAroundPuddle = smoothstep(_InTerra_GlobalPuddles.y, _InTerra_GlobalPuddles.x, heightSum);
            wetHorizontal = wettnessAroundPuddle * max(horizontalWeight, min(0.65, lightSlopeWeight));


            mixedNormal.xy = wetHorizontal > 0.4 ? mixedNormal.xy * (1 - min(0.9f, (wetHorizontal * 2.25f - 1))) : mixedNormal.xy;
            
            mixedDiffuse.a = lerp(mixedDiffuse.a, 1.0f, wetHorizontal);
            mixedDiffuse.a = lerp(mixedDiffuse.a, 1.0f, puddlesHeight.r * horizontalWeight);
            mixedNormal = lerp(mixedNormal, ripNormal, puddlesHeight.r * horizontalWeight); 
            occlusion = lerp(occlusion, lerp(occlusion, 0.9f, 0.75f) , puddlesHeight.r * horizontalWeight);
        }        
    #endif 
    smoothness = mixedDiffuse.a; 
    mixedNormal.xyz = IsNaN(mixedNormal.x) || IsNaN(mixedNormal.y) || IsNaN(mixedNormal.z) ? float3(0, 0, 1) : mixedNormal.xyz;
  
    //=========================================================================================             
}
