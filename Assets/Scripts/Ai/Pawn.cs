﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai
{
    public class Pawn
    {
        private float _speed;
        private readonly float _baseSpeed;
        private float _movementPercentage;

        private readonly List<IJob> _jobs;
        
        private Tile _currentTile;
        private Tile _nextTileToMove;
        private Tile _destinationTile;
        private IJob _currentJob;
        private readonly Pathfinder _pathfinder;
        private Queue<Tile> _path;
        private readonly WorldController _worldController;
        private readonly List<Tile> _doWorkTiles;
        private bool _isMoving;

        public Tile CurrentTile => _currentTile;
        
        public event Action<Pawn, Queue<Tile>, Tile> OnPathUpdate;

        public Pawn(Tile tile, float baseSpeed, List<IJob> jobs, Pathfinder pathfinder, WorldController worldController)
        {
            _currentTile = tile;
            _baseSpeed = baseSpeed;
            _jobs = jobs;
            _pathfinder = pathfinder;
            _worldController = worldController;
            _path = new();
            _doWorkTiles = new();
        }

        public void SetDestination(Tile tile)
        {
            if (tile.MoveSpeedMultiplier == 0) throw new ArgumentException();

            if (_currentJob.Tiles.Any(tile => tile == _currentTile))
            {
                var tiles = _worldController.GetTileNeighbours(_currentTile).Values.ToArray();
                _destinationTile = tiles[Random.Range(0, tiles.Length)];
                return;
            } //Todo Изменить решение ошибки, при размещении таила прямо на пешке

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
                    SetDestination(job.Tiles[0]);
                    _path = _pathfinder.FindPath(_currentTile, _destinationTile);
                    OnPathUpdate?.Invoke(this, _path, _nextTileToMove);
                    _doWorkTiles.Clear();
                    
                    foreach (var tile in _worldController.GetTileNeighbours(_currentJob.Tiles[0]).Values)
                    {
                        _doWorkTiles.Add(tile);
                    }
                    break;
                }
            }
            else
            {
                if (_doWorkTiles.Any(tile => tile == _currentTile))
                {
                    _currentJob.DoWork(1);
                }
                else
                {
                    if (_nextTileToMove == null)
                    {
                        _jobs.First(job => job == _currentJob).CancelJob();
                        _currentJob = null;
                        Debug.Log("Путь был найден, но персонаж не смог дойти до цели");
                    }
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
            if (_currentTile == _destinationTile)
            {
                _movementPercentage = 0;
                return;
            }

            DoMove();

            if (!_isMoving)
            {
                _path.TryDequeue(out _nextTileToMove);
            }

            OnPathUpdate?.Invoke(this, _path, _nextTileToMove);
        }

        private void DoMove()
        {
            if (_nextTileToMove == null)
            {
                return;
            }
            if (_nextTileToMove.MoveSpeedMultiplier == 0) // Попробовать заменить на событие изменения тайла для перестройки пути
            {
                _path = _pathfinder.FindPath(_currentTile, _destinationTile);
                _path.TryDequeue(out _nextTileToMove);
            }

            _speed = _baseSpeed * _nextTileToMove.MoveSpeedMultiplier;
            _movementPercentage += _speed;

            if (_movementPercentage >= 1)
            {
                _movementPercentage -= 1;
                _currentTile = _nextTileToMove;
                _nextTileToMove = null;
                _isMoving = false;
                return;
            }

            _isMoving = true;
        }
    }
}