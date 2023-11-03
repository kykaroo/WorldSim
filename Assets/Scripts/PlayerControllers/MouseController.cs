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
        private readonly Tile _tile;
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

            _tile = ScriptableObject.CreateInstance<Tile>();
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
                var vector3Int = new Vector3Int(tileUnderMouse.X, tileUnderMouse.Y, 0);
                _tilemap.SetTile(vector3Int, _tile);
                _tilemap.SetTileFlags(vector3Int, TileFlags.None);
                _tilemap.SetColor(vector3Int, _tileColor);
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
                for (var x = startX; x <= endX; x++)
                {
                    for (var y = startY; y <= endY; y++)
                    {
                        var t = _worldController.GetTile(x, y);

                        if (t != null)
                        {
                            var vector3Int = new Vector3Int(x, y, 0);
                            _tilemap.SetTile(vector3Int, _tile);
                            _tilemap.SetTileFlags(vector3Int, TileFlags.None);
                            _tilemap.SetColor(vector3Int, _tileColor);
                        }
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                for (var x = startX; x <= endX; x++)
                {
                    for (var y = startY; y <= endY; y++)
                    {
                        var t = _worldController.GetTile(x, y);

                        if (t != null)
                        {
                            switch (_config.tileToPlace)
                            {
                                case TileType.None:
                                    Debug.Log($"{t.X}, {t.Y}");
                                    break;
                                default:
                                    if (_config.drawMode == DrawMode.NoiseMap) return;
                                    t.Type = _config.tileToPlace;
                                    break;
                            }
                        }
                    }
                }

                _tilemap.ClearAllTiles();
            }
        }

        private Tile GetTileAtWorldCoord(float x, float y)
        {
            return _worldController.GetTile(Mathf.FloorToInt(x + TileOffset), Mathf.FloorToInt(y + TileOffset));
        }
    }
}