using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
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

    // Walls that are currently hidden for re-enabling after no longer intersecting
    Wall leftWall = null, rightWall = null;

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

    private void Update()
    {
        generateWallRaycasts(-3, ref leftWall, ref rightWall); // Left raycasts
        generateWallRaycasts(3, ref rightWall, ref leftWall); // Right raycasts
    }

    private void generateWallRaycasts(int directionOffset, ref Wall wallDirection, ref Wall oppositeWall)
    {
        // Generate three rays, each one vector unit apart
        for (int count = 0; count < 3; count++)
        {
            // Generate the offset
            int offset = directionOffset;
            if (directionOffset < 0 && count > 0)
                offset += -(count);
            else if (count > 0)
                offset += count;

            // Generate a ray from the camera, level out the y-axis, and add the offset
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2));
            ray.origin = new Vector3(ray.origin.x, 4, ray.origin.z) + (mainCamera.transform.right * offset);
            ray.direction = new Vector3(ray.direction.x, 0, ray.direction.z);

            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red); // DEBUG

            // Perform the raycast
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Wall wall = hit.collider.GetComponentInParent<Wall>(); // Check for actual Wall component
                if (wall != null)
                {
                    // Make sure the wall isn't the one we're already hitting
                    if (wall != wallDirection)
                    {
                        // Re-enable the last hit wall if it is valid
                        if (wallDirection != null && wallDirection != oppositeWall)
                            wallDirection.EnableUpperWall();

                        wallDirection = wall;
                        wall.DisableUpperWall();
                    }
                }
            }
        }
    }
}
