using Data;
using MapGenerator;
using UnityEngine;
using Zenject;

public class MouseController : ITickable
{
    private readonly WorldController _worldController;
    private readonly Camera _camera;
    private readonly GameObject _highLight;
    private readonly GenerationConfig _config;

    private Vector3 _dragStart;
    private const float TileOffset = 0.5f; // Равно половине размера тайла

    [Inject]
    public MouseController(WorldController worldController, Camera camera, GameObject highLight, GenerationConfig config)
    {
        _worldController = worldController;
        _camera = camera;
        _highLight = highLight;
        _config = config;
    }

    public void Tick()
    {
        var currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var tileUnderMouse = GetTileAtWorldCoord(currentMousePosition.x, currentMousePosition.y);

        if (tileUnderMouse == null)
        {
            _highLight.SetActive(false);
        }
        else
        {
            _highLight.transform.position = tileUnderMouse.transform.position;
            _highLight.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _dragStart = currentMousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
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
                                break;
                            default:
                                if(_config.drawMode == DrawMode.NoiseMap) return;
                                t.Type = _config.tileToPlace;
                                break;
                        }
                    }
                }
            }
        }
    }
    
    private Tile GetTileAtWorldCoord(float x, float y)
    {
        return _worldController.GetTile(Mathf.FloorToInt(x + TileOffset), Mathf.FloorToInt(y + TileOffset));
    }
}