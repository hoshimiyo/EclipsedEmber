using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : BaseEnemy
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject slashHitbox;
    [SerializeField] private GameObject slashHitbox2;
    [SerializeField] private GameObject rangedSlashPrefab;
    [SerializeField] private float rangedDamage;
    // Cooldowns for moves
    [SerializeField] private float slashAttackCooldown; // Cooldown between slashes
    [SerializeField] private float moveCooldown; // Cooldown after each move
    [SerializeField] private float teleportCooldown;
    [SerializeField] private float rangedAttackCooldown;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float teleportOffset; // The offset distance of player and boss after tp to player
    [SerializeField] private float knockbackPower;

    // Track cooldowns
    [SerializeField] private float lastAttackTime;
    private float lastTeleportTime;
    private float lastRangedAttackTime;
    [SerializeField] private bool isInactive = false;
    [SerializeField] private float teleportWindupTime; // Time before teleporting starts
    private bool isTeleporting = false; // Flag to check if the mob is dashing
    [SerializeField] private bool canMove = true; // To track if boss can move
    [SerializeField] private bool canTeleport = true;
    [SerializeField] private bool canUseRangedAttack = false;

    protected override void Start()
    {
        base.Start();
        lastTeleportTime = -teleportCooldown; // Initialize with a time in the past to allow an immediate dash
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        canTeleport = Time.time >= lastTeleportTime + teleportCooldown;
        base.Update();

        // If the boss is recovering from an attack, do nothing
        if (isInactive || isTeleporting) return;

        // If player is within aggro range and on ground, tp towards them
        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange && canTeleport)
        {
            StartTeleportWithWindup();
        }

        // If player is within attack range, attack
        if (isPlayerInAggroRange && isPlayerInMeleeAttackRange)
        {
            SlashAttack();
        }

        // If in ranged attack range but NOT immediately attacking
        if (isPlayerInAggroRange && !isPlayerInMeleeAttackRange && isPlayerInRangedAttackRange)
        {
            if (canTeleport)
            {
                StartTeleportWithWindup();
            }

            // Ensure the boss still moves forward before committing to an attack
            else
            {
                RangeAttack();
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

            // Check if player is within knockback range
            Vector2 distance = (Vector2)(player.transform.position - transform.position);
            if (Mathf.Abs(distance.x) <= meleeAttackRange && Mathf.Abs(distance.y) <= meleeAttackHeightRange)
            {
                MoveBackward();
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

    private void MoveBackward()
    {
        canMove = false;
        anim.SetTrigger("Dodge");
        // Ensure that the boss has a Rigidbody2D component
        if (rb != null)
        {
            // Get the direction the boss is facing
            Vector2 moveDirection = (transform.localScale.x > 0) ? Vector2.left : Vector2.right; // Move left if facing right, right if facing left

            // Apply movement force in the opposite direction (backward)
            rb.linearVelocity = moveDirection * 16f;  // Move at speed of 16 units per second

            // Optional: Stop the movement after a slight delay
            StartCoroutine(StopMovementAfterDelay(0.5f));  // Adjust time as necessary (0.5 seconds in this example)
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on the boss!");
        }
    }

    // Coroutine to stop the boss's movement after a slight delay
    private IEnumerator StopMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified time
        rb.linearVelocity = Vector2.zero;  // Stop movement
        canMove = true;
    }


    private void EndBlock()
    {
        anim.SetTrigger("EndBlock");
    }

    private void SlashAttack()
    {
        if (Time.time >= lastAttackTime + slashAttackCooldown && canMove)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown

            anim.SetTrigger("meleeAttack");
            isAttacking = true;

            // Delay effect and hitbox activation to sync with attack frame 20 (assuming 60 FPS)
            Invoke(nameof(EnableSlashHitbox), 20f / 60f); // 20 frames delay (assuming 60 FPS)

            // Disable hitbox after attack finishes at frame 35
            Invoke(nameof(DisableSlashHitbox), 35f / 60f); // 35 frames delay

            Invoke(nameof(EnableSlashHitbox2), 50f / 60f);

            Invoke(nameof(DisableSlashHitbox2), 65f / 60f);

            // Reset animation & attack state after the attack animation ends
            Invoke(nameof(ResetAttackState), 66f / 60f);
            Invoke(nameof(StartAttackRecovery), 65f / 60f);
        }
    }

    private void RangeAttack()
    {
        if (Time.time >= lastAttackTime + slashAttackCooldown)
        {
            isInactive = true;
            lastAttackTime = Time.time; // Reset cooldown

            anim.SetTrigger("doRangeSlash");
            isAttacking = true;

            // Delay the slash spawn until frame 30 (0.5 seconds)
            Invoke(nameof(SpawnRangedSlash), 30f / 60f);

            // Disable isAttacking at frame 50 (0.83 seconds)
            Invoke(nameof(ResetAttackState), 50f / 60f);
            Invoke(nameof(StartAttackRecovery), 50f / 60f);
        }
    }

    // Spawns the ranged slash
    private void SpawnRangedSlash()
    {
        if (rangedSlashPrefab == null)
        {
            Debug.LogError("Ranged slash prefab is not assigned!");
            return;
        }

        // Instantiate the ranged slash at the attack point
        GameObject slash = Instantiate(rangedSlashPrefab, attackPoint.position, Quaternion.identity);

        // Get the RangedSlash script from the prefab
        Projectile slashScript = slash.GetComponent<Projectile>();

        // If the slash has a script, initialize it to move toward the player
        if (slashScript != null)
        {
            slashScript.Initialize(player.transform.position, rangedDamage);
        }
    }

    private void StartAttackRecovery()
    {
        Invoke(nameof(EndAttackRecovery), moveCooldown); // Delay before boss can move again
    }

    private void EndAttackRecovery()
    {
        isInactive = false;
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

    private void EnableSlashHitbox2()
    {
        slashHitbox2.SetActive(true);
    }

    private void DisableSlashHitbox2()
    {
        slashHitbox2.SetActive(false);
    }


    private void TeleportToPlayer()
    {
        // First, determine if player is grounded or in the air
        bool playerIsGrounded = player.IsGrounded();

        // Reference position - we'll use actual player position if grounded,
        // or projected ground position if player is in the air
        Vector3 referencePosition;

        if (!playerIsGrounded)
        {
            // Find the ground position below the player
            RaycastHit2D groundHit = Physics2D.Raycast(player.transform.position, Vector2.down, 100f, LayerMask.GetMask("Ground"));
            if (groundHit.collider != null)
            {
                // Use the ground position as reference instead of player's air position
                referencePosition = new Vector3(player.transform.position.x, groundHit.point.y + 0.5f, 0);
                Debug.Log("Player is in air, using ground position as reference: " + referencePosition);
            }
            else
            {
                // If no ground found below player, use current player position as fallback
                referencePosition = player.transform.position;
                Debug.LogWarning("Player is in air but no ground found below, using current position");
            }
        }
        else
        {
            // Player is already on ground, use their position
            referencePosition = player.transform.position;
        }

        // Determine the left and right positions relative to the reference position
        Vector3 leftPosition = new Vector3(referencePosition.x - teleportOffset, referencePosition.y, 0);
        Vector3 rightPosition = new Vector3(referencePosition.x + teleportOffset, referencePosition.y, 0);

        // Check for obstacles/borders on both sides
        bool isLeftBlocked = Physics2D.Raycast(referencePosition, Vector2.left, teleportOffset, LayerMask.GetMask("Ground"));
        bool isRightBlocked = Physics2D.Raycast(referencePosition, Vector2.right, teleportOffset, LayerMask.GetMask("Ground"));

        // Check for platform edges (or lack of ground) on both sides
        // We cast downward from each potential teleport position to see if there's ground beneath
        bool isLeftSafe = Physics2D.Raycast(leftPosition + Vector3.up * 0.5f, Vector2.down, 2f, LayerMask.GetMask("Ground"));
        bool isRightSafe = Physics2D.Raycast(rightPosition + Vector3.up * 0.5f, Vector2.down, 2f, LayerMask.GetMask("Ground"));

        // Define the valid teleport positions based on all constraints
        List<Vector3> validPositions = new List<Vector3>();

        // Only add positions that are not blocked by borders and have ground beneath
        if (!isLeftBlocked && isLeftSafe)
        {
            // Find the exact ground height for the left position
            RaycastHit2D leftGroundHit = Physics2D.Raycast(leftPosition + Vector3.up * 0.5f, Vector2.down, 2f, LayerMask.GetMask("Ground"));
            if (leftGroundHit.collider != null)
            {
                // Adjust position to be slightly above ground
                Vector3 safeLeftPos = new Vector3(leftPosition.x, leftGroundHit.point.y + 0.5f, 0);
                validPositions.Add(safeLeftPos);
            }
        }

        if (!isRightBlocked && isRightSafe)
        {
            // Find the exact ground height for the right position
            RaycastHit2D rightGroundHit = Physics2D.Raycast(rightPosition + Vector3.up * 0.5f, Vector2.down, 2f, LayerMask.GetMask("Ground"));
            if (rightGroundHit.collider != null)
            {
                // Adjust position to be slightly above ground
                Vector3 safeRightPos = new Vector3(rightPosition.x, rightGroundHit.point.y + 0.5f, 0);
                validPositions.Add(safeRightPos);
            }
        }

        // Decide where to teleport
        Vector3 targetPosition;

        if (validPositions.Count > 0)
        {
            // If multiple positions are valid, choose one at random
            targetPosition = validPositions[Random.Range(0, validPositions.Count)];
            Debug.Log("Teleporting to " + (targetPosition.x < referencePosition.x ? "left" : "right") + " side of player");
        }
        else
        {
            // If no valid positions found, try teleporting to the player's current platform
            RaycastHit2D playerGroundHit = Physics2D.Raycast(referencePosition + Vector3.up * 0.5f, Vector2.down, 2f, LayerMask.GetMask("Ground"));
            if (playerGroundHit.collider != null)
            {
                targetPosition = new Vector3(referencePosition.x, playerGroundHit.point.y + 0.5f, 0);
                Debug.Log("No valid side positions found, teleporting to player's current platform");
            }
            else
            {
                // Last resort: teleport to player's position
                targetPosition = referencePosition;
                Debug.LogWarning("No safe ground positions found anywhere, teleporting to reference position");
            }
        }

        // Perform the teleportation
        transform.position = targetPosition;

        // Reset flags after teleporting
        Invoke(nameof(StopTeleporting), 0.1f);
    }

    public void StartTeleportWithWindup()
    {
        if (!isTeleporting)
        {
            isTeleporting = true;
            anim.SetTrigger("StartTeleportWindup"); // Trigger windup animation if necessary
            StartCoroutine(TeleportationWindup());
        }
    }

    private IEnumerator TeleportationWindup()
    {
        // Wait for the windup duration
        yield return new WaitForSeconds(teleportWindupTime);

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
}
