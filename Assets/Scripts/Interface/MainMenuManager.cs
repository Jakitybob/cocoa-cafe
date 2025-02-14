using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Disable the load game button if there is no save data
        if (!GameDataManager.instance.HasGameData())
            loadGameButton.interactable = false;
    }

    // Starts a new game with fresh save data
    public void OnStartNewGameButtonPressed()
    {
        DisableAllButtons();

        // Create a new game and initialize new game data
        GameDataManager.instance.NewGame();

        // Load the gameplay scene, which will write our new file
        // because of OnSceneUnloaded method
        // TODO: Add loading screen logic
        SceneManager.LoadSceneAsync("SampleScene");
    }

    // Loads the game from the found save data
    public void OnLoadSavedGameButtonPressed()
    {
        DisableAllButtons();

        // Load the gameplay scene and the data manager will load in the rest
        SceneManager.LoadSceneAsync("SampleScene");
    }

    // Opens the options menu
    public void OnOptionsButtonPressed()
    {

    }

    // Quits the game, for both editor and build
    public void OnQuitButtonPressed()
    {
        // Handle closing the game in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Disables all buttons, to be used when one has been pressed so no double-clicks can happen
    public void DisableAllButtons()
    {
        newGameButton.interactable = false;
        loadGameButton.interactable = false;
        optionsButton.interactable = false;
        quitButton.interactable = false;
    }
}
