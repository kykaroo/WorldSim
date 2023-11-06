using System.Collections.Generic;
using System.Linq;
using MapGenerator;

namespace Data
{
    public class InstalledObject
    {
        private readonly List<Tile> _tiles;
        public float WalkSpeedMultiplier;
        private ConstructionTileTypes _type;
        private int _width;
        private int _height;
        private readonly GenerationConfig _config;
        
        public ConstructionTileTypes Type
        {
            get => _type;
            private set
            {
                _type = value;
                var building = _config.constructionConfigs.First(b => b.type == _type);

                WalkSpeedMultiplier = building.moveSpeedMultiplier;
            }
        }

        public InstalledObject(List<Tile> tiles, GenerationConfig config, ConstructionTileTypes type)
        {
            _tiles = new(tiles);
            _config = config;
            Type = type;
        }

        public void UninstallObject()
        {
            foreach (var tile in _tiles)
            {
                tile.UninstallBuilding();
            }
        }
    }
}