using System;
using System.Collections.Generic;
using MapGenerator;

namespace Ai
{
    public class Pawn
    {
        private float _speed;
        private readonly float _baseSpeed;
        private float _movementPercentage;

        private readonly List<IJob> _jobs;
        
        private Tile _currentTile;
        private Tile _destinationTile;
        private IJob _currentJob;

        public Tile CurrentTile => _currentTile;

        public Tile DestinationTile => _destinationTile;
        
        

        public Pawn(Tile tile, float baseSpeed, List<IJob> jobs)
        {
            _currentTile = tile;
            _currentTile.Pawn = this;
            _baseSpeed = baseSpeed;
            _jobs = jobs;
        }

        public void SetDestination(Tile tile)
        {
            if (tile.WalkSpeedMultiplier == 0) throw new ArgumentException();
            if (tile.Pawn != null) return;

            _destinationTile = tile;
        }

        public void Update()
        {
            if (_currentJob == null)
            {
                foreach (var job in _jobs)
                {
                    if (job.IsTaskPerformed) continue;
                    job.IsTaskPerformed = true;
                    _currentJob = job;
                    _currentJob.OnJobComplete += JobDone;
                    _destinationTile = job.Tiles[0];
                    break;
                }
            }
            else
            {
                if (_currentTile == _currentJob.Tiles[0])
                {
                    _currentJob.DoWork(1);
                }
            }

            Move();
        }

        private void JobDone(IJob job)
        {
            _currentJob.OnJobComplete -= JobDone;
            _currentJob = null;
        }

        private void Move()
        {
            if (_destinationTile == null)
            {
                _movementPercentage = 0;
                return;
            }

            _speed = _baseSpeed * _destinationTile.WalkSpeedMultiplier;

            if (_destinationTile != null)
            {
                _movementPercentage += _speed;
            }

            if (_movementPercentage >= 1)
            {
                if (_destinationTile.Pawn != null) return;

                _movementPercentage -= 1;
                _currentTile.Pawn = null;
                _currentTile = _destinationTile;
                _currentTile.Pawn = this;
            }
        }
    }
}