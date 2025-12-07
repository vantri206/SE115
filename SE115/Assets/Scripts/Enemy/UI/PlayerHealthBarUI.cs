using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthBarFill;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        playerHealth.onHealthChanged += UpdateHealthUI;
    }
    private void UpdateHealthUI(float newHealth, float maxHealth)
    {
        float healthPercent = newHealth / maxHealth;
        if (healthPercent < 0.0f)
            healthPercent = 0.0f;
        else if (healthPercent > 1.0f)
            healthPercent = 1.0f;
        healthBarFill.fillAmount = healthPercent;
    }
    private void OnDisable()
    {
        playerHealth.onHealthChanged -= UpdateHealthUI;
    }
}
