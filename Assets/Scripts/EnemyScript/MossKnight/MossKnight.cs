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
    [SerializeField] private AudioClip[] attackSound;
    [SerializeField] private AudioClip dieSound;
    private bool canSlash = false;
    private bool isDead = false;
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
        canSlash = Time.time >= lastAttackTime + slashAttackCooldown;
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
        if (isDead == true) return;
        if (health > 0)
        {
            health -= damageTaken;
            StartCoroutine(BlinkRedEffect());
            if (health <= 0)
            {
                Die();
            }
        }
    }

    protected override void Die()
    {
        isDead = false;
        isInactive = true;
        anim.SetTrigger("Die");
        SFXManager.instance.PlaySFXClip(dieSound, PlayerStat.instance.transform, 1f);
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
        if (canSlash && !isInactive)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown
            anim.SetTrigger("Slash");
            SFXManager.instance.PlayRandomSFXClip(attackSound, transform, 1f);
            isAttacking = true;
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

    private void EndAttackRecovery()
    {
        isInactive = false;
    }

    private void EndAttackState()
    {
        isAttacking = false;
    }
}