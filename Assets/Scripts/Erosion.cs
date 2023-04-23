using UnityEngine;

public class Erosion
{
    public static (float[,], float[,], float[,]) Erode(float[,] heightMap, float[,] erosionMap, float[,] waterMap, int iterations, int maxLifetime, float switchSpeed, 
        float erosionRate, float depositRate, float evaporation, float gravity, int radius, float radiusFalloff
    )
    {
        if (switchSpeed < 0) { switchSpeed = 0; }

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        System.Random prng = new ();

        for(int i = 0; i < iterations; i++)
        {
            int x = prng.Next(0, width);
            int y = prng.Next(0, height);
            float sediment = 0f;
            float volume = 1f;

            for (int j = 0; j < maxLifetime; j++)
            {
                if (x < 1 || x > width - 2 || y < 1 || y > height - 2) { break; }

                float z = heightMap[x, y];
                Vector3[] dirs = new Vector3[] { new(x - 1, y, heightMap[x - 1, y]), new(x + 1, y, heightMap[x + 1, y]), new(x, y + 1, heightMap[x, y + 1]), new(x, y - 1, heightMap[x, y - 1]) };

                Vector3 dir = dirs[0];
                for(int k = 1; k < dirs.Length; k++)
                {
                    if (z - dirs[k].z > z - dir.z)
                    {
                        dir = dirs[k];
                    }
                }

                float deltaZ = z - dir.z;
                float speed = deltaZ * gravity;

                if(speed <= 0) {
                    //deposit and end if speed <= 0
                    float deposit = sediment * depositRate;
                    heightMap[x, y] = z + deposit;
                    erosionMap[x, y] += deposit;

                    radiusErosion(1, x, y, z, deposit, sediment);
                    break;
                }
                else if(speed < switchSpeed)
                {
                    //deposit
                    float deposit = Mathf.Min(sediment * depositRate * volume * Mathf.Min(speed, 1), deltaZ);
                    heightMap[(int)dir.x, (int)dir.y] = dir.z + deposit;
                    sediment -= deposit;
                    erosionMap[(int)dir.x, (int)dir.y] += deposit;

                    sediment = radiusErosion(1, x, y, z, deposit, sediment); //BUG
                }
                else
                {
                    //erode
                    float erode = deltaZ * erosionRate * volume * Mathf.Min(speed, 1);
                    heightMap[x, y] = z - erode;
                    sediment += erode;
                    erosionMap[x, y] -= erode;

                    sediment = radiusErosion(-1, x, y, z, erode, sediment);
                }

                waterMap[x, y] += volume;
                volume *= evaporation;
                x = (int)dir.x;
                y = (int)dir.y;
            }
        }

        return (heightMap, waterMap, erosionMap);


        // helper functions

        float radiusErosion(int m, int x, int y, float z, float amount, float sediment)
        {
            for (int d_x = -radius; d_x <= radius; d_x++)
            {
                for (int d_y = -radius; d_y <= radius; d_y++)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(d_x, 2) + Mathf.Pow(d_y, 2));
                    if(distance == 0) { break; }

                    float _amount = amount * Mathf.Pow(radiusFalloff, distance) * m;

                    int _x = x + d_x;
                    int _y = y + d_y;

                    if (_x >= 0 && _x < width && _y >= 0 && _y < height && m * (heightMap[_x, _y] - z) < 0)
                    {
                        heightMap[_x, _y] += _amount;
                        erosionMap[_x, _y] += _amount;
                        sediment -= _amount;
                    }
                }
            }

            return (sediment);
        }
    }
}
