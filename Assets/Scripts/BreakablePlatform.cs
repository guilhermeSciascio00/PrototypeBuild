using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{

    [SerializeField] Vector2 boxOffset;
    [SerializeField] Vector2 boxPosOffset;

    private void Update()
    {
        CheckForOverlaps();
    }

    private void CheckForOverlaps() 
    {
        Collider2D overlap = Physics2D.OverlapBox(GetBoxPosition(), GetCheckBoxSize(), 0f, LayerMask.GetMask("Player", "Objects"));

        if (overlap && IsOverlappingFromAbove(overlap.transform.position))
        {
            Debug.Log($"{overlap.gameObject.name} is here!");
        }
    }



    private Vector2 GetCheckBoxSize()
    {
        return new Vector2(transform.localScale.x - boxOffset.x, transform.localScale.y + boxOffset.y);
    }

    private Vector2 GetBoxPosition()
    {
        return new Vector2(transform.position.x + boxPosOffset.x, transform.position.y + boxPosOffset.y);
    }

    private bool IsOverlappingFromAbove(Vector2 objPosition)
    {
        return objPosition.y > transform.position.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetBoxPosition(), GetCheckBoxSize());
    }
}
