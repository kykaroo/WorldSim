using System;
using System.Collections.Generic;
using Data;
using MapGenerator;

namespace Ai
{
    public class FloorJob : IJob
    {
        private readonly WorldController _worldController;
        private float _jobTime;
        public float TotalJob { get; }
        public List<Tile> Tiles { get; }
        public FloorTileType Floor { get; }
        public float ConstructionProgress { get; set; }
        public event Action<IJob> OnJobComplete;
        public event Action<IJob> OnJobCancel;
        
        public FloorJob(List<Tile> tiles, FloorTileType floor, float totalJob, WorldController worldController)
        {
            Tiles = tiles;
            TotalJob = totalJob;
            Floor = floor;
            _worldController = worldController;
        }
        
        public bool DoWork(float workTime)
        {
            _jobTime += workTime;
            ConstructionProgress = _jobTime / TotalJob;
            _worldController.UpdateFloorConstructionProgress(this);

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