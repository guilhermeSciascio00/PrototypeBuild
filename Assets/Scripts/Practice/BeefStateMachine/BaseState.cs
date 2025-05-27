using UnityEngine;


public abstract class BaseState
{
    public abstract void EnterState(StateMachine state);

    public abstract void UpdateState(StateMachine state);

    public abstract void ExitState(StateMachine state);

    public abstract Color GetStateTargetColor();
    public abstract float GetMaxCookTime();
}
