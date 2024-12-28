using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedObjects = new(); // List of placed objects, can be used for undo system as well?
    
    // Instantiate and place a new game object
    public int PlaceObject(GameObject objectPrefab, Vector3 position, Vector3 rotation)
    {
        GameObject newObject = Instantiate(objectPrefab);
        newObject.transform.position = position;
        newObject.transform.GetChild(0).Rotate(rotation); // Rotate the object mesh as all meshes are parented to an empty
        placedObjects.Add(newObject);

        // Return the index of the newly placed object
        return placedObjects.Count - 1;
    }

    // Destroy an instantiated game object
    public void RemoveObject(int gameObjectIndex)
    {
        // Make sure the index exists in our data and return if not
        if (placedObjects.Count <= gameObjectIndex || placedObjects[gameObjectIndex] == null)
            return;

        // Destroy the game object and remove it from the list
        Destroy(placedObjects[gameObjectIndex]);
        placedObjects[gameObjectIndex] = null;
    }
}
