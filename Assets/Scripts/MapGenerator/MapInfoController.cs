using Data;
using Zenject;

namespace MapGenerator
{
    public class MapInfoController
    {
        private readonly WorldController _worldController;
        private readonly MapGraphicGenerator _mapGraphicGenerator;
        private readonly MapInfoGenerator _mapInfoGenerator;

        public bool GenerationComplete;
        
        [Inject]
        public MapInfoController(WorldController worldController, MapGraphicGenerator mapGraphicGenerator, 
            MapInfoGenerator mapInfoGenerator)
        {
            _worldController = worldController;
            _mapGraphicGenerator = mapGraphicGenerator;
            _mapInfoGenerator = mapInfoGenerator;

            mapInfoGenerator.OnMapGenerationStart += () => GenerationComplete = false;
            mapInfoGenerator.OnMapGenerationComplete += () => GenerationComplete = true;
        }

        public void CreateMapGraphic()
        {
            _mapGraphicGenerator.GenerateMap();
        }
        
        public void CreateMapInfo()
        {
            _mapInfoGenerator.GenerateMap();
        }
        
        public void ClearMap()
        {
            _worldController.ClearAllTiles();
        }
    }
}