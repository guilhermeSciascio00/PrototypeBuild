using UnityEngine;
using System;

public class GeyserV2 : MonoBehaviour
{
    //We use these events to prevent the player to jump when it is inside the geyser
    public static event Action OnGeyserEnter;
    public static event Action OnGeyserExit;

    //Getting the player Rigidbody;
    Rigidbody2D rb2d;
    bool isRb2DNull = false;

    [SerializeField] LayerMask targetLayer;

    //GeyserBoxCastSize
    [SerializeField] float boxHeight = 4f;
    [SerializeField] float boxLenght = 1f;
    
    //LineRenderer Properties
    LineRenderer lineRenderer;
    Vector2 traceEndPos = Vector2.zero;

    //GeyserTimers
    float delayTime = 2f; //Default value if you don't use SO
    float lerpTime = 0f;
    float tempDelayTime = 0f;

    //Geyser Checks
    bool isAtDesiredHeight = false;
    bool isGeyserOn = false;
    bool isAutoGeyserOn = false;
    bool isPlayerIn = false;
    bool flewAboveIt = false;
    bool wasVelocityCancelled = false;

    //Geyzer Force Mult and Offset
    float forceMultiplier = 50f;
    float thresholdZoneOffset = 1.3f;

    //ScriptableObject in action!!
    [SerializeField] GeyserStatsSO geyserSO;

    Vector2 startingPosV2;

    void Start()
    {
        delayTime = geyserSO.geyserDelay;
        lineRenderer = GetComponent<LineRenderer>();
        tempDelayTime = delayTime;
        startingPosV2 = lineRenderer.GetPosition(0);
        SetGeyserPos();
    }

    private void Update()
    {
        CheckForPlayer();
        if (geyserSO.needsThePlayer)
        {
            GeyserMoverWPlayer();
        }
        else
        {
            GeyserMoverAuto();
        }
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
        lineRenderer.SetPosition(0, startingPosV2);
        lineRenderer.SetPosition(1, startingPosV2);
    }

    private void CheckForPlayer()
    {
        //Distance between the starting point, and the line renderer detachment until it reaches its final destination
        float distanceV2 = Vector2.Distance(startingPosV2, traceEndPos);

        //BoxCast responsible for checking if the player is above it or not
        RaycastHit2D hitV2 = Physics2D.BoxCast(startingPosV2, new Vector2(boxLenght, boxHeight), 0f, Vector2.up, distanceV2, targetLayer);

        if (geyserSO.needsThePlayer)
        {
            if (hitV2)
            {
                PlayerCollisionSequence(hitV2);
            }
            else
            {
                PlayerOffCollisionSequence();
            }
        }
        else if (!geyserSO.needsThePlayer || geyserSO.isItPermanent) 
        {
            if(isAutoGeyserOn && hitV2) 
            {
                PlayerCollisionSequence(hitV2);
            }
            else
            {
                PlayerOffCollisionSequence();
            }
        }
    }

    private void GeyserMoverWPlayer()
    {
        //Method responsible for Moving the geyser between point A to B
        if(!isAtDesiredHeight && isGeyserOn) 
        {
            delayTime = tempDelayTime;
            lerpTime += Time.deltaTime * geyserSO.geyserSpeedUP;
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
                lerpTime -= Time.deltaTime * geyserSO.geyserSpeedDown;
                if(lerpTime <= 0f)
                {
                    lerpTime = 0f;
                    isAtDesiredHeight = false;
                    isGeyserOn = false;
                }
            }
        }

        traceEndPos = Vector2.Lerp(startingPosV2, GetGeyserFinalHeight(), lerpTime);

