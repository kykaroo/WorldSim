using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Zenject;
using Tile = MapGenerator.Tile;

namespace PlayerControllers
{
    public class MouseController : ITickable
    {
        private readonly WorldController _worldController;
        private readonly Camera _camera;
        private readonly GenerationConfig _config;
        private readonly Tilemap _tilemap;

        private Vector3 _dragStart;
        private Transform _highLightParent;
        private const float TileOffset = 0.5f; // Равно половине размера тайла
        private readonly BaseTile _tile;
        private Vector3 _currentMousePosition;
        private bool _dragAction;
        private bool _isDragAction;
        private readonly Color _highlightColor = new(1f, 1, 1, 0.45f);
        private readonly Color _validColor = new(0, 255, 0, 0.45f);
        private readonly Color _invalidColor = new(255f, 0, 0, 0.45f);
        private Tile _tileUnderMouse;
        private Tile _selectedTile;
        private int _width;
        private int _height;
        private List<Tile> _tilesToPlaceBuilding;

        public event Action<Tile> OnSelectedTileChanged;

        [Inject]
        public MouseController(WorldController worldController, Camera camera,
            GenerationConfig config, Tilemap tilemap, Sprite tileSprite)
        {
            _worldController = worldController;
            _camera = camera;
            _config = config;
            _tilemap = tilemap;

            _tile = ScriptableObject.CreateInstance<BaseTile>();
            _tile.sprite = tileSprite;
            _tilesToPlaceBuilding = new();
            ConstructionTileTypeChange();
        }

        public void Tick()
        {
            _currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _tilemap.ClearAllTiles();
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                HighlightSelectedTile();
                return;
            }

            GetTileUnderMouse();

            HighLightUnderMouse();

            switch (_config.buildMode)
            {
                case BuildMode.World:
                    DragMove(_currentMousePosition);
                    break;
                case BuildMode.Construction:
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        (_height,_width) = (_width, _height);
                    }
                    
                    if (TileValidation() && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        HandleTilePlacement();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTilePlacement()
        {
            if (_config.constructionTileToPlace == ConstructionTileTypes.None)
            {
                _tileUnderMouse.InstalledObject?.UninstallObject();
                return;
            }
            
            InstalledObject installedObject = new(_tilesToPlaceBuilding, _config, _config.constructionTileToPlace);
            
            foreach (var tile in _tilesToPlaceBuilding)
            {
                tile.InstallBuilding(installedObject);
            }
        }

        public void ConstructionTileTypeChange()
        {
            var constructionConfig = _config.constructionConfigs.First(b => b.type == _config.constructionTileToPlace);
            _width = constructionConfig.width;
            _height = constructionConfig.height;
        }

        private bool TileValidation()
        {
            _tilesToPlaceBuilding.Clear();
            
            if (_tileUnderMouse == null) return false;
            
            if (_config.constructionTileToPlace == ConstructionTileTypes.None)
            {
                var tilePos = new Vector3Int(_tileUnderMouse.X, _tileUnderMouse.Y, 0);
                _tilemap.SetTile(tilePos, _tile);
                _tilemap.SetTileFlags(tilePos, TileFlags.None);
                _tilemap.SetColor(tilePos, _invalidColor);
                return true;
            }
            
            for (var x = _tileUnderMouse.X; x <= _tileUnderMouse.X + _width - 1; x++)
            {
                for (var y = _tileUnderMouse.Y; y <= _tileUnderMouse.Y + _height - 1; y++)
                {
                    var tile = _worldController.GetTile(x, y);

                    if (tile == null) return false;
                    if (_config.drawMode == DrawMode.NoiseMap) return false;
                    
                    var tilePos = new Vector3Int(tile.X, tile.Y, 0);

                    if (tile.InstallObjectValid)
                    {
                        _tilemap.SetTile(tilePos, _tile);
                        _tilemap.SetTileFlags(tilePos, TileFlags.None);
                        _tilemap.SetColor(tilePos, _validColor);
                        _tilesToPlaceBuilding.Add(tile);
                        continue;
                    }
                    _tilemap.SetTile(tilePos, _tile);
                    _tilemap.SetTileFlags(tilePos, TileFlags.None);
                    _tilemap.SetColor(tilePos, _invalidColor);
                }
            }

            return _tilesToPlaceBuilding.Count == _width * _height;
        }

        private void GetTileUnderMouse()
        {
            var tileUnderMouse = GetTileAtWorldCoord(_currentMousePosition.x, _currentMousePosition.y);
            if (_tileUnderMouse == tileUnderMouse) return;

            _tileUnderMouse = tileUnderMouse;
        }

        private void HighLightUnderMouse()
        {
            if (_dragAction) return;

            _tilemap.ClearAllTiles();
            HighlightSelectedTile();

            if (_tileUnderMouse != null)
            {
                var tilePos = new Vector3Int(_tileUnderMouse.X, _tileUnderMouse.Y, 0);
                _tilemap.SetTile(tilePos, _tile);
                _tilemap.SetTileFlags(tilePos, TileFlags.None);
                _tilemap.SetColor(tilePos, _highlightColor);
            }
        }

        private void HighlightSelectedTile()
        {
            if (_selectedTile == null) return;
            var selectedTilePos = new Vector3Int(_selectedTile.X, _selectedTile.Y, 0);
            _tilemap.SetTile(selectedTilePos, _tile);
            _tilemap.SetTileFlags(selectedTilePos, TileFlags.None);
            _tilemap.SetColor(selectedTilePos, _highlightColor);
            OnSelectedTileChanged?.Invoke(_selectedTile);
        }

        private void DragMove(Vector3 currentMousePosition)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _dragStart = currentMousePosition;
                _dragAction = true;
            }

