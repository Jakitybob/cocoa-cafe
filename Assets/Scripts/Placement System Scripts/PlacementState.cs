using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The idea is to avoid giving the placement state ANY knowledge of the placement system
public class PlacementState : IBuildingState
{
    // Set up member variables for the state
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData floorData, furnitureData;
    ObjectPlacer objectPlacer;
    Quaternion rotation;

    // Constructor; also sets up placement preview as well as variables
    public PlacementState(int ID, Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.rotation = Quaternion.Euler(Vector3.zero);

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID); // Short hand lambda for-loop to find matching data
        
        // Make sure the object index is valid
        if (selectedObjectIndex > -1)
        {
            // Enable grid visualization and cell indicator
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {ID} found.");
    }

    // Stops preview and ends state of placement
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    // What to do when the state is "acted" upon
    public void OnAction(Vector3Int gridPosition)
    {
        // Check if placement in this position is valid and the user has enough money
        bool canPlace = CanPlaceObject(gridPosition, selectedObjectIndex);

        // Return now if placement cannot occur
        if (!canPlace)
            return;

        // Place the desired objectu
        int objectIndex = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), rotation.eulerAngles);

        // Remove the funds from the player
        GameManager.instance.moneyManager.AddMoney(database.objectsData[selectedObjectIndex].Price * -1);

        // Add the placed object the grid data
        furnitureData.AddObjectAt(gridPosition, rotation, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, objectIndex, selectedObjectIndex);

        // Update the preview to show validity
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        previewSystem.UpdateRotation(rotation.eulerAngles);
    }

    // Returns whether or not the object can be placed using information in the grid data
    private bool CanPlaceObject(Vector3Int gridPosition, int selectedObjectIndex)
    {
        // Return false if the player does not have enough balance to purchase the object
        if (GameManager.instance.moneyManager.GetMoney() < database.objectsData[selectedObjectIndex].Price)
            return false;

        // Set the selected grid data to furniture data as floor data has not yet been implemented
        GridData selectedData = furnitureData;

        // Return whether or not the object can be placed according to the grid data
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    // Updates the state, to be used on tick
    public void UpdateState(Vector3Int gridPosition, Vector3 rotation)
    {
        // Check if placement in this position is valid
        bool canPlace = CanPlaceObject(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), canPlace);

        // Update rotation and preview
        this.rotation.eulerAngles += rotation;
        previewSystem.UpdateRotation(this.rotation.eulerAngles);
    }
}
