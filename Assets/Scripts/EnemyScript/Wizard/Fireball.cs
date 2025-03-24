using UnityEngine;

public class WizardFireball : Projectile
{
    [SerializeField] private GameObject explosionPrefab;
    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Vector3 targetPosition, int dmg)
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
            PlayerStat.instance.TakeDamage(damage);
            GetComponent<Collider2D>().enabled = false;
            Debug.Log("Player got hit for " + damage);
            Destroy(gameObject);
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 16f / 60f);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<Collider2D>().enabled = false;
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 16f / 60f);
            Destroy(gameObject);
        }
    }
}