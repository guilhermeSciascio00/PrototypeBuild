using UnityEngine;
using System;

public class GeyserV2 : MonoBehaviour
{
    //We use these events to prevent the player to jump when it is inside the geyser
    public static event Action OnGeyserEnter;
    public static event Action OnGeyserExit;

    //Getting the player Rigidbody;
    private Rigidbody2D _rb2d;
    private bool _isRb2DNull = false;

    [SerializeField] LayerMask targetLayer;

    //GeyserBoxCastSize
    [SerializeField] float boxHeight = 4f;
    [SerializeField] float boxLenght = 1f;
    
    //LineRenderer Properties
    private LineRenderer _lineRenderer;
    private Vector2 _traceEndPos = Vector2.zero;

    //GeyserTimers
    const float DEFAULT_TIMER_V = 0f;
    const float MAX_TIMER_V = 1f;

    private float _delayTime = 2f; //Default value if you don't use SO
    private float _lerpTime = 0f;
    private float _tempDelayTime = 0f;

    //Geyser Checks
    private bool _isAtDesiredHeight = false;
    private bool _isGeyserOn = false;
    private bool _isAutoGeyserOn = false;
    private bool _isPlayerIn = false;
    private bool _flewAboveIt = false;
    private bool _wasVelocityCancelled = false;

    //Geyzer Force Mult and Offset
    private float _forceMultiplier = 50f;
    private float _thresholdZoneOffset = 1.3f;

    //ScriptableObject in action!!
    [SerializeField] GeyserStatsSO geyserSO;

    private Vector2 _startingPos;

    void Start()
    {
        SetGeyserPosAndData();
    }

    private void Update()
    {
        CheckForPlayer();
        GeyserStartV2(geyserSO.isItPermanent);
    }

    private void FixedUpdate()
    {
        ApplyGeyserForce();
    }

    private void SetGeyserPosAndData()
    {
        _delayTime = geyserSO.geyserDelay;

        _tempDelayTime = _delayTime;

        if(!geyserSO.needsThePlayer)
        {
            _isGeyserOn = true;
        }

        _lineRenderer = GetComponent<LineRenderer>();
        _startingPos = transform.position;

        //Setting the geyser position, using the line rendered at position 0 and 1

        _lineRenderer.SetPosition(0, _startingPos);
        _lineRenderer.SetPosition(1, _startingPos);
    }

    private void CheckForPlayer()
    {
        //Distance between the starting point, and the line renderer detachment until it reaches its final destination
        float distanceV2 = Vector2.Distance(_startingPos, _traceEndPos);

        //BoxCast responsible for checking if the player is above it or not
        RaycastHit2D hitV2 = Physics2D.BoxCast(_startingPos, new Vector2(boxLenght, boxHeight), 0f, Vector2.up, distanceV2, targetLayer);

        if (geyserSO.needsThePlayer)
        {
            if (hitV2)
            {
                PlayerCollisionSequence(hitV2, true);
            }
            else
            {
                PlayerCollisionSequence(hitV2, false);
            }
        }
        else if (!geyserSO.needsThePlayer || geyserSO.isItPermanent) 
        {
            if(_isAutoGeyserOn && hitV2) 
            {
                PlayerCollisionSequence(hitV2, true);
            }
            else
            {
                PlayerCollisionSequence(hitV2, false);
            }
        }
    }

    private void GeyserStartV2(bool isItPermanent)
    {
        if(!isItPermanent)
        {
            if (_isGeyserOn)
            {
                GeyserMover();
            }
        }
        else
        {
            _lerpTime = MAX_TIMER_V;
            _isAutoGeyserOn = true;
            _delayTime = DEFAULT_TIMER_V;
        }

        _traceEndPos = Vector2.Lerp(_startingPos, GetGeyserFinalHeight(), _lerpTime);

        _lineRenderer.SetPosition(1, _traceEndPos);
    }

