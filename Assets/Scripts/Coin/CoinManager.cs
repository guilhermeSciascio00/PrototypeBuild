using UnityEngine;

public class CoinManager : MonoBehaviour, ISavable
{
    [Header("Coin References")]
    [SerializeField] CoinAnimation _coinAnimationRef;

    private static int _sceneCoinsAmount = 0;

    public static void AddCoin()
    {
        _sceneCoinsAmount++;
    }

    public void OnLoad(GameData gameData)
    {
        _sceneCoinsAmount = gameData.sceneTotalCoinsAmount;
    }
    public void OnSave(GameData gameData)
    {
        gameData.sceneTotalCoinsAmount = _sceneCoinsAmount;
    }

    public static int GetCoinAmount() => _sceneCoinsAmount;
    public CoinAnimation GetCoinAnimRef() => _coinAnimationRef;
}
