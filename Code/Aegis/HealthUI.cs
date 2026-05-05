using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
 
    [SerializeField] private TextMeshProUGUI healthText;
 
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private RectTransform healthBarFill;

    private float maxBarHeight;

    void Start()
    {
        if (healthBarFill != null)
            maxBarHeight = healthBarFill.sizeDelta.y;
    }

    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            UpdateHealthText();
            UpdateHealthBar();
        }
    }

    void UpdateHealthText()
    {
        float current = playerHealth.GetCurrentHealth();
        float max = playerHealth.GetMaxHealth();
        healthText.text = $"{current}\n-\n{max}";
    }

    void UpdateHealthBar()
    {
        float current = playerHealth.GetCurrentHealth();
        float max = playerHealth.GetMaxHealth();

        float fillPercent = current / max;
        healthBarFill.sizeDelta = new Vector2(healthBarFill.sizeDelta.x, maxBarHeight * fillPercent);
    }
}