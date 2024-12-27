using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main manager for the game: handles saving, loading, etc
public class GameManager : MonoBehaviour
{
    // Singleton instance of this class
    public static GameManager instance;

    [SerializeField]
    public InterfaceManager interfaceManager;

    [SerializeField]
    public MoneyManager moneyManager;

    // Called before the first frame, used to ensure no second instance is created
    private void Awake()
    {
        // Destroy this secondary game manager if one already exists
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        // Otherwise set this game manager to the instance
        instance = this;
    }
}
