using Data;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapInfoGenerator
    {
        private readonly GenerationConfig _config;
        private readonly Tile _tilePrefab;
        private readonly WorldData _worldData;

        [Inject]
        public MapInfoGenerator(GenerationConfig config, Tile tilePrefab, WorldData worldData)
        {
            _config = config;
            _tilePrefab = tilePrefab;
            _worldData = worldData;
        }

        public void GenerateMap()
        {
            var configMapWidth = _config.mapWidth;
            var configMapHeight = _config.mapHeight;
            
            var noiseMap = Noise.GenerateNoiseMap(configMapWidth, configMapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);

            _worldData.FirstGeneration = true;
            _worldData.CreateNewData(configMapWidth, configMapHeight, _tilePrefab);
            
            var tilesParent = new GameObject
            {
                name = "WorldTiles",
                transform =
                {
                    position = Vector3.zero
                }
            };
            
            for (var y = 0; y < configMapHeight; y++)
            {
                for (var x = 0; x < configMapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    
                    foreach (var region in _config.OrderedRegions)
                    {   
                        if (!(currentHeight <= region.height)) continue;

                        GenerateTile(x, y, tilesParent.transform, region, _worldData);
                        break;
                    }
                }
            }
            
            _worldData.FirstGeneration = false;
        }

        private void GenerateTile(int x, int y, Transform tilesParent, RegionConfig region, WorldData worldData)
        {
            worldData.CreateTile(x, y, region.tileType, tilesParent);
        }
    }
}