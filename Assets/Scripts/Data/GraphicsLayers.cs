using UnityEngine.Tilemaps;
using Zenject;

namespace Data
{
    public class GraphicsLayers
    {
        public Tilemap WorldTilemap { get; }
        public Tilemap HighlightTilemap { get; }
        public Tilemap ConstructionTilemap { get; }
        public Tilemap FloorTilemap{ get; }

        [Inject]
        public GraphicsLayers(Tilemap worldTilemap, Tilemap highlightTilemap, Tilemap constructionTilemap, Tilemap floorTilemap)
        {
            WorldTilemap = worldTilemap;
            HighlightTilemap = highlightTilemap;
            ConstructionTilemap = constructionTilemap;
            FloorTilemap = floorTilemap;
        }
    }
}