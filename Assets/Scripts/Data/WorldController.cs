using System;
using System.Linq;
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


        [Inject]
        public WorldController(GenerationConfig config, WorldData worldData)
        {
            _config = config;
            _worldData = worldData;
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
            _worldData.WorldTilemap.SetColor(new(tile.X, tile.Y, 0), _config.regions.First(b => b.tileWorldType == tile.WorldType).color);
        }

        public void ClearAllTiles()
        {
            if (_worldData.Tiles == null) return;
            
            foreach (var tile in _worldData.Tiles)
            {
                tile.OnTileTypeChanged -= TileChanged;
                tile.OnConstructionTileTypeChanged -= ConstructionTileChanged;
            }

            _worldData.WorldTilemap.ClearAllTiles(); 
            _worldData.Tiles = null;
        }

        public void CreateTile(int x, int y, RegionConfig region)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            var tilePos = new Vector3Int(x,y,0);
            tile.Initialize(x, y, region.tileWorldType);
            tile.sprite = region.tileSprite;
            tile.OnTileTypeChanged += TileChanged;
            tile.OnConstructionTileTypeChanged += ConstructionTileChanged;
            
            _worldData.WorldTilemap.SetTile(tilePos, tile);
            _worldData.WorldTilemap.SetTileFlags(tilePos, TileFlags.None);
            _worldData.Tiles[x, y] = tile;
            _worldData.WorldTilemap.SetColor(tilePos, region.color);
        }

        private void ConstructionTileChanged(Tile tile)
        {
            var baseTile = ScriptableObject.CreateInstance<BaseTile>();
            baseTile.sprite = _config.constructionConfigs.First(b => b.type == tile.InstalledObject.Type).sprite;
            _worldData.ConstructionTilemap.SetTile(new(tile.X, tile.Y, 0), baseTile);
        }

        public Tile GetTile(int x, int y)
        {
            if (_worldData.Tiles == null) return null;
            if (x < 0 || x >= _config.mapWidth) return null;
            if (y < 0 || y >= _config.mapHeight) return null;

            return (Tile)_worldData.WorldTilemap.GetTile(new(x, y, 0));
        }
    }
}