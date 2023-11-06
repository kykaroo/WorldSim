﻿using UnityEngine.Tilemaps;
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
        public readonly Tilemap ConstructionTilemap;

        [Inject]
        public WorldData(GraphicsLayers layers)
        {
            WorldTilemap = layers.WorldTilemap;
            ConstructionTilemap = layers.ConstructionTilemap;
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
        }
    }
}