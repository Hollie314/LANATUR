using UnityEngine;
using Unity.Mathematics;
using System;
using System.IO;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine.Experimental.Rendering;


public class NoiseT3DGeneratorScript : MonoBehaviour
{


    public int imageSize = 32;
    public NoiseType noiseType = NoiseType.perlin_noise;
    public enum NoiseType
    {
        perlin_noise,
        simplex_noise,
        white_noise,
    }

    Texture2D textureGenerator3D(int imageSize, NoiseType noisetype)
    {
        //Texture3D outTexture = new Texture3D(imageSize, imageSize, imageSize, TextureFormat.RFloat, false);
        float value = 0;
        Texture2D sumTexture = new Texture2D((int)(imageSize * math.sqrt(imageSize)), (int)(imageSize * math.sqrt(imageSize)));
        Debug.Log("Starting Loop !");

        for (int zy = 0; zy < imageSize/2; zy++)
        {
            for (int zx = 0; zx < imageSize/2; zx++)
            {
                for (int x = 0; x < imageSize; x++)
                {
                    for (int y = 0; y < imageSize; y++)
                    {
                        float3 pos = new float3(x, y, zx + zy);
                        switch (noisetype)
                        {
                            case NoiseType.perlin_noise:
                                value = cnoise(pos);
                                break;
                            case NoiseType.simplex_noise:
                                value = snoise(pos);
                                break;
                            case NoiseType.white_noise:
                                value = rand3dTo1d(pos);
                                break;
                        }
                        sumTexture.SetPixel(x + (zx * imageSize), y + (zy * imageSize), new Color(value, value, value));
                    }
                }
            }
        }
        Debug.Log("Generation Done !");
        return sumTexture;
    }

    void textureBaker(Texture2D texture)
    {
        Debug.Log("Starting baking !");
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/Project/Material/Noisy-Nodes-master/";
        
        if(!Directory.Exists(dirPath)) 
        {
            Debug.Log("Creating folder !");
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "NoiseTexture" + ".png", bytes);
        Debug.Log("Baking done ! Path is : " + dirPath);
    }

    public void buttonClick(int imageSize, NoiseType noiseType)
    {
        textureBaker(textureGenerator3D(imageSize, noiseType));
    }

    float3 floor(float3 x)
    {
        return new float3(Mathf.Floor(x.x), Mathf.Floor(x.y), Mathf.Floor(x.z));
    }
    
    float4 floor(float4 x)
    {
        return new float4(Mathf.Floor(x.x), Mathf.Floor(x.y), Mathf.Floor(x.z), Mathf.Floor(x.w));
    }
    
    float floor(float x)
    {
        return Mathf.Floor(x);
    }

    float3 frac(float3 x)
    {
        return x - floor(x);
    }
    
    float4 frac(float4 x)
    {
        return x - floor(x);
    }
    
    float frac(float x)
    {
        return x - floor(x);
    }
    
    float3 mod289(float3 x)
    {
        return x - floor(x / 289.0f) * 289.0f;
    }
    
    float4 mod289(float4 x)
    {
        return x - floor(x / 289.0f) * 289.0f;
    }
    
    float4 permute(float4 x)
    {
        return mod289(((x*34.0f)+1.0f)*x);
    }

    float3 permute(float3 x)
    {
        return mod289((x * 34.0f + 1.0f) * x);
    }

    float3 abs(float3 x)
    {
        return new float3(Mathf.Abs(x.x), Mathf.Abs(x.y), Mathf.Abs(x.z));
    }
    
    float4 abs(float4 x)
    {
        return new float4(Mathf.Abs(x.x), Mathf.Abs(x.y), Mathf.Abs(x.z), Mathf.Abs(x.w));
        
    }

    float4 step(float4 x, float4 y)
    {
        return math.step(x, y);
    }
    float3 step(float3 x, float3 y)
    {
        return math.step(x, y);
    }
    
    float4 taylorInvSqrt(float4 r)
    {
        return (float4)1.79284291400159 - r * 0.85373472095314f;
    }

    float3 taylorInvSqrt(float3 r)
    {
        return 1.79284291400159f - 0.85373472095314f * r;
    }

    float dot(float4 x, float4 y)
    {
        return math.dot(x, y);
    }
    
