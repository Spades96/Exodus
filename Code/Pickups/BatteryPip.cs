using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BatteryPip : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;
    public AudioClip batteryPipSound;

    private AudioSource source;
    private SpriteRenderer spriteRenderer;
    private Collider2D pipCollider;
    private bool pickedUp = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pipCollider = GetComponent<Collider2D>();

        if (source != null)
        {
            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                pickedUp = true;
                
                playerHealth.Heal(healAmount);

                if (source != null && batteryPipSound != null)
                    source.PlayOneShot(batteryPipSound);

                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (pipCollider != null) pipCollider.enabled = false;

                float delay = (batteryPipSound != null) ? batteryPipSound.length : 0.1f;
                Destroy(gameObject, delay);
            }
        }
    }
}