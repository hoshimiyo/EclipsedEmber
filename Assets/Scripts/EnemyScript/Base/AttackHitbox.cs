using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private bool useDelayAttack;  // Flag to control if delay is used
    [SerializeField] private float damageStart;    // Start time for when damage can be applied
    [SerializeField] private float damageEnd;      // End time for when damage can be applied
    [SerializeField] private bool useLifetime;     // Flag to control if lifetime is used
    [SerializeField] private float lifeTime;       // Time before hitbox is destroyed
    [SerializeField] private int damage;           // Damage to apply
    [SerializeField] private float damageDelayTick; // Duration to disable hitbox after damage
    private bool hasDamaged = false;
    private Rigidbody2D rb;

    private HashSet<Collider2D> collidingPlayers = new HashSet<Collider2D>(); // Track players in the hitbox
    private float currentTime = 0f; // To track the time for damage window

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (useLifetime)
        {
            Invoke(nameof(DestroyHitbox), lifeTime); // Start countdown for self-destruction
        }
    }

    protected virtual void Update()
    {
        // Update current time, but only if the hitbox is active and a delay attack is used
        if (useDelayAttack)
        {
            currentTime += Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (rb != null)
            {
                rb.gravityScale = 0;
                rb.linearVelocity = Vector2.zero;  // Stop any existing movement
            }
        }

        if (other.CompareTag("Player"))
        {
            collidingPlayers.Add(other); // Track player inside the hitbox

            // Check if the player enters the hitbox within the valid damage window
            if (useDelayAttack)
            {
                if (currentTime >= damageStart && currentTime <= damageEnd && !hasDamaged)
                {
                    ApplyDamage(other);
                }
            }
            else
            {
                // If no delay, apply damage immediately
                ApplyDamage(other);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collidingPlayers.Remove(other); // Remove player when they leave
        }
    }

    private IEnumerator DelayedDamage(Collider2D player)
    {
        yield return new WaitForSeconds(damageStart); // Wait before applying damage

        // Check if player is still in the hitbox and damage hasn�t been applied yet
        if (collidingPlayers.Contains(player) && !hasDamaged)
        {
            ApplyDamage(player);
        }
    }

    protected virtual void ApplyDamage(Collider2D player)
    {
        if (hasDamaged) return;

        Debug.Log("Player got hit for " + damage);
        PlayerStat.instance.TakeDamage(damage, gameObject);
        hasDamaged = true;

        Invoke(nameof(ResetHitbox), damageDelayTick);
    }

    protected virtual void DestroyHitbox()
    {
        Destroy(gameObject);
    }

    protected virtual void ResetHitbox()
    {
        hasDamaged = false;
        currentTime = 0f;  // Reset time for the next attack cycle
    }
}
