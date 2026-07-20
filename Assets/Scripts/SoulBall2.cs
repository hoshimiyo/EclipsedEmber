using UnityEngine;

public class SoulBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] GameObject hitVFX;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.right;
    }
    //detect hit
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = _other.GetComponent<BaseEnemy>();
            enemy.TakeDamage(damage);
            Instantiate(hitVFX, transform.position, Quaternion.identity);
            SFXManager.instance.PlaySFXClip(hitSFX, transform, 1);
        }

        if (_other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject);
        }
    }
}
