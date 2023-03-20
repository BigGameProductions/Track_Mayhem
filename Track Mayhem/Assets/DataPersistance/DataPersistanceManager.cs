using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DataPersistanceManager : MonoBehaviour
{

    [Header("File Storage Config")]

    [SerializeField] private String fileName;

    [SerializeField] private bool useEncryption;

    private FileDataHandler dataHandler;

    private GameData gameData;

    public static DataPersistanceManager instance { get; private set; }

    private List<IDataPersistance> dataPersistanceObjects;

    private void Awake()
    {
       if (instance != null)
       {
            Debug.Log("There is more than one manager for data in the scene");
       }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistanceObjects = findAllDataPersistanceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.LoadData();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Making new data with defaults");
            NewGame();
        }

        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gameData);
        }
        dataHandler.SaveData(gameData);
        Debug.Log("Saving Data");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> findAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
