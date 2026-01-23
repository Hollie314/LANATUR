#define INTERRA_MESH_TERRAIN

#include "InTerra_HDRP_DefinedGlobalKeywords.hlsl"
#include "InTerra_Functions.hlsl"

void MeshTerrain_float(float3 positionOS, float4 normalWS, out float3 oPositionOS, out float4 oNormalOS, out float4 oNormalWS, out float3 oPsitionWS)
{
    float3 worldPos = GetAbsolutePositionWS(TransformObjectToWorld(positionOS));
    float2 hmUV = float2 ((worldPos.x - _TerrainPosition.x) * (1 / _TerrainSize.x), (worldPos.z - _TerrainPosition.z) * (1 / _TerrainSize.z));

    oPositionOS = positionOS;
    if (_CheckHeight) 
    {
        float hm = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, sampler_TerrainHeightmapTexture, hmUV, 0));
        float heightOffset = _TerrainPosition.y - (hm * -_TerrainHeightmapScale.y);
        float3 objVertPos = GetCameraRelativePositionWS(TransformWorldToObject(float3(worldPos.x, heightOffset, worldPos.z)));
        oPositionOS = TransformWorldToObject(GetCameraRelativePositionWS(float3(worldPos.x, heightOffset, worldPos.z)));
    }

    oNormalOS = normalWS;
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
        oNormalOS = float4((terrainNormal).xyz, 0);
    }

	oNormalWS = normalWS;
    oPsitionWS = worldPos;
}

