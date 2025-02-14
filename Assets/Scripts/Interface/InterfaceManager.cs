using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text moneyText;

    [SerializeField]
    private GameObject pauseMenuParent;

    // Keep track of which menu is currently active so none may overlap
    public GameObject activeMenu { get; private set; } = null;

    // Sets up default variables
    private void Start()
    {
        // Disable the pause menu so it's not showing
        pauseMenuParent.SetActive(false);
    }

    // Updates the balance text located on the user's interface
    public void UpdateBalanceText(float money)
    {
        moneyText.text = "$" + money.ToString();
    }

    // Opens the pause menu and sets it to the active menu
    public void OpenPauseMenu()
    {
        // Set the pause menu as active
        activeMenu = pauseMenuParent;
        activeMenu.SetActive(true);

        // Toggle the pause status of the game
        GameManager.instance.TogglePauseGame();
    }

    // Closes the current menu and re-nulls the active menu
    public void CloseCurrentMenu()
    {
        // Deactive and null the current menu
        activeMenu.SetActive(false);
        activeMenu = null;

        // Unpause the game if it is paused
        if (GameManager.instance.isPaused)
            GameManager.instance.TogglePauseGame();
    }

    // Resumes play by unpausing the game and closing the pause menu
    public void ResumeButton()
    {
        // Hide the current menu
        CloseCurrentMenu();

        // Toggle the pause status of the game
        GameManager.instance.TogglePauseGame();
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

    // Instructs the data handler to save data
    public void SaveButton()
    {
        // Save the game
        GameDataManager.instance.SaveGame();
        // TODO: confirm the data was saved to the user
    }

    // Instructs the data handler to load data
    public void LoadButton()
    {
        // Load the game
        GameDataManager.instance.LoadGame();
        UpdateBalanceText(GameManager.instance.moneyManager.GetMoney());
        // TOOD: loading screen here
    }
}
