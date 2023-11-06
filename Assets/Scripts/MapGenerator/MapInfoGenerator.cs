using System;
using System.Threading.Tasks;
using Data;
using Zenject;

namespace MapGenerator
{
    public class MapInfoGenerator
    {
        private readonly GenerationConfig _config;
        private readonly WorldController _worldController;

        public event Action OnMapGenerationStart;
        public event Action OnMapGenerationComplete;

        [Inject]
        public MapInfoGenerator(GenerationConfig config, WorldController worldController)
        {
            _config = config;
            _worldController = worldController;
        }

        public async Task GenerateMap()
        {
            OnMapGenerationStart?.Invoke();
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

                        _worldController.CreateTile(x, y, region);
                        break;
                    }
                }

                await Task.Yield();
            }
            
            OnMapGenerationComplete?.Invoke();
        }
    }
}