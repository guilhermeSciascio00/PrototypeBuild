using UnityEngine;
using System.IO;
using System;

public class FileManager
{
    private string _fileName = "";
    private string _filePath = "";

    public FileManager(string filePath, string fileName)
    {
        //esse vem primeiro óh
        _filePath = filePath;

        //esse depois
        _fileName = fileName;
    }

    public void SaveFile(GameData data)
    {
        string fullPath = Path.Combine(_filePath, _fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Erro ao salvar: {e}");
        }
    }

    public GameData LoadFile()
    {
        string _fullFilePath = Path.Combine(_filePath, _fileName);
        GameData newGameData = null;

        if (File.Exists(_fullFilePath))
        {
            try
            {
                string dataToLoad = " ";
                using (FileStream stream = new FileStream(_fullFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();//Lê o arquivo até o final
                    }
                }

                newGameData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch(Exception e)
            {
                Debug.LogError($"Erro ao ler o arquivo! {e}");
            }

        }

        return newGameData;
    }
}
