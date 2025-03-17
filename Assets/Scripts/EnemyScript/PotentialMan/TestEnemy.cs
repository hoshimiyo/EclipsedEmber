using System.Collections;
using UnityEngine;

public class TestEnemy : BaseEnemy
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject slashHitbox;
    [SerializeField] private GameObject rangedSlashPrefab;

    // Cooldowns for moves
    [SerializeField] private float slashAttackCooldown; // Cooldown between slashes
    [SerializeField] private float moveCooldown; // Cooldown after each move
    [SerializeField] private float teleportCooldown;
    [SerializeField] private float rangedAttackCooldown;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float teleportOffset; // The offset distance of player and boss after tp to player
    [SerializeField] private float knockbackPower;

    // Track cooldowns
    private float lastSlashTime;
    private float lastTeleportTime;
    private float lastRangedAttackTime;
    private bool isPreparingRangedAttack = false; // Ensures boss doesn't spam ranged attacks
    private bool isAttackRecovering = false;
    private float windupTime = 0.5f; // Time before teleporting starts
    private bool isInWindup = false;
    private bool isTeleporting = false; // Flag to check if the mob is dashing
    private bool canMove = true; // To track if boss can move
    private bool canTeleport = false;
    private bool canUseRangedAttack = false;

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
        rb.mass = 20f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        anim = GetComponent<Animator>(); // Get Animator component
        lastTeleportTime = -moveCooldown; // Initialize with a time in the past to allow an immediate dash
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        // If the boss is recovering from an attack, do nothing
        if (isAttackRecovering) return;

        // Check if cooldown has passed before dashing again
        canTeleport = Time.time >= lastTeleportTime + moveCooldown;
        canUseRangedAttack = Time.time >= lastRangedAttackTime + rangedAttackCooldown;

        // If player is within aggro range and on ground, tp towards them
        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange && canTeleport && canMove && player.IsGrounded())
        {
            StartTeleportWithWindup();
        }

        // If player is within attack range, attack
        if (isPlayerInAggroRange && isPlayerInMeleeAttackRange && canMove)
        {
            StopTeleporting(); // Stop dashing once in attack range
            Invoke(nameof(SlashAttack), 0.4f);
        }

        // If in ranged attack range but NOT immediately attacking
        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange && isPlayerInRangedAttackRange && canMove)
        {
            // Ensure the boss still moves forward before committing to an attack
            if (canUseRangedAttack && !isPreparingRangedAttack)
            {
                StopTeleporting();
                StartRangedAttackDecision();
            }
            else if(canTeleport)
            {
                StartTeleportWithWindup();
            }
        }

        // If the mob isn't dashing or attacking, return to idle (or other logic)
        if (!isPlayerInAggroRange)
        {
            StopTeleporting();  // Stop the dash animation and movement
        }
    }

    public override void TakeDamage(float damageTaken)
    {
        float blockChance = 0.2f; // 20% chance

        if (Random.value < blockChance) // Successful block
        {
            Debug.Log("Boss blocked the attack!");

            // Play block animation
            anim.SetTrigger("Block");

            // Disable movement temporarily
            canMove = false;

            // Check if player is within knockback range
            Vector2 distance = (Vector2)(player.transform.position - transform.position);
            if (Mathf.Abs(distance.x) <= 6f && Mathf.Abs(distance.y) <= 2f)
            {
                
            }

            // Restore movement after block animation (adjust duration as needed)
            Invoke(nameof(EndBlock), 0.5f);
        }
        else
        {
            StartCoroutine(base.BlinkRedEffect());
            base.TakeDamage(damageTaken);
        }
    }


    private void EndBlock()
    {
        canMove = true;
        anim.SetTrigger("EndBlock");
    }

    private void SlashAttack()
    {
        if (Time.time >= lastSlashTime + slashAttackCooldown && canMove)
        {
            lastSlashTime = Time.time; // Reset cooldown
            lastTeleportTime = Time.time;

            anim.SetTrigger("meleeAttack");
            isAttacking = true;

            // Delay effect and hitbox activation to sync with attack frame
            Invoke(nameof(EnableSlashHitbox), 0f);

            // Disable hitbox after attack finishes
            Invoke(nameof(DisableSlashHitbox), 0.5f);

            // Reset animation
            Invoke(nameof(ResetAttackState), 0.5f);
            StartAttackRecovery();
        }
    }

    private void StartRangedAttackDecision()
    {
        isPreparingRangedAttack = true;

        // Randomized delay between 0.3s and 0.8s before committing to a ranged attack
        float delay = Random.Range(0.3f, 0.8f);
        Invoke(nameof(PerformRangedAttack), delay);
    }

    private void PerformRangedAttack()
    {
        StopTeleporting();
        RangeAttack();
        isPreparingRangedAttack = false; // Reset flag
    }


    private void RangeAttack()
    {
        if (Time.time >= lastRangedAttackTime + rangedAttackCooldown)
        {
            lastSlashTime = Time.time; // Reset cooldown
            lastTeleportTime = Time.time;

            anim.SetTrigger("doRangeSlash");
            isAttacking = true;

            // Delay the slash spawn until frame 30 (0.5 seconds)
            Invoke(nameof(SpawnSlash), 0.5f);

            // Disable isAttacking at frame 50 (0.83 seconds)
            Invoke(nameof(ResetAttackState), 0.83f);
            StartAttackRecovery();
        }
    }

    // Spawns the ranged slash
    private void SpawnSlash()
    {
        if (rangedSlashPrefab == null)
        {
            Debug.LogError("Ranged slash prefab is not assigned!");
            return;
        }

        // Instantiate the ranged slash at the attack point
        GameObject slash = Instantiate(rangedSlashPrefab, attackPoint.position, Quaternion.identity);

        // Get the RangedSlash script from the prefab
        RangedSlash slashScript = slash.GetComponent<RangedSlash>();

        // If the slash has a script, initialize it to move toward the player
        if (slashScript != null)
        {
            slashScript.Initialize(player.transform.position);
        }
    }

    private void ResetAttackState()
    {
        isAttacking = false;
    }

    private void EnableSlashHitbox()
    {
        slashHitbox.SetActive(true);
    }

    private void DisableSlashHitbox()
    {
        slashHitbox.SetActive(false);
    }

    private void TeleportToPlayer()
    {
        // Ensure the player is grounded before dashing
        if (player.IsGrounded())
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // Define the possible target positions: in front and behind the player
            Vector3 frontPosition = (Vector2)player.transform.position + direction * teleportOffset;  // distance in front
            Vector3 backPosition = (Vector2)player.transform.position - direction * teleportOffset;   // distance behind

            // Check for obstacles in front of the player
            bool isFrontBlocked = IsObstacleBetween(player.transform.position, frontPosition);

            // Check for obstacles behind the player
            bool isBackBlocked = IsObstacleBetween(player.transform.position, backPosition);

            // Determine where to teleport based on which side is blocked
            Vector3 targetPosition = Vector3.zero;

            if (!isFrontBlocked && !isBackBlocked) // Both sides are clear
            {
                // Randomly decide whether to teleport in front or behind the player
                targetPosition = Random.Range(0, 2) == 0 ? frontPosition : backPosition;
            }
            else if (!isFrontBlocked) // Only back is blocked
            {
                targetPosition = frontPosition; // Teleport in front
            }
            else if (!isBackBlocked) // Only front is blocked
            {
                targetPosition = backPosition; // Teleport behind
            }
            else
            {
                // Both sides are blocked, do not teleport and log the situation
                Debug.Log("Both sides are blocked, teleport to player left.");
                targetPosition = player.transform.position + Vector3.left * teleportOffset;
            }

            if (Mathf.Abs(transform.position.y - player.transform.position.y) > 2f)
            {
                Debug.Log("Fixed tp, no random direction");
                targetPosition.y = player.transform.position.y;
                targetPosition.x = player.transform.position.x + (transform.position.x > player.transform.position.x ? teleportOffset : -teleportOffset);
            }

            // Perform the teleportation
            transform.position = targetPosition;

            // Reset flags after teleporting
            isInWindup = false; // End the windup state
            canMove = true; // Allow the boss to move again
            Invoke(nameof(StopTeleporting), 0.1f);
        }
        else
        {
            // If the player is not grounded, stop the teleporting animation and reset
            isInWindup = false;
            canMove = true;
            Invoke(nameof(StopTeleporting), 0.1f); // Ensure animation stops
        }

    }


    // Helper method to check if there's an obstacle between the current position and the target position
    private bool IsObstacleBetween(Vector3 startPos, Vector3 endPos)
    {
        // Raycast from the boss to the target position to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(startPos, endPos - startPos, 4f, _obstacleLayer);

        // Return true if there is an obstacle (hit something within 4 units)
        return hit.collider != null;
    }


    public void StartTeleportWithWindup()
    {
        if (!isTeleporting)
        {
            isTeleporting = true;
            isInWindup = true;  // Indicate that the windup is in progress
            anim.SetTrigger("StartTeleportWindup"); // Trigger windup animation if necessary
            canMove = false;
            StartCoroutine(TeleportationWindup());
        }
    }

    private IEnumerator TeleportationWindup()
    {
        // Wait for the windup duration
        yield return new WaitForSeconds(windupTime);

        // Once windup is complete, perform the teleportation
        TeleportToPlayer();

        // Proceed with the teleportation animation
        anim.SetBool("isTeleporting", true);
    }


    private void StopTeleporting()
    {
        isTeleporting = false;
        anim.SetBool("isTeleporting", false); // Stop dash animation

        lastTeleportTime = Time.time;
        rb.linearVelocity = Vector2.zero; // Stop movement
    }

    private void StartAttackRecovery()
    {
        isAttackRecovering = true;
        canMove = false; // Prevent moving while recovering
        Invoke(nameof(EndAttackRecovery), moveCooldown); // Delay before boss can move again
    }

    private void EndAttackRecovery()
    {
        isAttackRecovering = false;
        canMove = true; // Allow moving after recovery
    }
}
