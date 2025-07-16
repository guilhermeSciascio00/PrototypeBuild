using UnityEngine;

public class JumpingState : StateCore
{
    private Vector2 _lastPosition;
    private float _lerpDuration = 0f;

    public override void EnterState()
    {
        _lastPosition = BaseStateMachine.RootObjectTransform.position;
        PlayerStateMachineRef.UpdateParticlesColor();
        PlayerStateMachineRef.GetJumpParticlesSys().Play();
    }

    public override void UpdateState()
    {
        StrechAnimation();

        if (!PlayerStateMachineRef.HasPlayerJumped() && PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY < 0f)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.FallingState);
        }

        else if (PlayerStateMachineRef.GetGameInputRef().WasJumpReleased)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.FallingState);
        }
    }

    public override void PhysicsUpdateState()
    {
        if (PlayerStateMachineRef.HasPlayerJumped())
        {
            PlayerStateMachineRef.GetPlayerRB2D().AddForceY(PlayerStateMachineRef.GetPlayerJumpForce(), ForceMode2D.Impulse);

            PlayerStateMachineRef.StopPlayerJump();
        }
    }

    public override void ExitState()
    {
        _lerpDuration = 0f;
    }

    private void StrechAnimation()
    {
        Vector3 playerScaleRef = PlayerStateMachineRef.GetPlayerRB2D().gameObject.transform.localScale;

        _lerpDuration += Time.deltaTime * .25f;

        playerScaleRef = Vector3.Lerp(playerScaleRef, new Vector3(playerScaleRef.x, PlayerStateMachineRef.GetPlayerTargetYScale(), playerScaleRef.z), _lerpDuration);

        PlayerStateMachineRef.GetPlayerRB2D().gameObject.transform.localScale = playerScaleRef;
    }

}
