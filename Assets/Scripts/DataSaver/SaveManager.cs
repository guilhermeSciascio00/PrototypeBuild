using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{

    public bool save = false;
    public bool load = false;

    private string fileName = "dataSave.json";

    [SerializeField] private List<ISavable> savables = new List<ISavable>();
    FileManager fileManager;
    GameData _gameDataRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        fileManager = new FileManager(Application.persistentDataPath, fileName);

        savables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>().ToList();

        Load();
    }

    // Update is called once per frame
    void Update()
    {
        SaveAndLoadSyS();
    }

    private void Save()
    {
        Debug.Log("Save");
        foreach(ISavable savable in savables)
        {
            savable.OnSave(_gameDataRef);
        }
        fileManager.SaveFile(_gameDataRef);
    }

    private void Load()
    {
        Debug.Log("Load");

        _gameDataRef = fileManager.LoadFile();

        if (_gameDataRef == null) _gameDataRef = new GameData();

        foreach (ISavable savable in savables)
        {
            savable.OnLoad(_gameDataRef);
        }
    }

    private void SaveAndLoadSyS()
    {
        if (save)
        {
            save = false;
            Save();
        }

        if (load)
        {
            load = false;
            Load();
        }
    }
}
