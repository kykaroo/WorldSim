using UnityEngine;
using Zenject;

public class CameraController : ITickable
{
    private readonly Camera _camera;
    
    private Vector3 _lastMousePosition;
    private float _sensitivity = 10f;
    private bool _drag;

    [Inject]
    public CameraController(Camera camera)
    {
        _camera = camera;
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
                _camera.transform.position += Vector3.forward * 50;
                break;
            case < 0:
                _camera.transform.position -= Vector3.forward * 50;
                break;
        }
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var mouseDelta = _lastMousePosition - Input.mousePosition;

            _camera.transform.position += new Vector3(mouseDelta.x, mouseDelta.y, 0);

            _lastMousePosition = Input.mousePosition;
        }

        _lastMousePosition = Input.mousePosition;
    }
}