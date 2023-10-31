using System;
using Data;
using UnityEngine;

namespace MapGenerator
{
    public class Tile : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private TileType _type;
        public TileType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnTileTypeChanged?.Invoke(this);
            }
        }

        public bool IsPassable { get; private set; }
        public LooseObject LooseObject { get; private set; }
        public InstalledObject InstalledObject { get; private set; }
        public GenerationConfig Config { get; private set; }

        public event Action<Tile> OnTileTypeChanged;

        public void Initialize(int x, int y, TileType tileType, GenerationConfig config)
        {
            X = x;
            Y = y;
            Config = config;
            _type = tileType;
            
            IsPassable = tileType switch
            {
                TileType.Summit => false,
                _ => IsPassable = true
            };
        }
    }
}