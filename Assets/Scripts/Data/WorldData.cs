using MapGenerator;
using Zenject;

namespace Data
{
    public class WorldData
    {
        public Tile TilePrefab;
        public Tile[,] Tiles;
        public int Width;
        public int Height;

        [Inject]
        public WorldData() { }

        public void Initialize(int width, int height, Tile tilePrefab)
        {
            Width = width;
            Height = height;
            TilePrefab = tilePrefab;
            Tiles = new Tile[width, height];
        }
    }
}