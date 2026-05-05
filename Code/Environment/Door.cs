using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject boss;

    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource source;

    public float phaseOutDuration = 2f;
    public bool switchMusicOnEntry = true;

    public BossHealthUI bossHealthUI;

    private static bool musicSwitchedOnExit = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private enum DoorState
    {
        AwaitingEntry,
        PhasingOut,
        Locked,
        Opening,
        Destroyed
    }

    private DoorState state = DoorState.AwaitingEntry;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col            = GetComponent<Collider2D>();
        source         = GetComponent<AudioSource>();

        if (source != null)
        {
            source.playOnAwake  = false;
            source.spatialBlend = 0f;
        }
    }

    void Update()
    {
        if (state != DoorState.Opening && state != DoorState.Destroyed && boss == null)
        {
            StartCoroutine(BossDefeatedSequence());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == DoorState.AwaitingEntry && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(EntrySequence());
        }
    }

    private IEnumerator EntrySequence()
    {
        state = DoorState.PhasingOut;

        SetVisible(false);
        SetSolid(false);
        PlaySound(openSound);

        if (switchMusicOnEntry && MusicManager.Instance != null)
            MusicManager.Instance.SwitchToSaberMusic();

        if (bossHealthUI != null && boss != null)
        {
            Health bossHealth = boss.GetComponent<Health>();
            if (bossHealth != null)
                bossHealthUI.ShowForBoss(bossHealth);
        }

        yield return new WaitForSeconds(phaseOutDuration);
        yield return new WaitUntil(() => !IsPlayerOverlapping());

        SetVisible(true);
        SetSolid(true);
        PlaySound(closeSound);

        state = DoorState.Locked;
    }

    private IEnumerator BossDefeatedSequence()
    {
        state = DoorState.Opening;

        SetVisible(false);
        SetSolid(false);
        PlaySound(openSound);

        if (bossHealthUI != null)
            bossHealthUI.Hide();

        if (!musicSwitchedOnExit && MusicManager.Instance != null)
        {
            musicSwitchedOnExit = true;
            MusicManager.Instance.SwitchToMainMusic();
        }

        float delay = (openSound != null) ? openSound.length : phaseOutDuration;
        yield return new WaitForSeconds(delay);

        state = DoorState.Destroyed;
        Destroy(gameObject);
    }

    private void SetVisible(bool visible)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = visible;
    }

    private void SetSolid(bool solid)
    {
        if (col != null)
            col.enabled = solid;
    }

    private void PlaySound(AudioClip clip)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip);
    }

    private bool IsPlayerOverlapping()
    {
        if (col == null) return false;

        Bounds b          = col.bounds;
        Vector2 checkSize = (Vector2)b.size * 0.9f;

        int playerLayer = LayerMask.GetMask("Player");
        if (playerLayer != 0)
            return Physics2D.OverlapBox(b.center, checkSize, 0f, playerLayer) != null;

        foreach (var h in Physics2D.OverlapBoxAll(b.center, checkSize, 0f))
            if (h.CompareTag("Player")) return true;

        return false;
    }
}