    float dot(float3 x, float3 y)
    {
        return math.dot(x, y);
    }
    
    float3 fade(float3 t) {
        return t*t*t*(t*(t*6.0f-15.0f)+10.0f);
    }

    float2 fade(float2 t) {
        return t*t*t*(t*(t*6.0f-15.0f)+10.0f);
    }

    float4 lerp(float4 a, float4 b, float t)
    {
        return math.lerp(a, b, t);
    }
    
    float3 lerp(float3 a, float3 b, float t)
    {
        return math.lerp(a, b, t);
    }
    
    float2 lerp(float2 a, float2 b, float t)
    {
        return math.lerp(a, b, t);
    }

    float lerp(float a, float b, float t)
    {
        return math.lerp(a, b, t);
    }

    float3 min(float3 a, float3 b)
    {
        return math.min(a, b);
    }
    
    float4 min(float4 a, float4 b)
    {
        return math.min(a, b);
    }
    
    float3 max(float3 a, float3 b)
    {
        return math.max(a, b);
    }
    
    float4 max(float4 a, float4 b)
    {
        return math.max(a, b);
    }

    float3 sin(float3 x)
    {
        return math.sin(x);
    }
    
    float sin(float x)
    {
        return math.sin(x);
    }
    
    float cnoise(float3 P)
    {
      float3 Pi0 = floor(P); // Integer part for indexing
      float3 Pi1 = Pi0 + (float3)1.0; // Integer part + 1
      Pi0 = mod289(Pi0);
      Pi1 = mod289(Pi1);
      float3 Pf0 = frac(P); // Fractional part for interpolation
      float3 Pf1 = Pf0 - (float3)1.0; // Fractional part - 1.0
      float4 ix = new float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
      float4 iy = new float4(Pi0.y, Pi0.y, Pi1.y, Pi1.y);
      float4 iz0 = (float4)Pi0.z;
      float4 iz1 = (float4)Pi1.z;

      float4 ixy = permute(permute(ix) + iy);
      float4 ixy0 = permute(ixy + iz0);
      float4 ixy1 = permute(ixy + iz1);

      float4 gx0 = ixy0 / 7.0f;
      float4 gy0 = frac(floor(gx0) / 7.0f) - 0.5f;
      gx0 = frac(gx0);
      float4 gz0 = (float4)0.5 - abs(gx0) - abs(gy0);
      float4 sz0 = step(gz0, (float4)0.0);
      gx0 -= sz0 * (step((float4)0.0, gx0) - 0.5f);
      gy0 -= sz0 * (step((float4)0.0, gy0) - 0.5f);

      float4 gx1 = ixy1 / 7.0f;
      float4 gy1 = frac(floor(gx1) / 7.0f) - 0.5f;
      gx1 = frac(gx1);
      float4 gz1 = (float4)0.5 - abs(gx1) - abs(gy1);
      float4 sz1 = step(gz1, (float4)0.0);
      gx1 -= sz1 * (step((float4)0.0, gx1) - 0.5f);
      gy1 -= sz1 * (step((float4)0.0, gy1) - 0.5f);

      float3 g000 = new float3(gx0.x,gy0.x,gz0.x);
      float3 g100 = new float3(gx0.y,gy0.y,gz0.y);
      float3 g010 = new float3(gx0.z,gy0.z,gz0.z);
      float3 g110 = new float3(gx0.w,gy0.w,gz0.w);
      float3 g001 = new float3(gx1.x,gy1.x,gz1.x);
      float3 g101 = new float3(gx1.y,gy1.y,gz1.y);
      float3 g011 = new float3(gx1.z,gy1.z,gz1.z);
      float3 g111 = new float3(gx1.w,gy1.w,gz1.w);

      float4 norm0 = taylorInvSqrt(new float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
      g000 *= norm0.x;
      g010 *= norm0.y;
      g100 *= norm0.z;
      g110 *= norm0.w;

      float4 norm1 = taylorInvSqrt(new float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
      g001 *= norm1.x;
      g011 *= norm1.y;
      g101 *= norm1.z;
      g111 *= norm1.w;

      float n000 = dot(g000, Pf0);
      float n100 = dot(g100, new float3(Pf1.x, Pf0.y, Pf0.z));
      float n010 = dot(g010, new float3(Pf0.x, Pf1.y, Pf0.z));
      float n110 = dot(g110, new float3(Pf1.x, Pf1.y, Pf0.z));
      float n001 = dot(g001, new float3(Pf0.x, Pf0.y, Pf1.z));
      float n101 = dot(g101, new float3(Pf1.x, Pf0.y, Pf1.z));
      float n011 = dot(g011, new float3(Pf0.x, Pf1.y, Pf1.z));
      float n111 = dot(g111, Pf1);

      float3 fade_xyz = fade(Pf0);
      float4 n_z = lerp(new float4(n000, n100, n010, n110), new float4(n001, n101, n011, n111), fade_xyz.z);
      float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
      float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x);
      return 2.2f * n_xyz;
    }
    
    float snoise(float3 v)
    {
        //const float2 C = new float2(1.0f / 6.0f, 1.0f / 3.0f); A TESTER SANS CONST
        float2 C = new float2(1.0f / 6.0f, 1.0f / 3.0f);
        
        // First corner
        float3 i  = floor(v + dot(v, C.yyy));
        float3 x0 = v   - i + dot(i, C.xxx);

        // Other corners
        float3 g = step(x0.yzx, x0.xyz);
        float3 l = 1.0f - g;
        float3 i1 = min(g.xyz, l.zxy);
        float3 i2 = max(g.xyz, l.zxy);

        // x1 = x0 - i1  + 1.0 * C.xxx;
        // x2 = x0 - i2  + 2.0 * C.xxx;
        // x3 = x0 - 1.0 + 3.0 * C.xxx;
        float3 x1 = x0 - i1 + C.xxx;
        float3 x2 = x0 - i2 + C.yyy;
        float3 x3 = x0 - 0.5f;

        // Permutations
        i = mod289(i); // Avoid truncation effects in permutation
        float4 p =
          permute(permute(permute(i.z + new float4(0.0f, i1.z, i2.z, 1.0f))
                                + i.y + new float4(0.0f, i1.y, i2.y, 1.0f))
                                + i.x + new float4(0.0f, i1.x, i2.x, 1.0f));

        // Gradients: 7x7 points over a square, mapped onto an octahedron.
        // The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
        float4 j = p - 49.0f * floor(p / 49.0f);  // mod(p,7*7)

        float4 x_ = floor(j / 7.0f);
        float4 y_ = floor(j - 7.0f * x_);  // mod(j,N)

        float4 x = (x_ * 2.0f + 0.5f) / 7.0f - 1.0f;
        float4 y = (y_ * 2.0f + 0.5f) / 7.0f - 1.0f;

        float4 h = 1.0f - abs(x) - abs(y);

        float4 b0 = new float4(x.xy, y.xy);
        float4 b1 = new float4(x.zw, y.zw);

        //float4 s0 = float4(lessThan(b0, 0.0)) * 2.0 - 1.0;
        //float4 s1 = float4(lessThan(b1, 0.0)) * 2.0 - 1.0;
        float4 s0 = floor(b0) * 2.0f + 1.0f;
        float4 s1 = floor(b1) * 2.0f + 1.0f;
        float4 sh = -step(h, 0.0f);

        float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
        float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

        float3 g0 = new float3(a0.xy, h.x);
        float3 g1 = new float3(a0.zw, h.y);
        float3 g2 = new float3(a1.xy, h.z);
        float3 g3 = new float3(a1.zw, h.w);

        // Normalise gradients
        float4 norm = taylorInvSqrt(new float4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
        g0 *= norm.x;
        g1 *= norm.y;
        g2 *= norm.z;
        g3 *= norm.w;

        // Mix final noise value
        float4 m = max(0.6f - new float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0f);
        m = m * m;
        m = m * m;

        float4 px = new float4(dot(x0, g0), dot(x1, g1), dot(x2, g2), dot(x3, g3));
        return 42.0f * dot(m, px);
    }
    
    float rand3dTo1d(float3 value)
    {
        float3 dotDir = new float3(12.9898f, 78.233f, 37.719f);
        //make value smaller to avoid artefacts
        float3 smallValue = sin(value);
        //get scalar value from 3d vector
        float random = dot(smallValue, dotDir);
        //make value more random by making it bigger and then taking the factional part
        random = frac(sin(random) * 143758.5453f);
        return random;
    }
}




