using UnityEngine;

public class Geyser : MonoBehaviour
{

    const float GEYSER_FORCE = .8f;

    bool isPlayerIn = false;

    Rigidbody2D rb2D = null;
    BoxCollider2D boxCollider2D = null;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        Debug.Log(boxCollider2D.bounds);
        Debug.Log(boxCollider2D.bounds.max.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rb2D = collision.GetComponent<Rigidbody2D>();
            isPlayerIn = true;
        }
    }

    private void FixedUpdate()
    {
        if(isPlayerIn && rb2D != null)
        {
            rb2D.linearVelocityY += GEYSER_FORCE;
            Debug.Log($"R2LinearYVelocity {rb2D.linearVelocityY}");
            Debug.Log($"R2LinearPositionY {rb2D.position.y}");
            if(rb2D.position.y >= boxCollider2D.bounds.max.y)
            {
                Debug.Log("Player is in the top of the geyser");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() != null)
        {
            isPlayerIn = false;
        }
    }
}
