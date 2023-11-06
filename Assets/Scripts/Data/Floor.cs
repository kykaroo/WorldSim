using System.Linq;
using MapGenerator;

namespace Data
{
    public class Floor
    {
        private readonly Tile _tile;
        public float WalkSpeedMultiplier;
        private FloorTileType _type;
        private readonly GenerationConfig _config;
        
        public FloorTileType Type
        {
            get => _type;
            private set
            {
                _type = value;
                var building = _config.floorConfigs.First(b => b.type == _type);

                WalkSpeedMultiplier = building.moveSpeedMultiplier;
            }
        }

        public Floor(Tile tile, GenerationConfig config, FloorTileType type)
        {
            _tile = tile;
            _config = config;
            Type = type;
        }

        public void UninstallFloor()
        {
            _tile.UninstallFloor();
        }
    }
}