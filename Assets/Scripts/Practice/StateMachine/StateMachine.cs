using UnityEditor;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState _currentState;

    public RawBeef RawBeefState = new RawBeef();
    public MidCookedBeef MidCookedBeefState = new MidCookedBeef();
    public CookedBeef CookedBeefState = new CookedBeef();
    public OverCookedBeef OverCookedBeef = new OverCookedBeef();

    private float _currentRunTime = 0f;
    private Color _currentStateColor;

    void Start()
    {
        _currentState = RawBeefState;
        _currentStateColor = GetComponent<SpriteRenderer>().color;

        RawBeefState.EnterState(this);
    }

    private void Update()
    {
        _currentState.UpdateState(this);

        AddRunTime();
        LerpToTargetColor();
    }

    public void SwitchState(BaseState state)
    {
        _currentRunTime = 0f;
        _currentState.ExitState(this);
        _currentState = state;
        _currentState.EnterState(this);
    }

    public float GetRunTime()
    {
        return _currentRunTime;
    }

    private void AddRunTime()
    {
        _currentRunTime += Time.deltaTime;
    }

    private void LerpToTargetColor()
    {
        _currentStateColor = Color.Lerp(_currentStateColor, _currentState.GetStateTargetColor(), Time.deltaTime / _currentState.GetMaxCookTime());

        GetComponent<SpriteRenderer>().color = _currentStateColor;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;

        Handles.Label(new Vector3(transform.position.x - 2f, transform.position.y + 2f, transform.position.z), $"CurrentState: {_currentState}");

        if(Application.isPlaying)
        {
            Handles.Label(new Vector3(transform.position.x - 2f, transform.position.y + 1.5f, transform.position.z), $"CurrentRunTime: {GetRunTime():F2}");
        }
    }
}
