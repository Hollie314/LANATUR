#define INTERRA_MESH_TERRAIN

#include "InTerra_URP_DefinedGlobalKeywords.hlsl"
#include "InTerra_Functions.hlsl"

void MeshTerrain_float(float3 positionOS, float3 viewDirWS, float3 normalWS, out float3 vertexPos, out float3 normalOS, out float3 tangentViewDir)
{

    float3 worldPos = (TransformObjectToWorld(positionOS));
    float2 hmUV = float2 ((worldPos.x - _TerrainPosition.x) * (1 / _TerrainSize.x), (worldPos.z - _TerrainPosition.z) * (1 / _TerrainSize.z));

    vertexPos = positionOS;
    if (_CheckHeight)
    {
        float hm = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV, 0));
        float heightOffset = _TerrainPosition.y - (hm * -_TerrainHeightmapScale.y);
        float3 objVertPos = (TransformWorldToObject(float3(worldPos.x, heightOffset, worldPos.z)));
        vertexPos = TransformWorldToObject((float3(worldPos.x, heightOffset, worldPos.z)));
    }

    normalOS = normalWS;
    if (_NormalsFromHeightmap)
    {
        float2 ts = float2(_TerrainHeightmapTexture_TexelSize.x, _TerrainHeightmapTexture_TexelSize.y);
        float hsX = _TerrainHeightmapScale.w / _TerrainHeightmapScale.x;
        float hsZ = _TerrainHeightmapScale.w / _TerrainHeightmapScale.z;

        float height[4];
        float3 terrainNormal;

        height[0] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(0, -1)), 0)).r * hsZ;
        height[1] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(-1, 0)), 0)).r * hsX;
        height[2] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(1, 0)), 0)).r * hsX;
        height[3] = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV + float2(ts * float2(0, 1)), 0)).r * hsZ;

        terrainNormal.x = height[1] - height[2];
        terrainNormal.z = height[0] - height[3];
        terrainNormal.y = 1;
        normalOS = (terrainNormal).xyz;
    }




    tangentViewDir = 0;
    #if defined (_TERRAIN_PARALLAX) 
        half3 axisSign = normalWS < 0 ? -1 : 1;
        half3 tangentY = normalize(cross(normalWS.xyz, half3(1e-5f, 1e-5f, axisSign.y)));
        half3 bitangentY = normalize(cross(tangentY.xyz, normalWS.xyz)) * axisSign.y;
        half3x3 tbnY = half3x3(tangentY, bitangentY, normalWS);
        tangentViewDir =  mul(tbnY, viewDirWS);
    #endif
}