    private void ApplyGeyserForce()
    {
        if (_isPlayerIn)
        {
            //To apply the geyser force, we first cancel any incoming force(jump,etc) before doing so

            if (_rb2d.linearVelocityY >= 0f && !_wasVelocityCancelled)
            {
                _wasVelocityCancelled = true;
                _rb2d.linearVelocityY = geyserSO.geyserUPForce;
            }

            //The distance between the top and the threshold
            float thresholdZone = GetGeyserFinalHeight().y - _thresholdZoneOffset;

            if (_rb2d != null)
            {
                //If the player isn't at the top, we apply positive force, if he flies above the geyser, we apply negative force. However, if he is between the thresholdzone and the top, we apply a positive force + a multiplier to "fight" with the down force
                if (_rb2d.position.y <= GetGeyserFinalHeight().y)
                {
                    _rb2d.linearVelocityY += geyserSO.geyserUPForce;
                    _flewAboveIt = false;
                }
                else
                {
                    _flewAboveIt = true;
                    _rb2d.linearVelocityY -= geyserSO.geyserDownForce;
                }
                if (_rb2d.position.y <= GetGeyserFinalHeight().y && _rb2d.position.y >= GetGeyserFinalHeight().y - thresholdZone)
                {
                    if (_flewAboveIt)
                    {
                        _rb2d.linearVelocityY += geyserSO.geyserUPForce * _forceMultiplier;
                    }

                }
            }

        }

    }

    /// <summary>
    /// This method helps the computer to execute the sequence of events that happens when the player is being hit, use the isOnCollision to set when the player is interacting with the GeyserOrNot 
    /// </summary>
    /// <param name="hitV2"></param>
    /// <param name="isOnCollision"></param>
    private void PlayerCollisionSequence(RaycastHit2D hitV2, bool isOnCollision)
    {
        switch (isOnCollision)
        {
            case true:
                _rb2d = hitV2.rigidbody;
                _isRb2DNull = false;
                _isPlayerIn = true;
                if (!_isGeyserOn) { _isGeyserOn = true; }
                OnGeyserEnter?.Invoke();
                break;

            case false:
                if (!_isRb2DNull)
                {
                    _rb2d = null;
                    _isRb2DNull = true;
                }
                _isPlayerIn = false;
                _wasVelocityCancelled = false;
                OnGeyserExit?.Invoke();
                break;
        }
        
    }

    private Vector2 GetGeyserFinalHeight()
    {
        return new Vector2(_startingPos.x, _startingPos.y + geyserSO.geyserHeight);
    }

    /// <summary>
    /// Method responsible for moving the geyser up and down as needed;
    /// </summary>
    private void GeyserMover()
    {
        if (!_isAtDesiredHeight)
        {
            if (geyserSO.needsThePlayer)
            {
                _delayTime = DEFAULT_TIMER_V;
            }

            if (_delayTime >= DEFAULT_TIMER_V)
            {
                _delayTime -= Time.deltaTime;
                if (_delayTime <= DEFAULT_TIMER_V)
                {
                    _delayTime = DEFAULT_TIMER_V;
                    if (!geyserSO.needsThePlayer) { _isAutoGeyserOn = true; }

                    _lerpTime += Time.deltaTime * geyserSO.geyserSpeedUP;
                    if(_lerpTime >= MAX_TIMER_V)
                    {
                        _lerpTime = MAX_TIMER_V;
                        _isAtDesiredHeight = true;
                        _delayTime = _tempDelayTime;
                    }
                }
            }
        }
        else if(_isAtDesiredHeight)
        {
            _delayTime -= Time.deltaTime;
            if (_delayTime <= DEFAULT_TIMER_V)
            {
                _delayTime = DEFAULT_TIMER_V;
                _lerpTime -= Time.deltaTime * geyserSO.geyserSpeedDown;
                if (_lerpTime <= DEFAULT_TIMER_V)
                {
                    _lerpTime = DEFAULT_TIMER_V;
                    _isAtDesiredHeight = false;
                    if (geyserSO.needsThePlayer) { _isGeyserOn = false; }
                    _isAutoGeyserOn = false;
                    _delayTime = _tempDelayTime;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Visual Debugger Helpers

        Gizmos.color = _isPlayerIn ? Color.green : Color.red;
        _lineRenderer = GetComponent<LineRenderer>();
        _startingPos = _lineRenderer.GetPosition(0);

        Vector2 boxTotalSize = new Vector2(boxLenght, boxHeight);
        float totalDistance = Vector2.Distance(_startingPos, GetGeyserFinalHeight());

        Gizmos.DrawWireCube(GetGeyserFinalHeight(), boxTotalSize);
        Gizmos.DrawWireCube(_startingPos, boxTotalSize);
        Gizmos.DrawRay(_startingPos, Vector2.up + new Vector2(0f, totalDistance));

    }
}
