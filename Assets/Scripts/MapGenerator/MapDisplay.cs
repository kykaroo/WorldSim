using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapDisplay
    {
        private readonly SpriteRenderer _textureRenderer;

        [Inject]
        public MapDisplay(SpriteRenderer renderer)
        {
            _textureRenderer = renderer;
        }

        public void DrawTexture(Texture2D texture, GenerationConfig config)
        {
            var width = config.mapWidth;
            var height = config.mapHeight;
            var tileSize = config.tileSize;
            
            _textureRenderer.sprite = Sprite.Create(texture, new(0,0, texture.width, texture.height), new(0.5f, 0.5f), 1);
            _textureRenderer.transform.position = new (width / 2f - tileSize / 2f, height / 2f - tileSize / 2f, 0.001f);
        }
    }
}