using System;
using System.Collections.Generic;
using Data;
using MapGenerator;

namespace Ai
{
    public class Job
    {
        private readonly WorldController _worldController;
        private float _jobTime;

        public float TotalJob { get; }
        public List<Tile> Tiles { get; }
        public ConstructionTileTypes Building { get; }
        public float BuildingProgress { get; private set; }

        public event Action<Job> OnJobComplete;
        public event Action<Job> OnJobCancel;

        public Job(IEnumerable<Tile> tiles, ConstructionTileTypes building, float totalJob, WorldController worldController)
        {
            Tiles = new(tiles);
            TotalJob = totalJob;
            Building = building;
            _worldController = worldController;
        }

        public bool DoWork(float workTime)
        {
            _jobTime += workTime;
            BuildingProgress = _jobTime / TotalJob;
            _worldController.UpdateConstructionProgress(this);

            if (!(_jobTime >= TotalJob)) return false;

            BuildingProgress = 1;
            OnJobComplete?.Invoke(this);
            return true;

        }

        public void CancelJob()
        {
            OnJobCancel?.Invoke(this);
        }
    }
}