using UnityEngine;

public class FallingState : StateCore
{
    public override void EnterState()
    {
        Debug.Log("And way down we go...");
    }

    public override void UpdateState()
    {
        if (PlayerStateMachineRef.IsOnGround)
        {
            PlayerStateMachineRef.GetPlayerRB2D().linearVelocity = Vector2.zero;
            BaseStateMachine.SwitchState(PlayerStateMachineRef.IdlingState);
        }

        ////???????????
        //if (PlayerStateMachineRef.GetPlayerDirection().x > 0 || PlayerStateMachineRef.GetPlayerDirection().x < 0)
        //{
        //    BaseStateMachine.SwitchState(PlayerStateMachineRef.MovingState);
        //}
    }


    public override void PhysicsUpdateState()
    {
        
    }


    public override void ExitState() 
    {
    }
}
