using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthTextUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public TMP_Text healthText;

    public void Initialize(PlayerHealth currentPlayerHealth)
    {
        this.playerHealth = currentPlayerHealth;

        UpdateHealthUI(0.0f, playerHealth.currentHealth, playerHealth.maxHealth);

        this.playerHealth.onHealthChanged += UpdateHealthUI;
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthUI;
        }
    }
    public void UpdateHealthUI(float healthBefore, float healthAfter, float maxHealth)
    {
        healthText.text = healthAfter.ToString() + "/" + maxHealth.ToString();
    }
}
