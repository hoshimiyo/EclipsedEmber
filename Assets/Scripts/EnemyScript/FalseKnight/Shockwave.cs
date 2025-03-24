using TMPro;
using UnityEngine;

public class Shockwave : Projectile
{
    private PlayerMovement player;
    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>();
    }

    public override void Initialize(Vector3 spawnPosition, int dmg)
    {
        FlipTowardsPlayer();
        damage = dmg;
        // Calculate direction towards the player
        direction = (player.transform.position - spawnPosition).normalized;
        direction.y = 0;

    }

    protected override void Update()
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

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStat.instance.TakeDamage(damage);
            Debug.Log("Player got hit for " + damage);
        }
    }

    protected void FlipTowardsPlayer()
    {
        // Flip the enemy's sprite based on the player's position relative to the enemy
        if (player.transform.position.x > transform.position.x)
        {
            // Player is to the right of the mob, face right
            if (transform.localScale.x < 0) // Already facing left, so flip
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (player.transform.position.x < transform.position.x)
        {
            // Player is to the left of the mob, face left
            if (transform.localScale.x > 0) // Already facing right, so flip
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}