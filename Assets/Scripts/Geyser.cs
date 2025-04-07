using UnityEngine;

public class Geyser : MonoBehaviour
{

    [Header("Limit and Starting Points")]
    [Tooltip("Maximum Geyzer height")]
    [SerializeField] Transform desiredHeight;

    [Tooltip("GeyzerStartingPosition")]
    [SerializeField] Transform geyserStartingPos;

    [Header("Geyzer Up/Down Speed")]
    [Tooltip("Speed to reach out the maximum height point")]
    [SerializeField] float speedUP;

    [Tooltip("Speed to go back down")]
    [SerializeField] float speedDOWN;

    [Tooltip("Delay to the geyzer start going down")]
    [SerializeField] float delayTime;

    //Geyzer Force Variables
    const float GEYSER_FORCE = 1.4f;
    const float FORCE_MULTIPLIER = 8f;

    //Geyzer delay time and time to go up
    float lerpTime = 0f;
    float tempDelayTime = 0f;

    //Geyzer top offeset and trigger zone
    float y_OffSet = .5f;
    float thresholdZone;

    bool isPlayerIn = false;
    bool isGeyserON = false;
    bool isAtDesiredHeight = false;

    Rigidbody2D rb2D = null;
    BoxCollider2D boxCollider2D = null;

    Vector2 startingPos = Vector2.zero;

    private void Start()
    {
        GetGeyzerBoundsAndZone();
        transform.position = geyserStartingPos.position;
        startingPos = geyserStartingPos.position;
        tempDelayTime = delayTime;
    }

    private void Update()
    {
        GetGeyzerBoundsAndZone();

        if (isGeyserON && !isAtDesiredHeight)
        {
            delayTime = tempDelayTime;
            lerpTime += Time.deltaTime * speedUP;
            if(lerpTime >= 1f) 
            {
                lerpTime = 1f;
                isAtDesiredHeight = true;
            }
        }
        else if (isGeyserON && isAtDesiredHeight)
        {
            if (delayTime >= 0f)
            {
                delayTime -= Time.deltaTime;
                if (delayTime <= 0f)
                {
                    lerpTime -= Time.deltaTime * speedDOWN;
                    delayTime = 0f;
                    if(lerpTime <= 0f)
                    {
                        lerpTime = 0f;
                        isAtDesiredHeight = false;
                        isGeyserON = false;
                    }
                }
            }
        }

        transform.position = Vector2.Lerp(startingPos, desiredHeight.position, lerpTime);
    }

    private void FixedUpdate()
    {
        if(isPlayerIn && rb2D != null)
        {
            //Happens when the player reaches the top of the geyzer
            if(rb2D.position.y >= boxCollider2D.bounds.max.y)
            {
                rb2D.linearVelocityY -= GEYSER_FORCE;
            }
            //Happens if the player didn't go up yet
            else if(rb2D.position.y <= boxCollider2D.bounds.max.y && rb2D.linearVelocityY >= 0)
            {
                rb2D.linearVelocityY += GEYSER_FORCE;
            }
            //Happens when the player is between the geyser top and the threshold zone
            if (rb2D.linearVelocityY <= 0 && rb2D.position.y <= thresholdZone)
            {
                rb2D.linearVelocityY += GEYSER_FORCE * FORCE_MULTIPLIER;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rb2D = collision.GetComponent<Rigidbody2D>();
            isPlayerIn = true;
            isGeyserON = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() != null)
        {
            isPlayerIn = false;
        }
    }

    private void GetGeyzerBoundsAndZone()
    {
        if(this.gameObject.GetComponent<BoxCollider2D>() == null)
        {
            boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        }
        else
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
        //Zone to re-apply the geyser force, when the player is falling down
        thresholdZone = boxCollider2D.bounds.max.y - y_OffSet;
    }

   
}
