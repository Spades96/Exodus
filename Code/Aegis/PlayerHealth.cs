using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    public int flashCount = 3;

    public string gameOverSceneName = "GameOver";

    private int currentHealth;
    private bool isInvulnerable = false;

    void Start()
    {
        if (GameStateController.playerMaxHealth <= 0)
            GameStateController.playerMaxHealth = 10;

        currentHealth = GameStateController.playerMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;
        currentHealth -= damage;
        StartCoroutine(DamageFlicker());
        if (currentHealth <= 0) Die();
    }

    public void InstantDie() { Die(); }

    void Die()
    {
        GameStateController.lastCheckpointScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(gameOverSceneName);
    }

    IEnumerator DamageFlicker()
    {
        isInvulnerable = true;
        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer) spriteRenderer.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(flashDuration);
            if (spriteRenderer) spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }
        isInvulnerable = false;
    }

    public float GetCurrentHealth() { return (float)currentHealth; }
    public float GetMaxHealth() { return (float)GameStateController.playerMaxHealth; }
    public bool IsInvulnerable() { return isInvulnerable; }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, GameStateController.playerMaxHealth);
    }
}