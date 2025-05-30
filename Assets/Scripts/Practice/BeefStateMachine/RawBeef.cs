using UnityEngine;

public class RawBeef : BaseState
{
    private Color _targetColor = new Color(0.76f, 0.54f, 0.54f);
    private float _maxCookingTime = 5f;

    public override void EnterState(StateMachine state)
    {
        Debug.Log("Raw beef incoming!!");
    }

    public override void UpdateState(StateMachine state)
    {
        if (state.GetRunTime() >= _maxCookingTime)
        {
            state.SwitchState(state.MidCookedBeefState);
        }
    }

    public override void ExitState(StateMachine state)
    {
        Debug.Log("Into midcooked we go!!!");
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
