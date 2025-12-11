using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthTextUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TMP_Text healthText;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        playerHealth.onHealthChanged += UpdateHealthUI;
    }
    private void UpdateHealthUI(float newHealth, float maxHealth)
    {
        healthText.text = newHealth.ToString() + "/" + maxHealth.ToString();
    }
    private void OnDisable()
    {
        playerHealth.onHealthChanged -= UpdateHealthUI;
    }
}
