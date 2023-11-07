using System.Collections.Generic;
using System.Linq;
using MapGenerator;

namespace Data
{
    public class Floor
    {
        public readonly List<Tile> Tiles;
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

        public Floor(IEnumerable<Tile> tiles, GenerationConfig config, FloorTileType type)
        {
            Tiles = new(tiles);
            _config = config;
            Type = type;
        }

        public void UninstallFloor()
        {
            foreach (var tile in Tiles)
            {
                tile.UninstallFloor();
            }
        }
    }
}