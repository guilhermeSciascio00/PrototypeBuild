using UnityEngine;

public class PlayerStateMachine : BaseStateMachine, ISavable
{

    [Header("PlayerStateMachineAttributes")]
    [SerializeField] protected Rigidbody2D playerRefRB2D;
    [SerializeField] private InputManager _gameInputManager;

    [Header("PlayerGroundChecker")]
    [SerializeField] Vector2 groundPosition;
    [SerializeField] Vector2 groundDetectionSize;

    [Header("Player States")]
    public MovingState MovingState;
    public JumpingState JumpingState;
    public IdlingState IdlingState;
    public FallingState FallingState;

    [Header("Player Stats")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _playerJumpForce;
    private float _baseGravity;
    private Vector2 _playerDirection;
    private bool _hasJumped = false;
    public bool IsOnGround { get; private set; }

    void Start()
    {
        _baseGravity = playerRefRB2D.gravityScale;

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
    }

    private void FixedUpdate()
    {
        _currentState.PhysicsUpdateState();

        //PlayerMovement 
        MoveThePlayer();
    }

    private void MoveThePlayer()
    {
        playerRefRB2D.linearVelocityX = _gameInputManager.Axis.x * _playerSpeed;
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

    public bool HasPlayerJumped() => _hasJumped;
    public void StopPlayerJump()
    {
        _hasJumped = false;
    }

    public Vector2 GetPlayerDirection() => _playerDirection;

    public Rigidbody2D GetPlayerRB2D() => playerRefRB2D;

    public float GetPlayerSpeed() => _playerSpeed;

    public float GetPlayerJumpForce() => _playerJumpForce;

    public void ResetPlayerGravity() => playerRefRB2D.gravityScale = _baseGravity;

    public InputManager GetGameInputRef() => _gameInputManager;

    private void GetStatesComponents()
    {
        MovingState = GetComponentInChildren<MovingState>();
        JumpingState = GetComponentInChildren<JumpingState>();
        IdlingState = GetComponentInChildren<IdlingState>();
        FallingState = GetComponentInChildren<FallingState>();
    }

    private Vector2 GetGroundDetectorPos()
    {
        return new Vector2(RootObjectTransform.position.x + groundPosition.x, RootObjectTransform.position.y + groundPosition.y);
    }

    private Vector2 GetGroundSizeDetection()
    {
        return new Vector2(groundDetectionSize.x, groundDetectionSize.y);
    }

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
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GetGroundDetectorPos(), GetGroundSizeDetection());
    }
}
