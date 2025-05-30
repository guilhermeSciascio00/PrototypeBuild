using UnityEditor;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    [Header("State Machine Attributes")]
    private float _timeOnCurrentState = 0f;
    public StateCore _currentState;
    public StateCore _lastState;
    public Transform RootObjectTransform;
    public Animator RootObjectAnimator;

    [Header("Debug info!")]
    [SerializeField] Vector3 stateTextOffset;

    protected void SetParent()
    {
        _currentState.BaseStateMachine = this;
    }

    public void SwitchState(StateCore state)
    {
        _timeOnCurrentState = 0f;

        _lastState = _currentState;
        _currentState.ExitState();

        _currentState = state;
        SetParent();
        _currentState.EnterState();
    }

    protected void AddStateTime()
    {
        _timeOnCurrentState += Time.deltaTime;
    }

    protected virtual void OnDrawGizmos()
    {

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 9;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.green;

        if (Application.isPlaying)
        {
            Handles.Label(transform.position + stateTextOffset, $"Current State: {_currentState}", style);
        }
    }
}
