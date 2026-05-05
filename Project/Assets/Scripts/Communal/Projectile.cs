using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HitWall();
        }
        
        if (other.CompareTag("Player"))
        {
            HitWall();
        }
    }

    void HitWall()
    {
        Destroy(gameObject);
    }
}