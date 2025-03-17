using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 1f; // Adjust based on enemy settings
    [SerializeField] private float damageDuration = 0.5f; // Time window to apply damage
    private bool hasDamaged = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasDamaged)
        {
            Debug.Log("Player got hit for " + damage);
            PlayerMovement.instance.TakeDamage(damage);
            hasDamaged = true;

            // Optionally, disable the hitbox collider or destroy it after dealing damage
            // Disable the collider to prevent further triggers
            GetComponent<Collider2D>().enabled = false;

            // Destroy the hitbox object if you want it to be removed after the attack
            // Destroy(gameObject, damageDuration);

            // Optionally, you can reset the hitbox after a short delay if needed
            Invoke(nameof(ResetHitbox), damageDuration);
        }
    }

    private void ResetHitbox()
    {
        // Re-enable the collider and reset damage flag after a short delay
        GetComponent<Collider2D>().enabled = true;
        hasDamaged = false;
    }
}

