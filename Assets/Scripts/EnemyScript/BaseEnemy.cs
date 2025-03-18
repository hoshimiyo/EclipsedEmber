using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected float health = 1;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float meleeAttackRange = 1;
    [SerializeField] protected float meleeAttackHeightRange = 1;
    [SerializeField] protected float rangedAttackRange = 1;
    [SerializeField] protected float rangedAttackHeightRange = 1;
    [SerializeField] protected float aggroHorizontalRange = 20f;
    [SerializeField] protected float aggroVerticalRange = 20f;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected PlayerMovement player;
    [SerializeField] protected bool isPlayerInAggroRange = false;
    [SerializeField] protected bool isPlayerInMeleeAttackRange = false;
    [SerializeField] protected bool isPlayerInRangedAttackRange = false;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected Renderer mobRenderer;  // To store the renderer component
    protected Color originalColor;  // To store the original color of the mob
    protected Collider2D mobCollider;
    protected Animator anim;


    protected virtual void Start()
    {
        mobRenderer = GetComponent<Renderer>();  // Assumes the object has a Renderer component
        originalColor = mobRenderer.material.color;  // Store the original color
    }
    protected virtual void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        // Calculate horizontal distance and vertical distance from player
        float horizontalDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
        float verticalDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

        // Check if player is within aggro range (detects the player)
        isPlayerInAggroRange = horizontalDistance <= aggroHorizontalRange && verticalDistance <= aggroVerticalRange;

        // Check if player is within melee attack range (both horizontal and vertical)
        isPlayerInMeleeAttackRange = horizontalDistance <= meleeAttackRange && verticalDistance <= meleeAttackHeightRange;

        // Check if player is within ranged attack range (both horizontal and vertical)
        isPlayerInRangedAttackRange = horizontalDistance <= rangedAttackRange && verticalDistance <= rangedAttackHeightRange;
        FlipTowardsPlayer();
    }

    public virtual void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        StartCoroutine(BlinkRedEffect());
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual IEnumerator BlinkRedEffect()
    {
        // Change color to red
        mobRenderer.material.color = Color.red;

        // Wait for a short time (for example, 0.1 seconds)
        yield return new WaitForSeconds(0.1f);

        // Revert to the original color
        mobRenderer.material.color = originalColor;
    }


    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Prevent any movement caused by the player
            rb.linearVelocity = Vector2.zero;

            // Alternatively, to prevent pushing altogether, you can use:
            rb.AddForce(Vector2.zero); // This can stop the player from pushing by applying no force.
        }
    }

    protected void FlipTowardsPlayer()
    {
        if (isAttacking) return;
        // Flip the enemy's sprite based on the player's position relative to the enemy
        if (player.transform.position.x > transform.position.x)
        {
            // Player is to the right of the mob, face right
            if (transform.localScale.x < 0) // Already facing left, so flip
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (player.transform.position.x < transform.position.x)
        {
            // Player is to the left of the mob, face left
            if (transform.localScale.x > 0) // Already facing right, so flip
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }



    protected virtual void Attack()
    {
        PlayerStat.instance.TakeDamage(damage);
    }

    // This method will draw the attack range and aggro range in the editor
    private void OnDrawGizmos()
    {
        // Aggro Range (both horizontal and vertical)
        Gizmos.color = Color.yellow; // Aggro range color
        Gizmos.DrawWireCube(transform.position, new Vector3(aggroHorizontalRange * 2, aggroVerticalRange * 2, 1));

        // Melee Attack Range (as a wireframe sphere)
        Gizmos.color = Color.red; // Melee range color
        Gizmos.DrawWireCube(transform.position, new Vector3(meleeAttackRange * 2, meleeAttackHeightRange * 2, 1)); // Horizontal distance
        // Height Range for Melee Attack

        // Ranged Attack Range (as a wireframe sphere)
        Gizmos.color = Color.blue; // Ranged range color
        Gizmos.DrawWireCube(transform.position, new Vector3(rangedAttackRange * 2, rangedAttackHeightRange * 2, 1)); // Horizontal distance
        // Height Range for Ranged Attack

    }

}