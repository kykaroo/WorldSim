using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapGenerator
    {
        private readonly GenerationConfig _config;
        private readonly MapDisplay _display;
        private readonly TextureGenerator _textureGenerator;

        [Inject]
        public MapGenerator(GenerationConfig config, MapDisplay display, TextureGenerator textureGenerator)
        {
            _config = config;
            _display = display;
            _textureGenerator = textureGenerator;
            
            GenerateMap(_config.drawMode);
        }

        public void GenerateMap(DrawMode drawMode)
        {
            var mapData = GenerateMapData();

            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    _display.DrawTexture(_textureGenerator.TextureFromHeightMap(mapData.HeightMap));
                    break;
                case DrawMode.ColorMap:
                    _display.DrawTexture(_textureGenerator.TextureFromColourMap(mapData.ColorMap, _config.mapWidth, _config.mapHeight));
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
            _config.OrderedRegions = _config.regions.ToList().OrderBy(b => b.height);
            
            // Присваивает точке на карте цвет согласно регионам по высоте
            for (var y = 0; y < _config.mapHeight; y++)
            {
                for (var x = 0; x < _config.mapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    foreach (var region in _config.OrderedRegions)
                    {   
                        if (!(currentHeight <= region.height)) continue;
                        
                        colorMap[y * _config.mapHeight + x] = region.color;
                        break;
                    }
                }
            }

            return new(noiseMap, colorMap);
        }
    }
}