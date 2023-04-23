using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMatGenerator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Color forest;
    public Color grass;
    public Color rock;
    public Color snow;
    public Color swamp;

    public float forestBoundary;
    public float rockBoundary;
    public float snowBoundary;
    public float rockSteepness;
    public float swampBoundary;

    public float heightTransition;
    public float steepnessTransition;
    public float swampTransition;
    public int swampRadius;

    public Texture2D GenerateTexture(float[,] terrain, float[,] water)
    {
        int width = terrain.GetLength(0);
        int height = terrain.GetLength(1);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        terrain = display.Normalize(terrain);
        water = display.Normalize(water);

        Texture2D texture = new(width, height);
        Color[] colormap = new Color[width * height];

        for (int y = 0; y < height-1; y++)
        {
            for (int x = 0; x < width-1; x++)
            {
                float steepness = GetSteepness(terrain, x, y);
                float swampness = GetSwampness(water, x, y, width, height);
                Color colorVal;

                
                if (terrain[x, y] > forestBoundary + heightTransition)
                {
                    colorVal = grass;
                }

                else if (terrain[x, y] > forestBoundary - heightTransition)
                {
                    float normalizedTransition = (terrain[x, y] - forestBoundary + heightTransition) / (2 * heightTransition);
                    colorVal = Color.Lerp(forest, grass, normalizedTransition);
                }
                else
                {
                    colorVal = forest;
                }

                if(swampness > swampBoundary + swampTransition)
                {
                    colorVal = swamp;
                }
                else if(swampness > swampBoundary - swampTransition)
                {
                    float normalizedTransition = (swampness - swampBoundary + swampTransition) / (2 * swampTransition);
                    colorVal = Color.Lerp(colorVal, swamp, normalizedTransition);
                }

                if (terrain[x, y] > snowBoundary + heightTransition)
                {
                    colorVal = snow;
                }
                else if (terrain[x, y] > snowBoundary - heightTransition)
                {
                    float normalizedTransition = (terrain[x, y] - snowBoundary + heightTransition) / (2 * heightTransition);
                    colorVal = Color.Lerp(rock, snow, normalizedTransition);
                }
                else if (terrain[x, y] > rockBoundary + heightTransition)
                {
                    colorVal = rock;
                }
                else if (terrain[x, y] > rockBoundary - heightTransition)
                {
                    float normalizedTransition = (terrain[x, y] - rockBoundary + heightTransition) / (2 * heightTransition);
                    colorVal = Color.Lerp(colorVal, rock, normalizedTransition);
                }

                if (steepness > rockSteepness + steepnessTransition)
                {
                    colorVal = rock;
                }
                else if(steepness > rockSteepness - steepnessTransition)
                {
                    float normalizedTransition = (steepness - rockSteepness + steepnessTransition) / (2 * steepnessTransition);
                    colorVal = Color.Lerp(colorVal, rock, normalizedTransition);
                }

                colormap[y * width + x] = colorVal;
            }
        }

        texture.SetPixels(colormap);
        texture.Apply();

        return texture;
    }

    float GetSwampness(float[,] water, int x, int y, int width, int height)
    {
        //boundary check
        if (x - swampRadius < 0 || x + swampRadius >= width || y - swampRadius < 0 || y + swampRadius >= height) return 0f;

        float swampness = 0f;
        for(int dx = -swampRadius; dx <= swampRadius; dx++)
        {
            for(int dy = -swampRadius; dy <= swampRadius; dy++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Sqrt(Mathf.Pow(dy, 2)));
                if (distance == 0) swampness += water[x, y];
                else swampness += 1 / distance * water[x + dx, y + dy]; //inverse relationship with distance
            }
        }
        return swampness;
    }

    float GetSteepness(float[,] heightmap, int x, int y)
    {
        float height = heightmap[x, y];
        float dx = heightmap[x + 1, y] - height;
        float dy = heightmap[x, y + 1] - height;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
