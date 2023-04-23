using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {Mesh};
    public DrawMode drawMode;

    public int width;
    public int height;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public float frequency;
    public float amplitude;
    [Range(0.7f, 1.5f)]
    public float steepness;
    public bool randomSeed;
    public int seed;

    //erosion
    public int iterations;
    public int maxLifetime;
    [Range(0f, 2.5f)]
    public float switchSpeed;
    public float erosionRate;
    public float depositRate;
    [Range(0, 1)]
    public float evaporation;
    public float gravity;
    [Range(0, 5)]
    public int radius;
    [Range(0, 1)]
    public float radiusFalloff;

    public bool autoUpdate;
    public bool erosion;
    public bool radialErosion;
    public bool showWater;
    public bool showErosion;
    public bool terrain;

    private Texture2D map = null;

    public void GenerateMap()
    {
        if(randomSeed) { seed = Random.Range(0, 1000000); }

        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, octaves, persistance, lacunarity, frequency, amplitude, steepness, seed);
        float[,] waterMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];
        float[,] erosionMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];

        if(erosion && !radialErosion)
        {
            (noiseMap, waterMap, erosionMap) = Erosion.Erode(noiseMap, erosionMap, waterMap, iterations, maxLifetime, switchSpeed, erosionRate, depositRate, evaporation, gravity, radius, radiusFalloff);
        }
        if(erosion && radialErosion && radius > 0)
        {
            for(int r = radius; r >= 0; r--)
            {
                (noiseMap, waterMap, erosionMap) = Erosion.Erode(noiseMap, erosionMap, waterMap, iterations / radius, maxLifetime, switchSpeed, erosionRate, depositRate, evaporation, gravity, r, radiusFalloff);
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        TerrainMatGenerator terrainMat = FindObjectOfType<TerrainMatGenerator>();
        MeshData meshData = MeshGenerator.GenerateMesh(noiseMap, amplitude);

        if(erosion && showWater)
        {
            Texture2D texture = display.DrawWaterMap(waterMap);
            display.DrawMesh(meshData, texture);
        }
        else if(erosion && showErosion)
        {
            Texture2D texture = display.DrawErosionMap(erosionMap);
            display.DrawMesh(meshData, texture);
        }
        else if(terrain)
        {
            Texture2D texture = terrainMat.GenerateTexture(noiseMap, waterMap);
            display.DrawMesh(meshData, texture);
        }
        else
        {
            Texture2D texture = display.DrawNoiseMap(noiseMap);
            display.DrawMesh(meshData, texture);
        }

        map = display.DrawNoiseMap(noiseMap);
    }

    public void ExportMap()
    {
        if(!map) { return; }
        byte[] bytes = map.EncodeToPNG();
        var dirPath = Application.dataPath + "/RenderOutput";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/R_" + Random.Range(0, 100000) + ".png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
    }

    void OnValidate()
    {
        if (width < 1)
        {
            width = 1;
        }
        if (height < 1)
        {
            height = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}
