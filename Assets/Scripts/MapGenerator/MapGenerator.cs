using System;
using Data;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace MapGenerator
{
    public class MapGenerator
    {
        private readonly GenerationConfig _config;
        private WorldData _worldData;
        private TileData _tilePrefab;

        [Inject]
        public MapGenerator(GenerationConfig config, TileData tilePrefab)
        {
            _config = config;
            _tilePrefab = tilePrefab;
        }

        public void GenerateMap()
        {
            var configMapWidth = _config.mapWidth;
            var configMapHeight = _config.mapHeight;
            
            var noiseMap = Noise.GenerateNoiseMap(configMapWidth, configMapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);
            
            var tilesData = new TileData[configMapWidth, configMapHeight];
            _worldData?.ClearAllTiles();
            _worldData = new(configMapWidth, configMapHeight);
            var tilesParent = new GameObject
            {
                name = "WorldTiles",
                transform =
                {
                    position = Vector3.zero
                }
            };

            // Присваивает точке на карте цвет согласно регионам по высоте
            for (var y = 0; y < configMapHeight; y++)
            {
                for (var x = 0; x < configMapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    
                    foreach (var region in _config.OrderedRegions)
                    {   
                        if (!(currentHeight <= region.height)) continue;

                        tilesData[x, y] = GenerateTile(x, y, tilesParent, region, _worldData, currentHeight);
                        break;
                    }
                }
            }
        }

        private TileData GenerateTile(int x, int y, GameObject tilesParent, RegionConfig region, WorldData worldData, float currentHeight)
        {
            var drawMode = _config.drawMode;
            var tile = Object.Instantiate(_tilePrefab, new(x, y, 0), quaternion.identity, tilesParent.transform);
            tile.name = $"Tile_{x}_{y}";
            
            var tileData = tile.GetComponent<TileData>();
            tileData.Initialize(x, y, region.tileType);

            tileData.Renderer.material.color = drawMode switch
            {
                DrawMode.NoiseMap => Color.Lerp(Color.black, Color.white, currentHeight),
                DrawMode.ColorMap => region.color,
                _ => throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null)
            };

            worldData.AddTile(tileData);
            
            return tileData;
        }
    }
}