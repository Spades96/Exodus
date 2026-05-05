using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
    public bool isStationary = false;

    public GameObject pelletPrefab;
    public Transform firePoint;
    public float fireRate = 2f; 
    public float bulletSpeed = 20f;
    
    public float walkRange = 50f;
    public float shootRange = 20f;

    public float walkSpeed = 5f;
    [SerializeField] private float ramCooldown = 4f;

    public GameObject healthPipPrefab;
    [Range(0f, 1f)] public float dropChance = 0.1f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;

    private Transform player;
    private float nextFireTime;
    private Health myHealth;
    private Rigidbody2D rb;
    private bool isOnCooldown = false;

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

        if (isStationary || isOnCooldown)
        {
            StopWalking();
            return;
        }

        if (distanceToPlayer <= walkRange)
            Walk();
        else
            StopWalking();
    }

    void Walk()
    {
        float xDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(xDirection * walkSpeed, rb.linearVelocity.y);
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isOnCooldown)
            StartCoroutine(RamCooldownRoutine());
    }

    private IEnumerator RamCooldownRoutine()
    {
        isOnCooldown = true;
        StopWalking();
        yield return new WaitForSeconds(ramCooldown);
        isOnCooldown = false;
    }

    private void LoadLoreScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level 1") SceneManager.LoadScene("Boss 1 Lore");
        else if (currentScene == "Level 2") SceneManager.LoadScene("Boss 2 Lore");
        else if (currentScene == "Level 3") SceneManager.LoadScene("Boss 3 Lore");
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
                    LoadLoreScene();
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
                    LoadLoreScene();
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