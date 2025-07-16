using UnityEngine;

public class FallingState : StateCore
{
    private float _maxFallingSpeed = 30f;
    private Rigidbody2D _playerRB2D;
    private float _lerpDuration = 0f;

    //private float _lastGravityScale;
    //private float _currentGravityScale;
    //private float _initialGravity;
    //private float _gravityDifferenceNormalized;

    public override void EnterState()
    {
        PlayerStateMachineRef.UpdateParticlesColor();
        _playerRB2D = PlayerStateMachineRef.GetPlayerRB2D();
    }


    public override void UpdateState()
    {
        SquashAnim();
        if (PlayerStateMachineRef.IsOnGround)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.IdlingState);
        }
    }



    public override void PhysicsUpdateState()
    {
        _playerRB2D.linearVelocityY = Mathf.Clamp(_playerRB2D.linearVelocityY, -_maxFallingSpeed, 0f);
    }

    public override void ExitState() 
    {
        PlayerStateMachineRef.GetPlayerRB2D().linearVelocity = Vector2.zero;
        _lerpDuration = 0f;

        PlayerStateMachineRef.ResetPlayerGravity();
        PlayerStateMachineRef.ResetPlayerScale();

        PlayerStateMachineRef.GetLandParticlesSys().Play();
        LandParticleManager();
    }

    private void LandParticleManager()
    {
        if (PlayerStateMachineRef.GetPlayerVelocityChecker().HasVelocityExceeded)
        {
            PlayerStateMachineRef.GetHeavyLandParticlesSys().Play();
        }
        else
        {
            PlayerStateMachineRef.GetLandParticlesSys().Play();
        }
    }

    private void SquashAnim()
    {
        Vector3 playerLocalScaleRef = PlayerStateMachineRef.GetPlayerRB2D().gameObject.transform.localScale;

        _lerpDuration += Time.deltaTime * .125f;

        playerLocalScaleRef = Vector3.Lerp(playerLocalScaleRef, new Vector3(playerLocalScaleRef.x, PlayerStateMachineRef.GetPlayerBaseYScale(), playerLocalScaleRef.z), _lerpDuration);

        PlayerStateMachineRef.GetPlayerRB2D().gameObject.transform.localScale = playerLocalScaleRef;
    }


}
