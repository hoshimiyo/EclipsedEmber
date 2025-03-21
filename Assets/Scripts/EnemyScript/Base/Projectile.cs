using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected float damage;
    [SerializeField] protected float maxLifetime = 3f; // Time before the projectile is destroyed
    [SerializeField] protected float lifeTime = 0f; // Timer to track how long the projectile has been in the air
    [SerializeField] protected float speedIncreaseDuration = 0.2f; // Time duration to reach max speed
    [SerializeField] protected float damageDelayWindow = 20f; // Delay for player to take damage again on prolong contact
    protected bool hasDamaged = false;
    protected Animator anim;

    protected Vector2 direction;
    [SerializeField] protected float speed;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void Initialize(Vector3 targetPosition, float dmg)
    {
        damage = dmg;
        // Calculate direction towards the player
        direction = (targetPosition - transform.position).normalized;

        // Rotate the projectile to face the movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy the projectile after 'lifetime' seconds if it doesn't hit anything
        Destroy(gameObject, maxLifetime);
    }

    protected virtual void Update()
    {

        // Move the projectile forward
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Count down the timer to destroy the projectile after a certain time (lifetime)
        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifetime)
        {
            Destroy(gameObject);  // Destroy the projectile after 'lifetime' seconds
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
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
            Invoke(nameof(ResetHitbox), damageDelayWindow);
        }
    }

    protected virtual void ResetHitbox()
    {
        // Re-enable the collider and reset damage flag after a short delay
        GetComponent<Collider2D>().enabled = true;
        hasDamaged = false;
    }
}
