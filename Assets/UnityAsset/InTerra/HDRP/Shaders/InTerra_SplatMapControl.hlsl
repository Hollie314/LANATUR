    float2 uvSplat[_LAYER_COUNT];
    float4 mask[_LAYER_COUNT];
    #ifdef TRIPLANAR
        float2 uvSplat_front[_LAYER_COUNT], uvSplat_side[_LAYER_COUNT];
        float4 mask_front[_LAYER_COUNT], mask_side[_LAYER_COUNT];
    #endif
    #ifdef _TERRAIN_DISTANCEBLEND
        int dMode[_LAYER_COUNT];
        float2 uvScaled[_LAYER_COUNT];
        float4 dMask[_LAYER_COUNT];

        float2 uvStochR[_LAYER_COUNT], uvStochG[_LAYER_COUNT];
        float2 stochMask[_LAYER_COUNT];
        half coverMask = 0;

        #ifdef TRIPLANAR
            float2 uvScaled_front[_LAYER_COUNT], uvScaled_side[_LAYER_COUNT];
            float2 stochMaskFront[_LAYER_COUNT];

            float4 dMask_front[_LAYER_COUNT], dMask_side[_LAYER_COUNT];
            float2 stochMaskSide[_LAYER_COUNT];

            float2 uvStochR_front[_LAYER_COUNT], uvStochR_side[_LAYER_COUNT];
            float2 uvStochG_front[_LAYER_COUNT], uvStochG_side[_LAYER_COUNT];
            float2 stochMask_front[_LAYER_COUNT], stochMask_side[_LAYER_COUNT];
        #endif
    #endif

    #ifdef _TRACKS
        float4 trackSplats[_LAYER_COUNT];
        float4 trackSplatsColor[_LAYER_COUNT];
    #endif
 
    float distanceFromCamera = distance(worldPos, _WorldSpaceCameraPos);

    //-------------------- MIP MAP LOD ------------------------- 
    #if defined(PARALLAX) || defined(TESSELLATION_ON)
        float lod = smoothstep(_MipMapFade.x, _MipMapFade.y, distanceFromCamera);
    #endif

    //====================================================================================
    //--------------------------------- SPLAT MAP CONTROL --------------------------------
    //====================================================================================
    float4 blendMask[4];
    blendMask[0] = 0;
    blendMask[1] = 0;
    blendMask[2] = 0;
    blendMask[3] = 0;

    float4 trackDepth = 0;
    half4  mixedMask = 0;
    half4  dMixedMask = 0;

    #ifndef TESSELLATION_SAMPLING
        occlusion = mixedMask.g;
        metallic = mixedMask.r;
    #endif

    float halfTrackArea = _InTerra_TrackArea * 0.5f;
    float _InTerra_TrackFading = 28;
    float2 trackUV = float2((worldPos.x - _InTerra_TrackPosition.x) + (halfTrackArea), -(worldPos.z - _InTerra_TrackPosition.z - (halfTrackArea))) * 1.0f / _InTerra_TrackArea;

    float trackDist = smoothstep(_InTerra_TrackArea - 1.0f, _InTerra_TrackArea - _InTerra_TrackFading, (distance(worldPos, float3(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.y, _WorldSpaceCameraPos.z))));

    float2 minDist = step(float2(0.0f, 0.0f), trackUV);
    float2 maxDist = step(float2(0.0f, 0.0f), 1.0 - trackUV);
    trackDist *= (minDist.x * minDist.y * maxDist.x * maxDist.y);

    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN) 
        float2 splatBaseUV = (worldPos.xz - _TerrainPosition.xz) * (1 / _TerrainSize.xz);

        #ifndef _LAYERS_ONE     
            float2 splatMapUV = (splatBaseUV * (_Control_TexelSize.zw - 1.0f) + 0.5f) * _Control_TexelSize.xy; 

            #ifndef TESSELLATION_SAMPLING
                blendMask[0] = SAMPLE_TEXTURE2D(_Control, sampler_Control, splatMapUV);
            #else
                blendMask[0] = SAMPLE_TEXTURE2D_LOD(_Control, sampler_Control, splatMapUV, 0);
            #endif
            #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
                if (_NumLayersCount > 4)
                {
                #ifndef TESSELLATION_SAMPLING
                    blendMask[1] = SAMPLE_TEXTURE2D(_Control1, sampler_Control, splatMapUV);
                    #ifdef _LAYERS_SIXTEEN
                        if (_NumLayersCount > 8) blendMask[2] = SAMPLE_TEXTURE2D(_Control2, sampler_Control, splatMapUV);
                        if (_NumLayersCount > 12) blendMask[3] = SAMPLE_TEXTURE2D(_Control3, sampler_Control, splatMapUV);
                    #endif
                #else
                    blendMask[1] = SAMPLE_TEXTURE2D_LOD(_Control1, sampler_Control, splatMapUV, 0);
                     #ifdef _LAYERS_SIXTEEN
                        if (_NumLayersCount > 8) blendMask[2] = SAMPLE_TEXTURE2D_LOD(_Control2, sampler_Control, splatMapUV, 0);
                        if (_NumLayersCount > 12) blendMask[3] = SAMPLE_TEXTURE2D_LOD(_Control3, sampler_Control, splatMapUV, 0);
                    #endif
                #endif
                }
            #endif  
        #else
            blendMask[0] = float4(1, 0, 0, 0);
            blendMask[1] = float4(0, 0, 0, 0);
            blendMask[2] = float4(0, 0, 0, 0);
            blendMask[3] = float4(0, 0, 0, 0);
        #endif 
    #else
        float2 blendUV0 = (splatBaseUV.xy * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;                      
        #ifndef TESSELLATION_SAMPLING
            blendMask[0] = SAMPLE_TEXTURE2D(_Control0, sampler_Control0, blendUV0);
        #else
            blendMask[0] = SAMPLE_TEXTURE2D_LOD(_Control0, sampler_Control0, blendUV0, 0);
        #endif

        #if defined(_LAYERS_EIGHT) || defined(_LAYERS_SIXTEEN)
            if (_NumLayersCount > 4)
            {
            #ifndef TESSELLATION_SAMPLING
                blendMask[1] = SAMPLE_TEXTURE2D(_Control1, sampler_Control0, blendUV0);
                #ifdef _LAYERS_SIXTEEN
                    if (_NumLayersCount > 8) blendMask[2] = SAMPLE_TEXTURE2D(_Control2, sampler_Control0, blendUV0);
                    if (_NumLayersCount > 12) blendMask[3] = SAMPLE_TEXTURE2D(_Control3, sampler_Control0, blendUV0);                                    
                #endif
            #else
                blendMask[1] = SAMPLE_TEXTURE2D_LOD(_Control1, sampler_Control0, blendUV0, 0);
                 #ifdef _LAYERS_SIXTEEN
                    if (_NumLayersCount > 8) blendMask[2] = SAMPLE_TEXTURE2D_LOD(_Control2, sampler_Control0, blendUV0, 0);
                    if (_NumLayersCount > 12) blendMask[3] = SAMPLE_TEXTURE2D_LOD(_Control3, sampler_Control0, blendUV0, 0);
                #endif
            #endif   
            }
        #endif
    #endif
            
    #if defined(_TERRAIN_BLEND_HEIGHT)
        blendMask[0] += 0.00001; //to prevent nan when heightblending
    #endif
       
    float2 tintUV = splatBaseUV * _TerrainColorTintTexture_ST.xy + _TerrainColorTintTexture_ST.zw;
    float2 normalTintUV = splatBaseUV * _TerrainNormalTintTexture_ST.xy + _TerrainNormalTintTexture_ST.zw;  
 

    float3 flipUV = worldNormal.rgb < 0 ? -1 : 1;
    float3  triplanarWeights = abs(worldNormal.rgb);
    triplanarWeights = pow(triplanarWeights, _TriplanarSharpness);
    triplanarWeights = triplanarWeights / (triplanarWeights.x + triplanarWeights.y + triplanarWeights.z);

    #ifdef INTERRA_OBJECT
        float3  baseTriplanarWeights = pow(triplanarWeights, _BaseTexturesTriplanarSharpness);
        baseTriplanarWeights = baseTriplanarWeights / (baseTriplanarWeights.x + baseTriplanarWeights.y + baseTriplanarWeights.z);
        TriplanarOneToAllSteep(blendMask, (1 - terrainNormals.w));
    #else
        TriplanarOneToAllSteep(blendMask, (1 - triplanarWeights.y));
    #endif
 

    #if defined(_LAYERS_TWO) && defined(INTERRA_OBJECT)
            blendMask[0].r = _ControlNumber == 0 ? blendMask[0].r : _ControlNumber == 1 ? blendMask[0].g : _ControlNumber == 2 ? blendMask[0].b : blendMask[0].a;
            blendMask[0].g = 1 - blendMask[0].r;
    #endif
         

    #if defined(_TERRAIN_BASEMAP_GEN_TRIPLANAR) || defined(_LAYERS_ONE) 
        blendMask[0] = float4(1, 0, 0, 0);
        blendMask[1] = float4(0, 0, 0, 0);
        blendMask[2] = float4(0, 0, 0, 0);
        blendMask[3] = float4(0, 0, 0, 0);
    #endif

    #ifdef _TRACKS
        float4 tBlendMask[4];
        tBlendMask[0] = blendMask[0];
        tBlendMask[1] = blendMask[1];
        tBlendMask[2] = blendMask[2];
        tBlendMask[3] = blendMask[3];
    #endif


    //-------------------------  OBJECT INTERSECTION  ----------------------
    #ifdef INTERRA_OBJECT	
        float steeptriplanarWeights = _SteepIntersection == 1 ? saturate(worldNormal.y + _Steepness) : 1;
        float intersect1 = smoothstep(_Intersection.y, _Intersection.x, heightOffset) * steeptriplanarWeights;
        float intersect2 = smoothstep(_Intersection2.y, _Intersection2.x, heightOffset) * (1 - steeptriplanarWeights);
        float intersection = intersect1 + intersect2;
        float intersectNormal = smoothstep(_NormIntersect.y, _NormIntersect.x, heightOffset);
    #endif
    //---------------------------------------------------------------------
       
    float4 origBlendMask[4] = blendMask;
    #if defined(_TERRAIN_DISTANCEBLEND) && !defined(TESSELLATION_SAMPLING) 
        CoverMaskAndMode(blendMask, coverMask, dMode);

        float3 dTriplanarWeights = triplanarWeights;
        float4 dBlendMask[4];
        float4 dOrigBlendMask[4] = blendMask;

        dBlendMask[0] = blendMask[0];
        dBlendMask[1] = blendMask[1];
        dBlendMask[2] = blendMask[2];
        dBlendMask[3] = blendMask[3];

        float distBlend = smoothstep(_HT_distance.x, _HT_distance.y, distanceFromCamera);
        float sampleDistMask = distBlend < 0.001f ? 1 : 0;

        if(sampleDistMask)
        {
            dBlendMask[0] = 0;
            #ifdef _LAYERS_EIGHT
                dBlendMask[1] = 0;
            #endif
            #ifdef _LAYERS_SIXTEEN
                dBlendMask[2] = 0;
                dBlendMask[3] = 0;
            #endif
        }

        float sampleDistMaskFullCover = (distanceFromCamera > _HT_distance.y) && coverMask > 0.99f && _HT_cover > 0.99f ? 1 : 0;
        #ifdef INTERRA_OBJECT
            if ((intersection < 0.5f))
            {
                sampleDistMaskFullCover = 0;
            }
        #endif

        if(sampleDistMaskFullCover )
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

    float heightSum = 0.5f;
    float dHeightSum = 0.5f;

   

    //================================================================================
    //-------------------------------------- UVs -------------------------------------
    //================================================================================
    #if defined(INTERRA_OBJECT) || defined(INTERRA_MESH_TERRAIN)
        float3 positionOffset = _WorldMapping ? worldPos : (worldPos - _TerrainPosition);


        #if defined(INTERRA_OBJECT) 
            float2 objectFrontUV = float2(0.0f, 0.0f);
            float2 objectSideUV = float2(0.0f, 0.0f);

            UNITY_BRANCH if (_BaseTexturesTriplanar)
            {
                objectFrontUV = float2(worldPos.x * -flipUV.z, worldPos.y) * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
                objectSideUV = float2(worldPos.z * flipUV.x, worldPos.y) * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
            
                #if !defined(TESSELLATION_SAMPLING)    
                    float4 objectAlbedoSide = SAMPLE_TEXTURE2D(_BaseColorMap, sampler_BaseColorMap, objectSideUV) * _BaseColor;
                    float4 objectAlbedoFront = SAMPLE_TEXTURE2D(_BaseColorMap, sampler_BaseColorMap, objectFrontUV) * _BaseColor;
                   objectAlbedo = objectAlbedo * baseTriplanarWeights.y + objectAlbedoSide * baseTriplanarWeights.x + objectAlbedoFront * baseTriplanarWeights.z;
                #endif
            }
            
        #endif

        #ifndef TRIPLANAR
            
            float distortion = 0;
            #if defined(INTERRA_OBJECT) && !defined(TESSELLATION_SAMPLING)
                distortion = worldNormal.y > 0.5 ? 0 : (1 - worldNormal.y) * _SteepDistortion * objectAlbedo.r;
            #endif

            UvSplat(uvSplat, positionOffset, distortion);
        #else
            float offsetZ = -flipUV.z * worldPos.y;
            float offsetX = -flipUV.x * worldPos.y;
            #if defined(INTERRA_OBJECT) 
                offsetZ = _DisableOffsetY == 1 ? -flipUV.z * worldPos.y : heightOffset * -flipUV.z + (worldPos.z);
                offsetX = _DisableOffsetY == 1 ? -flipUV.x * worldPos.y : heightOffset * -flipUV.x + (worldPos.x);
            #endif
              
            offsetZ -= _TerrainPosition.z;
            offsetX -= _TerrainPosition.x;

            UvSplat(uvSplat, uvSplat_front, uvSplat_side, positionOffset, offsetZ, offsetX, flipUV);
        #endif
    #else
        
        #ifdef _TERRAIN_BASEMAP_GEN
            float2 uv = splatBaseUV.xy;
        #else
            float2 uv = _WorldMapping ? (worldPos.xz / _TerrainSizeXZPosY.xy) : splatBaseUV.xy;
        #endif
        #ifndef TRIPLANAR
            UvSplat(uvSplat, uv);
        #else
            UvSplat(uvSplat, uvSplat_front, uvSplat_side, worldPos, uv, flipUV);
        #endif
    #endif          
          
    //-------------------- PARALLAX OFFSET -------------------------                  
    #if defined(PARALLAX) && defined(_TERRAIN_PARALLAX) && !defined(TESSELLATION_ON) && !defined(_TERRAIN_BASEMAP_GEN)
    if (_Terrain_Parallax == 1)
    {
        ParallaxUV(uvSplat, tangentViewDirTerrain, origBlendMask, triplanarWeights.y, lod);
    }
    #endif

    //--------------------- DISTANT UV ------------------------
    #ifdef _TERRAIN_DISTANCEBLEND
        ScaleUV(uvScaled, uvSplat, dMode);
        #ifdef TRIPLANAR          
            ScaleUV(uvScaled_front, uvSplat_front, dMode);
            ScaleUV(uvScaled_side, uvSplat_side, dMode);
            StochasticMaskAndUV(dMode, uvScaled_front, stochMask_front, uvStochR_front, uvStochG_front);
            StochasticMaskAndUV(dMode, uvScaled_side, stochMask_side, uvStochR_side, uvStochG_side);            
        #endif
        StochasticMaskAndUV(dMode, uvScaled, stochMask, uvStochR, uvStochG);
    #endif

       
