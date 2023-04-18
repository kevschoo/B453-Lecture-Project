using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float screenEdgeThreshold = 10f;
    public float cameraSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 1f;
    public float maxZoom = 10f;
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);

    private Camera cam;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -cam.transform.position.z));

        float screenEdgeX = cam.pixelWidth - screenEdgeThreshold;
        float screenEdgeY = cam.pixelHeight - screenEdgeThreshold;

        if (mousePosition.x < screenEdgeThreshold || mousePosition.x > screenEdgeX ||
            mousePosition.y < screenEdgeThreshold || mousePosition.y > screenEdgeY)
        {
            targetPosition = new Vector3(worldMousePosition.x, worldMousePosition.y, transform.position.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);

        // Clamp the camera's position to the specified bounds
        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float camHalfHeight = cam.orthographicSize;
        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        // Zoom in and out using the scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            float newSize = Mathf.Clamp(cam.orthographicSize - scrollInput * zoomSpeed, minZoom, maxZoom);
            cam.orthographicSize = newSize;
        }
    }
}