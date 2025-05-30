using UnityEngine;

public abstract class StateCore : MonoBehaviour
{
    public BaseStateMachine BaseStateMachine {  get; set; }
    [SerializeField] protected PlayerStateMachine PlayerStateMachineRef;

    //Actions to be done as soon as the machine enter this state
    public abstract void EnterState();

    //Actions to be done while the machine is in this state
    public abstract void UpdateState();

    //Actions.... uses Physics
    public abstract void PhysicsUpdateState();

    //Actions.... when the machine has finished its job in this state
    public abstract void ExitState();
}
