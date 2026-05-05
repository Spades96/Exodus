using UnityEngine;

public class PelletSound : MonoBehaviour
{
    public AudioClip spawnSound;
    
    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source != null && spawnSound != null)
        {
            source.PlayOneShot(spawnSound);
        }
    }
}