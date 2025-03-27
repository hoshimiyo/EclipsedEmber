using UnityEngine;

public class SoulBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1;
    [SerializeField] AudioClip hitSFX;

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
        if (_other.gameObject.tag == "Enemy")
        {
            BaseEnemy enemy = _other.GetComponent<BaseEnemy>();
            enemy.TakeDamage(damage);
            SFXManager.instance.PlaySFXClip(hitSFX, transform, 1);
        }
    }
}
