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
        public bool IsTaskPerformed { get; set; }
        public Floor Floor { get; }
        public float ConstructionProgress { get; set; }
        public event Action<IJob> OnJobComplete;
        public event Action<IJob> OnJobCancel;
        
        public FloorJob(Floor floor, float totalJob, WorldController worldController)
        {
            Floor = floor;
            Tiles = Floor.Tiles;
            TotalJob = totalJob;
            _worldController = worldController;
            IsTaskPerformed = false;
            
            foreach (var tile in Tiles)
            {
                tile.PendingFloorJob = true;
            }
        }
        
        public bool DoWork(float workTime)
        {
            _jobTime += workTime;
            ConstructionProgress = _jobTime / TotalJob;
            _worldController.UpdateFloorConstructionProgress(this);

            if (!(_jobTime >= TotalJob)) return false;

            ConstructionProgress = 1;
            
            foreach (var tile in Tiles)
            {
                tile.PendingFloorJob = false;
            }
            
            OnJobComplete?.Invoke(this);
            return true;
        }

        public void CancelJob()
        {
            foreach (var tile in Tiles)
            {
                tile.PendingFloorJob = false;
            }
            
            OnJobCancel?.Invoke(this);
        }
    }
}