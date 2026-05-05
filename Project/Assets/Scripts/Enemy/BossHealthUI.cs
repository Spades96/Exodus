using UnityEngine;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform healthBarFill;

    private Health bossHealth;
    private float maxBarHeight;
    private bool active = false;

    void Start()
    {
        if (healthBarFill != null)
            maxBarHeight = healthBarFill.sizeDelta.y;

        SetUIVisible(false);
    }

    void Update()
    {
        if (!active || bossHealth == null) return;

        UpdateHealthText();
        UpdateHealthBar();
    }

    public void ShowForBoss(Health boss)
    {
        bossHealth = boss;
        active = true;
        SetUIVisible(true);
    }

    public void Hide()
    {
        active = false;
        SetUIVisible(false);
    }

    private void UpdateHealthText()
    {
        float current = bossHealth.GetCurrentHealth();
        float max     = bossHealth.GetMaxHealth();
        healthText.text = $"{current}\n-\n{max}";
    }

    private void UpdateHealthBar()
    {
        float current    = bossHealth.GetCurrentHealth();
        float max        = bossHealth.GetMaxHealth();
        float fillPercent = current / max;

        healthBarFill.sizeDelta = new Vector2(
            healthBarFill.sizeDelta.x,
            maxBarHeight * fillPercent
        );
    }

    private void SetUIVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}