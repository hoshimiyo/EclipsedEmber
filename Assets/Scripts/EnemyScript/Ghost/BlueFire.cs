using UnityEngine;

public class BlueFire : Projectile
{
    [SerializeField] private GameObject explosionPrefab;
    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Vector3 targetPosition, float dmg)
    {
        base.Initialize(targetPosition, dmg);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            speed = 0;
            PlayerMovement.instance.TakeDamage(damage);
            GetComponent<Collider2D>().enabled = false;
            Debug.Log("Player got hit for " + damage);
            Destroy(gameObject);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 16f / 60f);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            speed = 0;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 16f / 60f);
        }
    }
}