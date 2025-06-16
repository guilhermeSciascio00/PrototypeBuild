using UnityEngine;

public interface ISavable
{
    public void OnLoad(GameData gameData);
    public void OnSave(GameData gameData);
}
