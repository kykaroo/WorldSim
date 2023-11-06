using Data;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(menuName = "MapGeneration/TileTypeConfig", fileName = "Tile Type")]
    public class RegionConfig : ScriptableObject
    {
        public string Name;
        public float height;
        public TileWorldType tileWorldType;
        public float moveSpeedMultiplier;
        public Sprite tileSprite;
        public Color color;
    }
}