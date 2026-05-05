using UnityEngine;

public class SaberHitbox : MonoBehaviour
{
    public int damage = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        if (other.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
        }
    }
}