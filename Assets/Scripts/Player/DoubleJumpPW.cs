using System;
using UnityEngine;

public class DoubleJumpPW : MonoBehaviour
{
    public static event Action OnDoubleJumpCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //The only object who has a Rigidbody2D is the player
        if(collision.GetComponent<Rigidbody2D>() != null)
        {
            OnDoubleJumpCollected.Invoke();
        }
    }
}
