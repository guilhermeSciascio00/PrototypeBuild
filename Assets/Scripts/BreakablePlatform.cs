using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{

    private const float MIN_DURATION_VALUE = 0f;

    [SerializeField] GameObject breakablePlatRef;
    [SerializeField] ParticleSystem platformShakeParticle;
    [SerializeField] ParticleSystem fallPlatformParticle;

    private float _platformRespawnTime = 2.5f;
    private float _tempPlatformTimer;
    private SpriteRenderer _objSpriteRenderer;

    [SerializeField] Vector2 boxOffset;
    [SerializeField] Vector2 boxPosOffset;
    [SerializeField] float circleArea = 1.5f;
    private Vector2 _originalPos;

    private bool _startAnimSequence = false;
    private bool _breakGuard = false;

    private float _animationDuration = .8f;
    private float _tempAnimationDuration;
    private float _magnitude = .1f;
    private float _alphaValue = 1f;

    private void Start()
    {
        UpdatePlatPosition();

        _tempAnimationDuration = _animationDuration;
        _tempPlatformTimer = _platformRespawnTime;
        _objSpriteRenderer = breakablePlatRef.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdatePlatPosition();
        CheckForOverlaps();
        AnimatorRunner();
        ResetPlatform();
    }


    //This method checks for overlaps around the platform, if the overlap is being hit by a player or a box, they need to be at least in a certain velocity to break the platform! Otherwise it won't trigger.
    private void CheckForOverlaps() 
    {
        if (!breakablePlatRef.activeSelf) return;
            
        Collider2D overlap = Physics2D.OverlapBox(GetBoxPosition(), GetCheckBoxSize(), 0f, LayerMask.GetMask("Player", "Objects", "Geyser"));

        if(overlap  == null) return;

        if (overlap.CompareTag("Player") || overlap.CompareTag("Objects"))
        {
            _breakGuard = overlap && overlap.GetComponent<VelocityChecker>().HasVelocityExceeded;
        }

        else if (overlap.CompareTag("Geyser"))
        {
            _breakGuard = overlap;
        }

        if (_breakGuard) _startAnimSequence = true;
    }

    //Responsible to make the Animation starts based on the conveyed information passed by the CheckForOverlaps()
    private void AnimatorRunner()
    {
        platformShakeParticle.gameObject.SetActive(_startAnimSequence);

        if (_startAnimSequence && breakablePlatRef.activeSelf)
        {
            StartCoroutine(AnimationSequence());
        }
    }

    private void UpdatePlatPosition()
    {
        if (!_startAnimSequence)
        {
            _originalPos = transform.position;
        }
    }

    //Checks if the area around the platform is free, if it's the platform will respawn again.
    private bool IsRespawnAreaAvailable()
    {
        Collider2D areaDetection = Physics2D.OverlapCircle(transform.position, circleArea, LayerMask.GetMask("Player", "Objects", "Geyser"));

        return !areaDetection;
    }

    private Vector2 GetCheckBoxSize()
    {
        return new Vector2(breakablePlatRef.transform.localScale.x - boxOffset.x, transform.localScale.y + boxOffset.y);
    }

    private Vector2 GetBoxPosition()
    {
        return new Vector2(breakablePlatRef.transform.position.x + boxPosOffset.x, transform.position.y + boxPosOffset.y);
    }

    //Sequence of actions that are played as soon as the coroutine is called.
    private IEnumerator AnimationSequence()
    {
        if(!IsParticleTimerOver())
        {
            transform.position = _originalPos + Random.insideUnitCircle * _magnitude;

            platformShakeParticle.gameObject.SetActive(true);
            if(_animationDuration <= .5f) PlatformAlphaAnim();
        }
        else
        {
            
            platformShakeParticle.gameObject.SetActive(false);
            fallPlatformParticle.gameObject.SetActive(true);

            //A little breath room here
            yield return new WaitForSeconds(.4f);

            fallPlatformParticle.gameObject.SetActive(false);
            _animationDuration = _tempAnimationDuration;
            _alphaValue = 1f;

            transform.position = _originalPos;
            breakablePlatRef.SetActive(false);
            _startAnimSequence = false;
        }
    }

    //Animation timer.
    private bool IsParticleTimerOver()
    {
        if(_animationDuration > MIN_DURATION_VALUE)
        {
            _animationDuration -= Time.deltaTime;
            return false;
        }
        else
        {
            _animationDuration = MIN_DURATION_VALUE;
            return true;
        }
    }

    private void PlatformAlphaAnim() 
    { 

        if(_alphaValue > MIN_DURATION_VALUE)
        {
            _alphaValue -= Time.deltaTime * 1.8f;
            _objSpriteRenderer.color = new Vector4(_objSpriteRenderer.color.r, _objSpriteRenderer.color.g, _objSpriteRenderer.color.b, _alphaValue);
        }
    }


    //Resets the platform to its default values.
    private void ResetPlatform()
    {
        if (!IsRespawnAreaAvailable()) return;

        if (!breakablePlatRef.activeSelf)
        {
            _platformRespawnTime -= Time.deltaTime;

            if(_platformRespawnTime <= MIN_DURATION_VALUE)
            {
                platformShakeParticle.gameObject.SetActive(false);
                fallPlatformParticle.gameObject.SetActive(false);
                breakablePlatRef.SetActive(true);

                _objSpriteRenderer.color = new Vector4(_objSpriteRenderer.color.r, _objSpriteRenderer.color.g, _objSpriteRenderer.color.b, 1f);

                _platformRespawnTime = MIN_DURATION_VALUE;
            }
        }
        else
        {
            _platformRespawnTime = _tempPlatformTimer;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetBoxPosition(), GetCheckBoxSize());

        Gizmos.color = breakablePlatRef.activeSelf ? Color.green : Color.red;

        Gizmos.DrawWireCube(GetBoxPosition(), GetCheckBoxSize() * .5f);

        Gizmos.color = IsRespawnAreaAvailable() ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, circleArea);
    }
}
