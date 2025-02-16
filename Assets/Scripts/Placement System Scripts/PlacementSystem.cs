using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour, IGameData
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase database;
    private GridData floorData, furnitureData; // Since floor tiles can have furniture on them, keep two separate sets of data

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero; // Used to make Update() more performant

    IBuildingState buildingState;

    // Initialize member variables in Awake
    private void Awake()
    {
        // Initialize member variables
        floorData = new GridData();
        furnitureData = new GridData();
    }

    // Start placement off by default
    private void Start()
    {
        // Make sure game starts in non-placement mode
        StopCurrentState();
        gridVisualization.SetActive(false);

        // Bind the listener for entering build mode
        inputManager.OnBuildMode += EnterBuildMode;
    }

    // Enters build mode, calling the default state
    public void EnterBuildMode()
    {
        // Enter the default state
        DefaultState();

        // Handle listener bindings for build mode
        HandleBuildModeBindings();
    }

    // Handles the build mode bindings, to ensure they are bound correctly in each state
    private void HandleBuildModeBindings()
    {
        inputManager.OnBuildMode -= EnterBuildMode;
        inputManager.OnBuildMode += ExitBuildMode;
    }

    // Enters the default state which is movement, and handles all bindings
    private void DefaultState()
    {
        StopCurrentState(); // Make sure to stop any prior state
        StartMovement(); // Enter the movement state as default
    }

    // Starts the state of placement
    public void StartPlacement(int ID)
    {
        StopCurrentState(); // Make sure to stop placement before trying to place a new object
        HandleBuildModeBindings();
        gridVisualization.SetActive(true);

        // Instantiate the placement state
        buildingState = new PlacementState(ID, grid, preview, database, floorData, furnitureData, objectPlacer);

        // Assign listeners to on clicked and on exit
        inputManager.OnClicked += BuildingModeAction;
        inputManager.OnExit -= ExitBuildMode;
        inputManager.OnExit += DefaultState;
        inputManager.OnRotate += RotatePlacement;
    }

    // Starts the state of removal
    public void StartRemoval()
    {
        // Stop placement and turn on the grid
        StopCurrentState();
        HandleBuildModeBindings();
        gridVisualization.SetActive(true);
        buildingState = new RemovalState(grid, preview, database, floorData, furnitureData, objectPlacer);

        // Assign listeners to on clicked and on exit
        inputManager.OnClicked += BuildingModeAction;
        inputManager.OnExit -= ExitBuildMode;
        inputManager.OnExit += DefaultState;
    }

    // Starts the state of moving objects
    public void StartMovement()
    {
        // Stop previous state and handle build mode bindings
        StopCurrentState();
        HandleBuildModeBindings();

        // Instantiate the movement state
        buildingState = new MoveState(grid, preview, database, furnitureData, objectPlacer);
        gridVisualization.SetActive(true);

        // Assign listeners to on clicked and on exit
        inputManager.OnClicked += BuildingModeAction;
        inputManager.OnExit -= DefaultState;
        inputManager.OnExit += ExitBuildMode;
        inputManager.OnRotate += RotatePlacement;
    }

    // Instantiates the specified object from the database onto the grid position
    private void BuildingModeAction()
    {
        // Return early if the pointer is over the UI
        if (inputManager.IsPointerOverUI())
            return;

        // Calculate the mouse position and its position in the grid
        Vector3Int gridPosition = MouseToGrid(inputManager.GetSelectedMapPosition());

        // Call the placement state
        buildingState.OnAction(gridPosition);
    }

    // Resets member variables to default state
    private void StopCurrentState()
    {
        // Only stop placement if we are in the state of placing
        if (buildingState == null)
            return;

        // End the state of placement
        buildingState.EndState();
        buildingState = null;

        // Remove listeners on clicked and on exit
        inputManager.OnClicked -= BuildingModeAction;
        inputManager.OnExit -= DefaultState;
        inputManager.OnExit += ExitBuildMode;
        inputManager.OnRotate -= RotatePlacement;
        lastDetectedPosition = Vector3Int.zero; // Reset the last detected position since placement/removal is done
    }

    // Turns off the grid preview and ends the current state, fully "exiting" build mode
    private void ExitBuildMode()
    {
        // Disable grid visuals
        gridVisualization.SetActive(false);

        // Stop the current state
        StopCurrentState();

        // Unbind listeners and assign the build mode listener
        inputManager.OnExit -= ExitBuildMode; // Unbind the exit as we won't be in build mode anymore
        inputManager.OnExit -= ExitBuildMode; // Unbind it twice because for some reason it binds twice
        inputManager.OnExit -= DefaultState;
        inputManager.OnBuildMode -= ExitBuildMode;
        inputManager.OnBuildMode += EnterBuildMode;
    }

    // Rotates object in-placement 90 degrees
    private void RotatePlacement()
    {
        // Calculate the mouse position and its position in the grid to update the building state
        Vector3Int gridPosition = MouseToGrid(inputManager.GetSelectedMapPosition());

        // Update the preview and building state
        //preview.UpdateRotation(new Vector3(0, yAxisRotation, 0));
        buildingState.UpdateState(gridPosition, new Vector3(0, 90, 0));
    }

    private void Update()
    {
        // Return early if we are not in a building mode
        if (buildingState == null)
            return;

        // Calculate the mouse position and its position in the grid for preview purposes
        Vector3Int gridPosition = MouseToGrid(inputManager.GetSelectedMapPosition());

        // Only perform preview update logic if position has changed
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition, Vector3.zero);
            lastDetectedPosition = gridPosition; // Update the last detected position
        }
    }

    private Vector3Int MouseToGrid(Vector3 mousePosition)
    {
        return grid.WorldToCell(mousePosition);
    }


    //
    // SAVING & LOADING
    //

    public void LoadData(GameData data)
    {
        // Clear all placed objects and empty the grid data
        objectPlacer.RemoveAllObjects();

        if (floorData != null)
            floorData.RemoveAllObjects();
        else
            Debug.Log("Floor data was detected as null.");

        if (furnitureData != null)
            furnitureData.RemoveAllObjects();
        else
            Debug.Log("Furniture data was detected as null.");

        // Loop through all placement data found in the game data
        foreach (var placed in data.furniturePlacementData)
        {
            // Place the new object and update the index in the data
            int newIndex = objectPlacer.PlaceObject(database.objectsData[placed.databaseIndex].Prefab, placed.occupiedPositions[0], placed.rotation.eulerAngles);
            placed.placedObjectIndex = newIndex;

            // Add the new object's data to the grid data
            furnitureData.AddObjectAt(placed.occupiedPositions[0], placed.rotation, placed.objectSize, placed.ID, placed.placedObjectIndex, placed.databaseIndex);
        }
    }

    public void SaveData(ref GameData data)
    {
        // Save floor data
        data.floorPlacementData.Clear(); // Clear out all existing data before entering new data
        floorData.SavePlacementData(ref data.floorPlacementData);

        // Save furniture data
        data.furniturePlacementData.Clear(); // Clear out all existing data before entering new data
        furnitureData.SavePlacementData(ref data.furniturePlacementData);
    }
}
