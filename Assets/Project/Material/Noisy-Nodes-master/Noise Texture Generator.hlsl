#include "Assets/Project/Material/Noisy-Nodes-master/NoiseShader/HLSL/ClassicNoise3D.hlsl"
#include "Assets/Project/Material/Noisy-Nodes-master/NoiseShader/HLSL/SimplexNoise3D.hlsl"

enum NoiseType
{
    perlin_noise,
    simplex_noise,
    white_noise,
    
};

Texture3D<float> textureGenerator3D(int imageSize, NoiseType noisetype)
{
    Texture3D<float> outTexture;

    for (int x = 0; x < imageSize; x++)
    {
        for (int y = 0; y < imageSize; y++)
        {
            for (int z = 0; z < imageSize; z++)
            {
                float3 pos = float3(x, y, z);
                float value;
                switch (noisetype)
                {
                    case perlin_noise:
                        value = cnoise(pos);
                        break;
                    case simplex_noise:
                        value = snoise(pos);
                        break;
                    case white_noise:
                        value = rand3dTo1d(pos);
                        break;                    
                }
            }
        }
    }
    
    
}
