using UnityEngine;

public class TouchDoor : MonoBehaviour
{
    public AudioClip openingSound;
    private AudioSource source;

    public GameObject deathEffect; 

    void Start()
    {
        source = GetComponent<AudioSource>();
        
        if (source != null)
        {
            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (source != null && openingSound != null)
        {
            source.PlayOneShot(openingSound);
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        float delay = (openingSound != null) ? openingSound.length : 0.1f;
        Destroy(gameObject, delay);
    }
}