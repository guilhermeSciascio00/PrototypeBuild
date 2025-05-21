using System.Data.Common;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] Vector2 groundPosition;
    [SerializeField] Vector2 groundDetectionSize;

    InputSystem_Actions inputSystem;

    Vector2 _movingDirection;
    float _speedFactor = 7f;
    float _jumpForce = 18f;

    Rigidbody2D rb2D;
    bool _hasJumped = false;

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
        Collider2D groundDetector = Physics2D.OverlapBox(GetGroundDetectorPos(), GetGroundSizeDetection(), 0f, LayerMask.GetMask("Ground", "Objects"));

        if (groundDetector && !groundDetector.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    private Vector2 GetGroundDetectorPos()
    {
        return new Vector2(transform.position.x + groundPosition.x, transform.position.y + groundPosition.y);
    }

    private Vector2 GetGroundSizeDetection()
    {
        return new Vector2(groundDetectionSize.x, groundDetectionSize.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(GetGroundDetectorPos(), GetGroundSizeDetection());

        //Gizmos.DrawRay(transform.position, new Vector2(0f, -0.5f));
        //Gizmos.DrawRay(transform.position, hit.point);
    }
}
