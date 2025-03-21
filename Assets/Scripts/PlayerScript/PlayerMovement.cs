using System;
using System.Collections;
using NUnit;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    #region Fundementals
    [Header("Fundementals")]
    public static PlayerMovement instance;
    [SerializeField] private Collider2D _groundCollider;
    [SerializeField] public bool _isFacingRight = true;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _maxFallSpeed = -20f;
    [SerializeField] private float _maxFallSpeedMultiflier = 1.5f;

    [SerializeField, Range(0.5f, 2f)] private float _accelerationValue = 1.2f;
    [SerializeField, Range(0.5f, 2f)] private float _decelerationValue = 1.2f;
    [SerializeField, Range(0f, 1f)] private float _frictionAmountValue = 0.5f;
    private float _acceleration;
    private float _deceleration;
    private float _frictionAmount;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    public float timeScale = .9f;
    public bool active;
    public Vector2 _respawnPoint;
    public bool isWalled; //consider removing
    [SerializeField, Range(0f, 2f)] private float _iceDeceleration = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _iceFriction = 0.4f;


    #endregion

    #region Attack
    [Space]
    [Header("Attack")]
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float timeBetweenAttack, timeSinceAttack;
    [SerializeField] Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] Vector2 sideAttackSize, upAttackSize, downAttackSize;
    [SerializeField] LayerMask attackLayer;
    [SerializeField] float damage = 1;

    #endregion

    #region Dash
    [Space]
    [Header("Dash")]
    [SerializeField] private float _dashingPower = 24f;
    public bool _canDash;
    [SerializeField] private float _dashingTime = 0.2f;
    private const float _dashNormalizer = 0.707f;
    [SerializeField] private bool _freezeFrame = true;
    public bool _isDashing;
    private Vector2 _dashDirection;
    private bool _dashButtonPressed;
    #endregion
    #region Jump
    [Space]
    [Header("Jump")]
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _coyoteTime = 0.1f;
    [SerializeField] private float _jumpBufferTime = 0.05f;
    private float _jumpBufferTimeCounter;
    private float _coyoteTimeCounter;
    private float _fallMultiplier = 7f;
    [SerializeField] private float _jumpVelocityFallOff = 8f;
    [SerializeField] private int _extraJump = 1;
    public int _availableJump;
    private bool _jumpButtonPressed;
    #endregion
    #region Misc
    [Space]
    #region Wall Tech
    [Space]
    [Header("Wall Tech")]
    [SerializeField] private float _wallSlidingSpeedMultiplier;
    [SerializeField] private float _wallJumpingCoyoteTime = 0.05f;
    private bool _isWallSliding;
    private bool _isWallJumping;
    private float _wallJumpingCounter;
    private float _wallJumpingDirection;
    [SerializeField] private float _wallJumpingLerp = 10f;
    #endregion
    #region Collisions
    [Space]
    [Header("Collisions")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _iceLayer;
    [SerializeField] private Transform _wallCheckRight;
    [SerializeField] private Transform _wallCheckLeft;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _killable;
    [SerializeField] private LayerMask _damagable;
    [SerializeField] private Transform _cornerCheckLeft;
    [SerializeField] private Transform _cornerCheckRight;
    #endregion
    [Header("Misc")]

    [SerializeField] private TrailRenderer tr;
    private float _freezeDuration = 0.05f;
    private bool _isFrozen;
    private bool _wasGrounded;
    [SerializeField] private ParticleSystem slideParticle;
    [SerializeField] private ParticleSystem groundParticle;
    [SerializeField] private ParticleSystem dashParticle;
    // public FlashEffect flashEffect;
    private float _dashDustSpeed = 5f;
    public float xRaw, yRaw, x;
    public bool _hasDoubleJumped;
    private Renderer playerRenderer;  // To store the renderer component
    private Color originalColor;  // To store the original color of the mob
    #endregion
    #region Camera
    [Space]
    [Header("Camera")]
    // public ShockwaveManager _shockwaveManager;
    // public CameraShake cameraShake;
    private CinemachineImpulseSource _impulseSource;
    #endregion
    #region Colors
    [Space]
    [Header("Colors")]
    [ColorUsage(true, true)]
    [SerializeField] private Color _refillColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _dashColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _doubleJumpColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _deathColor;
    #endregion
    #region Audio
    [Space]
    [Header("Audio")]
    [SerializeField] private AudioClip deathSoundClip;
    [SerializeField] private AudioClip damageSoundClip;
    [SerializeField] private AudioClip jumpSoundClip;
    [SerializeField] private AudioClip[] landSoundClips;
    [SerializeField] private AudioClip[] runSoundClips;
    [SerializeField] private AudioClip[] wallJumpSoundClips;
    [SerializeField] private AudioClip dashSoundClip;
    [SerializeField] private AudioClip attackSoundClip;
    #endregion
    #region Amimator
    [Space]
    [Header("Animator")]
    public PlayerAnimation _playerAnim;
    // [SerializeField] private Deformation _jumpDeformation;
    // [SerializeField] private Deformation _landDeformation;
    private bool isSoundCoroutineRunning = false;
    #endregion

    #endregion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _collider = GetComponent<Collider2D>();
        _playerAnim = GetComponent<PlayerAnimation>();
        active = true;
        SetRespawnPoint(transform.position);
        Time.timeScale = timeScale;
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
        if (instance == null)
        {
            instance = this; // Assign the singleton instance
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }
    private void Update() //update sẽ chạy mỗi frame
    {
        if (_isDashing || !active)
        {
            return;
        }

        xRaw = Input.GetAxisRaw("Horizontal"); // -1 0 1
        yRaw = Input.GetAxisRaw("Vertical");   // -1 0 1
        x = Input.GetAxis("Horizontal");       //controller, joystick, analog control => slide từ -1 => 1 e.g: -0.323

        if (_playerAnim != null)
        {
            bool isJumping = !IsGrounded() && _rb.linearVelocityY > 0;
            _playerAnim.UpdateAnimation(_rb.linearVelocityX, _rb.linearVelocityY, IsGrounded(), _isWallSliding, isJumping);
        }

        JumpInput();
        DashInput();

    }
    private void FixedUpdate() //update mỗi số frame (2-3-4 frame) ít độc lập frame hơn => ít responsive hơn
    {
        if (_isDashing)
        {
            return;
        }
        HorizontalMovement();

        Jump();

        Dash();

        if (!_isWallJumping)
            Flip();
        SlipperyFloor();

        WallSlide();

        WallJump();

        WallDust();
        _wasGrounded = IsGrounded();



    }


    #region Collision Check
    public bool IsGrounded() => Physics2D.OverlapCircle(_groundCheck.position, 0.15f, _groundLayer);
    private bool IsOnIce() => Physics2D.OverlapCircle(_groundCheck.position, 0.15f, _iceLayer);
    private bool IsWalled() => Physics2D.OverlapCircle(_wallCheckRight.position, 0.1f, _wallLayer);
    private bool IsWalledLeft() => Physics2D.OverlapCircle(_wallCheckLeft.position, 0.1f, _wallLayer);

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Killable"))
        {
            Die();
        }
        else if (other.gameObject.CompareTag("Damagable"))
        {
            TakingDamage();
        }
        else if (other.gameObject.CompareTag("EnvironmentDamage"))
        {
            TakingEnvironmentalDamage(other.gameObject);
        }
    }

    #endregion
    #region Basic Movement
    private void HorizontalMovement()
    {
        float targetSpeed = xRaw * _speed;
        float speedDif = targetSpeed - _rb.linearVelocityX;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? (IsGrounded() ? _acceleration : _acceleration * 0.5f) : _deceleration;
        float movement = (float)(Math.Pow(Mathf.Abs(speedDif) * accelRate, 2f) * Mathf.Sign(speedDif));
        float frictionAmount = _frictionAmount;

        bool isWallJumpingAndAirborne = _isWallJumping && !IsGrounded();

        if (PlayerStat.healing)
            _rb.linearVelocity = new Vector2(0, 0);

        if (!_isWallJumping && !IsGrounded())
        {
            frictionAmount = _frictionAmount * 0.1f;
            _rb.AddForce(movement * Vector2.right);
        }
        else if (isWallJumpingAndAirborne)
        {
            frictionAmount = _frictionAmount * 0.1f;
            Vector2 targetVelocity = new Vector2(x * _speed, _rb.linearVelocityY);
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, _wallJumpingLerp * Time.fixedDeltaTime);
        }

        if (xRaw == 0)
        {
            float deccelAmount = Mathf.Min(Mathf.Abs(_rb.linearVelocityX), Mathf.Abs(frictionAmount));
            deccelAmount *= Mathf.Sign(_rb.linearVelocityX);
            _rb.AddForce(Vector2.right * -deccelAmount, ForceMode2D.Impulse);
        }

        // Apply movement force only if not wall jumping
        if (!isWallJumpingAndAirborne)
        {
            _rb.AddForce(movement * Vector2.right);
        }
        //land animation, sfx, effects and more
        if (!_wasGrounded && IsGrounded() && active)
        {
            PlayRandomSFXClip(landSoundClips);
            GroundDust();
            // if (_landDeformation != null) _landDeformation.PlayDeformation();
            // if (!_canDash || _availableJump <= 0)
            // if (flashEffect != null)
            //     flashEffect.CallFlash(0.5f, 0.1f, _refillColor);


        }
        if (Mathf.Abs(_rb.linearVelocityX) > 0.1f && IsGrounded() && !isSoundCoroutineRunning)
        {
            StartCoroutine(GroundEffect());
        }
    }

    private IEnumerator GroundEffect()
    {
        isSoundCoroutineRunning = true;
        //GroundDust();
        PlayRandomSFXClip(runSoundClips);
        yield return new WaitForSeconds(1f / 3f);
        isSoundCoroutineRunning = false;
    }

    private void JumpInput()
    {
        _jumpButtonPressed = Input.GetButton("Jump");
        
    }
    private void Jump()
    {
        //initialize jump buffering and coyote time
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
            //reset double jump when player is grounded
            _availableJump = _extraJump;
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        //also activate buffer is player is dashing
        if (_jumpButtonPressed)
        {
            _jumpBufferTimeCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimeCounter -= Time.fixedDeltaTime;
        }
        //starts ground jump
        if (_jumpBufferTimeCounter > 0f && _coyoteTimeCounter > 0f)
        {
            PlaySFXClip(jumpSoundClip);
            GroundDust();
            // if (_jumpDeformation != null) _jumpDeformation.PlayDeformation();
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);

            _jumpBufferTimeCounter = 0f;
            //if player touches wall, use wall jump instead 
            if (!IsWalled() && _coyoteTimeCounter <= 0f)
            {

                _availableJump--;
            }
            _coyoteTimeCounter = 0f;
            _jumpButtonPressed = false;
        }
        //double jump condition
        else if (_jumpButtonPressed && _coyoteTimeCounter <= 0f && _availableJump > 0 && canDoubleJump && _wallJumpingCounter <= 0f)
        {
            PlaySFXClip(jumpSoundClip);
            GroundDust();
            // if (_jumpDeformation != null) _jumpDeformation.PlayDeformation();
            // if (flashEffect != null) flashEffect.CallFlash(1f, 0.1f, _doubleJumpColor);
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            _hasDoubleJumped = true;
            if (!IsWalled())
            {
                _availableJump--;
            }
            _jumpButtonPressed = false;
        }
        //limit fall speed
        if (_rb.linearVelocityY < FallSpeed())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, FallSpeed());
        }
        //apply downward force for snappier jump feeling
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (!Input.GetButton("Jump") || _rb.linearVelocityY > _jumpVelocityFallOff)
        {
            _rb.linearVelocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.fixedDeltaTime;
        }
    }
    private float FallSpeed() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? _maxFallSpeed * _maxFallSpeedMultiflier : _maxFallSpeed;
    //flip player's entire model horizontally when moving opposite direction

    private void Flip()
    {
        if (_isFacingRight && xRaw < 0f || !_isFacingRight && xRaw > 0f)
        {
            Vector3 localScale = transform.localScale;
            _isFacingRight = !_isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    #endregion

    #region Attack

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);
        if (objectsToHit.Length > 0)
        {
            Debug.Log("Hit " + objectsToHit[0].name);
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
        timeSinceAttack += Time.deltaTime;
        if (isAttacking && timeSinceAttack >= timeBetweenAttack)
        {
            // Attack logic
            timeSinceAttack = 0;
            PlaySFXClip(attackSoundClip);
            if (yRaw == 0 || yRaw < 0 && IsGrounded())
            {
                Hit(sideAttackTransform, sideAttackSize);
            }
            else if (yRaw > 0)
            {
                Hit(upAttackTransform, upAttackSize);
            }
            else if (yRaw < 0 && !IsGrounded())
            {
                Hit(downAttackTransform, downAttackSize);
            }
        }
    }
    private void AttackInput()
    {
        isAttacking = Input.GetMouseButtonDown(0);
    }
    #endregion

    #region Dash
    private void DashInput()
    {
        //if (Input.GetButtonDown("Dash")) _dashButtonPressed = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            _dashButtonPressed = true;
        }
    }

    private void Dash()
    {
        //dash is refilled when player touches ground
        if (IsGrounded())
        {
            _canDash = true;
        }
        if (_dashButtonPressed && _canDash)
        {
            StartCoroutine(ExecuteDash());
            _dashButtonPressed = false;
        }
    }
    private IEnumerator ExecuteDash()
    {
        PlaySFXClip(dashSoundClip);
        if (_freezeFrame)
        {
            _isFrozen = true;
            var original = Time.timeScale;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(_freezeDuration);
            Time.timeScale = original;
            _isFrozen = false;
        }

        // if (_shockwaveManager != null) _shockwaveManager.CallShockwave();
        // if (flashEffect != null) flashEffect.CallFlash(1f, 0.3f, _dashColor);
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        //disable gravity for total straight dash movement
        _rb.gravityScale = 0f;

        _dashDirection = new Vector2(xRaw, yRaw).normalized * _dashingPower;
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = new Vector2(transform.localScale.x, 0).normalized * _dashingPower;
        }
        _rb.linearVelocity = _dashDirection;

        float yRec = yRaw;
        // //draw trial
        if (tr != null) tr.emitting = true;
        DashDust(DashDustDirectionX(), yRaw);


        yield return new WaitForSeconds(_dashingTime * _dashNormalizer);
        if (tr != null) tr.emitting = false;
        if (yRec > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0f);
        }
        _rb.gravityScale = originalGravity;
        //rb.linearVelocity = new Vector2(0f, 0f);
        _isDashing = false;
    }
    #endregion
    #region Wall Tech
    private void WallSlide()
    {
        if ((IsWalled() || IsWalledLeft()) && !IsGrounded() && xRaw != 0 && _rb.linearVelocityY <= 0)
        {
            _rb.linearVelocityY *= _wallSlidingSpeedMultiplier;
            _isWallSliding = true;
            if (IsWalled())
            {
                Vector3 localScale = transform.localScale;
                _isFacingRight = !_isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            else if (IsWalledLeft())
            {

            }
        }
        else
        {
            _isWallSliding = false;
        }
    }
    private void WallJump()
    {

        if ((IsWalled() && !IsGrounded()) || (IsWalledLeft() && !IsGrounded()))
        {
            if (Mathf.Abs(_rb.linearVelocityX) <= 0.001f)
                _isWallJumping = false;
            if (IsWalled())
            {
                _wallJumpingDirection = -transform.localScale.x;
            }
            else
            {
                _wallJumpingDirection = transform.localScale.x;

                //_isFacingRight = !_isFacingRight;
            }
            _wallJumpingCounter = _wallJumpingCoyoteTime;
            //CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            _wallJumpingCounter -= Time.fixedDeltaTime;
        }

        if (_jumpBufferTimeCounter > 0f && _wallJumpingCounter >= 0f)
        {
            _isWallJumping = true;
            PlayRandomSFXClip(wallJumpSoundClips);
            // if (_jumpDeformation != null) _jumpDeformation.PlayDeformation();
            _rb.linearVelocity = new Vector2(0f, 0f);
            Vector2 force = Vector2.right * _speed * 1.5f * _wallJumpingDirection + Vector2.up * _jumpSpeed;
            //DisableMovement(0.1f);
            _rb.linearVelocity += force;

            _wallJumpingCounter = 0f;
            _jumpBufferTimeCounter = 0f;
            if (_wallJumpingDirection != transform.localScale.x)
            {
                Vector3 localScale = transform.localScale;
                _isFacingRight = !_isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            //Invoke(nameof(StopWallJumping), 0.3f);

            _jumpButtonPressed = false;
        }
        if (IsGrounded())
        {
            _isWallJumping = false;
        }
    }

    #endregion
    #region Particles
    private void WallDust()
    {
        if (slideParticle == null) return;
        var main = slideParticle.main;
        if (_isWallSliding)
        {
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }
    private void GroundDust()
    {
        if (groundParticle == null) return;

        groundParticle.Play();
    }
    private void DashDust(float xRaw, float yRaw)
    {
        if (slideParticle == null) return;
        var velocityDir = dashParticle.velocityOverLifetime;
        if (xRaw != 0 && yRaw != 0)
        {
            velocityDir.x = new ParticleSystem.MinMaxCurve(xRaw * _dashDustSpeed * 0.5f * _dashNormalizer, xRaw * _dashDustSpeed * _dashNormalizer);
            velocityDir.y = new ParticleSystem.MinMaxCurve(yRaw * _dashDustSpeed * 0.5f * _dashNormalizer, yRaw * _dashDustSpeed * _dashNormalizer);
        }
        else
        {
            velocityDir.x = new ParticleSystem.MinMaxCurve(xRaw * _dashDustSpeed * 0.5f, xRaw * _dashDustSpeed);
            velocityDir.y = new ParticleSystem.MinMaxCurve(yRaw * _dashDustSpeed * 0.5f, yRaw * _dashDustSpeed);
        }
        dashParticle.Play();
    }
    private float DashDustDirectionX()
    {
        if (xRaw == 0 && yRaw != 0) return 0;
        return xRaw != 0 ? xRaw : _isFacingRight ? 1 : -1;
    }
    #endregion
    private IEnumerator ExecuteFreeze()
    {
        _isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(_freezeDuration);
        Time.timeScale = original;
        _isFrozen = false;
    }

    #region Damage
    public void Damage()
    {
        PlaySFXClip(damageSoundClip);
        active = false;
        _collider.enabled = false;
        if (_groundCollider != null) _groundCollider.GetComponent<Collider2D>().enabled = false;
        MiniJump();
        StartCoroutine(Disable(0.5f));
    }
    #endregion

    #region TakingDamage
    public void TakingDamage()
    {
        PlayerStat.instance.TakeDamage(1);
        PlaySFXClip(damageSoundClip);
        active = false;
        _collider.enabled = false;
        if (_groundCollider != null) _groundCollider.GetComponent<Collider2D>().enabled = false;
        MiniJump();
    }

    public void TakingEnvironmentalDamage(GameObject other)
    {
        TakingDamage();
        if (PlayerStat.instance.Health > 0)
        {
            StartCoroutine(DisablePhysics(0.5f));
            Transform child = other.transform.Find("RespawnPlatform");

            if (child != null)
            {
                // Get the position of the child
                Vector3 childPosition = child.position;
                StartCoroutine(EnvironmentalRespawn(childPosition));
            }
            else
            {
                Debug.LogError("Child not found!");
            }
        }
    }

    #endregion
    #region Death
    private void MiniJump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed / 2);
    }
    public void Die()
    {
        PlaySFXClip(deathSoundClip);
        active = false;
        _collider.enabled = false;
        if (_groundCollider != null) _groundCollider.GetComponent<Collider2D>().enabled = false;
        MiniJump();
        StartCoroutine(DisablePhysics(3f));
        StartCoroutine(Respawn());
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        PlayerStat.instance.Health = PlayerStat.healthCap;
        transform.position = GameManager.instance.respawnPoint;
        active = true;
        _collider.enabled = true;
        if (_groundCollider != null) _groundCollider.GetComponent<Collider2D>().enabled = true;
    }
    private IEnumerator EnvironmentalRespawn(Vector3 childPosition)
    {
        Debug.Log("pos");
        yield return new WaitForSeconds(0.5f);
        transform.position = childPosition;
        active = true;
        _collider.enabled = true;
        if (_groundCollider != null) _groundCollider.GetComponent<Collider2D>().enabled = true;
    }
    public void SetRespawnPoint(Vector2 position)
    {
        _respawnPoint = position;
    }
    #endregion

    #region Transition
    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if(_exitDir.y > 0)
        {
            _rb.linearVelocity = _jumpSpeed * _exitDir;
        }

        //If exit direction requires horizontal movement
        if(_exitDir.x != 0)
        {
            xRaw = _exitDir.x > 0 ? 1 : -1;
            HorizontalMovement();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
    }
    #endregion

    #region Misc
    public void CornerCorrection(Collider2D other)
    {
        // Bounds bounds = other.bounds;
        // float dir = Mathf.Min(this.transform.position.x - bounds.max.x, this.transform.position.x - bounds.min.x);
        // transform.position += new Vector3(dir, 0f);
    }
    private void DisableMovement(float seconds)
    {
        StartCoroutine(Disable(seconds));
    }
    private IEnumerator Disable(float seconds)
    {
        active = false;
        yield return new WaitForSeconds(seconds);
        active = true;
    }
    private IEnumerator DisablePhysics(float seconds)
    {
        _rb.simulated = false;
        yield return new WaitForSeconds(seconds);
        _rb.simulated = true;
    }
    private void PlayRandomSFXClip(AudioClip[] soundClips)
    {
        if (soundClips == null || SFXManager.instance == null) return;

        SFXManager.instance.PlayRandomSFXClip(soundClips, transform, 1f);
    }
    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }

    private void SlipperyFloor()
    {
        _acceleration = _accelerationValue;
        _deceleration = IsOnIce() ? _iceDeceleration : _decelerationValue;
        _frictionAmount = IsOnIce() ? _iceFriction : _frictionAmountValue;
    }

    private IEnumerator BlinkRedEffect()
    {
        // Change color to red
        playerRenderer.material.color = Color.red;

        // Wait for a short time (for example, 0.1 seconds)
        yield return new WaitForSeconds(0.1f);

        // Revert to the original color
        playerRenderer.material.color = originalColor;
    }
    #endregion
}
