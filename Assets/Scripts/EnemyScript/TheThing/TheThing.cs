using Unity.VisualScripting;
using UnityEngine;

public class TheThing : BaseEnemy
{
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float xArea;
    [SerializeField] private float yArea;
    private Vector3 minBounds;
    private Vector3 maxBounds; // Maximum position bounds

    protected override void Start()
    {
        base.Start();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        minBounds = new Vector3(transform.position.x - xArea, transform.position.y - yArea, transform.position.z);
        maxBounds = new Vector3(transform.position.x + xArea, transform.position.y + yArea, transform.position.z);
    }

    protected override void Update()
    {
        base.Update();

        if (isInactive) return;

        // If the player is within aggro range and not in attack range, move toward the player
        if (isPlayerInAggroRange && !isPlayerInRangedAttackRange)
        {
            Walk();
        }
        // If the player is outside the aggro range, resume patrolling
        else if (!isPlayerInAggroRange)
        {
            StopWalking();
        }

        ClampPosition();
    }
    
    private void Walk()
    {
        anim.SetBool("isWalking", true);
        // Determine direction towards player (not needed for patrolling, this will be done in Patrol())
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, 0f);
    }

    private void StopWalking()
    {
        anim.SetBool("isWalking", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerStat.instance.TakeDamage(1);
        }
    }

    protected override void Die()
    {
        isInactive = true;
        anim.SetTrigger("Die");
        Invoke(nameof(ExecuteDie), 31f / 60f);
    }

    private void ExecuteDie()
    {
        Destroy(gameObject);
    }


    private void ClampPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y)
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Calculate the 4 corners of the rectangle
        Vector3 bottomLeft = new Vector3(transform.position.x - xArea, 0, transform.position.y - yArea);
        Vector3 topLeft = new Vector3(transform.position.x - xArea, 0, transform.position.y + yArea);
        Vector3 topRight = new Vector3(transform.position.x + xArea, 0, transform.position.y + yArea);
        Vector3 bottomRight = new Vector3(transform.position.x + xArea, 0, transform.position.y - yArea);

        // Draw the boundary lines
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
