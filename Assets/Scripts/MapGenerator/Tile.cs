using System;
using Data;

namespace MapGenerator
{
    public class Tile : UnityEngine.Tilemaps.Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private TileType _type;
        private ConstructionTileTypes? _installedObject;
        public TileType Type
        {
            get => _type;
            set
            {
                _type = value;
                PassageCheck();
                
                OnTileTypeChanged?.Invoke(this);
            }
        }

        public bool IsPassable { get; private set; }
        public bool InstallObjectValid { get; private set; }
        public LooseObject LooseObject { get; private set; }

        public ConstructionTileTypes? InstalledObject
        {
            get => _installedObject;
            set
            {
                _installedObject = value;
                
                OnConstructionTileTypeChanged?.Invoke(this);
            }
        }
        public GenerationConfig Config { get; private set; }

        public event Action<Tile> OnTileTypeChanged;
        public event Action<Tile> OnConstructionTileTypeChanged;

        public void Initialize(int x, int y, TileType tileType, GenerationConfig config)
        {
            X = x;
            Y = y;
            Config = config;
            _type = tileType;
            
            PassageCheck();
        }
        
        private void PassageCheck()
        {
            switch (_type)
            {
                case TileType.Summit:
                    IsPassable = false;
                    InstallObjectValid = false;
                    break;
                default:
                    IsPassable = true;
                    InstallObjectValid = true;
                    break;
            }

            if (_installedObject == null) return;
            
            InstallObjectValid = false;
            IsPassable = false;
        }
    }
}