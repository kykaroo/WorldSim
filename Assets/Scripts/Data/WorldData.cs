using System;
using MapGenerator;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Data
{
    public class WorldData
    {
        private Tile _tilePrefab;
        private Tile[,] _tiles;
        private int _width;
        private int _height;
        private readonly GenerationConfig _config;
        public bool FirstGeneration;

        public Tile[,] Tiles => _tiles;

        public event Action<Tile> OnTileChanged;

        [Inject]
        public WorldData(GenerationConfig config)
        {
            _config = config;
        }
        
        public void CreateNewData(int width, int height, Tile tilePrefab)
        {
            ClearAllTiles();
            
            _tiles = new Tile[width, height];
            _width = width;
            _height = height;
            _tilePrefab = tilePrefab;
        }

        public void CreateTile(Tile tile)
        {
            var x = tile.X;
            var y = tile.Y;
            
            if (x > _width) throw new ArgumentException($"Неверное значение x: {x}, при ширине карты {_width})");
            if (y > _height) throw new ArgumentException($"Неверное значение y: {y}, при высоте карты {_height})");
            
            if (_tiles[x, y] != null) Debug.Log($"Таил в точке ({x},{y}) был замещен");

            _tiles[x, y] = tile;
            tile.OnTileTypeChanged += TileChanged;
            if (FirstGeneration) return;

            OnTileChanged?.Invoke(tile);
        }

        private void TileChanged(Tile tile)
        {
            OnTileChanged?.Invoke(tile);
        }

        public void ClearAllTiles()
        {
            if (_tiles == null) return;
            
            foreach (var tile in _tiles)
            {
                tile.OnTileTypeChanged -= TileChanged;
                Object.Destroy(tile.GameObject());
            }
        }

        public void CreateTile(int x, int y, TileType tileType, Transform parent)
        {
            var gameObject = Object.Instantiate(_tilePrefab, new(x,y,0), Quaternion.identity, parent);
            gameObject.name = $"Tile_{x}_{y}";
            
            var tile = gameObject.GetComponent<Tile>();
            tile.Initialize(x, y, tileType, _config);
            _tiles[x, y] = tile;

            tile.OnTileTypeChanged += TileChanged;
            
            if (FirstGeneration) return;
            
            OnTileChanged?.Invoke(tile);
        }

        public void ChangeTileType(int x, int y, TileType tileType)
        {
            
        }
    }
}