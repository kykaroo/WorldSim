using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "TileConfig/BuildingConfig", fileName = "Building Config")]
public class BuildingConfig : ScriptableObject
{
    public string Name;
    public BuildingsTileType type;
    public Sprite sprite;
    public Color color;
    public float moveSpeedMultiplier;
    public int width;
    public int height;
}