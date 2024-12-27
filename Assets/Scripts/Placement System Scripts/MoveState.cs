using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IBuildingState
{
    // Set up member variables for the state
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    // Constructor to set up variables as well as preview
    public MoveState(Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        // Set default values
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        // Turn on the cursor preview
        previewSystem.StartShowingCursorPreview();
    }

    // Stops preview and ends the state
    public void EndState()
    {
        // Turn off the cursor preview
        previewSystem.StopShowingPreview();
    }

    // Pick up the selected object and allow the user to place it back down wherever is valid
    public void OnAction(Vector3Int gridPosition, Vector3 rotation)
    {
        // If there is an object to pick up
        // Pick up the object
        // Transition to placing that object

        // Check if there is an object at the position on the grid
        int objectID = furnitureData.GetObjectIDAt(gridPosition);
        if (objectID != -1) // -1 indicates no ID was found
        {
            // Get the object's index from the furniture data
            int gameObjectIndex = furnitureData.GetObjectIndex(gridPosition);

            // Get the objects rotation and prefab from the grid data
            GameObject objectPrefab = objectPlacer.GetObjectPrefabAt(gameObjectIndex);
            Quaternion objectRotation = objectPrefab.transform.rotation;

            // Destroy the object from the grid
            furnitureData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObject(gameObjectIndex);
        }
    }

    // Returns whether or not the object can be placed using information in the grid data
    private bool CanPlaceObject(Vector3Int gridPosition, int selectedObjectIndex)
    {
        // Return whether or not the object can be placed according to the grid data
        return furnitureData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    // Updates the preview if there is something to pick up
    // TODO: Update cursor with the move arrow icon if valid?
    public void UpdateState(Vector3Int gridPosition, Vector3 rotation)
    {
        // If there is an object, preview white to pick up
        if (furnitureData.GetObjectIndex(gridPosition) != -1)
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
        // Otherwise preview red as there is nothing to pick up
        else
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }
}
