using UnityEngine;
using Zenject;

public class CameraController : ITickable
{
    private readonly Camera _camera;
    
    private Vector3 _lastMousePosition;
    private bool _isDragAction;
    private Vector3 _origin;
    private Vector3 _difference;

    [Inject]
    public CameraController(Camera camera)
    {
        _camera = camera;
        _camera.transform.position = new(50, 50, -10);
    }


    public void Tick()
    {
        MoveCamera();
        CameraZoom();
    }

    private void CameraZoom()
    {
        switch (Input.mouseScrollDelta.y)
        {
            case > 0:
                _camera.orthographicSize -= 0.5f;
                break;
            case < 0:
                _camera.orthographicSize += 0.5f;
                break;
        }
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var mousePositionX = Input.mousePosition.x;
            var mousePositionY = Input.mousePosition.y;
            _difference = _camera.ScreenToWorldPoint(new(mousePositionX, mousePositionY, _camera.transform.position.z)) - _camera.transform.position;
            
            if (!_isDragAction)
            {
                _isDragAction = true;
                _origin = _camera.ScreenToWorldPoint(new(mousePositionX, mousePositionY, _camera.transform.position.z));
            }
        }
        else
        {
            _isDragAction = false;
        }

        if (_isDragAction)
        {
            _camera.transform.position = _origin - _difference;
        }
    }
}