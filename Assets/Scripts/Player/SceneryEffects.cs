using UnityEngine;

public class SceneryEffects : MonoBehaviour
{
    [SerializeField] bool applyAirResistance;
    [SerializeField] CharacterMovement player;

    Rigidbody2D playerRB2D;

    [Header("Debug Only")]
    [SerializeField] float airResistance = 70f;

    private void Awake()
    {
        playerRB2D = player.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(applyAirResistance)
        {
            playerRB2D.AddForce(new Vector2(-airResistance, 0f), ForceMode2D.Force);
        }
    }
}
