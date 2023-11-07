using System.Collections.Generic;
using System.Linq;
using MapGenerator;

namespace Data
{
    public class Building
    {
        public readonly List<Tile> Tiles;
        public float WalkSpeedMultiplier;
        private BuildingsTileType _type;
        private int _width;
        private int _height;
        private readonly GenerationConfig _config;
        
        public BuildingsTileType Type
        {
            get => _type;
            private set
            {
                _type = value;
                var building = _config.buildingConfigs.First(b => b.type == _type);

                WalkSpeedMultiplier = building.moveSpeedMultiplier;
            }
        }

        public Building(IEnumerable<Tile> tiles, GenerationConfig config, BuildingsTileType type)
        {
            Tiles = new(tiles);
            _config = config;
            Type = type;
        }

        public void UninstallBuilding()
        {
            foreach (var tile in Tiles)
            {
                tile.UninstallBuilding();
            }
        }
    }
}