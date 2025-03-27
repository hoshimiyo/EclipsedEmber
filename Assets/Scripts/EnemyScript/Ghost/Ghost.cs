using System.Collections;
using UnityEngine;

public class Ghost : BaseEnemy
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float moveCooldown = 3f;
    [SerializeField] private GameObject rangedAttackPrefab;
    [SerializeField] private float rangeAttackCooldown;
    [SerializeField] private float blastAttackCooldown;
    [SerializeField] private int rangedAttackDamage;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathAudio;
    private float lastAttackTime;
    private float lastBlastAttackTime;
    private bool canDoRangeAttack;
    private bool canDoBlastAttack;

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
            RangeAttack();
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
    }

    protected override void Die()
    {
        isInactive = true;
        anim.SetTrigger("Die");
        SFXManager.instance.PlaySFXClip(deathAudio, transform, 1);
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
        if (Time.time >= lastAttackTime + rangeAttackCooldown)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown

            anim.SetTrigger("Attack");
            SFXManager.instance.PlaySFXClip(attackClip, transform, 1);
            isAttacking = true;

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

    private void StopWalking()
    {
        anim.SetBool("isWalking", false);
    }

    private void StartAttackRecovery()
    {
        Invoke(nameof(EndAttackRecovery), moveCooldown); // Delay before mob can move again
    }

    private void EndAttackRecovery()
    {
        isInactive = false;
    }

    private void EndAttackState()
    {
        isAttacking = false;
    }
}
