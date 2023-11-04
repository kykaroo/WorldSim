using System;
using Data;

namespace MapGenerator
{
    public class Tile : UnityEngine.Tilemaps.Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private TileWorldType _worldType;
        private float _tileWalkSpeedMultiplier;
        public TileWorldType WorldType
        {
            get => _worldType;
            set
            {
                _worldType = value;
                _tileWalkSpeedMultiplier = _worldType switch
                {
                    TileWorldType.Summit => 0,
                    TileWorldType.Water => 0.1f,
                    TileWorldType.Sand => 0.9f,
                    TileWorldType.Grass => 1,
                    TileWorldType.Rocks => 0.9f,
                    TileWorldType.Mountain => 0.4f,
                    _ => 0
                };
                
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

        public void Initialize(int x, int y, TileWorldType tileWorldType)
        {
            X = x;
            Y = y;
            WorldType = tileWorldType;
            InstalledObject = new(ConstructionTileTypes.None, this);
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
        }

        public void PlaceConstruction(ConstructionTileTypes type)
        {
            InstalledObject.Type = type;
        }
    }
}