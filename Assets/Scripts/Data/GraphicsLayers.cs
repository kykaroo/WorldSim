using UnityEngine.Tilemaps;
using Zenject;

namespace Data
{
    public class GraphicsLayers
    {
        public Tilemap WorldTilemap { get; }
        public Tilemap HighLightTilemap { get; }
        public Tilemap ConstructionTilemap { get; }

        [Inject]
        public GraphicsLayers(Tilemap worldTilemap, Tilemap highLightTilemap, Tilemap constructionTilemap)
        {
            WorldTilemap = worldTilemap;
            HighLightTilemap = highLightTilemap;
            ConstructionTilemap = constructionTilemap;
        }
    }
}