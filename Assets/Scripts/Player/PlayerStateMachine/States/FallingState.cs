using UnityEngine;

public class FallingState : StateCore
{
    public override void EnterState()
    {
       
    }

    public override void UpdateState()
    {
        if (PlayerStateMachineRef.IsOnGround)
        {
            BaseStateMachine.SwitchState(PlayerStateMachineRef.IdlingState);
        }
    }


    public override void PhysicsUpdateState()
    {
        
    }


    public override void ExitState() 
    {
        PlayerStateMachineRef.GetPlayerRB2D().linearVelocity = Vector2.zero;
        PlayerStateMachineRef.ResetPlayerGravity();
    }
}
