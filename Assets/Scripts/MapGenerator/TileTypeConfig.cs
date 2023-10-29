using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(menuName = "MapGeneration/TileTypeConfig", fileName = "Tile Type")]
    public class TileTypeConfig : ScriptableObject
    {
        public string Name;
        public float height;
        public Color color;
    }
}