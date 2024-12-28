using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IBuildingState
{
    // Set up member variables for the state
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    bool isHoldingObject = false; // Whether or not an object is currently being held
    heldObject held;

    // A struct containing data on the currently held object
    private struct heldObject
    {
        public int objectIndex;
        public Vector3Int lastPosition;
        public Quaternion lastRotation;
        public Quaternion currentRotation;

        public heldObject(int objectIndex, Vector3Int position, Quaternion rotation)
        {
            this.objectIndex = objectIndex;
            this.lastPosition = position;
            this.lastRotation = rotation;
            this.currentRotation = rotation;
        }
    }

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
        // If we are currently holding an object, place it back in its last valid position
        if (isHoldingObject)
        {
            // Place the object on the grid again
            int placerIndex = objectPlacer.PlaceObject(database.objectsData[held.objectIndex].Prefab, grid.CellToWorld(held.lastPosition), held.lastRotation.eulerAngles);
            furnitureData.AddObjectAt(held.lastPosition, held.lastRotation, database.objectsData[held.objectIndex].Size, database.objectsData[held.objectIndex].ID, placerIndex, held.objectIndex);
        }

        // Turn off the cursor preview
        previewSystem.StopShowingPreview();
    }

    // Pick up the selected object and allow the user to place it back down wherever is valid
    public void OnAction(Vector3Int gridPosition, Vector3 rotation)
    {
        // If not currently holding an object, try to pick up an object
        if (!isHoldingObject)
        {
            // Check if there is an object at the position on the grid
            int objectID = furnitureData.GetObjectIDAt(gridPosition);
            if (objectID != -1) // -1 indicates no ID was found
            {
                // Get the object's index from the furniture data
                int gameObjectIndex = furnitureData.GetDatabaseIndex(gridPosition);

                // Use the object's index from the database and create a new heldObject struct
                held = new heldObject(gameObjectIndex, gridPosition, furnitureData.GetObjectRotationAt(gridPosition));
                isHoldingObject = true;

                // Remove the object from the grid
                objectPlacer.RemoveObject(furnitureData.GetPlacerIndex(gridPosition));
                furnitureData.RemoveObjectAt(gridPosition);

                // Enable the preview of the now held object
                previewSystem.StartShowingPlacementPreview(database.objectsData[gameObjectIndex].Prefab, database.objectsData[gameObjectIndex].Size);
                previewSystem.UpdatePosition(gridPosition, true);
                previewSystem.UpdateRotation(held.currentRotation.eulerAngles);
            }
        }
        // Else try to place the object
        else
        {
            // Check if the object can be placed
            if (CanPlaceObject(gridPosition, held.objectIndex))
            {
                // Place the object on the grid again
                int placerIndex = objectPlacer.PlaceObject(database.objectsData[held.objectIndex].Prefab, grid.CellToWorld(gridPosition), held.currentRotation.eulerAngles);
                furnitureData.AddObjectAt(gridPosition, held.currentRotation, database.objectsData[held.objectIndex].Size, database.objectsData[held.objectIndex].ID, placerIndex, held.objectIndex);

                // Turn off the preview object
                previewSystem.StopShowingPreview();
                previewSystem.StartShowingCursorPreview();

                // Toggle held state
                isHoldingObject = false;
            }
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
        // If holding an object, preview whether or not the object can be placed
        if (isHoldingObject)
        {
            // Update the object's rotation
            // TODO: Fix this <-----
            //held.currentRotation = Quaternion.Euler(held.currentRotation.eulerAngles + rotation);
            //previewSystem.UpdateRotation(rotation);

            // Display white if the object can be placed
            if (CanPlaceObject(gridPosition, held.objectIndex))
                previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
            // Otherwise display red if it cannot
            else
                previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }
        // Else simply update the cursor as always "true"
        else
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
    }
}
