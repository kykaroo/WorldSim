using System;
using System.Collections.Generic;
using Data;
using MapGenerator;

namespace Ai
{
    public class BuildingJob : IJob
    {
        private readonly WorldController _worldController;
        private float _jobTime;

        public float TotalJob { get; }
        public List<Tile> Tiles { get; }
        public BuildingsTileType Building { get; }
        public float ConstructionProgress { get; set; }

        public event Action<IJob> OnJobComplete;
        public event Action<IJob> OnJobCancel;

        public BuildingJob(IEnumerable<Tile> tiles, BuildingsTileType building, float totalJob, WorldController worldController)
        {
            Tiles = new(tiles);
            TotalJob = totalJob;
            Building = building;
            _worldController = worldController;
        }

        public bool DoWork(float workTime)
        {
            _jobTime += workTime;
            ConstructionProgress = _jobTime / TotalJob;
            _worldController.UpdateBuildingConstructionProgress(this);

            if (!(_jobTime >= TotalJob)) return false;

            ConstructionProgress = 1;
            OnJobComplete?.Invoke(this);
            return true;

        }

        public void CancelJob()
        {
            OnJobCancel?.Invoke(this);
        }
    }
}