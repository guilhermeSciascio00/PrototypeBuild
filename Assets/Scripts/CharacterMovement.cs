using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    InputSystem_Actions inputSystem;

    Vector2 _movingDirection;
    float _speedFactor = 7f;
    float _jumpForce = 18f;

    Rigidbody2D rb2D;
    bool _hasJumped = false;

    RaycastHit2D hit;

    void Awake()
    {
        inputSystem = new InputSystem_Actions();
        rb2D = GetComponent<Rigidbody2D>();
       
    }


    private void OnEnable()
    {
        inputSystem.Player.Move.Enable();
        inputSystem.Player.Move.performed += Move_performed;

        inputSystem.Player.Jump.Enable();
        inputSystem.Player.Jump.performed += Jump_performed;

    }




    private void FixedUpdate()
    {
        CharacterJump();
        MoveCharacter();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (IsOnJumpableSurface())
        {
            //Here we start the jump process
            _hasJumped = true;
        }
    }


    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _movingDirection = obj.ReadValue<Vector2>();
    }

    private void CharacterJump()
    {
        if(_hasJumped)
        {
            rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _hasJumped = false;
        }
    }

    private void MoveCharacter()
    {
        rb2D.linearVelocity = new Vector2(_movingDirection.x * _speedFactor, rb2D.linearVelocity.y);
    }
    

    private bool IsOnJumpableSurface()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.down, .5f, LayerMask.GetMask("Ground", "Objects"));

        if (hit)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, new Vector2(0f, -0.5f));
        Gizmos.DrawRay(transform.position, hit.point);
    }
}
