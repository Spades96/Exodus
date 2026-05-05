using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;
    private float horizontalInput;

    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallMultiplier = 4f; 
    [SerializeField] private float lowJumpMultiplier = 3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.3f; 
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(15f, 18f);
    [SerializeField] private float wallJumpDuration = 0.2f;
    private bool isTouchingWall, isWallSliding, isWallJumping;

    [SerializeField] private float dashPower = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool canDash = true, isDashing, hasAirDashed; 

    [SerializeField] private GameObject saberHitbox;
    [SerializeField] private float attackDuration = 0.15f;
    [SerializeField] private float attackCooldown = 0.3f;
    private bool isAttacking;
    private float nextAttackTime;

    [SerializeField] private float parryWindow = 0.2f;
    private bool isBlocking;
    private float blockStartTime;

    [SerializeField] private float executionRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound, dashSound, hurtSound;
    [SerializeField] private AudioClip saberSwingSound, parrySound, executeSound;

    public bool hasSaber = false;
    public GameObject visualsFolder; 

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
        rb.freezeRotation = true;
        rb.gravityScale = 3f; 

        if (saberHitbox != null) saberHitbox.SetActive(false);
    }

    void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded || isWallSliding) hasAirDashed = false;

        CheckWallSlide();
        HandleCombatInput();
        HandleMovementInput();
        
        FlipController();
        HandleJumpGravity();
    }

    private void HandleCombatInput()
    {
        if (!hasSaber) return;

        if (Input.GetMouseButtonDown(1) && Time.time >= nextAttackTime && !isBlocking)
            StartCoroutine(SlashRoutine());

        if (Input.GetKeyDown(KeyCode.E))
        {
            isBlocking = true;
            blockStartTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.E))
            isBlocking = false;

        if (Input.GetKeyDown(KeyCode.F))
            TryExecute();
    }

    private void HandleMovementInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded) Jump();
            else if (isWallSliding) StartCoroutine(WallJumpRoutine());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            if (isGrounded || (!hasAirDashed && !isWallSliding))
            {
                if (!isGrounded) hasAirDashed = true;
                StartCoroutine(DashRoutine());
            }
        }
    }

    private IEnumerator SlashRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        if (audioSource && saberSwingSound) audioSource.PlayOneShot(saberSwingSound);

        saberHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        saberHitbox.SetActive(false);
        isAttacking = false;
    }

    private void TryExecute()
    {
        Vector2 checkPos = transform.position + (transform.right * (isFacingRight ? 1 : -1) * 0.8f);
        Collider2D enemy = Physics2D.OverlapCircle(checkPos, executionRange, enemyLayer);
        
        if (enemy != null)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 1)
            {
                if (audioSource && executeSound) audioSource.PlayOneShot(executeSound);
                enemyHealth.InstantDie();
            }
        }
    }

    private void CheckWallSlide()
    {
        float direction = isFacingRight ? 1 : -1;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * direction, wallCheckDistance, groundLayer);
        isWallSliding = (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0);
    }

    private void FixedUpdate()
    {
        if (isDashing || isWallJumping) return;

        if (isBlocking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            return; 
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump() 
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        float jumpDir = isFacingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y);
        if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);
        Flip();
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    private void HandleJumpGravity()
    {
        if (isWallSliding || isWallJumping || isDashing) return;

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private IEnumerator DashRoutine()
    {
        canDash = false; 
        isDashing = true;
        if (audioSource && dashSound) audioSource.PlayOneShot(dashSound);

        float oldGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashPower, 0);
        yield return new WaitForSeconds(dashDuration);
        
        rb.gravityScale = oldGravity;
        isDashing = false;
        yield return new WaitForSeconds(0.4f); 
        canDash = true;
    }

    private void FlipController()
    {
        if (isWallSliding || isWallJumping || isBlocking) return;
        if (horizontalInput != 0)
        {
            if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f) Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        visualsFolder.transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            if (playerHealth != null) playerHealth.InstantDie();
            return;
        }

        bool isProjectile = other.CompareTag("EnemyProjectile");
        bool isEnemy = other.CompareTag("Enemy");

        if (!isProjectile && !isEnemy) return;
        if (isAttacking) return;

        if (isBlocking)
        {
            if (Time.time - blockStartTime <= parryWindow)
            {
                if (audioSource && parrySound) audioSource.PlayOneShot(parrySound);
                if (isProjectile) Destroy(other.gameObject);
                return;
            }
            else
            {
                if (isProjectile) Destroy(other.gameObject);
                return;
            }
        }

        TakePlayerDamage(1);
        if (isProjectile) Destroy(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        if (isAttacking) return;
        if (isBlocking) return;

        TakePlayerDamage(1);
    }

    void TakePlayerDamage(int damage)
    {
        if (playerHealth != null && !playerHealth.IsInvulnerable())
        {
            playerHealth.TakeDamage(damage);
            if (audioSource && hurtSound) audioSource.PlayOneShot(hurtSound);
        }
    }
}