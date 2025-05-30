using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<IRespawnable>() is IRespawnable respawnAbleObject)
        {
            respawnAbleObject.Respawn();
        }
    }
}
