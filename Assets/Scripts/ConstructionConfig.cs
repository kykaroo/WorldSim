using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "TileConfig/ConstructionConfig", fileName = "Construction Config")]
public class ConstructionConfig : ScriptableObject
{
    public string Name;
    public ConstructionTileTypes type;
    public Sprite sprite;
    public Color color;
    public float moveSpeedMultiplier;
    public int width;
    public int height;
}