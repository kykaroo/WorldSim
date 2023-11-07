using System;
using System.Collections.Generic;
using System.Linq;
using Ai;
using MapGenerator;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Tile = MapGenerator.Tile;

namespace Data
{
    public class WorldController
    {
        private readonly WorldData _worldData;
        private readonly GenerationConfig _config;
        private readonly TurnManager _turnManager;

        private readonly List<Pawn> _characters;


        [Inject]
        public WorldController(GenerationConfig config, WorldData worldData, TurnManager turnManager)
        {
            _config = config;
            _worldData = worldData;
            _turnManager = turnManager;
            _characters = new();
            _turnManager.OnTurnTrigger += UpdateCharactersPosition;
        }
        
        public void CreateNewData()
        {
            ClearAllTiles();
            
            _worldData.Initialize(_config.mapWidth, _config.mapHeight);
        }

        public void CreateTile(Tile tile)
        {
            var x = tile.X;
            var y = tile.Y;
            
            if (x > _config.mapWidth) throw new ArgumentException($"Неверное значение x: {x}, при ширине карты {_config.mapWidth})");
            if (y > _config.mapHeight) throw new ArgumentException($"Неверное значение y: {y}, при высоте карты {_config.mapHeight})");
            
            if (_worldData.Tiles[x, y] != null) Debug.Log($"Таил в точке ({x},{y}) был замещен");

            _worldData.Tiles[x, y] = tile;
            tile.OnTileTypeChanged += TileChanged;
        }

        private void TileChanged(Tile tile)
        {
            _worldData.WorldTilemap.SetColor(new(tile.X, tile.Y, 0), _config.regions.First(b => b.worldTileType == tile.Type).color);
        }

        public void ClearAllTiles()
        {
            if (_worldData.Tiles == null) return;
            
            foreach (var tile in _worldData.Tiles)
            {
                tile.OnTileTypeChanged -= TileChanged;
                tile.OnBuildingTileTypeChanged -= BuildingTileChanged;
                tile.OnFloorTileTypeChanged -= FloorTileChanged;
            }

            _worldData.WorldTilemap.ClearAllTiles(); 
            _worldData.Tiles = null;
        }

        public void CreateTile(int x, int y, RegionConfig region)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            var tilePos = new Vector3Int(x,y,0);
            tile.Initialize(x, y, region.worldTileType, _config);
            tile.sprite = region.tileSprite;
            tile.OnTileTypeChanged += TileChanged;
            tile.OnBuildingTileTypeChanged += BuildingTileChanged;
            tile.OnFloorTileTypeChanged += FloorTileChanged;
            
            _worldData.WorldTilemap.SetTile(tilePos, tile);
            _worldData.WorldTilemap.SetTileFlags(tilePos, TileFlags.None);
            _worldData.Tiles[x, y] = tile;
            _worldData.WorldTilemap.SetColor(tilePos, region.color);
        }

        private void FloorTileChanged(Tile tile)
        {
            var baseTile = ScriptableObject.CreateInstance<BaseTile>();
            var tileChangeData = new Vector3Int(tile.X, tile.Y, 0);
            
            baseTile.sprite = tile.Floor == null ? null : _config.floorConfigs.First(config => config.type == tile.Floor.Type).sprite;
            _worldData.FloorTilemap.SetTile(tileChangeData, baseTile);
            _worldData.FloorTilemap.SetTileFlags(tileChangeData, TileFlags.None);
            _worldData.FloorTilemap.SetColor(tileChangeData, new(1, 1, 1, 0));
        }

        private void BuildingTileChanged(Tile tile)
        {
            var baseTile = ScriptableObject.CreateInstance<BaseTile>();
            var tileChangeData = new Vector3Int(tile.X, tile.Y, 0);
            
            baseTile.sprite = tile.Building == null ? null : _config.buildingConfigs.First(config => config.type == tile.Building.Type).sprite;
            _worldData.BuildingsTilemap.SetTile(tileChangeData, baseTile);
            _worldData.BuildingsTilemap.SetTileFlags(tileChangeData, TileFlags.None);
            _worldData.BuildingsTilemap.SetColor(tileChangeData, new(1, 1, 1, 0));
        }

        public Tile GetTile(int x, int y)
        {
            if (_worldData.Tiles == null) return null;
            if (x < 0 || x >= _config.mapWidth) return null;
            if (y < 0 || y >= _config.mapHeight) return null;

            return _worldData.Tiles[x, y];
        }


        public void UpdateBuildingConstructionProgress(BuildingJob buildingJob)
        {
            foreach (var tile in buildingJob.Tiles)
            {
                _worldData.BuildingsTilemap.SetColor(new(tile.X, tile.Y, 0),
                    new(1, 1, 1, buildingJob.ConstructionProgress));
            }
        }

        public void UpdateFloorConstructionProgress(FloorJob floorJob)
        {
            foreach (var tile in floorJob.Tiles)
            {
                _worldData.FloorTilemap.SetColor(new(tile.X, tile.Y, 0), 
                    new(1, 1, 1, floorJob.ConstructionProgress));
            }
        }

        public void CreateCharacter(Tile tile, Pawn pawn)
        {
            var baseTile = ScriptableObject.CreateInstance<BaseTile>();
            var tileChangeData = new Vector3Int(tile.X, tile.Y, 0);
            
            _characters.Add(pawn);
            baseTile.sprite = _config.characterSprite;
            _worldData.CharacterTilemap.SetTile(tileChangeData, baseTile);
        }

        private void UpdateCharactersPosition()
        {
            _worldData.CharacterTilemap.ClearAllTiles();

            foreach (var character in _characters)
            {
                var baseTile = ScriptableObject.CreateInstance<BaseTile>(); 
                
                baseTile.sprite = _config.characterSprite;
                _worldData.CharacterTilemap.SetTile(new(character.CurrentTile.X, character.CurrentTile.Y, 0), baseTile);
            }
        }
    }
}