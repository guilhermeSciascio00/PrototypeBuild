using UnityEngine;

public class JumpingState : StateCore
{
    Vector2 _lastPosition;

    public override void EnterState()
    {
        _lastPosition = BaseStateMachine.RootObjectTransform.position;
    }

    public override void UpdateState()
    {
        if (!PlayerStateMachineRef.HasPlayerJumped() && PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY < 0f)
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

    }

}
