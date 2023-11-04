using System;
using System.Linq;
using Data;

namespace MapGenerator
{
    public class Tile : UnityEngine.Tilemaps.Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private GenerationConfig Config { get; set; }
        private TileWorldType _worldTileType;
        private float _tileWalkSpeedMultiplier;
        public TileWorldType WorldType
        {
            get => _worldTileType;
            set
            {
                _worldTileType = value;
                _tileWalkSpeedMultiplier =
                    Config.regions.First(region => region.tileWorldType == _worldTileType).moveSpeedMultiplier;
                
                RecalculateSpeed();
                OnTileTypeChanged?.Invoke(this);
            }
        }

        public float WalkSpeedMultiplier { get; private set; }
        
        public bool InstallObjectValid { get; private set; }
        public LooseObject LooseObject { get; private set; }

        public InstalledObject InstalledObject { get; private set; }

        public event Action<Tile> OnTileTypeChanged;
        public event Action<Tile> OnConstructionTileTypeChanged;

        private void ConstructionTileTypeChanged()
        {
            RecalculateSpeed();
            OnConstructionTileTypeChanged?.Invoke(this);
        }

        public void Initialize(int x, int y, TileWorldType tileWorldType, GenerationConfig config)
        {
            X = x;
            Y = y;
            Config = config;
            
            WorldType = tileWorldType;
            InstalledObject = new(this, config);
            InstalledObject.OnTileTypeChanged += ConstructionTileTypeChanged;
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            if (InstalledObject == null)
            {
                WalkSpeedMultiplier = _tileWalkSpeedMultiplier;
                return;
            }
            
            WalkSpeedMultiplier = _tileWalkSpeedMultiplier * InstalledObject.WalkSpeedMultiplier;

            if (WalkSpeedMultiplier == 0)
            {
                InstallObjectValid = false;
                return;
            }

            InstallObjectValid = true;
        }
    }
}