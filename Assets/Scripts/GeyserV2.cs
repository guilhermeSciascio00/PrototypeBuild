using UnityEngine;
using System;

public class GeyserV2 : MonoBehaviour
{
    //We use these events to prevent the player to jump when it is inside the geyser
    public static event Action OnGeyserEnter;
    public static event Action OnGeyserExit;

    //The starting and ending poisiton, is defined by two gameObjects
    //Starting position of the geyser
    [SerializeField] Transform startingPos;

    //Ending position of the geyser
    [SerializeField] Transform endingPos;

    //Getting the player Rigidbody;
    [SerializeField] Rigidbody2D rb2d;

    [SerializeField] LayerMask targetLayer;

    //GeyserBoxCastSize
    [SerializeField] float boxHeight = 4f;
    [SerializeField] float boxLenght = 1f;
    
    //LineRenderer Properties
    LineRenderer lineRenderer;
    Vector2 traceEndPos = Vector2.zero;

    //GeyserTimers
    [SerializeField] float delayTime = 2f;
    float lerpTime = 0f;
    float lerpSpeed = 2f;
    float tempDelayTime = 0f;

    //Geyser Checks
    bool isAtDesiredHeight = false;
    bool isGeyserOn = false;
    bool isPlayerIn = false;
    bool flewAboveIt = false;
    bool wasVelocityCancelled = false;

    //Geyzer Force and Offset
    [SerializeField, Range(.5f, 5f)] float geyzerUpwardsForce;
    [SerializeField, Range(.2f, 5f)] float geyzerDownwadsForce;
    float forceMultiplier = 50f;
    float thresholdZoneOffset = 1.3f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        tempDelayTime = delayTime;
        SetGeyserPos();
    }

    private void Update()
    {
        CheckForPlayer();
        MoveGeyser();
    }

    private void FixedUpdate()
    {
        if (isPlayerIn)
        {
            ApplyGeyserForce();
        }
    }

    private void SetGeyserPos()
    {
        //Setting the geyser position, using the line rendered at position 0 and 1
        lineRenderer.SetPosition(0, startingPos.position);
        lineRenderer.SetPosition(1, startingPos.position);
    }

    private void CheckForPlayer()
    {
        //Distance between the starting point, and the line renderer detachment until it reaches its final destination
        float distanceV2 = Vector2.Distance(startingPos.position, traceEndPos);

        //BoxCast responsible for checking if the player is above it or not
        RaycastHit2D hitV2 = Physics2D.BoxCast(startingPos.position, new Vector2(boxLenght, boxHeight), 0f, Vector2.up, distanceV2, targetLayer);

        if (hitV2)
        {
            isPlayerIn = true;
            isGeyserOn = true;
            OnGeyserEnter?.Invoke();
        }
        else
        {
            isPlayerIn = false;
            wasVelocityCancelled = false;
            OnGeyserExit?.Invoke();
        }
    }

    private void MoveGeyser()
    {
        //Method responsible for Moving the geyser between point A to B
        if(!isAtDesiredHeight && isGeyserOn) 
        {
            delayTime = tempDelayTime;
            lerpTime += Time.deltaTime * lerpSpeed;
            if (lerpTime >= 1f)
            {
                lerpTime = 1f;
                isAtDesiredHeight = true;
            }
        }
        else if (isAtDesiredHeight && isGeyserOn)
        {
            delayTime -= Time.deltaTime;
            if(delayTime <= 0f)
            {
                delayTime = 0f;
                lerpTime -= Time.deltaTime * lerpSpeed;
                if(lerpTime <= 0f)
                {
                    lerpTime = 0f;
                    isAtDesiredHeight = false;
                    isGeyserOn = false;
                }
            }
        }

        traceEndPos = Vector2.Lerp(startingPos.position, endingPos.position, lerpTime);

        //On this line, we use traceEndPos position, because it will lerp the Line Renderer at index 1 between the starting point to the end point
        lineRenderer.SetPosition(1, traceEndPos);
    }

    private void ApplyGeyserForce()
    {
        //To apply the geyser force, we first cancel any inconming force(jump,etc) before doing so

        if(rb2d.linearVelocityY >= 0f && !wasVelocityCancelled)
        {
            wasVelocityCancelled = true;
            rb2d.linearVelocityY = geyzerUpwardsForce;
        }

        //The distance between the top and the threshold
        float thresholdZone = endingPos.position.y - thresholdZoneOffset;

        if(rb2d != null)
        {
            //If the player isn't at the top, we apply positive force, if he flies above the geyser, we apply negative force. However, if he is between the thresholdzone and the top, we apply a positive force + a multiplier to "fight" with the down force
            if(rb2d.position.y <= endingPos.position.y)
            {
                rb2d.linearVelocityY += geyzerUpwardsForce;
                flewAboveIt = false;
            }
            else
            {
                flewAboveIt = true;
                rb2d.linearVelocityY -= geyzerDownwadsForce;
            }
            if (rb2d.position.y <= endingPos.position.y && rb2d.position.y >= endingPos.position.y - thresholdZone)
            {
                if (flewAboveIt)
                {
                    rb2d.linearVelocityY += geyzerUpwardsForce * forceMultiplier;
                }
                
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        //Visual Debugger Helpers

        Gizmos.color = isPlayerIn ? Color.green : Color.red;
        lineRenderer = GetComponent<LineRenderer>();

        Vector2 boxTotalSize = new Vector2(boxLenght, boxHeight);
        float totalDistance = Vector2.Distance(startingPos.position, endingPos.position);

        Gizmos.DrawWireCube(endingPos.position, boxTotalSize);
        Gizmos.DrawWireCube(startingPos.position, boxTotalSize);
        Gizmos.DrawRay(startingPos.position, Vector2.up + new Vector2(0f, totalDistance));

    }
}
