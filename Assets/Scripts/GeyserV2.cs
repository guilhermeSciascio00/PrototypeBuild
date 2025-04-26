using UnityEngine;
using System;

public class GeyserV2 : MonoBehaviour
{
    //TODO: Arrange the variables by functionality


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
    private bool _isObjectIn = false;

    private bool _flewAboveIt = false;
    private bool _wasVelocityCancelled = false;

    //Geyzer Force Mult and Offset
    private float _forceMultiplier = 50f;
    private float _thresholdZoneOffset = 1.3f;

    //ScriptableObject in action!!
    [SerializeField] GeyserStatsSO geyserSO;

    private Vector2 _startingPos;
    [SerializeField] Vector3 offset;

    Rigidbody2D playerReference;

    void Start()
    {
        SetGeyserStartValues();
    }

    private void Update()
    {
        CheckForOverLapsV2();
        GeyserStartV2(geyserSO.isItPermanent);

        InsertMethodNameHere();
        SetGeyserPosition();
    }

    private void FixedUpdate()
    {
        ApplyGeyserForce();
    }

    //This method set the default geyser values
    private void SetGeyserStartValues()
    {
        SetGeyserDLTimer();

        offset = new Vector2(0f, geyserSO.geyserHeight * .5f);
        _lineRenderer = GetComponent<LineRenderer>();

        if (!_lineRenderer.enabled)
        {
            _lineRenderer.enabled = true;
        }

        _startingPos = transform.position;
        _lineRenderer.SetPosition(0, _startingPos);
        _lineRenderer.SetPosition(1, _startingPos);
    }

    private void SetGeyserPosition()
    {
        _startingPos = transform.position;
        _lineRenderer.SetPosition(0, _startingPos);
    }

    //This method sets the delay timer to the default
    private void SetGeyserDLTimer()
    {
        _delayTime = geyserSO.geyserDelay;
        _tempDelayTime = _delayTime;
    }


    //This method checks who is overlaping the geyser hitbox
    private void CheckForOverLapsV2()
    {

        Vector3 checkPosition = transform.position + offset;

        Collider2D playerOverlap = Physics2D.OverlapBox(checkPosition, new Vector2(boxLenght, geyserSO.geyserHeight), 0f, LayerMask.GetMask("Player"));

        Collider2D boxOverlap = Physics2D.OverlapBox(checkPosition, new Vector2(boxLenght, geyserSO.geyserHeight), 0f, LayerMask.GetMask("Objects"));

        _isPlayerIn = playerOverlap;

        if(_isPlayerIn)
        {
            playerReference = playerOverlap.GetComponent<Rigidbody2D>();
        }
        _isObjectIn = boxOverlap;

    }

    //This method affects the player when him goes in the geyser
    private void InsertMethodNameHere()
    {
        if(_isPlayerIn && playerReference != null) 
        {
            _isPlayerIn = true;
            _rb2d = playerReference;
            _isRb2DNull = false;
            OnGeyserEnter.Invoke();
        }
        else
        {
            _isPlayerIn = false;
            _rb2d = null;
            _isRb2DNull = true;
            _wasVelocityCancelled = false;
            OnGeyserExit.Invoke();
        }
    }

    //private void CheckForOverlaps()
    //{
        
    //    //BoxCast responsible for checking if the player is above it or not
    //    //RaycastHit2D hitV2 = Physics2D.BoxCast(_startingPos + Vector2.down * 0.5f, new Vector2(boxLenght, boxHeight), 0f, Vector2.up, geyserSO.geyserHeight, targetLayer);


    //    Collider2D hit = Physics2D.OverlapBox(transform.position + offset, new Vector2(boxLenght, geyserSO.geyserHeight), 0f, LayerMask.GetMask("Player", "Objects"));

    //    //Debug.Log($"Is PlayerIN {_isPlayerIn}");
    //    //Debug.Log($"Is RB2D Null {_isRb2DNull}");
    //    //Debug.Log($"Is GeyserON {_isGeyserOn}");

