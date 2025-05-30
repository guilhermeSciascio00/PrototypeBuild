
using UnityEngine;

public class OverCookedBeef : BaseState
{

    private Color _targetColor = new Color(0.1f, 0.1f, 0.1f);
    private float _maxCookingTime = 5f; 

    public override void EnterState(StateMachine state)
    {
        Debug.Log("IT'S JOVER FELLAS!");
    }

    public override void UpdateState(StateMachine state)
    {
        
    }

    public override void ExitState(StateMachine state)
    {

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
