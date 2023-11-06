using System;
using System.Collections.Generic;
using Data;
using MapGenerator;

namespace Ai
{
    public class Job
    {
        private float _jobTime;

        public List<Tile> Tiles { get; }
        public ConstructionTileTypes Building { get; }

        public event Action<Job> OnJobComplete;
        public event Action<Job> OnJobCancel;

        public Job(List<Tile> tiles, ConstructionTileTypes building, float jobTile)
        {
            Tiles = tiles;
            _jobTime = jobTile;
            Building = building;
        }

        public bool DoWork(float workTime)
        {
            _jobTime -= workTime;

            if (!(_jobTime <= 0)) return false;
            
            OnJobComplete?.Invoke(this);
            return true;

        }

        public void CancelJob()
        {
            OnJobCancel?.Invoke(this);
        }
    }
}