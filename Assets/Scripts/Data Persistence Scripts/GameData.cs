using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float money;
    public List<PlacementData> floorPlacementData;
    public List<PlacementData> furniturePlacementData;

    // The values in this constructor are the "default values" for a new game
    public GameData()
    {
        money = 1000f;
        floorPlacementData = new();
        furniturePlacementData = new();
    }
}