        //On this line, we use traceEndPos position, because it will lerp the Line Renderer at index 1 between the starting point to the end point
        lineRenderer.SetPosition(1, traceEndPos);
    }

    private void GeyserMoverAuto()
    {
        if (!geyserSO.isItPermanent)
        {
            if (delayTime >= 0f)
            {
                delayTime -= Time.deltaTime;
                if (delayTime <= 0)
                {
                    delayTime = 0f;
                    if (!isAtDesiredHeight)
                    {
                        isAutoGeyserOn = true;
                        lerpTime += Time.deltaTime * geyserSO.geyserSpeedUP;
                        if (lerpTime >= 1f)
                        {
                            lerpTime = 1f;
                            isAtDesiredHeight = true;
                            delayTime = tempDelayTime;
                        }
                    }
                    else
                    {
                        lerpTime -= Time.deltaTime *            geyserSO.geyserSpeedDown;
                        if (lerpTime <= 0f)
                        {
                            lerpTime = 0f;
                            isAtDesiredHeight = false;
                            isAutoGeyserOn = false;
                            delayTime = tempDelayTime;

                        }
                    }
                }
            }
        }
        else
        {
            lerpTime = 1f;
            isAutoGeyserOn = true;
            delayTime = 0f;
        }

        traceEndPos = Vector2.Lerp(startingPosV2, GetGeyserFinalHeight(), lerpTime);

        lineRenderer.SetPosition(1, traceEndPos);
    }

    private void ApplyGeyserForce()
    {
        //To apply the geyser force, we first cancel any inconming force(jump,etc) before doing so

        if(rb2d.linearVelocityY >= 0f && !wasVelocityCancelled)
        {
            wasVelocityCancelled = true;
            rb2d.linearVelocityY = geyserSO.geyserUPForce;
        }

        //The distance between the top and the threshold
        float thresholdZone = GetGeyserFinalHeight().y - thresholdZoneOffset;

        if(rb2d != null)
        {
            //If the player isn't at the top, we apply positive force, if he flies above the geyser, we apply negative force. However, if he is between the thresholdzone and the top, we apply a positive force + a multiplier to "fight" with the down force
            if(rb2d.position.y <= GetGeyserFinalHeight().y)
            {
                rb2d.linearVelocityY += geyserSO.geyserUPForce;
                flewAboveIt = false;
            }
            else
            {
                flewAboveIt = true;
                rb2d.linearVelocityY -= geyserSO.geyserDownForce;
            }
            if (rb2d.position.y <= GetGeyserFinalHeight().y && rb2d.position.y >= GetGeyserFinalHeight().y - thresholdZone)
            {
                if (flewAboveIt)
                {
                    rb2d.linearVelocityY += geyserSO.geyserUPForce * forceMultiplier;
                }
                
            }
        }
        
    }

    private void PlayerCollisionSequence(RaycastHit2D hitV2)
    {
        rb2d = hitV2.rigidbody;
        isRb2DNull = false;
        isPlayerIn = true;
        isGeyserOn = true;
        OnGeyserEnter?.Invoke();
    }

    private void PlayerOffCollisionSequence()
    {
        if (!isRb2DNull)
        {
            rb2d = null;
            isRb2DNull = true;
        }
        isPlayerIn = false;
        wasVelocityCancelled = false;
        OnGeyserExit?.Invoke();
    }

    private Vector2 GetGeyserFinalHeight()
    {
        return new Vector2(startingPosV2.x, startingPosV2.y + geyserSO.geyserHeight);
    }

    private void OnDrawGizmos()
    {
        //Visual Debugger Helpers

        Gizmos.color = isPlayerIn ? Color.green : Color.red;
        lineRenderer = GetComponent<LineRenderer>();
        startingPosV2 = lineRenderer.GetPosition(0);

        Vector2 boxTotalSize = new Vector2(boxLenght, boxHeight);
        float totalDistance = Vector2.Distance(startingPosV2, GetGeyserFinalHeight());

        Gizmos.DrawWireCube(GetGeyserFinalHeight(), boxTotalSize);
        Gizmos.DrawWireCube(startingPosV2, boxTotalSize);
        Gizmos.DrawRay(startingPosV2, Vector2.up + new Vector2(0f, totalDistance));

    }
}
