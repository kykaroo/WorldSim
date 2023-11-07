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
        public bool IsTaskPerformed { get; set; }
        public Building Building { get; }
        public float ConstructionProgress { get; set; }

        public event Action<IJob> OnJobComplete;
        public event Action<IJob> OnJobCancel;

        public BuildingJob(Building building, float totalJob, WorldController worldController)
        {
            Tiles = building.Tiles;
            TotalJob = totalJob;
            Building = building;
            _worldController = worldController;
            IsTaskPerformed = false;

            foreach (var tile in Tiles)
            {
                tile.PendingBuildingJob = true;
                tile.BuildingComplete = false;
            }
        }

        public bool DoWork(float workTime)
        {
            _jobTime += workTime;
            ConstructionProgress = _jobTime / TotalJob;
            _worldController.UpdateBuildingConstructionProgress(this);

            if (!(_jobTime >= TotalJob)) return false;

            ConstructionProgress = 1;

            foreach (var tile in Tiles)
            {
                tile.PendingBuildingJob = false;
                tile.BuildingComplete = true;
            }
            
            OnJobComplete?.Invoke(this);
            return true;
        }

        public void CancelJob()
        {
            foreach (var tile in Tiles)
            {
                tile.PendingBuildingJob = false;
            }
            
            OnJobCancel?.Invoke(this);
        }
    }
}