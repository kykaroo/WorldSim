using System.Threading.Tasks;
using Data;
using Zenject;

namespace MapGenerator
{
    public class MapInfoGenerator
    {
        private readonly GenerationConfig _config;
        private readonly WorldController _worldController;

        [Inject]
        public MapInfoGenerator(GenerationConfig config, WorldController worldController)
        {
            _config = config;
            _worldController = worldController;
        }

        public async Task GenerateMap()
        {
            var configMapWidth = _config.mapWidth;
            var configMapHeight = _config.mapHeight;
            
            var noiseMap = Noise.GenerateNoiseMap(configMapWidth, configMapHeight, _config.seed, _config.noiseScale,
                _config.octaves, _config.persistence, _config.lacunarity, _config.offset);
            
            _worldController.CreateNewData();

            for (var y = 0; y < configMapHeight; y++)
            {
                for (var x = 0; x < configMapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    
                    foreach (var region in _config.OrderedRegions)
                    {   
                        if (!(currentHeight <= region.height)) continue;

                        GenerateTile(x, y, region);
                        break;
                    }
                }

                await Task.Yield();
            }
        }

        private void GenerateTile(int x, int y, RegionConfig region)
        {
            _worldController.CreateTile(x, y, region);
        }
    }
}