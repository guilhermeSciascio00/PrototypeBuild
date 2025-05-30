using UnityEngine;

public class PlayerStateMachine : BaseStateMachine
{

    [Header("PlayerStateMachineAttributes")]
    [SerializeField] protected Rigidbody2D playerRefRB2D;

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
    private Vector2 _playerDirection;
    private bool _hasJumped = false;
    public bool IsOnGround { get; private set; }

    InputSystem_Actions _playerInputSystem;

    private void Awake()
    {
        _playerInputSystem = new InputSystem_Actions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _playerInputSystem.Player.Move.Enable();
        _playerInputSystem.Player.Jump.Enable();

        _playerInputSystem.Player.Move.performed += Move_performed;
        _playerInputSystem.Player.Jump.performed += Jump_performed;

        GetStatesComponents();
        _currentState = IdlingState;

        SetParent();
        _currentState.EnterState();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(IsOnGround){
            _hasJumped = true;
        }
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _playerDirection = obj.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
        AddStateTime();
        IsOnJumpAbleSurface();
    }

    private void FixedUpdate()
    {
        _currentState.PhysicsUpdateState();

        //PlayerMovement 
        MoveThePlayer();
    }

    private void MoveThePlayer()
    {
        playerRefRB2D.linearVelocityX = _playerDirection.x * _playerSpeed;
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

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GetGroundDetectorPos(), GetGroundSizeDetection());
    }
}
