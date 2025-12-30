using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsBarUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerMana playerMana;
    public Image healthFill;
    public Transform manaContainer;
    public GameObject manaOrbPrefab;

    public void Initialize(PlayerHealth currentPlayerHealth, PlayerMana currentPlayerMana)
    {
        this.playerHealth = currentPlayerHealth;
        this.playerMana = currentPlayerMana;

        UpdateHealthUI(0.0f, playerHealth.currentHealth, playerHealth.maxHealth);
        UpdateManaUI(playerMana.currentMana, playerMana.maxMana);

        this.playerHealth.onHealthChanged += UpdateHealthUI;
        this.playerMana.onManaChanged += UpdateManaUI;
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthUI;
        }
        if (playerMana != null)
        {
            playerMana.onManaChanged -= UpdateManaUI;
        }
    }
    public void UpdateHealthUI(float oldHealth, float newHealth, float maxHealth)
    {
        float healthPercent = newHealth / maxHealth;
        if (healthPercent < 0.0f)
            healthPercent = 0.0f;
        else if (healthPercent > 1.0f)
            healthPercent = 1.0f;
        healthFill.fillAmount = healthPercent;
    }
    public void UpdateManaUI(float currentMana, float maxMana)
    {
        int targetOrbCount = Mathf.FloorToInt(currentMana);

        int currentOrbCount = manaContainer.childCount;

        if (currentOrbCount < targetOrbCount)
        {
            int amountToCreate = targetOrbCount - currentOrbCount;
            for (int i = 0; i < amountToCreate; i++)
            {
                Instantiate(manaOrbPrefab, manaContainer);
            }
        }

        else if (currentOrbCount > targetOrbCount)
        {
            int destroyCount = currentOrbCount - targetOrbCount;

            for (int i = 0; i < destroyCount; i++)
            {
                Transform childToRemove = manaContainer.GetChild(manaContainer.childCount - 1);
                Destroy(childToRemove.gameObject);
            }
        }
    }
}
