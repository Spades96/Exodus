using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SaberPickup : MonoBehaviour
{
    public AudioClip pickupSound;
    private AudioSource source;
    private SpriteRenderer spriteRenderer;
    private Collider2D pickupCollider;
    private bool pickedUp = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pickupCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                pickedUp = true;
                
                player.hasSaber = true;

                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.SwitchToSaberMusic();
                }

                if (source != null && pickupSound != null)
                {
                    source.PlayOneShot(pickupSound);
                }

                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (pickupCollider != null) pickupCollider.enabled = false;

                float delay = (pickupSound != null) ? pickupSound.length : 0.1f;
                Destroy(gameObject, delay);
            }
        }
    }
}