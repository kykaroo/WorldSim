using Data;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapInfoController
    {
        private readonly WorldController _worldController;
        private readonly MapGraphicGenerator _mapGraphicGenerator;
        private readonly MapInfoGenerator _mapInfoGenerator;
        private readonly MapDisplay _mapDisplay;
        private readonly TextureGenerator _textureGenerator;
        private readonly GenerationConfig _config;
        private Texture2D _texture;

        [Inject]
        public MapInfoController(WorldController worldController, MapGraphicGenerator mapGraphicGenerator, 
            MapInfoGenerator mapInfoGenerator, MapDisplay mapDisplay, TextureGenerator textureGenerator,
            GenerationConfig config)
        {
            _worldController = worldController;
            _mapGraphicGenerator = mapGraphicGenerator;
            _mapInfoGenerator = mapInfoGenerator;
            _mapDisplay = mapDisplay;
            _textureGenerator = textureGenerator;
            _config = config;

            // _worldController.OnTileChanged += ChangePixel;
        }

        public void CreateMapGraphic()
        {
            _texture = _mapGraphicGenerator.GenerateMap();
        }
        
        public void CreateMapInfo()
        {
            _mapInfoGenerator.GenerateMap();
        }
        
        public void ClearMap()
        {
            _worldController.ClearAllTiles();
        }

        private void ChangePixel(Tile tile)
        {
            _texture = _textureGenerator.ChangePixel(_texture, tile.X, tile.Y, tile.Type, _config);
            _mapDisplay.DrawTexture(_texture, _config);
        }
    }
}