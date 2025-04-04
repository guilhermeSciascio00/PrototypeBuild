using UnityEngine;

public class Geyser : MonoBehaviour
{

    const float GEYSER_FORCE = .8f;

    bool isPlayerIn = false;
    Rigidbody2D rb2D = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<CharacterMovement>() != null)
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
