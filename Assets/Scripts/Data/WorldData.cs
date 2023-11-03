using UnityEngine.Tilemaps;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Data
{
    public class WorldData
    {
        public Tile[,] Tiles;
        public int Width;
        public int Height;
        public readonly Tilemap Tilemap;

        [Inject]
        public WorldData(Tilemap tilemap)
        {
            Tilemap = tilemap;
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
        }
    }
}