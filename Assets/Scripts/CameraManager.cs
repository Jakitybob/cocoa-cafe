using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float rotationSpeed = 25f;
    [SerializeField]
    private float cameraSpeed = 5f;

    [SerializeField]
    private float minX = -5f, maxX = 5f;
    [SerializeField]
    private float minY = -5f, maxY = 5f;

    // Rotates the camera based on the x-axis of the mouse
    public void RotateCamera(Vector2 mouseAxes, float deltaTime)
    {
        // Calculate the rotation speed
        float yRotation = (float)(mouseAxes.x * rotationSpeed) * deltaTime;

        // Rotate the camera about the y axis in world space
        mainCamera.transform.Rotate(0, yRotation, 0, Space.World);
    }

    // Moves the camera based on the pressed keys
    public void MoveCamera(Vector2 movementAxes, float deltaTime)
    {
        // Calculate the movement direction
        Vector3 moveDirection = movementAxes.x * mainCamera.transform.right + movementAxes.y * mainCamera.transform.forward; // Unit vector for directional movement
        moveDirection.y = 0; // Zero out the y-axis as the camera is intended to stay level

        // Add the movement direction to our speed against delta time to the transform of the camera
        mainCamera.transform.position += moveDirection * cameraSpeed * deltaTime;

        // Clamp the position inside of the specified X and Y bounds
        mainCamera.transform.position = new Vector3(Mathf.Clamp(mainCamera.transform.position.x, minX, maxX), 0.6f, Mathf.Clamp(mainCamera.transform.position.z, minY, maxY));
    }
}
