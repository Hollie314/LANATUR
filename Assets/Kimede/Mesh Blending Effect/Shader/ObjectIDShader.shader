Shader "Kimede/GenerateIDShader"
{

	Properties {
	}


    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "ObjectPositionToColor"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            CBUFFER_START(CameraFrustumCulling)
                float4 _CameraFrustumPlanes[6];
            CBUFFER_END
                

            struct Attributes
            {
                float4 positionOS : POSITION;
				    float3 normalOS : NORMAL;
				    UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float isCulled : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };


            bool IsInCameraFrustum(float3 worldPos)
            {
                for(int i = 0; i < 6; i++)
                {
                    float dist = dot(_CameraFrustumPlanes[i].xyz, worldPos) + _CameraFrustumPlanes[i].w;
                    if(dist < 0.0)
                    {
                        return false;
                    }
                }
                return true;
            }

		    float Hash(float3 p)
            {
				p = p + float3(1.1723930,1.1723930,1.1723930);
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.worldPos = TransformObjectToWorld(vertexInput.positionWS);
                            
                output.isCulled = IsInCameraFrustum(output.worldPos) ? 0.0 : 1.0;
              
                if(output.isCulled > 0.5)
                {
                    output.positionCS = float4(0, 0, 0, 0); 
                }
                else
                {
                    output.positionCS = vertexInput.positionCS;
                }
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
			    output.normalWS = normalInput.normalWS;
			    
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            
                if(input.isCulled > 0.5)
                {
                    discard;
                }
                 
				float3 normal = normalize(input.normalWS);
       
                float3 objectPosition = input.positionCS.xyz;
        
				float hashValue = Hash(objectPosition);
        
                float3 encodedNormal = normal * 0.5 + 0.5;
           
				return half4(hashValue, encodedNormal.x, encodedNormal.y, encodedNormal.z);

                
            }


            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}

