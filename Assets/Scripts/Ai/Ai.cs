using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Ai
{
    public class Ai : ITickable
    {
        private readonly List<Job> _jobList;
        private readonly GenerationConfig _config;
        private readonly WorldController _worldController;
        private readonly List<Tile> _tilesToPlaceBuilding;

        private float _timer;
        private const float Time = 4;

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
            
            var job = new Job(_tilesToPlaceBuilding, ConstructionTileTypes.Statue, 2f);
            job.OnJobComplete += CompleteJob;
            _jobList.Add(job);
        }

        private void CompleteJob(Job job)
        {
            _worldController.InstallBuilding(job.Tiles, job.Building);
            _jobList.Remove(job);
        }

        private bool CheckValidation()
        {
            var xCord = 50;
            var yCord = 50;
            var building = _config.constructionConfigs.First(b => b.type == ConstructionTileTypes.Statue);
            var width = building.width;
            var height = building.height;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (TileValidation(xCord - x, yCord - y, width, height)) return true;
                    if (TileValidation(xCord - x, yCord - y, height, width)) return true;
                    if (TileValidation(xCord + x, yCord + y, width, height)) return true;
                    if (TileValidation(xCord + x, yCord + y, height, width)) return true;
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
            foreach (var job in _jobList)
            {
                if (job.DoWork(UnityEngine.Time.fixedDeltaTime))
                {
                    break;
                }
            }
            
            _timer -= UnityEngine.Time.deltaTime;
            
            if (_timer >= 0) return;
            _timer = Time;
            PlaceTile();
        }
    }
}