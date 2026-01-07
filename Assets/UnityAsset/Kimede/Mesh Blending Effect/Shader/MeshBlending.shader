Shader "Kimede/MeshBlending"
{

	Properties {
	   [Header(Quality)]
       [MaterialToggle] _Enabled("Enable Quality Settings", int) = 1
       	[IntRange] _ProcessingIterations("Steps (Performance)", Range(1,8)) = 3
	   [Header(Look)]
	    _BlendingRadius("Blend Radius", Range(0.01, 1)) = 0.1
       	[IntRange] _ScalingFactor("Scaling Factor",Range(1,500)) = 50
	    _DistanceFade("Distance Fade",Range(0, 10)) = 2
	    _ColorIntensity("Color Saturation", Range(0, 5)) = 1.0
	    _OpacityLevel("Blend Transparency", Range(0, 5)) = 1.0
		[Header(Filtering)]
	   	    _SurfaceThreshold("Surface Threshold (0-1)",Range(0, 1)) = 0.5
	   	    _EntityTolerance("Entity Tolerance", Range(-1, 1)) = 0.2

	   [Header(Range)]
	    _MinimumRange("Min Active Distance",float) = 0.1
	    _MaximumRange("Max Active Distance",float) = 100
	    _RangeFalloff("Outside Active Distance",float) = 3
	}


    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Opaque" }
        Pass
        {
            Name "FullscreenPass"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite Off
            Cull Off
            ZTest Always
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		    CBUFFER_START(UnityPerMaterial)
   			int _ProcessingIterations;
			int _ScalingFactor;
			float _BlendingRadius;
			float _DistanceFade;
			float _MinimumRange;
			float _MaximumRange;
			float _RangeFalloff;
			float _SurfaceThreshold;
			float _ColorIntensity;
			float _OpacityLevel;
			float _EntityTolerance;
            int _Enabled;
			CBUFFER_END


		   TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

		   TEXTURE2D(_BlitTexture);
SAMPLER(sampler_BlitTexture);

TEXTURE2D(_CameraOpaqueTexture);
SAMPLER(sampler_CameraOpaqueTexture);


            TEXTURE2D(_ObjectIDTexture);
			SamplerState sampler_ObjectIDTexture
			{
				Filter = MIN_MAG_MIP_LINEAR;
				AddressU = Clamp;
				AddressV = Clamp;
			};

		    TEXTURE2D(_ObjectDepthTexture);
            SAMPLER(sampler_ObjectDepthTexture);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varyings Vert(uint vertexID : SV_VertexID)
			{
				Varyings o;

				float2 pos = float2((vertexID << 1) & 2, vertexID & 2);
				o.positionHCS = float4(pos * 2.0 - 1.0, 0, 1);
				o.uv = pos;

				return o;
			}

			
float CalculateLOD(float depth, float blendingScale)
{
    float distanceLOD = clamp(log2(depth / 5.0), 0.0, 3.0);
    float scaleLOD = clamp(log2(blendingScale * 100.0), 0.0, 2.0);
    return min(distanceLOD + scaleLOD * 0.5, 4.0);
}

float2 FindClosestSeam(float2 UV, float4 entityColor, float3 surfaceNormal, float blendingScale, float lodLevel)
{
    float2 bestSeam = float2(0.0, 0.0);
    float bestDist = 999999.0;

    float maxRadius = blendingScale * 4.0;
    int steps = _ProcessingIterations;

    [unroll(8)]
    for(int step = 0; step < steps; step++) {
        float currentRadius = maxRadius * pow(0.6, step);

        float stepLOD = lodLevel + step * 0.5;

        float2 directions[8] = {
            float2(0, -1 ),
            float2(1, 0),
            float2(0, 1 ),
            float2(-1, 0),
            float2(0, -2 ),
            float2(2, 0),
            float2(0, 2 ),
            float2(-2, 0)
        };

        [unroll(8)]
        for(int i = 0; i < 8; i++) {
            float2 offset = directions[i] * currentRadius;
            float2 sampleUV = UV + offset;

            if (sampleUV.x < 0.0 || sampleUV.x > 1.0 || sampleUV.y < 0.0 || sampleUV.y > 1.0) continue;

            float4 sampleID =  SAMPLE_TEXTURE2D_LOD(_ObjectIDTexture, sampler_ObjectIDTexture, sampleUV, stepLOD);

            if (abs(sampleID.x - entityColor.x) > _EntityTolerance) {
                float3 surfaceB = (sampleID.yzw) * 2.0 - 1.0;
                float surfaceDiff = length(surfaceB - surfaceNormal);

                if (surfaceDiff > _SurfaceThreshold) {
                    float dist = length(offset);
                    if (dist < bestDist) {
                        bestDist = dist;
                        bestSeam = offset;
                    }
                }
            }
        }

        if (bestDist < blendingScale) break;
    }

    return bestSeam;
}
			
            float4 Frag(Varyings input) : SV_Target
            {	

				float2 screenCoords = float2(input.uv.x, 1-input.uv.y);
			    
			    
				//float4 primaryColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, screenCoords);
               float4 primaryColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenCoords);

                if(_Enabled != 1)
                return primaryColor;

                
				//float entityDepth = LinearEyeDepth(SAMPLE_TEXTURE2D(_ObjectDepthTexture, sampler_ObjectDepthTexture, screenCoords).r, _ZBufferParams);
				float actualDepth = LinearEyeDepth(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenCoords).r, _ZBufferParams);
                float entityDepth = actualDepth;
				
				float4 entityColor = SAMPLE_TEXTURE2D(_ObjectIDTexture, sampler_ObjectIDTexture, screenCoords);
				
    /*
    bool isValidData = !any(isnan(primaryColor.rgb)) && !any(isinf(primaryColor.rgb)) &&
                       !isnan(entityDepth) && !isinf(entityDepth) &&
                       !isnan(actualDepth) && !isinf(actualDepth);

    if (!isValidData) {
        return primaryColor;

    }
    */

    bool shouldSkip = (length(entityColor.xyz) < 0.01) ||
                      (entityDepth > actualDepth) ||
                      (entityDepth < _MinimumRange) ||
                      (entityDepth > _MaximumRange + _RangeFalloff);

    if (shouldSkip) {
        return primaryColor;

    }
							
				float3 surfaceNormal = (entityColor.yzw) * 2.0 - 1.0;
				float blendingScale = _BlendingRadius / (entityDepth * _ScalingFactor);
				    float lodLevel = CalculateLOD(entityDepth, blendingScale);

				bool hasNearbySeam = false;
			
	  static const float2 quickOffsets[4] = {
        float2(1, 0), float2(-1, 0), float2(0, 1), float2(0, -1)
    };

    [unroll(4)]
    for(int i = 0; i < 4; i++) {
        float2 quickOffset = quickOffsets[i] * blendingScale;
        float4 quickID = SAMPLE_TEXTURE2D_LOD(_ObjectIDTexture, sampler_ObjectIDTexture, screenCoords + quickOffset,lodLevel);
        hasNearbySeam = hasNearbySeam || (abs(quickID.x - entityColor.x) > _EntityTolerance);
    }
     

    float2 closestSeamLocation = float2(0.0, 0.0);

        closestSeamLocation = hasNearbySeam ?
            FindClosestSeam(screenCoords, entityColor, surfaceNormal, blendingScale,lodLevel) :
            float2(0.0, 0.0);

				float minimumDistance = dot(closestSeamLocation, closestSeamLocation);
			
								
				float4 neighborEntity = SAMPLE_TEXTURE2D_LOD(_ObjectIDTexture, sampler_ObjectIDTexture, screenCoords + closestSeamLocation*2,lodLevel);
				//float4 neighborColor = SAMPLE_TEXTURE2D_LOD(_BlitTexture, sampler_BlitTexture, screenCoords + closestSeamLocation*2,lodLevel);
                float4 neighborColor = SAMPLE_TEXTURE2D_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenCoords + closestSeamLocation*2,lodLevel);
				float neighborDepth = LinearEyeDepth(SAMPLE_TEXTURE2D(_ObjectDepthTexture, sampler_ObjectDepthTexture, screenCoords + closestSeamLocation*2).r,_ZBufferParams);
				float depthVariance = abs(neighborDepth-entityDepth);
				float maxFalloffDistance = (_BlendingRadius)/entityDepth;
				float spatialWeight = saturate(0.5 - sqrt(minimumDistance) / maxFalloffDistance);
				float depthWeight = saturate(1.0 - depthVariance / (_DistanceFade*_BlendingRadius));
				float compositeWeight = spatialWeight * depthWeight;
				if (entityDepth > _MaximumRange) {
					compositeWeight *= 1-saturate((entityDepth-_MaximumRange)/_RangeFalloff);
				}
				if (entityDepth < _MinimumRange) {
					compositeWeight *= 1-saturate((_MinimumRange-entityDepth)/_RangeFalloff);
				}
				if (entityDepth > actualDepth) {
					compositeWeight = 0;
					}

			    float3 neighborColorRGB = neighborColor.rgb;
			    float brightness = dot(neighborColorRGB, float3(0.299, 0.587, 0.114));
			    float3 enhancedColor = lerp(float3(brightness, brightness, brightness), neighborColorRGB, _ColorIntensity);
			    float4 adjustedNeighborColor = float4(enhancedColor, neighborColor.a);

			    float adjustedWeight = compositeWeight * _OpacityLevel;

			    return float4(lerp(primaryColor.rgb, adjustedNeighborColor.rgb, adjustedWeight), 1.0);

            }
            ENDHLSL
        }
    }
}
