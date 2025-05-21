using UnityEngine;

public class VelocityChecker : MonoBehaviour
{
    [SerializeField] float velocityLimit = 25f;
    public bool HasVelocityExceeded {  get; private set; }
    private float _resetter = .2f;
    private float _resetterTemp;

    private void Start()
    {
        _resetterTemp = _resetter;
    }

    private void Update()
    {
        ResetTheVelocity();
    }

    private void FixedUpdate()
    {
        if (HasPassedTheVelocityLimit())
        {
           HasVelocityExceeded = true;
           _resetter = _resetterTemp;
        }
    }

    private void ResetTheVelocity()
    {
        Collider2D hit = Physics2D.OverlapBox(gameObject.transform.position, gameObject.transform.localScale, 0f, LayerMask.GetMask("Ground"));

        if (hit != null && IsResetterTimeOver())
        {
            HasVelocityExceeded = false;
        }
    }

    private bool IsResetterTimeOver()
    {
        _resetter -= Time.deltaTime;
        if( _resetter < 0f )
        {
            _resetter = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    //Checks if the object exceeded a certain speed.
    private bool HasPassedTheVelocityLimit()
    {
        return gameObject.GetComponent<Rigidbody2D>().linearVelocityY <= -velocityLimit || gameObject.GetComponent<Rigidbody2D>().linearVelocityY >= velocityLimit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = HasPassedTheVelocityLimit() ? Color.cyan : Color.magenta;

        Gizmos.DrawWireCube(gameObject.transform.position, gameObject.transform.localScale);
    }
}
