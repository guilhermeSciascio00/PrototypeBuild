using UnityEngine;

public class Geyser : MonoBehaviour
{

    [SerializeField] GeyserStatsSO geyserSO;
    public GeyserStatsSO GeyserSORef { get; private set; }

    private const float MIN_TIMER_VALUE = 0f;
    private const float MAX_LERP_VALUE = 1f;

    //GeyserVariables
    private float _lerpTimer = 0f;
    private float _geyserDelay;
    private float _tempDelayTime = 0f;
    private float _tinyDelay = .89f;
    private float _tempTinyDelay;
    private bool _canResetIT = false;
    private bool _canSkip = false;
    private bool _geyserGuard = false;
    private LineRenderer _lineRenderer = null;
    private Vector2 _geyserCurrentPos = Vector2.zero;


    //GeyserDetectionVariables
    [SerializeField] Vector3 playerDetectionOffset;
    [SerializeField] Vector3 _boxOffset;
    [SerializeField] float boxLenghtPlayer = 1f;
    [SerializeField] float boxLenghtObjects = 1f;
    private float _geyserBoxHeight = 1f;
    private bool _isPlayerIn;
    private bool _isObjectIn;
    public bool IsObjectIn { get; private set; }


    //PlayerReferences/Variables
    private GameObject _playerRef;
    private bool _wasVelocityCancelled = false;

    private void Start()
    {
        GeyserSORef = geyserSO;
        _geyserDelay = geyserSO.geyserDelay;
        _tempTinyDelay = _tinyDelay;
        _tempDelayTime = _geyserDelay;
        _lineRenderer = GetComponent<LineRenderer>();

        if (!_lineRenderer.enabled)
        {
            _lineRenderer.enabled = true;
        }

        SetBottomPosition();
        _lineRenderer.SetPosition(1, transform.position);

        if(_geyserDelay <= MIN_TIMER_VALUE)
        {
            _canSkip = true;
        }
    }

    private void Update()
    {
        SetBottomPosition();
        MoveGeyser();
        CheckForOverLaps();
    }

    private void FixedUpdate()
    {
        ApplyGeyserForce();
    }

    //This method checks who is overlaping the geyser hitbox
    private void CheckForOverLaps()
    {

        Collider2D playerOverlap = Physics2D.OverlapBox(PlayerCheckBox(), new Vector2(boxLenghtPlayer, geyserSO.geyserHeightPlayer), 0f, LayerMask.GetMask("Player"));

        _playerRef = playerOverlap?.gameObject;

        Collider2D boxOverlap = Physics2D.OverlapBox(ObjectCheckBox(false), new Vector2(boxLenghtObjects, _geyserBoxHeight), 0f, LayerMask.GetMask("Objects"));

        Collider2D boxOverlap2 = Physics2D.OverlapBox(ObjectCheckBox(true),
            new Vector2(boxLenghtObjects, _geyserBoxHeight), 0f, LayerMask.GetMask("Objects"));

        _isPlayerIn = playerOverlap;
        if (!_isPlayerIn) _wasVelocityCancelled = false;
        if (_isPlayerIn) _geyserGuard = true;
        _isObjectIn = boxOverlap && boxOverlap2;
        IsObjectIn = _isObjectIn;
    }

    //GeyserTimer
    private bool IsTimerOver()
    {
        if(_geyserDelay >= MIN_TIMER_VALUE)
        {
            _geyserDelay -= Time.deltaTime;
            if(_geyserDelay <= MIN_TIMER_VALUE)
            {
                _geyserDelay = MIN_TIMER_VALUE;
                return true;
            }
        }
        return false;
    }

    private bool IsTinyDelayOver()
    {
        
        if (_tinyDelay >= MIN_TIMER_VALUE)
        {
            _tinyDelay -= Time.deltaTime;
            if (_tinyDelay <= MIN_TIMER_VALUE)
            {
                _tinyDelay = MIN_TIMER_VALUE;
                return true;
            }
        }
        return false;
    
    }

    /// <summary>
    /// If the parameter is true, the geyser will go down however if the parameter is false, it will go up.
    /// </summary>
    /// <param name="invert"></param>
    private void LerpTimer(bool invert)
    {
        
        _lerpTimer += invert ? -(Time.deltaTime * geyserSO.geyserSpeedDown) : Time.deltaTime * geyserSO.geyserSpeedUP;

        bool lerpChecker = invert ? _lerpTimer <= MIN_TIMER_VALUE : _lerpTimer >= MAX_LERP_VALUE;

        if(_lerpTimer <= MIN_TIMER_VALUE && geyserSO.needsThePlayer)
        {
            _tinyDelay = _tempTinyDelay;
        }

        if (lerpChecker)
        {
            _lerpTimer = invert ? MIN_TIMER_VALUE : MAX_LERP_VALUE;
            _canResetIT = invert ? false : true;
            if(!geyserSO.needsThePlayer) ResetTimer();
        }
    }

