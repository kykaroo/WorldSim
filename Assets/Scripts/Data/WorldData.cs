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
        public readonly Tilemap WorldTilemap;
        public readonly Tilemap BuildingsTilemap;
        public readonly Tilemap FloorTilemap;
        public readonly Tilemap CharacterTilemap;

        [Inject]
        public WorldData(GraphicsLayers layers)
        {
            WorldTilemap = layers.WorldTilemap;
            BuildingsTilemap = layers.ConstructionTilemap;
            FloorTilemap = layers.FloorTilemap;
            CharacterTilemap = layers.CharacterTilemap;
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
        }
    }
}