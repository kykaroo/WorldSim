using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Data
{
    public class WorldData
    {
        private readonly TileData[,] _tiles;
        private readonly int _width;
        private readonly int _height;

        public WorldData(int width, int height)
        {
            _tiles = new TileData[width, height];
            _width = width;
            _height = height;
        }

        public void AddTile(TileData tileData)
        {
            var x = tileData.X;
            var y = tileData.Y;
            
            if (x > _width) throw new ArgumentException($"Неверное значение x: {x}, при ширине карты {_width})");
            if (y > _height) throw new ArgumentException($"Неверное значение y: {y}, при высоте карты {_height})");
            
            if (_tiles[x, y] != null) Debug.Log($"Таил в точке ({x},{y}) был замещен");

            _tiles[x, y] = tileData;
        }

        public void ClearAllTiles()
        {
            foreach (var tile in _tiles)
            {
                Object.Destroy(tile.GameObject());
            }
        }
    }
}