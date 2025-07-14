using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, ISavable
{
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private InputActionReference _jumpActionReference;
    [SerializeField] private InputActionReference _movementActionReference;
    [SerializeField] private InputActionReference _attackActionReference;
    [SerializeField] private InputActionReference _pauseActionReference;

    private InputActionMap _actionMap;

    private PlayerInputActions _playerInput;
    private InputAction _moveAction, _jumpAction, _pauseAction, _attackAction;

    public bool IsGamePaused { get; private set; }

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

    public bool WasPauseButtonPressed { get; private set; }

    public bool WasAttackButtonPressed { get; private set; }

    private void Awake()
    {
        _actionMap = _jumpActionReference.action.actionMap;
        IsGamePaused = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = _movementActionReference.action;
        _jumpAction = _jumpActionReference.action;
        _pauseAction = _pauseActionReference.action;
        _attackAction = _attackActionReference.action;
    }

    private void OnEnable()
    {
        //_playerInput.Player.Enable();
        _actionMap.Enable();
    }

    private void OnDisable()
    {
        _actionMap.Disable();
    }

    // Update is called once per frames
    void Update()
    {

        Axis = _moveAction.ReadValue<Vector2>();
        IsJumping = _jumpAction.WasPerformedThisFrame();
        WasJumpReleased = _jumpAction.WasReleasedThisFrame();

        WasAttackButtonPressed = _attackAction.WasPerformedThisFrame();
        WasPauseButtonPressed = _pauseAction.WasPerformedThisFrame();

        PauseButtonPressed();
        PauseActions();
    }

    //Test only

    private void PauseButtonPressed()
    {
        if (WasPauseButtonPressed)
        {
            IsGamePaused = !IsGamePaused;

        }
    }

    private void PauseActions()
    {
        if(IsGamePaused)
        {
            //_playerInput.Player.Disable();
            //_playerInput.Player.Pause.Enable();
            _actionMap.Disable();
            _pauseAction.Enable();
        }
        else if (!IsGamePaused && WasPauseButtonPressed)
        {
            //_playerInput.Player.Enable();
            _actionMap.Enable();
        }
    }

    //The code below is responsible for loading and saving the currently actionMap

    public void OnLoad(GameData gameData)
    {
        _actionMap.LoadBindingOverridesFromJson(gameData.inputActionMap);
    }

    public void OnSave(GameData gameData)
    {
        gameData.inputActionMap = _actionMap.SaveBindingOverridesAsJson();
    }
}
