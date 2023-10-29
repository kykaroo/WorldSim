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

        public void DrawTexture(Texture2D texture)
        {
            _textureRenderer.sharedMaterial.mainTexture = texture; //Применив textureRenderer.material нельзя будет тестировать в эдиторе
            _textureRenderer.transform.localScale = new(texture.width, 1, texture.height); //Подстраивает размер объекта под размер текстуры
        }
    }
}