            var startX = Mathf.FloorToInt(_dragStart.x + TileOffset);
            var endX = Mathf.FloorToInt(currentMousePosition.x + TileOffset);
            var startY = Mathf.FloorToInt(_dragStart.y + TileOffset);
            var endY = Mathf.FloorToInt(currentMousePosition.y + TileOffset);

            if (endX < startX)
            {
                (endX, startX) = (startX, endX);
            }

            if (endY < startY)
            {
                (endY, startY) = (startY, endY);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                DragHighLight(startX, endX, startY, endY);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                DragEnd(startX, endX, startY, endY);

                if (_isDragAction) return;

                _selectedTile = _tileUnderMouse;
                OnSelectedTileChanged?.Invoke(_selectedTile);
            }
        }

        private void DragEnd(int startX, int endX, int startY, int endY)
        {
            _isDragAction = !(startX == endX && startY == endY);

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    var t = _worldController.GetTile(x, y);

                    if (t == null) continue;
                    if (_config.worldTileWorldToPlace == TileWorldType.None) continue;
                    if (_config.drawMode == DrawMode.NoiseMap) return;

                    t.WorldType = _config.worldTileWorldToPlace;
                    OnSelectedTileChanged?.Invoke(_tileUnderMouse);
                }
            }

            _dragAction = false;
            _tilemap.ClearAllTiles();
        }

        private void DragHighLight(int startX, int endX, int startY, int endY)
        {
            _tilemap.ClearAllTiles();
            HighlightSelectedTile();

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    var t = _worldController.GetTile(x, y);

                    if (t == null) continue;

                    var vector3Int = new Vector3Int(x, y, 0);
                    _tilemap.SetTile(vector3Int, _tile);
                    _tilemap.SetTileFlags(vector3Int, TileFlags.None);
                    _tilemap.SetColor(vector3Int, _highlightColor);
                }
            }
        }

        private Tile GetTileAtWorldCoord(float x, float y)
        {
            return _worldController.GetTile(Mathf.FloorToInt(x + TileOffset), Mathf.FloorToInt(y + TileOffset));
        }
    }
}