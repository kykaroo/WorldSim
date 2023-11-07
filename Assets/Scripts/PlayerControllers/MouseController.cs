using System;
using System.Collections.Generic;
using System.Linq;
using Ai;
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
        private readonly Tilemap _highlightTilemap;
        private readonly List<Tile> _tilesToPlaceBuilding;
        private readonly List<IJob> _jobList;
        private readonly List<Pawn> _characters;
        private readonly TurnManager _turnManager;

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

        public bool IsInstantBuild;
        
        public event Action<Tile> OnSelectedTileChanged;

        [Inject]
        public MouseController(WorldController worldController, Camera camera,
            GenerationConfig config, Sprite tileSprite, GraphicsLayers graphicsLayers, TurnManager turnManager)
        {
            _worldController = worldController;
            _camera = camera;
            _config = config;
            _highlightTilemap = graphicsLayers.HighlightTilemap;
            _turnManager = turnManager;
            _turnManager.OnTurnTrigger += UpdateCharacters;
            
            _jobList = new();
            _characters = new();

            _tile = ScriptableObject.CreateInstance<BaseTile>();
            _tile.sprite = tileSprite;
            _tilesToPlaceBuilding = new();
            ConstructionTileTypeChange();
        }

        private void UpdateCharacters()
        {
            foreach (var character in _characters)
            {
                character.Update();
            }
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _turnManager.TogglePause();
            }
            
            _currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _highlightTilemap.ClearAllTiles();

            if (EventSystem.current.IsPointerOverGameObject())
            {
                HighlightSelectedTile();
                return;
            }

            _tileUnderMouse = _worldController.GetTile(Mathf.FloorToInt(_currentMousePosition.x + TileOffset),
                Mathf.FloorToInt(_currentMousePosition.y + TileOffset));

            HighLightUnderMouse();

            switch (_config.buildMode)
            {
                case BuildMode.World:
                    DragMove(_currentMousePosition);
                    break;
                case BuildMode.Building:
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        (_height,_width) = (_width, _height);
                    }
                    
                    if (BuildingValidation() && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        HandleBuildingPlacement();
                    }
                    break;
                case BuildMode.Floor:
                    if (FloorValidation() && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        HandleFloorPlacement();
                    }
                    break;
                case BuildMode.Character:
                    if (BuildingValidation() && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        HandleCharacterPlacement();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleCharacterPlacement()
        {
            var character = new Pawn(_tileUnderMouse, 1, _jobList);
            _worldController.CreateCharacter(_tileUnderMouse, character);
            _characters.Add(character);
        }

        private bool FloorValidation()
        {
            _tilesToPlaceBuilding.Clear();
            
            if (_tileUnderMouse == null) return false;
            
            if (_config.floorTileToPlace == FloorTileType.None)
            {
                var tilePos = new Vector3Int(_tileUnderMouse.X, _tileUnderMouse.Y, 0);
                _highlightTilemap.SetTile(tilePos, _tile);
                _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                _highlightTilemap.SetColor(tilePos, _invalidColor);
                return true;
            }
            
            for (var x = _tileUnderMouse.X; x <= _tileUnderMouse.X; x++)
            {
                for (var y = _tileUnderMouse.Y; y <= _tileUnderMouse.Y; y++)
                {
                    var tile = _worldController.GetTile(x, y);

                    if (tile == null) return false;
                    if (_config.drawMode == DrawMode.NoiseMap) return false;
                    
                    var tilePos = new Vector3Int(tile.X, tile.Y, 0);

                    if (tile.FloorValid)
                    {
                        _highlightTilemap.SetTile(tilePos, _tile);
                        _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                        _highlightTilemap.SetColor(tilePos, _validColor);
                        _tilesToPlaceBuilding.Add(tile);
                        continue;
                    }
                    _highlightTilemap.SetTile(tilePos, _tile);
                    _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                    _highlightTilemap.SetColor(tilePos, _invalidColor);
                }
            }

            return _tilesToPlaceBuilding.Count == 1;
        }

        private void HandleFloorPlacement()
        {
            if (_config.floorTileToPlace == FloorTileType.None)
            {
                _tileUnderMouse.Floor?.UninstallFloor();
                return;
            }

            var floor = new Floor(_tilesToPlaceBuilding, _config, _config.floorTileToPlace);
            
            var job = new FloorJob(floor, _config.buildTime, _worldController);
            
            foreach (var tile in _tilesToPlaceBuilding)
            {
                tile.InstallFloor(floor);
            }
            
            job.OnJobComplete += job1 => _jobList.Remove(job1);
            _jobList.Add(job);
        }

        private void DoJobs()
        {
            foreach (var job in _jobList)
            {
                if (IsInstantBuild)
                {
                    job.DoWork(job.TotalJob);
                    break;
                }
                
                if (job.DoWork(Time.fixedDeltaTime))
                {
                    break;
                }
            }
        }

        private void HandleBuildingPlacement()
        {
            if (_config.buildingsTileToPlace == BuildingsTileType.None)
            {
                _tileUnderMouse.Building?.UninstallBuilding();
                return;
            }
            
            var building = new Building(_tilesToPlaceBuilding, _config, _config.buildingsTileToPlace);
            
            var job = new BuildingJob(building, _config.buildTime, _worldController);
            
            foreach (var tile in _tilesToPlaceBuilding)
            {
                tile.InstallBuilding(building);
            }
            
            job.OnJobComplete += job1 => _jobList.Remove(job1);
            _jobList.Add(job);
        }

        public void ConstructionTileTypeChange()
        {
            var constructionConfig = _config.buildingConfigs.First(b => b.type == _config.buildingsTileToPlace);
            _width = constructionConfig.width;
            _height = constructionConfig.height;
        }

        private bool BuildingValidation()
        {
            _tilesToPlaceBuilding.Clear();
            
            if (_tileUnderMouse == null) return false;
            
            if (_config.buildingsTileToPlace == BuildingsTileType.None)
            {
                var tilePos = new Vector3Int(_tileUnderMouse.X, _tileUnderMouse.Y, 0);
                _highlightTilemap.SetTile(tilePos, _tile);
                _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                _highlightTilemap.SetColor(tilePos, _invalidColor);
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

                    if (tile.BuildingValid)
                    {
                        _highlightTilemap.SetTile(tilePos, _tile);
                        _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                        _highlightTilemap.SetColor(tilePos, _validColor);
                        _tilesToPlaceBuilding.Add(tile);
                        continue;
                    }
                    _highlightTilemap.SetTile(tilePos, _tile);
                    _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
                    _highlightTilemap.SetColor(tilePos, _invalidColor);
                }
            }

            return _tilesToPlaceBuilding.Count == _width * _height;
        }

        private void HighLightUnderMouse()
        {
            if (_dragAction) return;

            _highlightTilemap.ClearAllTiles();
            HighlightSelectedTile();

            if (_tileUnderMouse == null) return;
            
            var tilePos = new Vector3Int(_tileUnderMouse.X, _tileUnderMouse.Y, 0);
            _highlightTilemap.SetTile(tilePos, _tile);
            _highlightTilemap.SetTileFlags(tilePos, TileFlags.None);
            _highlightTilemap.SetColor(tilePos, _highlightColor);
        }

        private void HighlightSelectedTile()
        {
            if (_selectedTile == null) return;
            var selectedTilePos = new Vector3Int(_selectedTile.X, _selectedTile.Y, 0);
            _highlightTilemap.SetTile(selectedTilePos, _tile);
            _highlightTilemap.SetTileFlags(selectedTilePos, TileFlags.None);
            _highlightTilemap.SetColor(selectedTilePos, _highlightColor);
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

            if (!Input.GetKeyUp(KeyCode.Mouse0)) return;
            
            DragEnd(startX, endX, startY, endY);

            if (_isDragAction) return;

            _selectedTile = _tileUnderMouse;
            OnSelectedTileChanged?.Invoke(_selectedTile);
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
                    if (_config.worldTileToPlace == WorldTileType.None) continue;
                    if (_config.drawMode == DrawMode.NoiseMap) return;

                    t.Type = _config.worldTileToPlace;
                }
            }

            _dragAction = false;
            _highlightTilemap.ClearAllTiles();
        }

        private void DragHighLight(int startX, int endX, int startY, int endY)
        {
            _highlightTilemap.ClearAllTiles();
            HighlightSelectedTile();

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    var t = _worldController.GetTile(x, y);

                    if (t == null) continue;

                    var vector3Int = new Vector3Int(x, y, 0);
                    _highlightTilemap.SetTile(vector3Int, _tile);
                    _highlightTilemap.SetTileFlags(vector3Int, TileFlags.None);
                    _highlightTilemap.SetColor(vector3Int, _highlightColor);
                }
            }
        }
    }
}