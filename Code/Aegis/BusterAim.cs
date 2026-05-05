using UnityEngine;

public class BusterAim : MonoBehaviour
{
    public GameObject pelletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public SpriteRenderer busterRenderer;

    public float fireRate = 0.15f; 
    private float nextFireTime = 0f;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 lookDir = (Vector2)mouseWorldPos - (Vector2)transform.position;
        
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot(lookDir.normalized);
            
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject pellet = Instantiate(pelletPrefab, firePoint.position, Quaternion.identity);
        
        pellet.tag = "PlayerProjectile";

        Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pellet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(pellet, 2f);
    }
}