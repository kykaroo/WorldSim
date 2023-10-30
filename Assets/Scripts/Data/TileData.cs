using UnityEngine;

namespace Data
{
    public class TileData : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public TileType TileType { get; private set; }
        public bool IsPassable { get; private set; }
        [field: SerializeField] public SpriteRenderer Renderer { get; private set; }

        public void Initialize(int x, int y, TileType tileType)
        {
            X = x;
            Y = y;
            TileType = tileType;

            IsPassable = tileType switch
            {
                TileType.Summit => false,
                _ => IsPassable = true
            };
        }
    }
}