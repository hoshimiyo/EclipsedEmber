using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;
    private SpriteRenderer spriteRenderer;

    #region Awake, Update 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _playerAnim = GetComponent<PlayerAnimation>();
        }
        else
        {
            Destroy(gameObject);
        }
        Health = healthCap;
    }

    private void Start()
    {
        Mana = mana;
    }
    private void Update()
    {
        AttackInput();
        Heal();
        if (healing) return;
    }

    private void FixedUpdate()
    {
        Attack();
    }
    #endregion

    #region Attack
    [Space]
    [Header("Attack")]
    public static int damage = 1;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float timeBetweenAttack, timeSinceAttack;
    [SerializeField] private Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 sideAttackSize, upAttackSize, downAttackSize;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private GameObject slashEffect;
    private PlayerAnimation _playerAnim;
    private GameObject _slashEffectInstance;

    #region AttackFunction
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackSize);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackSize);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackSize);
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);
        if (objectsToHit.Length > 0)
        {
            // _recoilDir = true;
            Mana += manaGain;
            Debug.Log("Hit");
        }
        // for (int i = 0; i < objectsToHit.Length; i++)
        // {
        //     Enemy e = objectsToHit[i].GetComponent<Enemy>();
        //     if (e && !hitEnemies.Contains(e))
        //     {
        //         e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
        //         hitEnemies.Add(e);

        //         if (objectsToHit[i].CompareTag("Enemy"))
        //         {
        //             Mana += manaGain;
        //         }
        //     }
        // }
    }
    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (isAttacking && timeSinceAttack >= timeBetweenAttack)
        {
            // Attack logic
            timeSinceAttack = 0;

            if (PlayerMovement.instance.yRaw == 0 || PlayerMovement.instance.yRaw < 0 && PlayerMovement.instance.IsGrounded())
            {
                Hit(sideAttackTransform, sideAttackSize);
                if (!PlayerMovement.instance._isFacingRight)
                {
                    _playerAnim.TriggerAttack();
                    Instantiate(slashEffect, sideAttackTransform.position, Quaternion.Euler(0, 0, 180));
                }
                else
                {
                    _playerAnim.TriggerAttack();
                    Instantiate(slashEffect, sideAttackTransform.position, Quaternion.identity);
                }
            }
            else if (PlayerMovement.instance.yRaw > 0)
            {
                Hit(upAttackTransform, upAttackSize);
                CreateSlashEffect(slashEffect, 80, upAttackTransform);
            }
            else if (PlayerMovement.instance.yRaw < 0 && !PlayerMovement.instance.IsGrounded())
            {
                Hit(downAttackTransform, downAttackSize);
                CreateSlashEffect(slashEffect, -90, downAttackTransform);
            }
        }
    }

    private GameObject CreateSlashEffect(GameObject slashEffectPrefab, int effectAngle, Transform attackTransform)
    {
        _slashEffectInstance = Instantiate(slashEffectPrefab, attackTransform.position, Quaternion.identity);
        _slashEffectInstance.transform.rotation = Quaternion.Euler(0, 0, effectAngle);
        _slashEffectInstance.transform.localScale = attackTransform.localScale;
        _playerAnim.TriggerAttack();
        return _slashEffectInstance;
    }

    private void AttackInput()
    {
        isAttacking = Input.GetMouseButtonDown(0);
    }
    #endregion
    #endregion

    #region Health
    [Header("Health Settings")]
    public static int currentHealth = 3;
    public static int healthCap = 3;
    public static bool healing = false;
    float healTimer;
    [SerializeField] float timeToHeal;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    public int Health
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = Mathf.Clamp(value, 0, healthCap);


                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (iFrame) return;

        Health -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            PlayerMovement.instance.Die();
        }
        StartCoroutine(FlashSprite()); // Flash sprite on damage
        StartCoroutine(InvincibilityFrame());
    }

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < healthCap && Mana > 0 && PlayerMovement.instance.IsGrounded() && !PlayerMovement.instance._isDashing)
        {
            healing = true;
            _playerAnim.Healing(healing);
            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
            //drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            healing = false;
            _playerAnim.Healing(healing);
            healTimer = 0;
        }
    }
    #endregion

    #region Mana
    [Header("Mana Settings")]
    [SerializeField] public float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
            }
        }
    }

    #endregion

    #region IFrame
    // Invincibility frames (iFrame)
    public static bool iFrame = false;
    public static float iFrameDuration = 2f;
    #endregion

    private IEnumerator FlashSprite()
    {
        for (int i = 0; i < 3; i++) // Flash 3 times
        {
            spriteRenderer.color = Color.red; // Change to red
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white; // Change back to normal
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator InvincibilityFrame()
    {
        iFrame = true;
        yield return new WaitForSeconds(iFrameDuration);
        iFrame = false;
    }

}
