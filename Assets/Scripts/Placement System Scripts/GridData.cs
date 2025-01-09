using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // Storage type (map-like data structure) for storing cell placement
    Dictionary<Vector3Int, PlacementData> placedObjects = new(); // New C# shorthand to avoid re-defining dictionary

    // Add an object to the list of stored grid data
    public void AddObjectAt(Vector3Int gridPosition, Quaternion rotation, Vector2Int objectSize, int ID, int placerIndex, int databaseIndex)
    {
        // Get the list of positions the object occupies
        List<Vector3Int> positions = CalculatePositions(gridPosition, objectSize);

        // Create our placement data object to add to the dictionary
        PlacementData data = new PlacementData(positions, objectSize, rotation, ID, placerIndex, databaseIndex);

        // Loop through each place in the dictionary and add the data or return a collision detected
        foreach(var position in positions)
        {
            // Check for collision and throw exception, as no collision should be possible here
            if (placedObjects.ContainsKey(position))
                throw new System.Exception($"Dictionary already contains this cell position {position}");

            // Add the key to the dictionary with the data
            placedObjects[position] = data;
        }
    }

    // Remove an object from the list of stored grid data
    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        // Remove every entry in the dictionary for the related object
        foreach(var position in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(position);
        }
    }

    // Clear out the dictionary of stored data, to be used for loading in new data
    public void RemoveAllObjects()
    {
        placedObjects.Clear();
    }

    // Calculates the positions an object takes up given its starting position and size
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // Create the list to fill out and return
        List<Vector3Int> positions = new();

        // Iterate through each row and column of the size of our object
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                // Add the space offset from the original position to the list of filled positions
                positions.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        // Return our filled out list of positions
        return positions;
    }

    // Checks whether or not the cell positions are occupied or not
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        // Get the positions the object will occupy
        List<Vector3Int> positions = CalculatePositions(gridPosition, objectSize);

        // Loop through each position and check if it is occupied, and return false if it is
        foreach (var position in positions)
        {
            if (placedObjects.ContainsKey(position))
                return false;
        }

        // Return true if no collisions were detected
        return true;
    }

    // Returns the objects index from the internal dictionary or -1 if not found
    public int GetPlacerIndex(Vector3Int gridPosition)
    {
        // Return -1 if no entry exists for that position
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;

        // Return the object index if it does exist in the dictionary
        return placedObjects[gridPosition].placedObjectIndex;
    }

    // Returns the object's database index or -1 if not found
    public int GetDatabaseIndex(Vector3Int gridPosition)
    {
        // Return -1 if no entry exists for that position
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;

        // Return the database index if it does exist in the dictionary
        return placedObjects[gridPosition].databaseIndex;
    }

    // Returns the objects ID from the internal dictionary or -1 if not found
    public int GetObjectIDAt(Vector3Int gridPosition)
    {
        // Return -1 if no entry exists for that position
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;

        // Return the object ID if it does exist in the dictionary
        return placedObjects[gridPosition].ID;
    }

    // Returns the objects rotation from the internal dictionary or -1 if not found
    public Quaternion GetObjectRotationAt(Vector3Int gridPosition)
    {
        // Return -1 if no entry exists for that position
        if (!placedObjects.ContainsKey(gridPosition))
            return Quaternion.identity;

        // Return the object ID if it does exist in the dictionary
        return placedObjects[gridPosition].rotation;
    }


    //
    // SAVING & LOADING
    //

    // Saves each placed object to the GameData's list of PlacementData
    public void SavePlacementData(ref List<PlacementData> list)
    {
        // Loop through each placed key in the dictionary of objects
        foreach(var key in placedObjects.Keys)
        {
            list.Add(placedObjects[key]);
        }
    }

    // Loads in placement data from the GameData's list of PlacementData
    public void LoadPlacementData(GameData data)
    {
        // for each placement data stored in the game data list
            // add the placement data to the grid data
            // place the object in the world
    }
}

// Contains data on object placed in grid cells
[System.Serializable]
public class PlacementData
{
    // List of all occupied positions on the grid, for the case of things like 2x1+ objects
    public List<Vector3Int> occupiedPositions;

    // The size of the object
    public Vector2Int objectSize;

    // The quaternion rotation of the object
    public Quaternion rotation;

    // The ID of the object type
    public int ID; // Useful for saving/loading grid data

    // The index of the object in the object placer's storage
    public int placedObjectIndex;

    // The index of the object's information in the object database it is from
    public int databaseIndex;

    // Constructor
    public PlacementData(List<Vector3Int> occupiedPositions, Vector2Int objectSize, Quaternion rotation, int ID, int placedObjectIndex, int databaseIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.objectSize = objectSize;
        this.rotation = rotation;
        this.ID = ID;
        this.placedObjectIndex = placedObjectIndex;
        this.databaseIndex = databaseIndex;
    }
}
