Shader "InTerra/Tracks Material"
{
    Properties
    {
        _HeightTex ("Heightmap Texture", 2D) = "black" {}
        _TerrainTrackContrast("Contrast", Range(0,3)) = 1.0 
        _TerrainTrackHeightmapOffset("Heightmap offset", Range(-0.5,0.5)) = 0.0
        _EdgeFading("Edge Fading",  vector) = ( -0.1,0.6,-0.4,0.8)
        _TrackFadeTime("", float) = 30
        _TrackTime("", float) = 0
        _WheelTrackFading("", Range(0,10)) = 10
    }

    SubShader
    {
     Tags {"Queue" = "Transparent+100" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

        ZWrite Off
        ZTest On
        Lighting Off

        Blend SrcAlpha OneMinusSrcAlpha
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma shader_feature_local _INVERT
            #pragma shader_feature_local _ORIENTATION
            #pragma shader_feature_local __ _TRACKS _FOOTPRINTS
          

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;  
            };

            sampler2D _HeightTex;
            float4 _HeightTex_ST;
            float _TrackFadeTime, _TrackTime;
            float _TerrainTrackContrast;
            float _TerrainTrackHeightmapOffset;
            float4 _EdgeFading;
            float _WheelTrackFading;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv.xy, _HeightTex);
                o.uv2 = v.uv2;
                o.color = v.color;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float height = 1;
                float2 texUV = i.uv;

                #ifdef _ORIENTATION
                    texUV.xy = float2(1-texUV.x, 1- texUV.y);
                #else
                    texUV.xy = float2(texUV.y, 1 - texUV.x);
                #endif

                #if defined(_TRACKS) || defined(_FOOTPRINTS)
                    height = tex2D(_HeightTex, texUV);
                    _TerrainTrackHeightmapOffset *= -1.0f;
                   
                    #ifndef _INVERT
                        height = 1 - height;
                    #endif
                #endif

                float fadingInTime = (1.0f - ((_TrackTime - i.color.b) / _TrackFadeTime));
                float fade;

                #ifdef _TRACKS
                    fade = saturate(i.color.a  * (11.0f - _WheelTrackFading));
                #else                    
                    fade = i.color.a;              
                #endif

                if(fadingInTime > 1)
                {
                    fadingInTime = fade; 
                }
                else
                {
                    fadingInTime = saturate(fade * fadingInTime);
                }

                float2 uv = i.uv2;
                float strenght = 1;

                float2 uv2 = i.uv2;
                float alpha = 1.0;
                float2 center = float2(0.5f, 0.5f); 
                float2 edgeNormals;
                float2  fadingUv = i.uv2;
                float2 distances;
                float normalsOffset = 0.01f;

                #ifdef _TRACKS
                    distances = distance(fadingUv.y, center.y);
                #else                    
                    distances = distance(fadingUv, center);                   
                #endif

                alpha = 1 - smoothstep(_EdgeFading.x, _EdgeFading.y, distances);
                    
                float alpha2 =  1-step(0.5, distances.xy) ;

                alpha *= alpha2 * fadingInTime;
               

                #if defined(_TRACKS) || defined(_FOOTPRINTS)
                    height += _TerrainTrackHeightmapOffset; 
                    if (_TerrainTrackContrast <= 1)
                    {              
                        height = lerp(1.0f, height, _TerrainTrackContrast);
                    }
                    else 
                    {
                      height =  saturate(lerp(0.5f, height, _TerrainTrackContrast));
                    }                 
                #endif           
     
                return  float4(height.xxx, alpha);
            }
            ENDCG
        }
    }
    CustomEditor "InTerra.InTerra_TracksShaderGUI"
}
