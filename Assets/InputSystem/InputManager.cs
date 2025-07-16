using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, ISavable
{

    //TODO Refactory the code!!(Action Map Profiles, find a better way to deal with rebinding)

    [Header("Action map References")]
    [SerializeField] private InputActionAsset _actionAsset;
    [SerializeField] private InputActionReference _jumpActionReference;
    [SerializeField] private InputActionReference _movementActionReference;
    [SerializeField] private InputActionReference _attackActionReference;
    [SerializeField] private InputActionReference _pauseActionReference;
    [SerializeField] private InputActionReference _specialAttackReference;

    [Header("References")]
    [SerializeField] PlayerStateMachine _playerStateMachine;
    [SerializeField] HulkSmashPower _playerSpecialAttack;

    private InputActionMap _actionMap;

    private PlayerInputActions _playerInput;
    private InputAction _moveAction, _jumpAction, _pauseAction, _attackAction, _specialAttackAction;

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

    public bool WasSpecialAttackButtonpressed { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsGamePaused = false;
        _moveAction = _movementActionReference.action;
        _jumpAction = _jumpActionReference.action;
        _pauseAction = _pauseActionReference.action;
        _attackAction = _attackActionReference.action;
        _specialAttackAction = _specialAttackReference.action;
    }

    private void OnEnable()
    {
        //_playerInput.Player.Enable();
        _actionMap = _jumpActionReference.action.actionMap;
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
        WasSpecialAttackButtonpressed = _specialAttackAction.WasPerformedThisFrame();

        WasPauseButtonPressed = _pauseAction.WasPerformedThisFrame();


        PauseButtonPressed();
        PauseActions();
        SpecialRecovery();
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

    /// <summary>
    /// Disables the action map
    /// </summary>
    private void SpecialRecovery()
    {
        if(WasSpecialAttackButtonpressed && _playerStateMachine.IsOnGround && !_playerSpecialAttack.HasTheSpecialBeenUsed)
        {
            _actionMap.Disable();
        }
    }

    public bool IsSpecialRecoveryOn()
    {
        return !_actionMap.enabled;
    }

    /// <summary>
    /// Enables the action map 
    /// </summary>
    public void EndSpecialRecovery()
    {
        if (!_actionMap.enabled) { _actionMap.Enable(); }
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
