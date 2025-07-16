using System.Collections;
using UnityEngine;

public class ColorDetector : MonoBehaviour
{


    [Header("GroundDetectorSize")]
    [SerializeField] private Vector2 _detectionBoxSize;
    [SerializeField] private Vector3 _detectionBoxPositionOffset;
    [SerializeField] LayerMask _targetLayerMask;

    [Header("Debug")]
    [SerializeField] Color _startColor;
    [SerializeField] Color _currentColor;
    [SerializeField] Color _targetColor;
    [SerializeField] float _timerMaxAmount = 2f;
    [SerializeField] float _currentTimer = 0f;

    private Coroutine _colorCoroutine;

    void Start()
    {
        _startColor = GetComponent<SpriteRenderer>().color;
    }
    // Update is called once per frame
    void Update()
    {

        if (ColorGuard() && _colorCoroutine == null)
        {
            _colorCoroutine = StartCoroutine(ChangeColors());
        }
    }

    private IEnumerator ChangeColors()
    {
        _currentTimer = 0f;

        while (_currentTimer <= _timerMaxAmount)
        {
            _currentTimer += Time.deltaTime / _timerMaxAmount;
            _currentColor = Color.Lerp(_startColor, _targetColor, _currentTimer);
            this.GetComponent<SpriteRenderer>().color = _currentColor;
            yield return null;
        }

        //Delay after changing the colors;
        yield return new WaitForSeconds(1f);
        _startColor = _currentColor;
        //Testing
        StopCoroutine(ChangeColors());
        _colorCoroutine = null;
    }

    private bool ShouldIChangeColors()
    {

        Collider2D _groundChecker = Physics2D.OverlapBox(transform.position + _detectionBoxPositionOffset, _detectionBoxSize, 0f, _targetLayerMask);

        if( _groundChecker != null && _groundChecker.GetComponent<SpriteRenderer>() != null && _colorCoroutine == null)
        {
            _targetColor = _groundChecker.GetComponent<SpriteRenderer>().color;
            return true;
        }
        return false;
    }

    private bool ColorGuard()
    {
        return ShouldIChangeColors() && _currentColor != _targetColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + _detectionBoxPositionOffset, new Vector3(_detectionBoxSize.x, _detectionBoxSize.y));
    }
}
