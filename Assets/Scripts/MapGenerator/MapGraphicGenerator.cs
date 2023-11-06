using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapGraphicGenerator
    {
        private readonly GenerationConfig _config;
        private readonly MapDisplay _display;
        private readonly TextureGenerator _textureGenerator;
        private Texture2D _texture;

        [Inject]
        public MapGraphicGenerator(GenerationConfig config, MapDisplay display, TextureGenerator textureGenerator)
        {
            _config = config;
            _display = display;
            _textureGenerator = textureGenerator;
            _config.OrderedRegions = _config.regions.OrderBy(b => b.height);
        }

        public Texture2D GenerateMap()
        {
            var mapData = GenerateMapData();

            switch (_config.drawMode)
            {
                case DrawMode.NoiseMap:
                    _texture = _textureGenerator.TextureFromHeightMap(mapData.HeightMap);
                    _display.DrawTexture(_texture, _config);
                    return _texture;
                case DrawMode.ColorMap:
                    _texture = _textureGenerator.TextureFromColourMap(mapData.ColorMap, _config.mapWidth, _config.mapHeight);
                    _display.DrawTexture(_texture, _config);
                    return _texture;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MapData GenerateMapData()
        {
            var noiseMap = Noise.GenerateNoiseMap(_config.mapWidth, _config.mapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);
            
            var colorMap = new Color[_config.mapWidth * _config.mapHeight];
            
            for (var y = 0; y < _config.mapHeight; y++)
            {
                for (var x = 0; x < _config.mapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    
                    foreach (var region in _config.OrderedRegions)
                    {   
                        if (!(currentHeight <= region.height)) continue;
                        
                        colorMap[y * _config.mapWidth + x] = region.color;
                        break;
                    }
                }
            }

            return new(noiseMap, colorMap);
        }
    }
}