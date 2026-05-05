using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Rigidbody2D))]
public class Harpe : MonoBehaviour
{
    public bool isStationary = false;

    public GameObject pelletPrefab;
    public Transform firePoint;
    public float fireRate = 2f; 
    public float bulletSpeed = 20f;

    public float shootRange = 20f;

    public float walkSpeed = 5f;
    public float retreatSpeed = 4f;
    [SerializeField] private float ramCooldown = 4f;
    [SerializeField] private float walkRange = 50f;

    public GameObject healthPipPrefab;
    [Range(0f, 1f)] public float dropChance = 0.1f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;

    private Transform player;
    private float nextFireTime;
    private Health myHealth;
    private Rigidbody2D rb;

    private enum HarpeState { Retreating, Charging, Cooldown }
    private HarpeState state = HarpeState.Retreating;

    private float retreatTargetX;
    private bool retreatTargetSet = false;

    void Start()
    {
        myHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;
        if (myHealth != null && myHealth.IsInvulnerable()) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        LookAtPlayer();

        if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (isStationary) { StopWalking(); return; }

        switch (state)
        {
            case HarpeState.Retreating:
                HandleRetreat();
                break;
            case HarpeState.Charging:
                HandleCharge();
                break;
            case HarpeState.Cooldown:
                StopWalking();
                break;
        }
    }

    void HandleRetreat()
    {
        if (!retreatTargetSet)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float retreatDistance = distanceToPlayer * 0.5f;
            float awayDirection = (transform.position.x > player.position.x) ? 1f : -1f;
            retreatTargetX = transform.position.x + (awayDirection * retreatDistance);
            retreatTargetSet = true;
        }

        float moveDir = (retreatTargetX > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDir * retreatSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - retreatTargetX) < 0.2f)
        {
            retreatTargetSet = false;
            StopWalking();
            state = HarpeState.Charging;
        }
    }

    void HandleCharge()
    {
        float xDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(xDirection * walkSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && state == HarpeState.Charging)
            StartCoroutine(RamCooldownRoutine());
    }

    private IEnumerator RamCooldownRoutine()
    {
        state = HarpeState.Cooldown;
        StopWalking();
        yield return new WaitForSeconds(ramCooldown);
        retreatTargetSet = false;
        state = HarpeState.Retreating;
    }

    void StopWalking()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void LookAtPlayer()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = (player.position.x > transform.position.x)
            ? Mathf.Abs(localScale.x)
            : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    void Shoot()
    {
        if (pelletPrefab == null || firePoint == null || player == null) return;

        float xDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        Vector2 horizontalDirection = new Vector2(xDirection, 0);
        float angle = (xDirection > 0) ? 0f : 180f;

        GameObject pellet = Instantiate(pelletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        pellet.tag = "EnemyProjectile"; 
        Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
        
        if (pelletRb != null)
            pelletRb.linearVelocity = horizontalDirection * bulletSpeed;

        Destroy(pellet, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            if (myHealth != null)
            {
                if (audioSource != null && hurtSound != null)
                    audioSource.PlayOneShot(hurtSound);

                if (myHealth.GetCurrentHealth() - 1 <= 0)
                {
                    TryDropItem();
                    SceneManager.LoadScene("Ending");
                }

                myHealth.TakeDamage(1);
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Sabre"))
        {
            if (myHealth != null)
            {
                if (audioSource != null && hurtSound != null)
                    audioSource.PlayOneShot(hurtSound);

                if (myHealth.GetCurrentHealth() - 1 <= 0)
                {
                    TryDropItem();
                    SceneManager.LoadScene("Ending");
                }
            }
        }
    }

    private void TryDropItem()
    {
        if (healthPipPrefab == null) return;
        if (Random.value <= dropChance)
            Instantiate(healthPipPrefab, transform.position, Quaternion.identity);
    }
}