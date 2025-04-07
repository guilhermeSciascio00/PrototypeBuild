using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    InputSystem_Actions inputSystem;

    Vector2 movingDirection;
    float speedFactor = 7f;
    float jumpForce = 18f;
    float gravityScaleForce;

    Rigidbody2D rb2D;
    bool hasJumped = false;
    bool isOnGeyzer = false;
    //bool hasDoubleJump = false;

    void Awake()
    {
        inputSystem = new InputSystem_Actions();
        rb2D = GetComponent<Rigidbody2D>();
        gravityScaleForce = rb2D.gravityScale;
    }


    private void OnEnable()
    {
        inputSystem.Player.Move.Enable();
        inputSystem.Player.Move.performed += Move_performed;

        inputSystem.Player.Jump.Enable();
        inputSystem.Player.Jump.performed += Jump_performed;

        Geyser.OnGeyserEnter += Geyser_OnGeyserEnter;
        Geyser.OnGeyserExit += Geyser_OnGeyserExit;

        //DoubleJumpPW.OnDoubleJumpCollected += DoubleJumpPW_OnDoubleJumpCollected;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Here we start the jump process
        hasJumped = true;
    }


    //private void DoubleJumpPW_OnDoubleJumpCollected()
    //{
    //    hasDoubleJump = true;
    //}


    private void FixedUpdate()
    {
        //Here we use hasJump==true, because when we press the spaceBar, hasJumped will be assigned to true, and when the fixedUpdate check, if its true, the character will jump
        if (hasJumped && IsOnGround() )
        {
            CharacterJump();
        }

        MoveCharacter();
        
    }

    private void Geyser_OnGeyserExit()
    {
        isOnGeyzer = false;
        rb2D.gravityScale = gravityScaleForce;
    }

    private void Geyser_OnGeyserEnter()
    {
        isOnGeyzer = true;
        rb2D.gravityScale = 0f;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        movingDirection = obj.ReadValue<Vector2>();
    }

    private void CharacterJump()
    {
        if(!isOnGeyzer)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            hasJumped = false;
        }
    }

    //private void CharacterDoubleJump()
    //{
    //    if (hasDoubleJump)
    //    {
    //        StartCoroutine(DoubleJumpPowerUp());
    //    }
    //}

    //IEnumerator DoubleJumpPowerUp()
    //{
    //    //Test it with linearVelocityY instead of AddForce
    //    rb2D.gravityScale = 0f;
    //    rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    //    yield return new WaitForSeconds(0.2f);
    //    hasDoubleJump = false;
    //    hasJumped = false;
    //    rb2D.gravityScale = gravityScaleForce;
    //}

    private void MoveCharacter()
    {
        rb2D.linearVelocity = new Vector2(movingDirection.x * speedFactor, rb2D.linearVelocity.y);
    }
    

    private bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (hit)
        {
            Debug.Log($"We're hitting the {hit.collider.name}");
            return true;
        }
        else
        {
            return false;
        }
    }
}
