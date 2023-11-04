using System;
using MapGenerator;

namespace Data
{
    public class InstalledObject
    {
        public Tile Tile;
        public float WalkSpeedMultiplier;
        private ConstructionTileTypes _type;
        
        public ConstructionTileTypes Type
        {
            get => _type;
            set
            {
                _type = value;
                
                WalkSpeedMultiplier = Type switch
                {
                    ConstructionTileTypes.None => 1,
                    ConstructionTileTypes.Wall => 0,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                OnTileTypeChanged?.Invoke();
            }
        }

        public event Action OnTileTypeChanged;

        public InstalledObject(ConstructionTileTypes type, Tile tile)
        {
            Tile = tile;
            Type = type;
        }
    }
}