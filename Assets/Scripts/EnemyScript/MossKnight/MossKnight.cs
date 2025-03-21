using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MossKnight : BaseEnemy
{
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float moveCooldown = 3f;
    [SerializeField] private GameObject slashHitbox;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float slashAttackCooldown; // Cooldown between slashes
    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (isInactive) return;

        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange)
        {
            Walk();
        }

        if (isPlayerInAggroRange && isPlayerInMeleeAttackRange)
        {
            StopWalking();
            SlashAttack();
        }

        if (!isPlayerInAggroRange)
        {
            StopWalking();
        }
    }

    public override void TakeDamage(float damageTaken)
    {
        base.TakeDamage(damageTaken);
        if(health <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        isInactive = true;
        anim.SetTrigger("Die");
        Invoke(nameof(ExecuteDie), 53f / 60f);
    }

    private void ExecuteDie()
    {
        Destroy(gameObject);
    }

    private void Walk()
    {
        anim.SetBool("isWalking", true);
        // Determine direction towards player
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, 0f);
    }

    private void SlashAttack()
    {
        if (Time.time >= lastAttackTime + slashAttackCooldown)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown
            anim.SetTrigger("Slash");
            isAttacking = true;
            // Delay effect and hitbox activation to sync with attack frame 30 (assuming 60 FPS)
            Invoke(nameof(EnableSlashHitbox), 30f / 60f); // 30 frames delay (assuming 60 FPS)

            // Disable hitbox after attack finishes at frame 45
            Invoke(nameof(DisableSlashHitbox), 45f / 60f); // 10 frames delay

            // Reset animation & attack state after the attack animation ends
            Invoke(nameof(ResetAttackState), 45f / 60f);
            Invoke(nameof(StartAttackRecovery), 45f / 60f);
        }
    }


    private void EnableSlashHitbox()
    {
        slashHitbox.SetActive(true);
    }

    private void DisableSlashHitbox()
    {
        slashHitbox.SetActive(false);
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