    //    if (hit)
    //    {
    //        //Maybe I need to do it here, I will see it it tomorrow!
    //        switch(hit.tag)
    //        {
    //            case "Player":
    //                //Debug only
    //                if (_isObjectIn)
    //                {
    //                    return;
    //                }
    //                _isPlayerIn = true;
    //                _rb2d = hit.GetComponent<Rigidbody2D>();
    //                _isRb2DNull = false;
    //                OnGeyserEnter.Invoke();
    //                //Debug.Log(hit.gameObject.name);
    //                break;
    //            case "Objects":
    //                //Debug.Log(hit.gameObject.name);
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        _isPlayerIn = false;
    //        _rb2d = null;
    //        _isRb2DNull = true;
    //        _wasVelocityCancelled = false;
    //        OnGeyserExit.Invoke();  
    //    }

    //    //if (hit)
    //    //{
    //    //    if (hit.gameObject.CompareTag("Player"))
    //    //    {
    //    //        _isPlayerIn = true;
    //    //        _rb2d = hit.GetComponent<Rigidbody2D>();
    //    //        _isRb2DNull = false;
    //    //        //_isBlockedByBox = false;
    //    //        OnGeyserEnter.Invoke();
    //    //        //Debug.Log(hit.gameObject.name);
    //    //    }
    //    //    else if (hit.gameObject.CompareTag("Objects"))
    //    //    {
    //    //        //Debug.Log(hit.gameObject.name);
    //    //        _isBlockedByBox = true;
    //    //    }
    //    //    //else
    //    //    //{
    //    //    //    _isPlayerIn = false;
    //    //    //    _rb2d = null;
    //    //    //    _isRb2DNull = true;
    //    //    //    //    _wasVelocityCancelled = false;
    //    //    //    //    OnGeyserExit.Invoke();
    //    //    //}
    //    //}
    //    //else
    //    //{
    //    //    _isPlayerIn = false;
    //    //    _rb2d = null;
    //    //    _isRb2DNull = true;
    //    //    _isBlockedByBox = false;
    //    //    _wasVelocityCancelled = false;
    //    //    OnGeyserExit.Invoke();
    //    //}

    //    //if (geyserSO.needsThePlayer)
    //    //{
    //    //    if (hitV2)
    //    //    {
    //    //        PlayerCollisionSequence(hitV2, true);
    //    //    }
    //    //    else
    //    //    {
    //    //        PlayerCollisionSequence(hitV2, false);
    //    //    }
    //    //}
    //    //else if (!geyserSO.needsThePlayer || geyserSO.isItPermanent) 
    //    //{
    //    //    if(_isAutoGeyserOn && hitV2) 
    //    //    {
    //    //        PlayerCollisionSequence(hitV2, true);
    //    //    }
    //    //    else
    //    //    {
    //    //        PlayerCollisionSequence(hitV2, false);
    //    //    }
    //    //}
    //}

