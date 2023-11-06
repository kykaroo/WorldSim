using UnityEngine;
using Zenject;

namespace PlayerControllers
{
    public class CameraController : ITickable
    {
        private readonly Camera _camera;
        private readonly CameraConfig _cameraConfig;
    
        private Vector3 _lastMousePosition;
        private bool _isDragAction;
        private Vector3 _origin;
        private Vector3 _difference;

        [Inject]
        public CameraController(CameraConfig cameraConfig, Camera camera)
        {
            _cameraConfig = cameraConfig;
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
                    if (_camera.orthographicSize <= _cameraConfig.zoomMin) return;
                    _camera.orthographicSize -= _cameraConfig.zoomSensitivity;
                    break;
                case < 0:
                    if (_camera.orthographicSize >= _cameraConfig.zoomMax) return;
                    _camera.orthographicSize += _cameraConfig.zoomSensitivity;
                    break;
            }
        }

        private void MoveCamera()
        {
            if (Input.GetKey(KeyCode.Mouse2))
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
}