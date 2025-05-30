using UnityEngine;

public class Square : MonoBehaviour, IFallable
{
    [SerializeField] float squareFallForce;
    Rigidbody2D rb2d;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Fall();
    }

    public void Fall()
    {
        rb2d.AddForceY(-squareFallForce);
    }
}
