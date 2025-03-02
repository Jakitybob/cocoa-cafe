using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main manager for the game: handles saving, loading, etc
public class GameManager : MonoBehaviour
{
    // Singleton instance of this class
    public static GameManager instance;

    [Header("Managers")]
    [SerializeField] public InterfaceManager interfaceManager;
    [SerializeField] public MoneyManager moneyManager;

    public bool isPaused = false;
    private float defaultGameTimescale;

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

        // Get the default timescale from launch
        defaultGameTimescale = Time.timeScale;
    }

    // Toggles the paused status of the game, and slows down game time or returns it to normal based on that state
    public void TogglePauseGame()
    {
        isPaused = !isPaused;

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = defaultGameTimescale;
    }
}
