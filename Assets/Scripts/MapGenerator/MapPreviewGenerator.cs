using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapPreviewGenerator
    {
        private readonly GenerationConfig _config;
        private readonly MapDisplay _display;
        private readonly TextureGenerator _textureGenerator;

        [Inject]
        public MapPreviewGenerator(GenerationConfig config, MapDisplay display, TextureGenerator textureGenerator)
        {
            _config = config;
            _display = display;
            _textureGenerator = textureGenerator;
            _config.OrderedRegions = _config.regions.OrderBy(b => b.height);
        }

        public void GenerateMapPreview()
        {
            var mapData = GenerateMapData();

            switch (_config.drawMode)
            {
                case DrawMode.NoiseMap:
                    _display.DrawTexture(_textureGenerator.TextureFromHeightMap(mapData.HeightMap), _config);
                    break;
                case DrawMode.ColorMap:
                    _display.DrawTexture(_textureGenerator.TextureFromColourMap(mapData.ColorMap, _config.mapWidth, _config.mapHeight), _config);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MapData GenerateMapData()
        {
            var noiseMap = Noise.GenerateNoiseMap(_config.mapWidth, _config.mapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);
            
            var colorMap = new Color[_config.mapWidth * _config.mapHeight];

            // Присваивает точке на карте цвет согласно регионам по высоте
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