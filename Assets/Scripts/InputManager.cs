using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;

    [SerializeField]
    private CameraManager cameraManager;

    [SerializeField]
    private LayerMask placementLayerMask;
    public event Action OnClicked, OnExit, OnRotate, OnBuildMode;

    // Input actions
    InputAction movement;
    InputAction leftClick;
    InputAction escape;
    InputAction rotateCameraAction;
    InputAction mouseAxes;
    InputAction rotate;
    InputAction buildMode;

    // Sets up default member variables
    public void Start()
    {
        // Get input actions
        movement = InputSystem.actions.FindAction("Move");
        leftClick = InputSystem.actions.FindAction("LeftMouse");
        escape = InputSystem.actions.FindAction("Escape");
        rotateCameraAction = InputSystem.actions.FindAction("RotateCamera");
        mouseAxes = InputSystem.actions.FindAction("Look");
        rotate = InputSystem.actions.FindAction("Rotate");
        buildMode = InputSystem.actions.FindAction("Build");
    }

    // Listens for all inputs
    private void Update()
    {
        // Call any listeners bound to left click
        if (leftClick.WasPressedThisFrame())
            OnClicked?.Invoke();

        // Call any listeners bound to the escape action
        if (escape.WasPressedThisFrame())
        {
            // If there are no other delegates bound to exit, open the pause menu
            if (OnExit == null || OnExit.GetInvocationList().Length <= 0)
            {
                // Open pause if there is no menu open
                if (GameManager.instance.interfaceManager.activeMenu == null)
                    GameManager.instance.interfaceManager.OpenPauseMenu();
                // Otherwise close the current menu out
                else
                    GameManager.instance.interfaceManager.CloseCurrentMenu();
            }
            // Otherwise invoke its delegates
            else
                OnExit?.Invoke();
        }
            
        // Call any listeners bound to the OnRotate action
        if (rotate.WasPressedThisFrame())
            OnRotate?.Invoke();

        // Rotate the camera while the camera action is being pressed
        if (rotateCameraAction.IsInProgress())
            cameraManager.RotateCamera(mouseAxes.ReadValue<Vector2>(), Time.deltaTime);

        // Move the camera
        if (movement.IsInProgress())
            cameraManager.MoveCamera(movement.ReadValue<Vector2>(), Time.deltaTime);

        // Enter build mode
        if (buildMode.WasPressedThisFrame())
            OnBuildMode?.Invoke();

    }

    // Lambda that ensures the pointer is over a game object and not a UI element
    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    // Gets the position in game where the cursor is.
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane; // Make sure only visible objects can be detected
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        placementLayerMask = LayerMask.GetMask("Default");

        // Perform a raycast to the mouse position
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
}
