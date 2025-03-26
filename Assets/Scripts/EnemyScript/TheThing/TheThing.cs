using Unity.VisualScripting;
using UnityEngine;

public class TheThing : BaseEnemy
{
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float xArea;
    [SerializeField] private float yArea;
    [SerializeField] private AudioClip idleSFX;
    [SerializeField] private AudioClip DieSFX;
    private Vector3 minBounds;
    private Vector3 maxBounds; // Maximum position bounds

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        minBounds = new Vector3(transform.position.x - xArea, transform.position.y - yArea, transform.position.z);
        maxBounds = new Vector3(transform.position.x + xArea, transform.position.y + yArea, transform.position.z);
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    protected override void Update()
    {
        base.Update();

        if (isInactive) return;

        // If the player is within aggro range and not in attack range, move toward the player
        if (isPlayerInAggroRange && !isPlayerInRangedAttackRange)
        {
            Walk();
            // Play only if not already playing
            if (!SFXManager.instance.isPlayingSFX)
            {
                SFXManager.instance.PlaySFXClipRepeat(idleSFX, transform, 1f, 6f);
                SFXManager.instance.isPlayingSFX = true;
            }
        }

        ClampPosition();
    }

    private void Walk()
    {
        // Determine direction towards player (not needed for patrolling, this will be done in Patrol())
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, 0f);
    }

    protected override void Die()
    {
        isInactive = true;
        anim.SetTrigger("Die");
        DisableCollisionsWithPlayer();
        SFXManager.instance.PlaySFXClip(DieSFX, PlayerStat.instance.transform, 1);
        Invoke(nameof(ExecuteDie), 31f / 60f);
    }

    private void ExecuteDie()
    {
        Destroy(gameObject);
        SFXManager.instance.StopSFXClipRepeat(); // Stop sound when the object is destroyed
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
        // Aggro Range (both horizontal and vertical)
        Gizmos.color = Color.yellow; // Aggro range color
        Gizmos.DrawWireCube(transform.position, new Vector3(aggroHorizontalRange * 2, aggroVerticalRange * 2, 1));

        Gizmos.color = Color.red;

        // Calculate the 4 corners of the rectangle
        Vector3 bottomLeft = new Vector3(transform.position.x - xArea, transform.position.y - yArea, 0);
        Vector3 topLeft = new Vector3(transform.position.x - xArea, transform.position.y + yArea, 0);
        Vector3 topRight = new Vector3(transform.position.x + xArea, transform.position.y + yArea, 0);
        Vector3 bottomRight = new Vector3(transform.position.x + xArea, transform.position.y - yArea, 0);

        // Draw the boundary lines
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
