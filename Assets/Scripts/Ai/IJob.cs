using System;
using System.Collections.Generic;
using MapGenerator;

namespace Ai
{
    public interface IJob
    {
        public float TotalJob { get; }
        public List<Tile> Tiles { get; }
        public float ConstructionProgress { get; set; }

        public event Action<IJob> OnJobComplete;
        public event Action<IJob> OnJobCancel;

        bool DoWork(float workTime);
        void CancelJob();
    }
}