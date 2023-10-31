﻿using System.Linq;
using Data;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(menuName = "MapGeneration/MapGenerationConfig", fileName = "Map Generation")]
    public class GenerationConfig : ScriptableObject
    {
        public Tile tilePrefab;
        public DrawMode drawMode;
        public TileType tileToPlace;
        public int tileSize;

        public int mapWidth;
        public int mapHeight;
        [Range(0, 6)] 
        public int levelOfDetail;
        public float noiseScale;
    
        public int octaves;
        [Range(0,1)]
        public float persistence;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool autoUpdate;

        public RegionConfig[] regions;

        public IOrderedEnumerable<RegionConfig> OrderedRegions;
    }
}