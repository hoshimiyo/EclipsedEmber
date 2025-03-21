using System.Collections;
using UnityEngine;

public class Wizard : BaseEnemy
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float moveCooldown = 3f;
    [SerializeField] private GameObject rangedAttackPrefab;
    [SerializeField] private GameObject blashAttackPrefab;
    [SerializeField] private float rangeAttackCooldown;
    [SerializeField] private float blastAttackCooldown;
    [SerializeField] private float rangedAttackDamage;
    private float lastAttackTime;
    private float lastBlastAttackTime;
    private bool canDoRangeAttack = false;
    private bool canDoBlastAttack = false;

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 0;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        canDoRangeAttack = Time.time >= lastAttackTime + rangeAttackCooldown;
        canDoBlastAttack = Time.time >= lastBlastAttackTime + blastAttackCooldown;
        if (isInactive) return;

        // If the player is within aggro range and not in attack range, move toward the player
        if (isPlayerInAggroRange && !isPlayerInRangedAttackRange)
        {
            Walk();
        }
        else if (isPlayerInAggroRange && isPlayerInRangedAttackRange)
        {
            StopWalking();
            if (canDoBlastAttack)
            {
                BlastAttack();
            }
            else RangeAttack();

        }
        // If the player is outside the aggro range, resume patrolling
        else if (!isPlayerInAggroRange)
        {
            StopWalking();
        }
    }

    public override void TakeDamage(float damageTaken)
    {
        base.TakeDamage(damageTaken);
        if (health <= 0)
        {
            Die();
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


    private void Walk()
    {
        anim.SetBool("isWalking", true);
        // Determine direction towards player (not needed for patrolling, this will be done in Patrol())
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, 0f);
    }

    private void RangeAttack()
    {
        if (canDoRangeAttack)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown

            anim.SetTrigger("Attack");
            isAttacking = true;

            // Delay the slash spawn until frame 30 (0.5 seconds)
            Invoke(nameof(SpawnProjectile), 45f / 60f);

            // Disable isAttacking at frame 50 (0.83 seconds)
            Invoke(nameof(ResetAttackState), 46f / 60f);
            Invoke(nameof(StartAttackRecovery), 46f / 60f);
        }
    }


    private void BlastAttack()
    {
        if (canDoBlastAttack)
        {
            isInactive = true;
            lastBlastAttackTime = Time.time; // Reset cooldown

            anim.SetTrigger("Attack");
            isAttacking = true;

            // Delay the slash spawn until frame 30 (0.5 seconds)
            Invoke(nameof(SpawnBlast), 45f / 60f);

            // Disable isAttacking at frame 50 (0.83 seconds)
            Invoke(nameof(ResetAttackState), 46f / 60f);
            Invoke(nameof(StartAttackRecovery), 46f / 60f);
        }
    }

    private void SpawnProjectile()
    {
        if (rangedAttackPrefab == null)
        {
            Debug.LogError("Ranged slash prefab is not assigned!");
            return;
        }

        // Instantiate the ranged slash at the attack point
        GameObject projectile = Instantiate(rangedAttackPrefab, attackPoint.position, Quaternion.identity);

        // Get the RangedSlash script from the prefab
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // If the slash has a script, initialize it to move toward the player
        if (projectileScript != null)
        {
            projectileScript.Initialize(player.transform.position, rangedAttackDamage);
        }
    }

    private void SpawnBlast()
    {
        if (blashAttackPrefab == null)
        {
            Debug.LogError("Blast slash prefab is not assigned!");
            return;
        }

        // Lock onto the player's current position
        Vector3 lockedPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        // Start the coroutine to delay the blast spawn
        StartCoroutine(SpawnBlastAfterDelay(lockedPosition, 0f)); // half-second delay
    }

    private IEnumerator SpawnBlastAfterDelay(Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for delay time

        // Generate a random offset (left or right)
        float randomOffset = Random.Range(0.5f, 3f);
        randomOffset *= Random.value < 0.5f ? -1 : 1; // 50% chance to go left (-) or right (+)

        // Apply the offset to the X position
        Vector3 spawnPosition = new Vector3(targetPosition.x + randomOffset, targetPosition.y, targetPosition.z);

        // Instantiate the blast at the offset position
        GameObject blast = Instantiate(blashAttackPrefab, spawnPosition, Quaternion.identity);
    }



    private void StopWalking()
    {
        anim.SetBool("isWalking", false);
    }

    private void StartAttackRecovery()
    {
        Invoke(nameof(EndAttackRecovery), moveCooldown); // Delay before mob can move again
    }

    private void ResetAttackState()
    {
        isAttacking = false;
    }

    private void EndAttackRecovery()
    {
        isInactive = false;
    }
}
