using Data;
using UnityEngine;
using Zenject;

namespace MapGenerator
{
    public class MapInfoGenerator
    {
        private readonly GenerationConfig _config;
        private readonly Tile _tilePrefab;
        private readonly WorldController _worldController;

        [Inject]
        public MapInfoGenerator(GenerationConfig config, Tile tilePrefab, WorldController worldController)
        {
            _config = config;
            _tilePrefab = tilePrefab;
            _worldController = worldController;
        }

        public void GenerateMap()
        {
            var configMapWidth = _config.mapWidth;
            var configMapHeight = _config.mapHeight;
            
            var noiseMap = Noise.GenerateNoiseMap(configMapWidth, configMapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);

            _worldController.FirstGeneration = true;
            _worldController.CreateNewData();
            
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

                        GenerateTile(x, y, tilesParent.transform, region);
                        break;
                    }
                }
            }
            
            _worldController.FirstGeneration = false;
        }

        private void GenerateTile(int x, int y, Transform tilesParent, RegionConfig region)
        {
            _worldController.CreateTile(x, y, region.tileType, tilesParent);
        }
    }
}