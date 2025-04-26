using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    InputSystem_Actions inputSystem;

    Vector2 movingDirection;
    float speedFactor = 7f;
    float jumpForce = 18f;
    float gravityScaleForce;

    Rigidbody2D rb2D;
    bool hasJumped = false;
    bool isOnGeyzer = false;
    //bool hasDoubleJump = false;

    RaycastHit2D hit;

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

        GeyserV2.OnGeyserEnter += Geyser_OnGeyserEnter;
        GeyserV2.OnGeyserExit += Geyser_OnGeyserExit;

        //DoubleJumpPW.OnDoubleJumpCollected += DoubleJumpPW_OnDoubleJumpCollected;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (IsOnJumpableSurface())
        {
            //Here we start the jump process
            hasJumped = true;
        }
    }
    //private void DoubleJumpPW_OnDoubleJumpCollected()
    //{
    //    hasDoubleJump = true;
    //}


    private void FixedUpdate()
    {
        //Here we use hasJump==true, because when we press the spaceBar, hasJumped will be assigned to true, and when the fixedUpdate check, if its true, the character will jump

        CharacterJump();
        MoveCharacter();
        
    }

    private void Geyser_OnGeyserExit()
    {
        isOnGeyzer = false;
    }

    private void Geyser_OnGeyserEnter()
    {
        isOnGeyzer = true;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        movingDirection = obj.ReadValue<Vector2>();
    }

    private void CharacterJump()
    {
        if(!isOnGeyzer && hasJumped)
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
    

    private bool IsOnJumpableSurface()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.down, .5f, LayerMask.GetMask("Ground", "Objects"));

        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 1f);

        //if (hit)
        //{
        //    foreach (RaycastHit2D rayHits in hits)
        //    {
        //        Debug.Log(rayHits.collider.tag);
        //    }
        //    return true;
        //}
        //return false;

        if (hit)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
        //if (hit)
        //{
        //    Debug.Log($"We're hitting the {hit.collider.name} and it holds the tag {hit.collider.tag}");
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, new Vector2(0f, -0.5f));
        Gizmos.DrawRay(transform.position, hit.point);
    }
}
