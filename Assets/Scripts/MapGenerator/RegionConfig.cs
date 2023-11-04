using Data;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(menuName = "MapGeneration/TileTypeConfig", fileName = "Tile Type")]
    public class RegionConfig : ScriptableObject
    {
        public TileWorldType tileWorldType;
        public Sprite tileSprite;
        public Material tileMaterial;
        public string Name;
        public float height;
        public Color color;
    }
}