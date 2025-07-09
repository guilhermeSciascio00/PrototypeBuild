using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class PlayerStateMachine : BaseStateMachine, ISavable
{

    [Header("PlayerStateMachineAttributes")]
    [SerializeField] protected Rigidbody2D playerRefRB2D;
    [SerializeField] private VelocityChecker _velocityCheckerRef;
    [SerializeField] private InputManager _gameInputManager;

    [Header("PlayerVisuals")]
    [SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private ParticleSystem _landParticles;
    [SerializeField] private ParticleSystem _heavyLandParticles;
    [SerializeField] private LineRenderer _playerLineRenderer;
    [Header("LineRendererAdjusts")]
    [SerializeField] private float _distanceTreshold = 0.10f;
    [SerializeField] private float _fadeOutTimer = .4f;
    [SerializeField] private float _fadeTimerCountdown;

    [Header("PlayerGroundChecker")]
    [SerializeField] Vector2 groundPosition;
    [SerializeField] Vector2 groundDetectionSize;

    [Header("Player States")]
    public MovingState MovingState;
    public JumpingState JumpingState;
    public IdlingState IdlingState;
    public FallingState FallingState;

    [Header("Player Stats")]
    [SerializeField] private float _playerMovementSpeed;
    [SerializeField] private float _playerJumpForce;
    [SerializeField] private float _simulateGravityForce = 2f;
    [SerializeField] private float _playerBaseYScale = .7f;
    [SerializeField] private float _playerTargetYScale = 1f;
    private Vector2 _playerDefaultScale = Vector2.zero;
    private float _baseGravity;
    private bool _hasJumped = false;
    public bool IsOnGround { get; private set; }

    [Header("Debug settings(PlayerStateMachine)")]
    [SerializeField] private Vector3 _velocityTextOffset;
    [SerializeField] private List<Vector3> _recentPlayerPosition = new List<Vector3>();
    

    void Start()
    {
        
        _recentPlayerPosition.Add(RootObjectTransform.position);
        _playerLineRenderer.SetPosition(0, RootObjectTransform.position);

        _fadeTimerCountdown = _fadeOutTimer;

        _baseGravity = playerRefRB2D.gravityScale;

        RootObjectTransform.localScale = new Vector2(RootObjectTransform.localScale.x, _playerBaseYScale);

        _playerDefaultScale = RootObjectTransform.localScale;

        GetStatesComponents();
        _currentState = IdlingState;

        SetParent();
        _currentState.EnterState();
    }

    void Update()
    {
        _currentState.UpdateState();
        AddStateTime();
        IsOnJumpAbleSurface();
        PlayerJump();

        PositionPointsManager();
    }

    private void FixedUpdate()
    {
        _currentState.PhysicsUpdateState();

        //PlayerMovement 
        MoveThePlayer();
        GravitySimulation();
    }

    #region GravityTweak

    private void GravitySimulation()
    {
        playerRefRB2D.linearVelocityY -= _simulateGravityForce;
    }

    #endregion

    private void MoveThePlayer()
    {
        playerRefRB2D.linearVelocityX = _gameInputManager.Axis.x * _playerMovementSpeed;
    }

    private void PlayerJump()
    {
        if (IsOnGround && _gameInputManager.IsJumping)
        {
            _hasJumped = true;
        }
    }

    private bool IsOnJumpAbleSurface()
    {
        Collider2D groundChecker = Physics2D.OverlapBox(GetGroundDetectorPos(), GetGroundSizeDetection(), 0f, LayerMask.GetMask("Objects", "Ground"));

        if (groundChecker != null && !groundChecker.CompareTag("Player"))
        {
            IsOnGround = true;
            return true;
        }
        IsOnGround = false;
        return false;
    }

    private void PositionPointsManager()
    {
        
        Vector3 _lastPosition = _recentPlayerPosition.Last();

        if(Vector3.Distance(RootObjectTransform.position, _lastPosition) >= _distanceTreshold)
        {
            if(_recentPlayerPosition.Count < 5)
            {
                _recentPlayerPosition.Add(RootObjectTransform.position);
            }
        }

        _playerLineRenderer.positionCount = _recentPlayerPosition.Count;
        _playerLineRenderer.SetPositions(_recentPlayerPosition.ToArray());

        if (IsFadeOutTimerOut() && _recentPlayerPosition.Count > 1)
        {
            _recentPlayerPosition.RemoveAt(0);
            _fadeTimerCountdown = _fadeOutTimer;
        }
    }

    private bool IsFadeOutTimerOut()
    {
        _fadeTimerCountdown -= Time.deltaTime;
        if(_fadeTimerCountdown < 0f)
        {
            _fadeTimerCountdown = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    #region GetMethods

    //Jumps
    public bool HasPlayerJumped() => _hasJumped;
    public void StopPlayerJump()
    {
        _hasJumped = false;
    }

    //Velocity and Gravity
    public VelocityChecker GetPlayerVelocityChecker() => _velocityCheckerRef;

    public Vector2 GetPlayerDirection() => _gameInputManager.Axis;

    public Rigidbody2D GetPlayerRB2D() => playerRefRB2D;

    public float GetPlayerSpeed() => _playerMovementSpeed;

    public float GetPlayerJumpForce() => _playerJumpForce;

    public float GetPlayerInitialGravity() => _baseGravity;

    //Input Ref
    public InputManager GetGameInputRef() => _gameInputManager;

    //Scale
    public float GetPlayerBaseYScale() => _playerBaseYScale;

    public float GetPlayerTargetYScale() => _playerTargetYScale;

    //States
    private void GetStatesComponents()
    {
        MovingState = GetComponentInChildren<MovingState>();
        JumpingState = GetComponentInChildren<JumpingState>();
        IdlingState = GetComponentInChildren<IdlingState>();
        FallingState = GetComponentInChildren<FallingState>();
    }

    //Ground detectors
    private Vector2 GetGroundDetectorPos()
    {
        return new Vector2(RootObjectTransform.position.x + groundPosition.x, RootObjectTransform.position.y + groundPosition.y);
    }

    private Vector2 GetGroundSizeDetection()
    {
        return new Vector2(groundDetectionSize.x, groundDetectionSize.y);
    }

    //ParticlesSyS
    public ParticleSystem GetJumpParticlesSys() => _jumpParticles;
    public ParticleSystem GetLandParticlesSys() => _landParticles;
    public ParticleSystem GetHeavyLandParticlesSys() => _heavyLandParticles;

    #endregion
    public void ResetPlayerGravity() => playerRefRB2D.gravityScale = _baseGravity;
    public void ResetPlayerScale() => RootObjectTransform.localScale = _playerDefaultScale;

    //SAVING ZONE, IF ANYTHING BAD HAPPENS, IT'S PROBABLY HERE

    public void OnLoad(GameData gameData)
    {
        RootObjectTransform.transform.position = gameData.playerPos;
    }

    public void OnSave(GameData gameData)
    {
        gameData.playerPos = RootObjectTransform.transform.position;
        gameData.savedTime = System.DateTime.Now;
    }


    protected override void OnDrawGizmos()
    {
        if(!Application.isPlaying) { return; }
        base.OnDrawGizmos();

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 16;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.red;

        Handles.Label(transform.position + _velocityTextOffset, $"Velocity X: {playerRefRB2D.linearVelocityX:F2}, Y: {playerRefRB2D.linearVelocityY:F2}", style);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GetGroundDetectorPos(), GetGroundSizeDetection());
    }
}
