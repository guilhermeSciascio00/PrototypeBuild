using UnityEngine;

public class MovingState : StateCore
{

    private Vector3 _firstPosition;

    public override void EnterState()
    {
        //Debug.Log("We are walking, let's go!!");
        _firstPosition = BaseStateMachine.RootObjectTransform.position;
    }

    public override void UpdateState()
    {
        if(PlayerStateMachineRef.GetPlayerDirection().x == 0)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.IdlingState);
            PlayerStateMachineRef.GetPlayerRB2D().linearVelocity = new Vector2(0f, PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY);
        }

        if (PlayerStateMachineRef.HasPlayerJumped())
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.JumpingState);
        }
    }

    public override void PhysicsUpdateState()
    {
        //PlayerStateMachineRef.GetPlayerRB2D().linearVelocityX = PlayerStateMachineRef.GetPlayerDirection().x * PlayerStateMachineRef.GetPlayerSpeed();
    }

    public override void ExitState()
    {
    }
}
