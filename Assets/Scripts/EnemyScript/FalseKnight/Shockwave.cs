using TMPro;
using UnityEngine;

public class Shockwave : Projectile
{
    private PlayerMovement player;
    private FalseKnight knight;

    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        knight = FindAnyObjectByType<FalseKnight>();
    }

    // Initialize method where you get the direction of the boss
    public override void Initialize(Vector3 bossPosition, int dmg)
    {
        damage = dmg;

        // Set direction to the boss's facing direction
        direction = bossPosition.normalized;
        direction.y = 0; // Ensure it only moves horizontally

        // Flip based on the direction
        FlipBasedOnDirection();
    }

    protected override void Update()    
    {
        // Move the projectile forward in the direction the boss is facing
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Count down the timer to destroy the projectile after a certain time (lifetime)
        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifetime)
        {
            Destroy(gameObject);  // Destroy the projectile after 'lifetime' seconds
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStat.instance.TakeDamage(damage, gameObject);
            Debug.Log("Player got hit for " + damage);
        }
    }

    private void FlipBasedOnDirection()
    {
        if (direction.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
