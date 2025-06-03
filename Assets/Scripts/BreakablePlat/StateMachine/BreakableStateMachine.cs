using UnityEditor;
using UnityEngine;

public class BreakableStateMachine : BaseStateMachine
{

    [Header("PlatformVariables")]
    public GameObject VisualPlatRef;
    public ParticleSystem breakingParticleSyS;
    public ParticleSystem fallingParticleSyS;
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private Vector2 _boxPositioning;
    [SerializeField] private float _quakeMagnitude;
    [SerializeField] private float circleRadius;

    [Header("MachineStates")]
    public ActiveState ActiveState;
    public BreakingState BreakingState;
    public BrokenState BrokenState;

    public bool StartAnimSequence {  get; private set; }
    public bool BreakGuard { get; private set; }
    public Vector2 OriginalPos { get; private set; }

    private void Start()
    {
        GetComponentStates();
        UpdatePlatPosition();

        _currentState = ActiveState;
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateState();
        AddStateTime();
        UpdatePlatPosition();
    }

    public Vector2 GetBoxSize() => new Vector2(RootObjectTransform.localScale.x + _boxSize.x, RootObjectTransform.localScale.y + _boxSize.y);
    public Vector2 GetBoxPos() => new Vector2(RootObjectTransform.transform.position.x + _boxPositioning.x, RootObjectTransform.transform.position.y + _boxPositioning.y);

    public float GetQuakeMagnitude() => _quakeMagnitude;

    public float GetCircleRadius() => circleRadius;

    private void UpdatePlatPosition()
    {
        if (_currentState == ActiveState)
        {
            OriginalPos = RootObjectTransform.position;
        }
    }

    private void GetComponentStates()
    {
        ActiveState = GetComponentInChildren<ActiveState>();
        BreakingState = GetComponentInChildren<BreakingState>();
        BrokenState = GetComponentInChildren<BrokenState>();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetBoxPos(), GetBoxSize());

        if(Application.isPlaying )
        {
            Gizmos.color = BrokenState.IsAreaSpawnable() ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, circleRadius);
        }
    }
}
