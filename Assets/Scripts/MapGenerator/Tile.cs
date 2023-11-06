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
        
        public bool BuildingValid { get; private set; }
        public bool PendingBuildingJob { get; set; }
        public TileInventory TileInventory { get; private set; }

        public Building Building { get; private set; }

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
            PendingBuildingJob = false;
            
            WorldType = tileWorldType;
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            WalkSpeedMultiplier = _tileWalkSpeedMultiplier * Building?.WalkSpeedMultiplier ?? _tileWalkSpeedMultiplier;

            BuildingValid = WalkSpeedMultiplier != 0;
        }
        
        public void InstallBuilding(Building building)
        {
            Building = building;
            ConstructionTileTypeChanged();
        }

        public void UninstallBuilding()
        {
            Building = null;
            ConstructionTileTypeChanged();
        }
    }
}