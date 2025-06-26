using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private PlayerInputActions _playerInput;
    private InputAction _moveAction, _jumpAction;

    /// <summary>
    /// Returns the direction that the movement button is being hold, up, down, left or right.
    /// </summary>
    public Vector2 Axis { get; private set; }

    /// <summary>
    /// Returns whether the jump button is being pressed or not.
    /// </summary>
    public bool IsJumping { get; private set; }

    /// <summary>
    /// Returns whether the jump was released or not.
    /// </summary>
    public bool WasJumpReleased { get; private set; }

    private void Awake()
    {
        _playerInput = new PlayerInputActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = _playerInput.Player.Movement;
        _jumpAction = _playerInput.Player.Jump;
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    // Update is called once per frames
    void Update()
    {
        Axis = _moveAction.ReadValue<Vector2>();
        IsJumping = _jumpAction.IsPressed();
        WasJumpReleased = _jumpAction.WasReleasedThisFrame();
    }
}
