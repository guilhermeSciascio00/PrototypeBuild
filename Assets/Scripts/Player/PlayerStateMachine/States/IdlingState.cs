using UnityEngine;

public class IdlingState : StateCore
{

    public override void EnterState()
    {
        //Debug.Log($"My father is {BaseStateMachine}");
        //Debug.Log("Waiting for inputs!");
    }

    public override void UpdateState()
    {
        if(PlayerStateMachineRef.GetPlayerDirection().x > 0f || PlayerStateMachineRef.GetPlayerDirection().x < 0f)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.MovingState);
        }

        else if (PlayerStateMachineRef.HasPlayerJumped())
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.JumpingState);
        }

        else if (PlayerStateMachineRef.GetPlayerRB2D().linearVelocityY < 0f)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.FallingState);
        }
    }

    public override void PhysicsUpdateState()
    {
       
    }

    public override void ExitState() 
    {

    }
}
