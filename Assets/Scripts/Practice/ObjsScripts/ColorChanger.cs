using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private SpriteRenderer _cubeRenderer;

    [SerializeField, Range(0f,1f)] private float _colorLerp;

    [SerializeField] private Color _currentColor;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Color _tempColor;

    void Start()
    {
        _cubeRenderer = GetComponent<SpriteRenderer>();
        _currentColor = _cubeRenderer.color;
        _targetColor = GetRandomColor();
    }

    private void Update()
    {
        ColorLerp();
        ChangeLerpColor();
    }

    private void ColorLerp()
    { 
        _cubeRenderer.color = Vector4.Lerp(_currentColor, _targetColor, _colorLerp);
    }

    private bool IsLerpAtMax()
    {
        _colorLerp += Time.deltaTime;
        if( _colorLerp >= 1f ) 
        {
            _colorLerp = 1f;
            return true;
        }
        else { return false; }
    }

    private void ChangeLerpColor()
    {
        if(IsLerpAtMax())
        {
            _tempColor = _targetColor;
            _currentColor = _tempColor;
            _targetColor = GetRandomColor();
            _colorLerp = 0f;
        }
    }

    private Color GetRandomColor()
    {
        return new Color(GetRandomNumber(), GetRandomNumber(), GetRandomNumber());
    }

    private float GetRandomNumber()
    {
        return Random.Range(0f, 1f);
    }
}
