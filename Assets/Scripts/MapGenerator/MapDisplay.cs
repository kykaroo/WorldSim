using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapDisplay
    {
        private readonly Renderer _textureRenderer;

        [Inject]
        public MapDisplay(Renderer renderer)
        {
            _textureRenderer = renderer;
        }

        public void DrawTexture(Texture2D texture, GenerationConfig config)
        {
            var width = config.mapWidth;
            var height = config.mapHeight;
            var tileSize = config.tileSize;
            
            _textureRenderer.sharedMaterial.mainTexture = texture; //Применив textureRenderer.material нельзя будет тестировать в эдиторе
            _textureRenderer.transform.localScale = new(1 * width / 10f, 1, 1 * height / 10f); //Подстраивает размер объекта под размер текстуры
            _textureRenderer.transform.position = new (width / 2f - tileSize / 2f, height / 2f - tileSize / 2f, 0.001f);
        }
    }
}