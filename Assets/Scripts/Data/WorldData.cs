using UnityEngine.Tilemaps;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Data
{
    public class WorldData
    {
        public Tile TilePrefab;
        public Tile[,] Tiles;
        public int Width;
        public int Height;
        public readonly Tilemap Tilemap;

        [Inject]
        public WorldData(Tilemap tilemap)
        {
            Tilemap = tilemap;
        }

        public void Initialize(int width, int height, Tile tilePrefab)
        {
            Width = width;
            Height = height;
            TilePrefab = tilePrefab;
            Tiles = new Tile[width, height];
        }
    }
}