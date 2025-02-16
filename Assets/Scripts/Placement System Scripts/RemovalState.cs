using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalState : IBuildingState
{
    // Set up member variables for the state
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData floorData, furnitureData;
    ObjectPlacer objectPlacer;

    public RemovalState(Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingCursorPreview();
    }

    // Ends the state of removal and disables the preview
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    // Removes the object at the position on action of the state
    public void OnAction(Vector3Int gridPosition)
    {
        GridData data = null;

        // If furniture exists in the tile, select that as it has to be removed before anything below
        if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
            data = furnitureData;

        // Otherwise if the floor exists, select that for removal
        else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
            data = floorData;

        // Make sure something was found to remove and reutrn if not
        if (data == null)
            return;

        // Remove the object here
        else
        {
            // Find the index of the object to remove and ensure it is valid
            gameObjectIndex = data.GetPlacerIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;

            // Find the object in the database from its ID
            int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == furnitureData.GetObjectIDAt(gridPosition)); // Short hand lambda for-loop to find matching data

            // Refund the money from the object to the player
            // TODO: Evaluate refunding less than full price?
            if (selectedObjectIndex != -1)
                GameManager.instance.moneyManager.AddMoney(database.objectsData[selectedObjectIndex].Price);

            // Remove the object from the grid data
            data.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObject(gameObjectIndex);
        }

        // Update preview system visuals
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, true);
    }

    // Update the preview for the state
    public void UpdateState(Vector3Int gridPosition, Vector3 rotation)
    {
        bool canPlace = furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), canPlace);
    }
}
