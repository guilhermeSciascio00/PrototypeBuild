using UnityEngine;

public class BreakingState : StateCore
{

    private float _alphaValue = 1f;
    private float _alphaCountdown;
    private SpriteRenderer _objSpriteRenderer;

    public override void EnterState()
    {
        BrkableStateMachineRef.breakingParticleSyS.gameObject.SetActive(true);

        _objSpriteRenderer = BrkableStateMachineRef.VisualPlatRef.GetComponent<SpriteRenderer>();

        _alphaCountdown = _alphaValue;
    }

    public override void UpdateState()
    {
        ShakePlatform();
        PlatformAlphaAnim();
        CheckForPlatformAlpha();
    }

    public override void PhysicsUpdateState()
    {
       
    }

    public override void ExitState() 
    {
        BrkableStateMachineRef.breakingParticleSyS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        BrkableStateMachineRef.breakingParticleSyS.gameObject.SetActive(false);
        BrkableStateMachineRef.fallingParticleSyS.gameObject.SetActive(true);

        _alphaCountdown = _alphaValue;

        _objSpriteRenderer.color = new Vector4(_objSpriteRenderer.color.r, _objSpriteRenderer.color.g, _objSpriteRenderer.color.b, _alphaCountdown);
    }

    private void CheckForPlatformAlpha()
    {
        if (_alphaCountdown <= 0f)
        {
            BrkableStateMachineRef.SwitchState(BrkableStateMachineRef.BrokenState);
        }
    }

    private void PlatformAlphaAnim()
    {
        if (_alphaCountdown > 0f) 
        {
            _alphaCountdown -= Time.deltaTime;
            _objSpriteRenderer.color = new Vector4(_objSpriteRenderer.color.r, _objSpriteRenderer.color.g, _objSpriteRenderer.color.b, _alphaCountdown);
        }
    }

    private void ShakePlatform()
    {
        BrkableStateMachineRef.RootObjectTransform.position = BrkableStateMachineRef.OriginalPos + Random.insideUnitCircle * BrkableStateMachineRef.GetQuakeMagnitude();
    }
}
