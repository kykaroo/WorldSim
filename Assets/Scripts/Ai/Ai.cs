using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using Pathfinding;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Ai
{
    public class Ai : IFixedTickable
    {
        private readonly List<IJob> _jobList;
        private readonly GenerationConfig _config;
        private readonly WorldController _worldController;
        private readonly List<Tile> _tilesToPlaceBuilding;
        private readonly TurnManager _turnManager;
        private readonly Pathfinder _pathfinder;
        
        private const int XCord = 50;
        private const int YCord = 50;
        private const float Time = 4;

        private readonly List<Pawn> _characters;
        private float _timer;

        [Inject]
        public Ai(GenerationConfig config, WorldController worldController, TurnManager turnManager)
        {
            _jobList = new();
            _tilesToPlaceBuilding = new();
            _characters = new();
            
            _config = config;
            _worldController = worldController;
            _timer = Time;
            _turnManager = turnManager;
            _turnManager.OnTurnTrigger += CharactersUpdate;
        }

        private void CharactersUpdate()
        {
            foreach (var character in _characters)
            {
                character.Update();
            }
        }

        private void PlaceBuilding()
        {
            if (!CheckValidation()) return;

            foreach (var tile in _tilesToPlaceBuilding)
            {
                tile.PendingBuildingJob = true;
            }

            var building = new Building(_tilesToPlaceBuilding, _config, BuildingsTileType.Statue);
            
            var job = new BuildingJob(building, 5f, _worldController);
            job.OnJobComplete += job1 => _jobList.Remove(job1);
            _jobList.Add(job);
        }

        private bool CheckValidation()
        {
            var building = _config.buildingConfigs.First(b => b.type == BuildingsTileType.Statue);
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

        public void CreatePop()
        {
            var character = new Pawn(_worldController.GetTile(50, 50), 1, _jobList, _pathfinder, _worldController);
            _characters.Add(character);
        }

        public void FixedTick()
        {
            _timer -= UnityEngine.Time.deltaTime;
            
            if (_timer >= 0) return;
            _timer = Time;
            PlaceBuilding();
        }
    }
}