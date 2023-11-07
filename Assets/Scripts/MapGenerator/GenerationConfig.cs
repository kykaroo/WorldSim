using System.Linq;
using Data;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(menuName = "MapGeneration/MapGenerationConfig", fileName = "Map Generation")]
    public class GenerationConfig : ScriptableObject
    {
        public BuildMode buildMode;
        public BuildingsTileType buildingsTileToPlace;
        public WorldTileType worldTileToPlace;
        public FloorTileType floorTileToPlace;
        public DrawMode drawMode;
        public Sprite characterSprite;
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
        public float buildTime;
        public bool instantBuild;

        public RegionConfig[] regions;
        public BuildingConfig[] buildingConfigs;
        public FloorConfig[] floorConfigs;

        public IOrderedEnumerable<RegionConfig> OrderedRegions;
    }
}