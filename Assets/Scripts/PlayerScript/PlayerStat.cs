
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Mana = mana;
        Health = healthCap;
    }
    private void Update()
    {
        if (PlayerMovement.instance.active == false)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        Heal();
        HandleRecoil();
        if (healing) return;
    }


    #endregion

    #region Attack
    [Space]
    [Header("Attack")]
    public static int damage = 1;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float timeBetweenAttack, timeSinceAttack;
    [SerializeField] public Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 sideAttackSize, upAttackSize, downAttackSize;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private float attackRate;
    float nextAttackTime = 0f;

    [SerializeField] private AudioClip attackSoundClip;
    [SerializeField] private AudioClip healSFX;
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
        bool hitEnemy = false;

        if (objectsToHit.Length > 0)
        {
            // _recoilDir = true;
            Mana += manaGain;
            PlaySFXClip(hitSFX);
            hitEnemy = true;
            Debug.Log(hitEnemy);
        }
        else PlaySFXClip(attackSoundClip);

        // Apply recoil if an enemy was hit
        if (hitEnemy)
        {
            ApplyRecoil(objectsToHit[0].transform.position);
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            BaseEnemy enemy = objectsToHit[i].GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }


    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / attackRate;
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

    [SerializeField] private AudioClip _takeDamageSound;
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
        SFXManager.instance.PlaySFXClip(_takeDamageSound, GameManager.instance.transform, 1f);
        Health -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            PlayerMovement.instance.Die();
        }
        StartCoroutine(BlinkRedEffect()); // Flash sprite on damage
        StartCoroutine(InvincibilityFrame(iFrameDuration));
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
                PlaySFXClip(healSFX);
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
    public static float iFrameDuration = 3f;
    #endregion

    private IEnumerator BlinkRedEffect()
    {
        float elasped = 0f;
        while (elasped < iFrameDuration)
        {
            spriteRenderer.color = Color.red; // Change to red
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white; // Change back to normal
            yield return new WaitForSeconds(0.1f);
            elasped += 0.2f;
        }
    }

    public static IEnumerator InvincibilityFrame(float iFrameDuration)
    {
        iFrame = true;
        yield return new WaitForSeconds(iFrameDuration);
        iFrame = false;
    }

    #region Misc
    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }
    #endregion

    #region Recoil
    [Header("Recoil Settings")]
    public float recoilForce;       // How strong the pushback is
    public float verticalRecoilForce = 15f;
    public float horizontalRecoilForce = 5f;
    public float recoilDuration;  // How long recoil lasts
    public float verticalRecoilDuration = 0.3f;
    public float horizontalRecoilDuration = 0.2f;
    bool isVerticalRecoil;
    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    private Vector2 recoilDirection;

    public void ApplyRecoil(Vector2 enemyPosition)
    {
        // Calculate direction (player -> enemy) and reverse it for pushback
        recoilDirection = (transform.position - (Vector3)enemyPosition).normalized;

        

        isVerticalRecoil = Mathf.Abs(recoilDirection.y) > 0.7f; // 0.7 = ~45-degree angle
        if(transform.position.y <= enemyPosition.y) isVerticalRecoil = false;

        recoilForce = isVerticalRecoil ? verticalRecoilForce : horizontalRecoilForce;

        isRecoiling = true;
        recoilTimer = 0.2f;
    }

    private void HandleRecoil()
    {
        if (isRecoiling)
        {
            recoilTimer -= Time.deltaTime;
            if (recoilTimer > 0)
            {
                // Apply continuous force (alternatively use Rigidbody.AddForce)
                transform.Translate(recoilDirection * recoilForce * Time.deltaTime, Space.World);
            }
            else
            {
                isRecoiling = false;
            }
        }
    }
    #endregion
}