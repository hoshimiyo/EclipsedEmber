using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnight : BaseEnemy
{
    // Game Objects
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform reverseAttackPoint;
    [SerializeField] private GameObject groundCrackPrefab;
    [SerializeField] private GameObject shockwavePrefab;

    // Stats
    [SerializeField] private float normalAttackCooldown;
    [SerializeField] private float lastNormalAttackTime;
    [SerializeField] private float recoveryTime = 3f;

    // Condition Checks
    [SerializeField] private bool isInactive = false;
    [SerializeField] private bool canNormalAttack = false;
    private bool isRunning = false;

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
        canNormalAttack = Time.time >= lastNormalAttackTime + normalAttackCooldown && !isInactive;
        base.Update();
        if (isInactive) return;

        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange)
        {
            Run();
        }

        if (isPlayerInAggroRange && isPlayerInMeleeAttackRange)
        {
            StopRun();
            StartNormalAttack();
        }

        if (!isPlayerInAggroRange)
        {
            StopRun();
        }

    }

    public override void TakeDamage(float damageTaken)
    {
        base.TakeDamage(damageTaken);   
    }

    protected override void Die()
    {
        base.Die();
    }

    private void Run()
    {
        anim.SetBool("isRunning", true);
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, 0f);
    }

    private void StopRun()
    {
        anim.SetBool("isRunning", false);
        isRunning = false;
    }

    private void StartNormalAttack()
    {
        if (Time.time >= lastNormalAttackTime + normalAttackCooldown && !isInactive)
        {
            isInactive = true;
            

            anim.SetTrigger("StartAttack");
            isAttacking = true;

        }
    }

    private void NormalAttack()
    {
        anim.SetTrigger("Attack");
        
    }

    private void SpawnGroundCrack()
    {
        Instantiate(groundCrackPrefab, attackPoint.position, Quaternion.identity);
    }

    private void SpawnShockWave()
    {
        Instantiate(shockwavePrefab, attackPoint.position, Quaternion.identity);
    }

    private void StartAttackRecovery()
    {
        anim.SetTrigger("AttackRecover");
    }

    private void EndAttackRecovery()
    {
        isInactive = false;
        lastNormalAttackTime = Time.time;
    }

    private void EndAttackState()
    {
        isAttacking = false;
    }
}