using UnityEngine;

public class ActiveState : StateCore
{
    public override void EnterState()
    {
        BrkableStateMachineRef.VisualPlatRef.SetActive(true);
    }

    public override void UpdateState()
    {
        Collider2D objectOverlap = Physics2D.OverlapBox(BrkableStateMachineRef.GetBoxPos(), BrkableStateMachineRef.GetBoxSize(), 0f, LayerMask.GetMask("Player", "Objects", "Geyser"));

        if (!BrkableStateMachineRef.VisualPlatRef.activeSelf || objectOverlap == null) { return; }

        if (objectOverlap.CompareTag("Geyser"))
        {
            if(objectOverlap != null)
            {
                BrkableStateMachineRef.SwitchState(BrkableStateMachineRef.BreakingState);
            }
        }

        else if (objectOverlap != null && objectOverlap.GetComponent<VelocityChecker>().HasVelocityExceeded)
        {
            BrkableStateMachineRef.SwitchState(BrkableStateMachineRef.BreakingState);
        }

    }

    public override void PhysicsUpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}
