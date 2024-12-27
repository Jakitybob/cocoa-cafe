using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabase : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    // Name of the object to display to the user
    [field: SerializeField]
    public string Name { get; private set; } // Ensure access to the setter is private, this is a property

    // Unique ID of the object
    [field: SerializeField]
    public int ID { get; private set; }

    // Size in grid-cells of the object
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    // Prefab containing the mesh for the object
    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public float Price { get; private set; } = 0f;
}