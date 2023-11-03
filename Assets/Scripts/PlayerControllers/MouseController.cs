using System;
using Data;
using MapGenerator;
using UnityEngine;
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
        private readonly Color _tileColor = new(1f, 1, 1, 0.45f);

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
        }

        public void Tick()
        {
            _currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var tileUnderMouse = GetTileAtWorldCoord(_currentMousePosition.x, _currentMousePosition.y);

            HighLightUnderMouse(tileUnderMouse);

            DragMove(_currentMousePosition);
        }

        private void HighLightUnderMouse(Tile tileUnderMouse)
        {
            if (_dragAction) return;

            if (tileUnderMouse == null)
            {
                _tilemap.ClearAllTiles();
            }
            else
            {
                _tilemap.ClearAllTiles();
                var tilePos = new Vector3Int(tileUnderMouse.X, tileUnderMouse.Y, 0);
                _tilemap.SetTile(tilePos, _tile);
                _tilemap.SetTileFlags(tilePos, TileFlags.None);
                _tilemap.SetColor(tilePos, _tileColor);
            }
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
            }
        }

        private void DragEnd(int startX, int endX, int startY, int endY)
        {
            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    var t = _worldController.GetTile(x, y);

                    if (t != null)
                    {
                        switch (_config.buildMode)
                        {
                            case BuildMode.World:
                                switch (_config.worldTileToPlace)
                                {
                                    case TileType.None:
                                        if (startX == endX && startY == endY)
                                        {
                                            Debug.Log($"({t.X}, {t.Y})");
                                        }
                                        else
                                        {
                                            Debug.Log($"({startX}, {startY}) - ({endX}, {endY})");
                                        }

                                        _dragAction = false;
                                        _tilemap.ClearAllTiles();
                                        return;
                                    default:
                                        if (_config.drawMode == DrawMode.NoiseMap) return;
                                        t.Type = _config.worldTileToPlace;
                                        break;
                                }
                                break;
                                    case BuildMode.Construction:
                                        switch (_config.constructionTileToPlace)
                                        {
                                            default:
                                                if (_config.drawMode == DrawMode.NoiseMap) return;
                                                t.InstalledObject = _config.constructionTileToPlace;
                                                break; 
                                        } 
                                        break;
                        }
                    }
                    
                    _dragAction = false;
                    _tilemap.ClearAllTiles();
                }
            }

            _dragAction = false;
            _tilemap.ClearAllTiles();
            }

            private void DragHighLight(int startX, int endX, int startY, int endY)
            {
                _tilemap.ClearAllTiles();

                for (var x = startX; x <= endX; x++)
                {
                    for (var y = startY; y <= endY; y++)
                    {
                        var t = _worldController.GetTile(x, y);

                        if (t == null) continue;
                    
                        var vector3Int = new Vector3Int(x, y, 0);
                        _tilemap.SetTile(vector3Int, _tile);
                        _tilemap.SetTileFlags(vector3Int, TileFlags.None);
                        _tilemap.SetColor(vector3Int, _tileColor);
                    }
                }
            }

            private Tile GetTileAtWorldCoord(float x, float y)
            {
                return _worldController.GetTile(Mathf.FloorToInt(x + TileOffset), Mathf.FloorToInt(y + TileOffset));
            }
    }
}