using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class TextureGenerator
    {
        [Inject]
        public TextureGenerator() { }

        public Texture2D TextureFromColourMap(Color[] colorMap, int width, int height)
        {
            var texture = new Texture2D(width, height); //Инициализания графической карты
            texture.filterMode = FilterMode.Point; // Убирает блюр карты
            texture.wrapMode = TextureWrapMode.Clamp;
            //Применяет все цвета пикселей на текстуре
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            //Быстрее будет создать массив всех цветов всех пикселей а затем применить их все одновременно
            var colorMap = new Color[width * height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);

            return TextureFromColourMap(colorMap, width, height);
        }
    }
}