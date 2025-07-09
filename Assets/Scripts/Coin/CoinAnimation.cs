using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("Coin References")]
    [SerializeField] private GameObject _coinVisual;
    [SerializeField] private GameObject _coinManager;

    [Header("Coin Values")]
    [SerializeField] private float _coinYFloat = 1f;
    [SerializeField] private int _coinID = 0;

    #region AnimationVariables
    private Vector2 _startTransform;
    private Vector2 _targetDirection;
    private bool _moveAnimFlag = false;

    [SerializeField, Range(0f, 1f)] private float _lerpValue = 0f;
    [SerializeField, Range(1f, 3f)] private float _rotationSpeed;

    private bool _hasCoinBeenPickedUp = false;
    private float _transparencyAmount = 1f;

    //References
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _circleCollider2D;


    #endregion

    void Start()
    {
        _startTransform = _coinManager.transform.position;
        _targetDirection = GetTargetDirection();

        _spriteRenderer = _coinVisual.GetComponent<SpriteRenderer>();
        _circleCollider2D = _coinManager.GetComponent<CircleCollider2D>();

        CoinManager.AddCoin();
        _coinID = CoinManager.GetCoinAmount();

        this.gameObject.name = $"CoinAnim_ID_{_coinID}";
    }

    private void Update()
    {
        CoinAnim();
    }

    private void CoinAnim()
    {
        //Move Animation
        _coinManager.transform.position = Vector2.Lerp(_startTransform, _targetDirection, ReturnLerpValue());

        //Rotation Animation
        _coinManager.transform.eulerAngles += new Vector3(0f, _rotationSpeed, 0f);

        //Transparency Anim
        CoinTransparency();
    }

    #region LerpCode
    private float ReturnLerpValue()
    {
        return LerpGuard() ? _lerpValue -= Time.deltaTime : _lerpValue += Time.deltaTime;
    }

    private bool LerpGuard()
    {
        if(_lerpValue <= 0f)
        {
            _moveAnimFlag = false;
        }
        //_lerpValue += Time.deltaTime;
        if (_lerpValue >= 1f)
        {
            _moveAnimFlag = true;
        }

        return _moveAnimFlag;
    }
    #endregion

    private Vector2 GetTargetDirection()
    {
        return new Vector2(_coinManager.transform.position.x, _coinManager.transform.position.y + _coinYFloat);
    }

    private void CoinTransparency()
    {
        if (!_hasCoinBeenPickedUp) return;

        _transparencyAmount -= Time.deltaTime;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.material.color.g, _spriteRenderer.color.b, _transparencyAmount);

        _circleCollider2D.enabled = false;

        if (_transparencyAmount <= 0f)
        {
            _transparencyAmount = 0f;
            
            this.gameObject.SetActive(false);
        }
    }

    public void ActivateCollision()
    {
        _hasCoinBeenPickedUp = true;
    }

    public GameObject GetCoinRef() => this.gameObject;
}
