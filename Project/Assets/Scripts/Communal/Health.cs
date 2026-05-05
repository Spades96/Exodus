using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    public int flashCount = 3;

    public string gameOverSceneName = "GameOver"; 

    private bool isInvulnerable = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //StartCoroutine(DamageFlicker());
        }
    }

    public void InstantDie()
    {
        Die();
    }

    void Die()
    {
        if (gameObject.CompareTag("Player"))
        {
            GameStateController.lastCheckpointScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DamageFlicker()
    {
        isInvulnerable = true;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.2f); 
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white; 
            yield return new WaitForSeconds(flashDuration);
        }

        isInvulnerable = false;
    }

    public float GetCurrentHealth()
    {
        return (float)currentHealth;
    }

    public float GetMaxHealth()
    {
        return (float)maxHealth;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}