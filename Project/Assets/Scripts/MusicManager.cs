using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip mainTheme;
    public AudioClip saberTheme;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.clip = mainTheme;
        musicSource.Play();
    }

    public void SwitchToSaberMusic()
    {
        if (musicSource.clip == saberTheme) return;
        StartCoroutine(FadeTrack(saberTheme));
    }

    public void SwitchToMainMusic()
    {
        if (musicSource.clip == mainTheme) return;
        StartCoroutine(FadeTrack(mainTheme));
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float fadeTime = 0.5f;
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        musicSource.volume = startVolume;
    }
}