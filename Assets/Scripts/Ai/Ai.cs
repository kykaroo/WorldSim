using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Ai
{
    public class Ai : ITickable, IFixedTickable
    {
        private readonly List<Job> _jobList;
        private readonly GenerationConfig _config;
        private readonly WorldController _worldController;
        private readonly List<Tile> _tilesToPlaceBuilding;
        
        private const int XCord = 50;
        private const int YCord = 50;
        private const float Time = 4;

        private float _timer;

        [Inject]
        public Ai(GenerationConfig config, WorldController worldController)
        {
            _jobList = new();
            _tilesToPlaceBuilding = new();
            _config = config;
            _worldController = worldController;
            _timer = Time;
        }

        private void PlaceTile()
        {
            if (!CheckValidation()) return;

            foreach (var tile in _tilesToPlaceBuilding)
            {
                tile.PendingBuildingJob = true;
            }
            
            var job = new Job(_tilesToPlaceBuilding, ConstructionTileTypes.Statue, 5f, _worldController);
            job.OnJobComplete += job1 => _jobList.Remove(job1);
            _worldController.InstallBuilding(job.Tiles, job.Building);
            _jobList.Add(job);
        }

        private bool CheckValidation()
        {
            var building = _config.constructionConfigs.First(b => b.type == ConstructionTileTypes.Statue);
            var width = building.width;
            var height = building.height;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (TileValidation(XCord - x, YCord - y, width, height)) return true;
                    if (TileValidation(XCord - x, YCord - y, height, width)) return true;
                    if (TileValidation(XCord + x, YCord + y, width, height)) return true;
                    if (TileValidation(XCord + x, YCord + y, height, width)) return true;
                }
            }

            return false;
        }

        private bool TileValidation(int xCord, int yCord, int width, int height)
        {
            _tilesToPlaceBuilding.Clear();

            for (var x = xCord; x <=  xCord + width - 1; x++)
            {
                for (var y = yCord; y <= yCord + height - 1; y++)
                {
                    var tile = _worldController.GetTile(x, y);

                    if (tile == null) return false;
                    if (!tile.BuildingValid || tile.PendingBuildingJob) return false;
                    
                    _tilesToPlaceBuilding.Add(tile);
                }
            }
            
            return _tilesToPlaceBuilding.Count == width * height;
        }

        public void Tick()
        {
            _timer -= UnityEngine.Time.deltaTime;
            
            if (_timer >= 0) return;
            _timer = Time;
            PlaceTile();
        }

        public void FixedTick()
        {
            foreach (var job in _jobList)
            {
                if (job.DoWork(UnityEngine.Time.fixedDeltaTime))
                {
                    break;
                }
            }
        }
    }
}