//NOTE! This file is based on Unity file "TerrainLitBase.shader" which was used as a template for adding all the InTerra features.
Shader "Hidden/InTerra/Lit (Base Pass)"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo(RGB), Smoothness(A)", 2D) = "white" {}
        _MetallicTex("Metallic, Occlusion, Splat01", 2D) = "black" {}
        _TriplanarTex("Triplanar Albedo(RGB), Smoothness(A)", 2D) = "black" {}
        _Triplanar_MetallicAO("Metallic, Occlusion", 2D) = "black" {}
        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}
    }

        HLSLINCLUDE

#pragma multi_compile_fragment __ _ALPHATEST_ON

        ENDHLSL

        SubShader
    {
        PackageRequirements { "com.unity.render-pipelines.universal":"[12.0,19.0]" }
        Tags { "Queue" = "Geometry-100" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 200

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            Name "ForwardLit"
            // Lightmode matches the ShaderPassName set in UniversalPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Pipeline
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma target 2.0

        // -------------------------------------
        // Material Keywords
        #define _METALLICSPECGLOSSMAP 1
        #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

        #include "InTerra_URP_DefinedGlobalKeywords.hlsl"

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX

        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

        #pragma multi_compile_fragment _ _LIGHT_COOKIES                                

        #ifdef UNITY_2021_2_TO_2022_1
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #elif defined(UNITY_2022_2_TO_2022_3)

            #if (!defined(UNITY_COMPILER_DXC) && (defined(UNITY_PLATFORM_OSX) || defined(UNITY_PLATFORM_IOS))) || defined(SHADER_API_PS5)

                #if defined(SHADER_API_PS5) || defined(SHADER_API_METAL)

                    #define SUPPORTS_FOVEATED_RENDERING_NON_UNIFORM_RASTER 1

                        #pragma warning (disable : 3568) // unknown pragma ignored
                        #pragma never_use_dxc metal
                        #pragma dynamic_branch _ _FOVEATED_RENDERING_NON_UNIFORM_RASTER
                        #pragma warning (default : 3568) // restore unknown pragma ignored
                #endif
            #endif

            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #pragma target 4.5 _WRITE_RENDERING_LAYERS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fog

        #else
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            #if defined(UNITY_6000_0) || defined(UNITY_6000_1) || defined(UNITY_6000_2)
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
            #endif

            #if defined(UNITY_6000_1) || defined(UNITY_6000_2)
                #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
                #pragma multi_compile_fragment _ _REFLECTION_PROBE_ATLAS                 
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
            #else             
                #pragma multi_compile _ _FORWARD_PLUS               
                #pragma multi_compile_fog              
            #endif
        #endif

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile_instancing
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

        #pragma vertex SplatmapVert
        #pragma fragment SplatmapFragment

        #define _NORMALMAP
        // Sample normal in pixel shader when doing instancing
        #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
        #define TERRAIN_SPLAT_BASEPASS 1

        #pragma shader_feature_local __ _TERRAIN_TRIPLANAR_ONE _TERRAIN_TRIPLANAR

        #define INTERRA_TERRAIN 

        #include "InTerra_TerrainLitInput.hlsl"
        #include "InTerra_TerrainLitPasses.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "ShadowCaster"
        Tags{"LightMode" = "ShadowCaster"}

        ZWrite On
        ColorMask 0

        HLSLPROGRAM
        #pragma target 4.5

        // Deferred Rendering Path does not support the OpenGL-based graphics API:
        // Desktop OpenGL, OpenGL ES 3.0, WebGL 2.0.
        #pragma exclude_renderers gles3 glcore

        #pragma multi_compile_instancing
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment

        #define INTERRA_TERRAIN 
        #include "InTerra_URP_DefinedGlobalKeywords.hlsl"
        #include "InTerra_TerrainLitInput.hlsl"
        #include "InTerra_TerrainLitPasses.hlsl"
        ENDHLSL
    }

        // ------------------------------------------------------------------
        //  GBuffer pass. Does GI + emission. All additional lights are done deferred as well as fog
        Pass
        {
            Name "GBuffer"
            Tags{"LightMode" = "UniversalGBuffer"}

            HLSLPROGRAM
            #pragma exclude_renderers gles
            #pragma target 2.0

        // -------------------------------------
        // Material Keywords
        #define _METALLICSPECGLOSSMAP 1
        #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile_fragment _ _LIGHT_LAYERS

        #include "InTerra_URP_DefinedGlobalKeywords.hlsl"

        #if defined(UNITY_2021_2_TO_2022_1) || defined(UNITY_2022_2_TO_2022_3)
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
        #else
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #endif


        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

        #pragma multi_compile_instancing
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

        #pragma vertex SplatmapVert
        #pragma fragment SplatmapFragment

        #define _NORMALMAP

        // Sample normal in pixel shader when doing instancing
        #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
        #define TERRAIN_SPLAT_BASEPASS 1
        #define TERRAIN_GBUFFER 1

        #define INTERRA_TERRAIN 
        #include "InTerra_URP_DefinedGlobalKeywords.hlsl"
        #include "InTerra_TerrainLitInput.hlsl"
        #include "InTerra_TerrainLitPasses.hlsl"

        ENDHLSL
    }

    Pass
    {
        Name "DepthOnly"
        Tags{"LightMode" = "DepthOnly"}

        ZWrite On
        ColorMask R

        HLSLPROGRAM
        #pragma target 2.0

        #pragma vertex DepthOnlyVertex
        #pragma fragment DepthOnlyFragment

        #pragma multi_compile_instancing
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

        #define INTERRA_TERRAIN 
        #include "InTerra_URP_DefinedGlobalKeywords.hlsl"
        #include "InTerra_TerrainLitInput.hlsl"
        #include "InTerra_TerrainLitPasses.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "DepthNormals"
        Tags{"LightMode" = "DepthNormals"}

        ZWrite On

        HLSLPROGRAM
        #pragma target 2.0

        #pragma vertex DepthNormalOnlyVertex
        #pragma fragment DepthNormalOnlyFragment

        #pragma multi_compile_instancing
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap
        #pragma shader_feature_local _NORMALMAP

        #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitDepthNormalsPass.hlsl"
        ENDHLSL
    }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma vertex TerrainVertexMeta
            #pragma fragment TerrainFragmentMeta

            #pragma shader_feature EDITOR_VISUALIZATION
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap
            #define _METALLICSPECGLOSSMAP 1
            #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitMetaPass.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Universal Render Pipeline/Terrain/Lit/SceneSelectionPass"
    }
        FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
