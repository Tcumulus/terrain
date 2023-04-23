using System.Collections;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int octaves, float persistance, float lacunarity, float frequency, float amplitude, float steepness, int seed)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        System.Random prng = new (seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float noiseHeight = 0f;
                float _amplitude = amplitude;
                float _frequency = frequency;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x * _frequency + octaveOffsets[i].x;
                    float sampleY = y * _frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.Abs(Mathf.PerlinNoise(sampleX, sampleY)-0.5f);
                    noiseHeight += perlinValue * _amplitude;
                    noiseHeight = Mathf.Pow(noiseHeight, steepness);

                    _amplitude *= persistance;
                    _frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight) { maxNoiseHeight = noiseHeight; }
                else if(noiseHeight < minNoiseHeight) { minNoiseHeight = noiseHeight; }
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
