using System.Collections;
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
    private Vector2 _originalPos;

    private bool _startAnimSequence = false;
    private float _animationDuration = 1.4f;
    private float _tempAnimationDuration;
    private float _magnitude = .1f;

    private void Start()
    {
        _originalPos = breakablePlatRef.transform.position;
        _tempAnimationDuration = _animationDuration;
        _tempPlatformTimer = _platformRespawnTime;
        _objSpriteRenderer = breakablePlatRef.GetComponent<SpriteRenderer>();
    }


    private void Update()
    {
        CheckForOverlaps();
        ResetPlatform();
    }

    private void CheckForOverlaps() 
    {
        Collider2D overlap = Physics2D.OverlapBox(GetBoxPosition(), GetCheckBoxSize(), 0f, LayerMask.GetMask("Player", "Objects"));

        _startAnimSequence = overlap;
        platformShakeParticle.gameObject.SetActive(overlap);
        if (overlap && IsOverlappingFromAbove(overlap.transform.position))
        {
            if (_startAnimSequence)
            {
                StartCoroutine(AnimationSequence());
            }
        }
    }

    private Vector2 GetCheckBoxSize()
    {
        return new Vector2(breakablePlatRef.transform.localScale.x - boxOffset.x, transform.localScale.y + boxOffset.y);
    }

    private Vector2 GetBoxPosition()
    {
        return new Vector2(breakablePlatRef.transform.position.x + boxPosOffset.x, transform.position.y + boxPosOffset.y);
    }

    private bool IsOverlappingFromAbove(Vector2 objPosition)
    {
        return objPosition.y > transform.position.y;
    }

    private IEnumerator AnimationSequence()
    {
        if(!IsParticleTimerOver())
        {
            breakablePlatRef.transform.position = _originalPos + Random.insideUnitCircle * _magnitude;
            PlatformAlphaAnim();
            platformShakeParticle.gameObject.SetActive(true);
        }
        else
        {
            platformShakeParticle.gameObject.SetActive(false);
            fallPlatformParticle.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            fallPlatformParticle.gameObject.SetActive(false);
            _animationDuration = _tempAnimationDuration;
            breakablePlatRef.SetActive(false);
        }
    }

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
        if (_animationDuration <= 1f)
        {
            _objSpriteRenderer.color = new Vector4(_objSpriteRenderer.color.r, _objSpriteRenderer.color.g, _objSpriteRenderer.color.b, _animationDuration);
        }
    }


    private void ResetPlatform()
    {
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
    }
}
