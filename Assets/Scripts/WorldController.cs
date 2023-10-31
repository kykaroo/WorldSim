using System;
using Data;
using MapGenerator;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class WorldController
{
    private readonly WorldData _worldData;
    private readonly GenerationConfig _config;
    
    public bool FirstGeneration;
    
    public event Action<Tile> OnTileChanged;

    [Inject]
    public WorldController(GenerationConfig config, WorldData worldData)
    {
        _config = config;
        _worldData = worldData;
    }
        
    public void CreateNewData()
    {
        ClearAllTiles();
            
        _worldData.Initialize(_config.mapWidth, _config.mapHeight, _config.tilePrefab);
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
        if (FirstGeneration) return;

        OnTileChanged?.Invoke(tile);
    }

    private void TileChanged(Tile tile)
    {
        OnTileChanged?.Invoke(tile);
    }

    public void ClearAllTiles()
    {
        if (_worldData.Tiles == null) return;
            
        foreach (var tile in _worldData.Tiles)
        {
            tile.OnTileTypeChanged -= TileChanged;
            Object.Destroy(tile.GameObject());
        }
    }

    public void CreateTile(int x, int y, TileType tileType, Transform parent)
    {
        var gameObject = Object.Instantiate(_config.tilePrefab, new(x,y,0), Quaternion.identity, parent);
        gameObject.name = $"Tile_{x}_{y}";
            
        var tile = gameObject.GetComponent<Tile>();
        tile.Initialize(x, y, tileType, _config);
        _worldData.Tiles[x, y] = tile;

        tile.OnTileTypeChanged += TileChanged;
            
        if (FirstGeneration) return;
            
        OnTileChanged?.Invoke(tile);
    }

    public void ChangeTileType(int x, int y, TileType tileType)
    {
            
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x > _config.mapWidth) return null;
        if (y < 0 || y > _config.mapHeight) return null;

        return _worldData.Tiles[x, y];
    }
}