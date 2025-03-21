using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private bool useDelayAttack;  // Flag to control if delay is used
    [SerializeField] private float delayTime;        // Time before damage is applied
    [SerializeField] private bool useLifetime;      // Flag to control if lifetime is used
    [SerializeField] private float lifeTime;        // Time before hitbox is destroyed
    [SerializeField] private float damage;          // Damage to apply
    [SerializeField] private float damageDuration;   // Duration to disable hitbox after damage
    private bool hasDamaged = false;

    private HashSet<Collider2D> collidingPlayers = new HashSet<Collider2D>(); // Track players in the hitbox

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collidingPlayers.Add(other); // Track player inside the hitbox

            if (useDelayAttack)
            {
                StartCoroutine(DelayedDamage(other)); // Start delay before damage if useDelayAttack is true
            }
            else
            {
                // If no delay, apply damage immediately
                ApplyDamage(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collidingPlayers.Remove(other); // Remove player when they leave
        }
    }

    private IEnumerator DelayedDamage(Collider2D player)
    {
        yield return new WaitForSeconds(delayTime); // Wait before applying damage

        // Check if player is still in the hitbox and damage hasn’t been applied yet
        if (collidingPlayers.Contains(player) && !hasDamaged)
        {
            ApplyDamage(player);
        }
    }

    private void ApplyDamage(Collider2D player)
    {
        if (hasDamaged) return;

        Debug.Log("Player got hit for " + damage);
        PlayerMovement.instance.TakeDamage(damage);
        hasDamaged = true;

        GetComponent<Collider2D>().enabled = false; // Disable hitbox
        Invoke(nameof(ResetHitbox), damageDuration);

        if (useLifetime)
        {
            Invoke(nameof(DestroyHitbox), lifeTime);
        }
    }

    private void DestroyHitbox()
    {
        Destroy(gameObject);
    }

    private void ResetHitbox()
    {
        GetComponent<Collider2D>().enabled = true;
        hasDamaged = false;
    }
}
