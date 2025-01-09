using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }

    [SerializeField]
    private string fileName;

    private GameData data;
    private List<IGameData> gameDataObjects;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("There is more than one game data manager detected which shouldn't happen.");

        instance = this;
    }

    //
    private void Start()
    {
        this.gameDataObjects = FindAllGameDataObjects();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
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

        // Load a new game if no data was found
        if (this.data == null)
            NewGame();

        // Send the loaded data out to all the IGameData classes
        foreach (IGameData gameDataObject in gameDataObjects)
            gameDataObject.LoadData(data);
    }   
    
    // Writes out data from all IGameData objects
    public void SaveGame()
    {
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
}
