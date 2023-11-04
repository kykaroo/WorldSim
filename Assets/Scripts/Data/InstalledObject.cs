using System;
using System.Collections.Generic;
using System.Linq;
using MapGenerator;

namespace Data
{
    public class InstalledObject
    {
        public List<Tile> MainTile;
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


                OnTileTypeChanged?.Invoke();
            }
        }

        public event Action OnTileTypeChanged;

        public InstalledObject(Tile mainTile, GenerationConfig config)
        {
            _config = config;
            Type = ConstructionTileTypes.None;
        }

        public void InstallObject(ConstructionTileTypes type)
        {
            Type = type;
        }
    }
}