    private void GeyserStartV2(bool isItPermanent)
    {
        if(!isItPermanent)
        {
            if(_isPlayerIn && geyserSO.needsThePlayer)
            {
                _isGeyserOn = true;
            }
            else if(!geyserSO.needsThePlayer)
            {
                _isGeyserOn = true;
                _isAutoGeyserOn = true ;
            }
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
        //if(_isPlayerIn)
        //{
        //    _isGeyserOn = true;
        //}

        ////Only works if the geyser isn't permanent
        //if (!isItPermanent)
        //{
        //    //if (_isObjectIn)
        //    //{
        //    //    TurnOffGeyser();
        //    //}
        //    //else if(!_isObjectIn && !geyserSO.needsThePlayer)
        //    //{
        //    //    _isAutoGeyserOn = true;
        //    //    _isGeyserOn = true;
        //    //}
        //    //else
        //    //{
        //    //    GeyserMover();
        //    //}
        //}
        ////IF it's permanent
        //else
        //{
        //    if (_isObjectIn)
        //    {
        //        TurnOffGeyser();
        //    }
        //    else
        //    {
        //        _lerpTime = MAX_TIMER_V;
        //        _isAutoGeyserOn = true;
        //        _delayTime = DEFAULT_TIMER_V;
        //    }
        //}

        //if (_isGeyserOn)
        //{
        //    GeyserMover();
        //}

        Debug.Log("Does this part work?");
        _traceEndPos = Vector2.Lerp(_startingPos, GetGeyserFinalHeight(), _lerpTime);

        _lineRenderer.SetPosition(1, _traceEndPos);
    }

    private void ApplyGeyserForce()
    {
        //Only works if the player is in the geyser, the geyser isn't blocked and the player position(rb2d) is lower than the geyserPos
        if (_isPlayerIn && !_isObjectIn && _rb2d?.position.y < _traceEndPos.y)
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
    //private void PlayerCollisionSequence(RaycastHit2D hitV2, bool isOnCollision)
    //{
    //    switch (isOnCollision)
    //    {
    //        case true:
    //            _rb2d = hitV2.rigidbody;
    //            _isRb2DNull = false;
    //            _isPlayerIn = true;
    //            if (!_isGeyserOn) { _isGeyserOn = true; }
    //            OnGeyserEnter?.Invoke();
    //            break;

    //        case false:
    //            if (!_isRb2DNull)
    //            {
    //                _rb2d = null;
    //                _isRb2DNull = true;
    //            }
    //            _isPlayerIn = false;
    //            _wasVelocityCancelled = false;
    //            OnGeyserExit?.Invoke();
    //            break;
    //    }

    //}
    private Vector2 GetGeyserFinalHeight()
    {

        return new Vector2(_startingPos.x, _startingPos.y + geyserSO.geyserHeight);
    }

    private void TurnOffGeyser()
    {
        if (_isObjectIn)
        {
            _isGeyserOn = false;
            _isAutoGeyserOn = false;
            _lerpTime = DEFAULT_TIMER_V;
            _delayTime = _tempDelayTime;
            _isAtDesiredHeight = false;
        }
    }

    /// <summary>
    /// Method responsible for moving the geyser up and down as needed;
    /// </summary>
    private void GeyserMover()
    {

        if (!_isObjectIn)
        {
            if (!_isAtDesiredHeight && _isGeyserOn)
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
                        if (_lerpTime >= MAX_TIMER_V)
                        {
                            _lerpTime = MAX_TIMER_V;
                            _isAtDesiredHeight = true;
                            _delayTime = _tempDelayTime;
                        }
                    }
                }
            }
            else if (_isAtDesiredHeight)
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
        else
        {
            TurnOffGeyser();
        }
        

    }

    private void OnDrawGizmos()
    {
        //Visual Debugger Helpers

        Gizmos.matrix = transform.localToWorldMatrix;

        //_startingPos = transform.position;

        //Vector2 boxTotalSize = new Vector2(boxLenght, boxHeight);
        //float totalDistance = Vector2.Distance(_startingPos, GetGeyserFinalHeight());
        if (_isPlayerIn)
        {
            Gizmos.color = Color.green;
        }
        else if(_isObjectIn)
        {
            Gizmos.color = Color.blue;
        }
        else if(!(_isPlayerIn && _isObjectIn))
        {
            Gizmos.color = Color.red;
        }

        //Gizmos.DrawWireCube(new Vector2(0, geyserSO.geyserHeight), boxTotalSize);
        Gizmos.DrawWireCube(Vector3.zero + offset, new Vector2(boxLenght, geyserSO.geyserHeight));


        //Gizmos.DrawRay(Vector2.zero, Vector2.up + new Vector2(0f, totalDistance));

    }
}
