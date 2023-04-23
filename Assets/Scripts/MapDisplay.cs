using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public MeshFilter meshFilter2;
    public MeshRenderer meshRenderer2;

    public Texture2D DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        noiseMap = Normalize(noiseMap);

        Texture2D texture = new (width, height);
        Color[] colormap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++)
            {
                colormap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
            }
        }
        texture.SetPixels(colormap);
        texture.Apply();

        return texture;
    }

    public Texture2D DrawWaterMap(float[,] water)
    {
        int width = water.GetLength(0);
        int height = water.GetLength(1);

        water = Normalize(water);

        Texture2D texture = new(width, height);
        Color[] colormap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colormap[y * width + x] = Color.Lerp(Color.white, Color.blue, water[x, y]);
            }
        }
        texture.SetPixels(colormap);
        texture.Apply();

        return texture;
    }

    public Texture2D DrawErosionMap(float[,] erosion)
    {
        int width = erosion.GetLength(0);
        int height = erosion.GetLength(1);

        erosion = Normalize(erosion);

        Texture2D texture = new(width, height);
        Color[] colormap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colormap[y * width + x] = Color.Lerp(Color.red, Color.blue, erosion[x, y]);
            }
        }
        texture.SetPixels(colormap);
        texture.Apply();

        return texture;
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

        meshFilter2.sharedMesh = meshData.CreateMesh();
        meshRenderer2.sharedMaterial.mainTexture = texture;
    }

    public float[,] Normalize(float[,] matrix)
    {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);

        float max = Mathf.NegativeInfinity;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (matrix[x, y] > max)
                {
                    max = matrix[x, y];
                }
            }
        }

        float min = Mathf.Infinity;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (matrix[x, y] < min)
                {
                    min = matrix[x, y];
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                matrix[x, y] = Mathf.InverseLerp(min, max, matrix[x, y]);
            }
        }

        return matrix;
    }
}
