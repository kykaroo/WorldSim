using UnityEngine;
using Zenject;
using Random = System.Random;

namespace MapGenerator
{
    public class Noise
    {
        [Inject]
        public Noise() { }

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,
            float noiseScale, int octaves, float persistence, float lacunarity,
            Vector2 offset)
        {
            var noiseMap = new float[mapWidth, mapHeight];

            var rnd = new Random(seed);
            var octaveOffsets = new Vector2[octaves];

            for (var i = 0; i < octaves; i++)
            {
                var offsetX = rnd.Next(-100000, 100000) + offset.x;
                var offsetY = rnd.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new(offsetX, offsetY);
            }

            if (noiseScale <= 0)
            {
                noiseScale = 0.00001f;
            }
            
            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;
            
            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;
            
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (var i = 0; i < octaves; i++)
                    {
                        // Интовый x и у выдает одинаковый результат, поэтому делиться на scale
                        // Умножая на frequency отдаляются samplePoint, карта высот будет меняться сильнее
                        // Добавляя octaveOffsets значение точек еще больше искажается
                        var sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
                        var sampleY = (y - halfHeight) / noiseScale * frequency + octaveOffsets[i].y;
                        // Для получения более интересного шума числа умножаются на 2 и от них отнимается 1, что делает возможным получение отрицательных чисел
                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    //Возврщаем значения в рамки float
                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }
            
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
        
            return noiseMap;
        }
    }
}