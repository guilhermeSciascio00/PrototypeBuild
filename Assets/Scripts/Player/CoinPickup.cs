using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour, ISavable
{
    [SerializeField] private int _coinAmount = 0;
    [SerializeField] private List<GameObject> _collectedCoins;

    private void Update()
    {
        CheckForCollision();
    }

    private void CheckForCollision()
    {
        Collider2D collision = Physics2D.OverlapBox(transform.position, transform.lossyScale , 0f);

        if (collision.GetComponent<CoinManager>() != null)
        {
            CoinManager coinManager = collision.GetComponentInParent<CoinManager>();
            coinManager.GetCoinAnimRef().ActivateCollision();

            _collectedCoins.Add(coinManager.GetCoinAnimRef().GetCoinRef());
            _coinAmount++;
            
        }
    }

    public void OnLoad(GameData gameData)
    {
        _coinAmount = gameData.collectedCoinsAmount;
        if(_collectedCoins != null)
        {
            _collectedCoins = gameData.collectedCoins;
        }
    }

    public void OnSave(GameData gameData)
    {
        gameData.collectedCoinsAmount = _coinAmount;
        gameData.collectedCoins = _collectedCoins;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
