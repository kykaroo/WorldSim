using UnityEngine;

[CreateAssetMenu(menuName = "Camera/CameraConfig", fileName = "Camera Config")]
public class CameraConfig : ScriptableObject
{
    public Camera camera;
    public Vector3 startPosition;
    public float zoomSensitivity;
    public float zoomMin;
    public float zoomMax;
}