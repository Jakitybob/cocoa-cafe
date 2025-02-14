using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }

    [Header("Debugging")]
    [SerializeField] private bool initDataIfFalse = false;

    [Header("Settings")]
    [SerializeField] private string fileName;

    private GameData data;
    private List<IGameData> gameDataObjects;
    private FileDataHandler dataHandler;

    // Called before anything else
    private void Awake()
    {
        // Destroy any extra data persistence managers if they are found
        if (instance != null)
        {
            Debug.Log("There is more than one game data manager detected. The newest one has been destroyed.");
            Destroy(this.gameObject);
        }

        // Update the instance
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Initialize our data handler
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    // Called after awake but before start
    private void OnEnable()
    {
        // Subscribe to scene-related events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene-related events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.gameDataObjects = FindAllGameDataObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame(); // Autosave when a scene unloads
    }

    // Make sure to save the game when someone quits, autosaving :)
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // Initializes a new game by creating a new default GameData object
    public void NewGame()
    {
        this.data = new GameData();
    }

    // Loads in data from a file and sends it out to all IGameData objects to load
    public void LoadGame()
    {
        // Load in our data from the file handler
        this.data = dataHandler.Load();

        // Load a new game if no data is found but debug loading is enabled
        if (this.data == null && initDataIfFalse)
            NewGame();

        // Load a new game if no data was found
        if (this.data == null)
        {
            Debug.Log("No data was found. A new game needs to be started.");
            return;
        }

        // Send the loaded data out to all the IGameData classes
        foreach (IGameData gameDataObject in gameDataObjects)
            gameDataObject.LoadData(data);
    }   
    
    // Writes out data from all IGameData objects
    public void SaveGame()
    {
        // Make sure there is data to save
        if (this.data == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }

        // Send the loaded data out to all the IGameData classes to update
        foreach (IGameData gameDataObject in gameDataObjects)
            gameDataObject.SaveData(ref data);

        // Save out the data to the file
        dataHandler.Save(data);
    }

    // Find all game objects that inherit from both MonoBehavior and IGameData and return the list of them
    private List<IGameData>FindAllGameDataObjects()
    {
        // Return a list of found MonoBehavior, IGameData objects
        IEnumerable<IGameData> gameDataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IGameData>();
        return new List<IGameData>(gameDataObjects);
    }

    // Checks whether or not there is valid game data
    public bool HasGameData()
    {
        return this.data != null;
    }
}
