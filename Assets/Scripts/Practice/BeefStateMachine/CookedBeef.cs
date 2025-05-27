
using UnityEngine;

public class CookedBeef : BaseState
{

    private Color _targetColor = new Color(0.1f, 0.1f, 0.1f);
    private float _maxCookingTime = 5f;

    public override void EnterState(StateMachine state)
    {
        Debug.Log("OH YEAH, WE ARE COOKED!!!!");
    }

    public override void UpdateState(StateMachine state)
    {
        if (state.GetRunTime() >= _maxCookingTime)
        {
            state.SwitchState(state.OverCookedBeef);
        }
    }

    public override void ExitState(StateMachine state)
    {
        Debug.Log("SOMEONE TURN IT OFF!!!!");
    }

    public override Color GetStateTargetColor()
    {
        return _targetColor;
    }

    public override float GetMaxCookTime()
    {
        return _maxCookingTime;
    }
}
