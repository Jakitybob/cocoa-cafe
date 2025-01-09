using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Starts a new game with fresh save data
    public void StartNewGameButton()
    {

    }

    // Loads the game from the found save data
    public void LoadSavedGameButton()
    {

    }

    // Opens the options menu
    public void OptionsButton()
    {

    }

    // Quits the game, for both editor and build
    public void QuitButton()
    {
        // Handle closing the game in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
