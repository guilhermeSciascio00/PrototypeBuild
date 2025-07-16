using System.Collections;
using UnityEngine;

public class HulkSmashPower : MonoBehaviour
{
    // When the key is pressed(Input Manager probably) Disable all the action scheme(so you can't do anything)

    //applies upward force to the player, pauses him midair for like 2 or 3 seconds

    //drop him down with double the previous velocity

    //when he hits the ground again enable the controls


    [Header("Power attributes")]
    [SerializeField] private float _upForce;
    [SerializeField] private float _downForce;
    public bool HasTheSpecialBeenUsed { get; private set; }

    [Header("References")]
    [SerializeField] PlayerStateMachine _playerStateMachine;

    private Coroutine _specialAttackCoroutine;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = _playerStateMachine.GetGameInputRef();
        HasTheSpecialBeenUsed = false;
    }

    private void Update()
    {
        if (SpecialGuard())
        {
            _specialAttackCoroutine = StartCoroutine(HulkSmashSuper());
        }
    }

    private IEnumerator HulkSmashSuper()
    {

        HasTheSpecialBeenUsed = true;
        //Reseting the velocity before applying the superVelocity
        _playerStateMachine.GetPlayerRB2D().linearVelocity = Vector2.zero;

        _playerStateMachine.GetPlayerRB2D().AddForceY(_upForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        yield return new WaitForSeconds(.5f);

        _playerStateMachine.GetPlayerRB2D().linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(.3f);
        _playerStateMachine.GetPlayerRB2D().AddForceY(-_downForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        StopCoroutine(HulkSmashSuper());
        inputManager.EndSpecialRecovery();
        _specialAttackCoroutine = null;
    }

    public bool IsSpecialAttackBeingPlayed()
    {
        return _specialAttackCoroutine != null;
    }

    public bool SpecialGuard()
    {
        return _specialAttackCoroutine == null && inputManager.IsSpecialRecoveryOn() && _playerStateMachine.IsOnGround && !HasTheSpecialBeenUsed;
    }
}
