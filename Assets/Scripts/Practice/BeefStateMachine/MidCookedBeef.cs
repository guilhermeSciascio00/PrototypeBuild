
using UnityEngine;


public class MidCookedBeef : BaseState
{
    private Color _targetColor = new Color(0.41f, 0.24f, 0.24f);
    private float _maxCookingTime = 10f;

    public override void EnterState(StateMachine state)
    {
        Debug.Log("The beef is halfway there!!!!!!!");
    }

    public override void UpdateState(StateMachine state)
    {
        if (state.GetRunTime() >= _maxCookingTime)
        {
            state.SwitchState(state.CookedBeefState);
        }
    }

    public override void ExitState(StateMachine state) 
    {
        Debug.Log("GOING ALL THE WAY TO COOKED LFG !!!!!!!");
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
