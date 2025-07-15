using UnityEngine;
using System;

public class GeyserV2 : MonoBehaviour
{
    //TODO: Arrange the variables by functionality

    //Getting the player Rigidbody;
    private Rigidbody2D _rb2d;
    //private bool _isRb2DNull = false;

    //GeyserBoxCastSize
    [SerializeField] float boxLenghtPlayer = 1f;
    [SerializeField] float boxLenghtObjects = 1f;
    
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

    //ScriptableObject in action!!
    [SerializeField] GeyserStatsSO geyserSO;

    private Vector2 _startingPos;

    //Geyzer Force Mult and Offset
    private float _forceMultiplier = 50f;
    private float _thresholdZoneOffset = 1.3f;

    [SerializeField] Vector3 playerDetectionOffset;
    [SerializeField] Vector2 _boxOffset;
    private float _geyserHeightBox = 1f;

    Rigidbody2D playerReference;

    void Start()
    {
        SetGeyserStartValues();
    }

    private void Update()
    {
        CheckForOverLaps();
        GeyserStart(geyserSO.isItPermanent);
        PlayerVariablesHelper();
        SetGeyserBottomPosition();
    }

    private void FixedUpdate()
    {
        ApplyGeyserForce();
    }

    private Vector3 PlayerCheckBox()
    {
        return transform.position + playerDetectionOffset;
    }

    /// <summary>
    /// Use false if you wanted the check position to be normal and use true if you want it to be inverted.
    /// </summary>
    /// <param name="_isInverted"></param>
    /// <returns></returns>
    private Vector3 ObjectCheckBox(bool _isInverted)
    {
        if(_isInverted)
        {
            return transform.position + new Vector3(-_boxOffset.x, _boxOffset.y, 0f);
        }
        else
        {
            return transform.position + new Vector3(_boxOffset.x, _boxOffset.y, 0f);
        }
    }

    //This method set the default geyser values
    private void SetGeyserStartValues()
    {
        SetGeyserDLTimer();

        // *.5f is usually faster than / 2
        _lineRenderer = GetComponent<LineRenderer>();

        if (!_lineRenderer.enabled)
        {
            _lineRenderer.enabled = true;
        }

        SetGeyserBottomPosition();
        //This line is the direction that we're aiming towards
        //However, in the start we set to be the samething as the transform.position, so its stays hidden in the ground
        _lineRenderer.SetPosition(1, _startingPos);
    }

    //Calculate the height of the geyser based in the y bottom position.
    private void SetGeyserBottomPosition()
    {
        playerDetectionOffset = new Vector2(0f, geyserSO.geyserHeightPlayer * .5f);
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
    private void CheckForOverLaps()
    {

        Collider2D playerOverlap = Physics2D.OverlapBox(PlayerCheckBox(), new Vector2(boxLenghtPlayer, geyserSO.geyserHeightPlayer), 0f, LayerMask.GetMask("Player"));

        Collider2D boxOverlap = Physics2D.OverlapBox(ObjectCheckBox(false), new Vector2(boxLenghtObjects, _geyserHeightBox), 0f, LayerMask.GetMask("Objects"));

        Collider2D boxOverlap2 = Physics2D.OverlapBox(ObjectCheckBox(true),
            new Vector2(boxLenghtObjects, _geyserHeightBox), 0f, LayerMask.GetMask("Objects"));

        _isPlayerIn = playerOverlap;

        if(_isPlayerIn)
        {
            playerReference = playerOverlap.GetComponent<Rigidbody2D>();
        }

        _isObjectIn = boxOverlap && boxOverlap2;
    }

    //This method affects the player when he goes in the geyser
    private void PlayerVariablesHelper()
    {
        if(_isPlayerIn && playerReference != null) 
        {
            _isPlayerIn = true;
            _rb2d = playerReference;
            //_isRb2DNull = false;
        }
        else
        {
            _isPlayerIn = false;
            _rb2d = null;
            //_isRb2DNull = true;
            _wasVelocityCancelled = false;
        }
    }

    //Start the geyser within specific conditions.
    private void GeyserStart(bool isItPermanent)
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
            }
            if (_isGeyserOn)
            {
                GeyserMover();
            }
        }
        else if (isItPermanent && !_isObjectIn)
        {
            _lerpTime = MAX_TIMER_V;
            //_isAutoGeyserOn = true;
            _delayTime = DEFAULT_TIMER_V;
        }
        else if(isItPermanent && _isObjectIn)
        {
            TurnOffGeyser();
        }

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

            //The distance between the top of the geyser and the threshold
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

    private Vector2 GetGeyserFinalHeight()
    {
        return new Vector2(_startingPos.x, _startingPos.y + geyserSO.geyserHeightPlayer);
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

    //Two parameters, delay timer and if it's permanent or no
    public bool IsParticlesDelayOver()
    {
        if (_isObjectIn)
        {
            return true;
        }
        //TODO, geyser delay.
        if (geyserSO.needsThePlayer)
        {
            return _isGeyserOn;
        }
        else if (!geyserSO.needsThePlayer) 
        {
            return _isAutoGeyserOn;
        }
        else if (geyserSO.isItPermanent)
        {
            return true;
        }
        //if (_particleGroundDelay >= DEFAULT_TIMER_V)
        //{
        //    _particleGroundDelay -= Time.deltaTime;
        //    if (_particleGroundDelay <= DEFAULT_TIMER_V) 
        //    {
        //        _particleGroundDelay = DEFAULT_TIMER_V;
        //        return true;
        //    }
        //}
        return false;
    }

    public bool GetGeyserStatus()
    {
        return _isGeyserOn;
    }

    public bool GetAutoGeyserStatus()
    {
        return _isAutoGeyserOn;
    }

    public bool IsGeyserBlocked()
    {
        return _isObjectIn;
    }

    public GeyserStatsSO GetGeyserSORef()
    {
        return geyserSO;
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

        playerDetectionOffset = new Vector3(0f, geyserSO.geyserHeightPlayer * .5f, 0f);


        if (_isPlayerIn)
        {
            Gizmos.color = Color.green;
        }
        else if(!(_isPlayerIn && _isObjectIn))
        {
            Gizmos.color = Color.red;
        }

        //Gizmos.matrix = transform.localToWorldMatrix;


        //GeyserCheck(Player)
        Gizmos.DrawWireCube(PlayerCheckBox(), new Vector2(boxLenghtPlayer, geyserSO.geyserHeightPlayer));

        if (_isObjectIn)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        //GeyserCheck(Box)

        Gizmos.DrawWireCube(ObjectCheckBox(true), new Vector2(boxLenghtObjects, _geyserHeightBox));

        Gizmos.DrawWireCube(ObjectCheckBox(false), new Vector2(boxLenghtObjects, _geyserHeightBox));
    }
}