    private void ResetTimer()
    {
        if (_geyserDelay <= MIN_TIMER_VALUE)
        {
            _geyserDelay = _tempDelayTime;
            if (geyserSO.needsThePlayer) 
            {
                _geyserGuard = false;
            }   
        }
    }

    private void ResetGeyser()
    {
        _lerpTimer = MIN_TIMER_VALUE;
        _geyserDelay = _tempDelayTime;
        _canResetIT = false;
    }

    //Moves the geyser up and down
    private void MoveGeyser()
    {
        if (_isObjectIn) ResetGeyser();

        if (IsTimerOver() && !_canSkip && !geyserSO.needsThePlayer)
        {
            if(_lerpTimer <= MAX_LERP_VALUE && !_canResetIT)
            {
                LerpTimer(false);
            }
            else if(_canResetIT && _geyserDelay <= MIN_TIMER_VALUE)
            {
                LerpTimer(true);
            }

        }
        else if(geyserSO.needsThePlayer)
        {
            if (_geyserGuard && IsTinyDelayOver())
            {
                LerpTimer(false);
                ResetTimer();
            }
            else if (!_geyserGuard)
            { 
                LerpTimer(true); 
            }
        }
        else if(_canSkip && !_isObjectIn && !geyserSO.needsThePlayer)
        {
            _lerpTimer = MAX_LERP_VALUE;
        }

        _geyserCurrentPos = Vector2.Lerp(transform.position, GetGeyserFinalHeight(), _lerpTimer);

        _lineRenderer.SetPosition(1, _geyserCurrentPos);

    }

    private void ApplyGeyserForce()
    {
        Rigidbody2D rb2D = _playerRef?.GetComponent<Rigidbody2D>();

        if (!_wasVelocityCancelled && _isPlayerIn)
        {
            rb2D.linearVelocityY = MAX_LERP_VALUE;
            _wasVelocityCancelled = true;
        }

        if (_isPlayerIn && _playerRef.transform.position.y <= _geyserCurrentPos.y)
        {
            rb2D.linearVelocityY += geyserSO.geyserUPForce;
        }
    }

    public bool HasGeyserStarted()
    {
        if (_lerpTimer <= MIN_TIMER_VALUE)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private Vector2 GetGeyserFinalHeight()
    {
        return new Vector2(transform.position.x, transform.position.y + geyserSO.geyserHeightPlayer);

    }

    //Aligns the bottom of the geyser in real time.
    private void SetBottomPosition()
    {
        playerDetectionOffset = new Vector2(0f, geyserSO.geyserHeightPlayer * .5f);
        _lineRenderer.SetPosition(0, transform.position);
    }

    private Vector3 PlayerCheckBox()
    {
        return transform.position + playerDetectionOffset;
    }

    /// <summary>
    /// Use false if you want the check position to be normal and use true if you want it to be inverted.
    /// </summary>
    /// <param name="_isInverted"></param>
    /// <returns></returns>
    private Vector3 ObjectCheckBox(bool _isInverted)
    {
        if (_isInverted)
        {
            return transform.position + new Vector3(-_boxOffset.x, _boxOffset.y, 0f);
        }
        else
        {
            return transform.position + new Vector3(_boxOffset.x, _boxOffset.y, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        //Visual Debugger Helpers

        playerDetectionOffset = new Vector3(0f, geyserSO.geyserHeightPlayer * .5f, 0f);

        //GeyserCheck(Player)
        Gizmos.color = _isPlayerIn ? Color.green : Color.red;

        Gizmos.DrawWireCube(PlayerCheckBox(), new Vector2(boxLenghtPlayer, geyserSO.geyserHeightPlayer));

        //GeyserCheck(Box)

        Gizmos.color = _isObjectIn ? Color.blue : Color.yellow;

        Gizmos.DrawWireCube(ObjectCheckBox(true), new Vector2(boxLenghtObjects, _geyserBoxHeight));

        Gizmos.DrawWireCube(ObjectCheckBox(false), new Vector2(boxLenghtObjects, _geyserBoxHeight));
    }
}
