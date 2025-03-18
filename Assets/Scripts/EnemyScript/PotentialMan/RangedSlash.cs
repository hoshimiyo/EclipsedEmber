using UnityEngine;

public class RangedSlash : MonoBehaviour
{
    [SerializeField] private int damage = 1; // Damage dealt by the slash
    [SerializeField] private float lifetime = 3f; // Time before the slash is destroyed
    [SerializeField] private float timer = 0f; // Timer to track how long the slash has been in the air
    [SerializeField] private float initialSpeed = 6f; // Initial speed of the slash
    [SerializeField] private float maxSpeed = 10f; // Max speed of the slash
    [SerializeField] private float speedIncreaseDuration = 1f; // Time duration to reach max speed
    [SerializeField] private float damageDelayWindow = 0.5f; // Time window to apply damage
    private bool hasDamaged = false;

    private Vector2 direction;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = initialSpeed; // Set initial speed
    }

    public void Initialize(Vector3 targetPosition)
    {
        // Calculate direction towards the player
        direction = (targetPosition - transform.position).normalized;

        // Rotate the slash to face the movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy the slash after 'lifetime' seconds if it doesn't hit anything
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Gradually increase the speed over time
        if (timer < speedIncreaseDuration)
        {
            // Gradually increase speed using Lerp (you could use Mathf.Lerp, Mathf.SmoothStep or other interpolation functions)
            currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, timer / speedIncreaseDuration);
        }

        // Move the slash forward
        transform.position += (Vector3)direction * currentSpeed * Time.deltaTime;

        // Count down the timer to destroy the slash after a certain time (lifetime)
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);  // Destroy the slash after 'lifetime' seconds
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasDamaged)
        {
            Debug.Log("Player got hit for " + damage);
            PlayerStat.instance.TakeDamage(damage);
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

    private void ResetHitbox()
    {
        // Re-enable the collider and reset damage flag after a short delay
        GetComponent<Collider2D>().enabled = true;
        hasDamaged = false;
    }
}
