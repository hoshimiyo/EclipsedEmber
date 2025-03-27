using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FalseKnight : BaseEnemy
{
    // Game Objects
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform reverseAttackPoint;
    [SerializeField] private Transform landingPoint;
    [SerializeField] private GameObject groundCrackPrefab;
    [SerializeField] private GameObject shockwavePrefab;


    // Stats
    [SerializeField] private float normalAttackCooldown;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float lastNormalAttackTime;
    [SerializeField] private float lastJumpTime;
    //[SerializeField] private float recoveryTime = 3f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpSpeed;

    // Condition Checks
    [SerializeField] private bool isInactive = false;
    [SerializeField] private bool canNormalAttack = false;
    [SerializeField] private bool canJump = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private Vector3 lockedPlayerPosition; // To store the locked player position during the attack

    //Audio
    [SerializeField] AudioClip[] attackAudio;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip landAudio;
    [SerializeField] AudioClip hitGroundAudio;
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
        canJump = Time.time >= lastJumpTime + jumpCooldown && !isInactive;
        base.Update();
        if (isInactive || isJumping || isDead) return;

        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange && canJump)
        {
            StopRun();
            StartJump();
        }

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
        isDead = true;
        isInactive = true;
        anim.SetTrigger("Die");
        SFXManager.instance.PlaySFXClip(deathAudio, transform, 2f);
    }

    private void ExecudeDie()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("EndMenu");
    }

    private void Run()
    {
        anim.SetBool("isRunning", true);
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Move the mob towards the player
        //transform.position += new Vector3, 0f, 0f);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void StopRun()
    {
        anim.SetBool("isRunning", false);
        isRunning = false;
    }

    #region Normal Attack
    private void StartNormalAttack()
    {
        if (canNormalAttack)
        {
            isInactive = true;
            anim.SetTrigger("StartAttack");
            SFXManager.instance.PlayRandomSFXClip(attackAudio, transform, 2f);
            isAttacking = true;
            lockedPlayerPosition = (player.transform.position - transform.position).normalized;
        }
    }

    private void NormalAttack()
    {
        anim.SetTrigger("Attack");

    }

    private void SpawnGroundCrack()
    {
        Instantiate(groundCrackPrefab, attackPoint.position, Quaternion.identity);
        SFXManager.instance.PlaySFXClip(hitGroundAudio, transform, 1f);
        PlayerCamera.instance.ShakeCamera(1f, 0.3f);
    }

    private void SpawnGroundCrackOnLanding()
    {
        PlayerCamera.instance.ShakeCamera(1f, 0.3f);
        // Instantiate the ground crack prefab at the landing point position
        GameObject groundCrack = Instantiate(groundCrackPrefab, landingPoint.position, Quaternion.identity);
        SFXManager.instance.PlaySFXClip(landAudio, transform, 1f);
        // Get the Transform of the instantiated prefab to modify its scale
        Transform crackTransform = groundCrack.transform;

        // Double the size of the prefab by scaling it
        crackTransform.localScale = new Vector3(1.3f, 1f, 1f);  // Doubles the size on X
        // Now adjust the BoxCollider2D to match the new scale
        BoxCollider2D collider = groundCrack.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // Adjust the size of the collider based on the new scale of the prefab
            collider.size = new Vector2(collider.size.x * 1f, collider.size.y);  // Scale the collider X 
        }
        ParticleSystem particleSystem = groundCrack.GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            // Optionally, we can set the simulation space to world to avoid the particle system being affected by local scale
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            // Reset the scale of the particle system
            particleSystem.transform.localScale = Vector3.one;

            // If you want to ensure it doesn't inherit any scale from the parent at all
            var shape = particleSystem.shape;
            shape.scale = new Vector3(1f, 1f, 1f);  // Keep the shape's scale unaffected
        }
    }



    private void SpawnShockWave()
    {
        GameObject shockwave = Instantiate(shockwavePrefab, attackPoint.position, Quaternion.identity);

        Projectile shockwaveScript = shockwave.GetComponent<Shockwave>();

        if (shockwaveScript != null)
        {
            Vector3 bossDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            shockwaveScript.Initialize(bossDirection, meleeDamage);
        }
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
    #endregion

    #region Jump

    private void StartJump()
    {
        isInactive = true;
        isJumping = true;
        anim.SetTrigger("StartJump");
        SFXManager.instance.PlaySFXClip(jumpAudio, transform, 1f);
    }
    private void Jump()
    {
        anim.SetBool("isJumping", true);

        // Calculate direction towards the player
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        //// Set only the horizontal direction (X) and keep the Y direction for jumping
        //directionToPlayer.y = 0;  // Keep only the horizontal direction

        // Apply a force in the direction of the player
        rb.linearVelocity = new Vector3(directionToPlayer.x * jumpSpeed, jumpForce);
        
    }

    private void JumpBackward()
    {
        anim.SetBool("isJumping", true);

        // Calculate direction towards the player
        Vector2 directionToPlayer = (transform.position - player.transform.position).normalized;

        //// Set only the horizontal direction (X) and keep the Y direction for jumping
        //directionToPlayer.y = 0;  // Keep only the horizontal direction

        // Apply a force in the direction of the player
        rb.linearVelocity = new Vector3(directionToPlayer.x * jumpSpeed, jumpForce);

    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerStat.instance.TakeDamage(1);
        }

        // Check if the collision object is on the "Ground" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            anim.SetTrigger("Land"); // Trigger landing animation (optional)
            anim.SetBool("isJumping", false);
        }
    }


    private void JumpRecovery()
    {
        isInactive = false;
        isJumping = false;
        lastJumpTime = Time.time;
    }
}


