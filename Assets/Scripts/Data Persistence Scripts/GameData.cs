using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float money;

    // The values in this constructor are the "default values" for a new game
    public GameData()
    {
        money = 1000f;
    }
}
