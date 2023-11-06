using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "TileConfig/FloorConfig", fileName = "Floor Config")]
    public class FloorConfig : ScriptableObject
    { 
        public string Name;
        public FloorTileType type;
        public Sprite sprite;
        public float moveSpeedMultiplier;
    }
}