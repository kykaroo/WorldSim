using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = MapGenerator.Tile;

namespace Data
{
    [CreateAssetMenu(menuName = "MapGeneration/TileConfig", fileName = "Tile Config")]
    public class TileData : TileBase
    {
        [field: SerializeField] public GameObject TilePrefab { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public TileType TileType { get; private set; }
        public bool IsPassable { get; private set; }

        public TileData Initialize(int x, int y, TileType tileType, Transform parent)
        {
            var gameObject = Instantiate(TilePrefab, new(x,y,0), Quaternion.identity, parent);
            gameObject.GetComponent<Tile>().Initialize(x, y);

            gameObject.name = $"Tile_{x}_{y}";
            
            X = x;
            Y = y;
            TileType = tileType;

            IsPassable = tileType switch
            {
                TileType.Summit => false,
                _ => IsPassable = true
            };

            return this;
        }
    }
}