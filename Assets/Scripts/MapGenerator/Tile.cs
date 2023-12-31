﻿using System;
using System.Linq;
using Ai;
using Data;

namespace MapGenerator
{
    public class Tile : UnityEngine.Tilemaps.Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private GenerationConfig Config { get; set; }
        private WorldTileType _worldTileType;
        private float _tileWalkSpeedMultiplier;
        public WorldTileType Type
        {
            get => _worldTileType;
            set
            {
                _worldTileType = value;
                _tileWalkSpeedMultiplier =
                    Config.regions.First(region => region.worldTileType == _worldTileType).moveSpeedMultiplier;
                
                RecalculateSpeed();
                OnTileTypeChanged?.Invoke(this);
            }
        }

        public float MoveSpeedMultiplier { get; private set; }
        
        public bool BuildingValid { get; private set; }
        public bool PendingBuildingJob { get; set; }
        public bool PendingFloorJob { get; set; }
        public bool BuildingComplete { get; set; }
        public bool FloorValid { get; set; }
        public Floor Floor { get; set; }
        public TileInventory TileInventory { get; private set; }

        public Building Building { get; private set; }

        public event Action<Tile> OnTileTypeChanged;
        public event Action<Tile> OnBuildingTileTypeChanged;
        public event Action<Tile> OnFloorTileTypeChanged;

        private void BuildingTileTypeChanged()
        {
            RecalculateSpeed();
            OnBuildingTileTypeChanged?.Invoke(this);
        }
        
        private void FloorTileTypeChanged()
        {
            RecalculateSpeed();
            OnFloorTileTypeChanged?.Invoke(this);
        }

        public void Initialize(int x, int y, WorldTileType worldTileType, GenerationConfig config)
        {
            X = x;
            Y = y;
            Config = config;
            PendingBuildingJob = false;
            PendingFloorJob = false;
            
            Type = worldTileType;
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            if (Building != null && BuildingComplete)
            {
                MoveSpeedMultiplier = _tileWalkSpeedMultiplier * Building.WalkSpeedMultiplier;
                return;
            }

            MoveSpeedMultiplier = Floor?.WalkSpeedMultiplier ?? _tileWalkSpeedMultiplier;

            BuildingValid = MoveSpeedMultiplier != 0 && Building == null;
            FloorValid = _tileWalkSpeedMultiplier != 0 && Floor == null;
        }
        
        public void InstallBuilding(Building building)
        {
            Building = building;
            BuildingTileTypeChanged();
        }

        public void UninstallBuilding()
        {
            Building = null;
            BuildingTileTypeChanged();
        }
        
        public void InstallFloor(Floor floor)
        {
            Floor = floor;
            FloorTileTypeChanged();
        }

        public void UninstallFloor()
        {
            Floor = null;
            FloorTileTypeChanged();
        }

        public bool IsNeighbour(Tile tile)
        {
            var diff = MathF.Abs(X - tile.X) + MathF.Abs(Y - tile.Y);

            return (int)diff == 1 || ((int)MathF.Abs(X - tile.X) == 1 && (int)MathF.Abs(Y - tile.Y) == 1);
        }
    }
}