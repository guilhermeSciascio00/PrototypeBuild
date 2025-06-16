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
            PlayerStateMachineRef.GetPlayerRB2D().linearVelocity = new Vector2(0f, PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY);
            PlayerStateMachineRef.SwitchState(PlayerStateMachineRef.IdlingState);
        }

        if (PlayerStateMachineRef.HasPlayerJumped())
        {
            PlayerStateMachineRef.SwitchState(PlayerStateMachineRef.JumpingState);
        }

        //Wall Slide
        //if (PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY < 0f)
        //{
        //    BaseStateMachine.SwitchState(PlayerStateMachineRef.FallingState);
        //}
    }

    public override void PhysicsUpdateState()
    {

    }

    public override void ExitState()
    {
    }
}
