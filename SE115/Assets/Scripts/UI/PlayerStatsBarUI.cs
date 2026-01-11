using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 

public class PlayerStatsBarUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerMana playerMana;

    [Header("Health Settings")]
    public Transform healthContainer;  
    public GameObject heartPrefab;
    public float healthPerHeart = 20.0f;
    private List<Image> heartFills = new List<Image>();

    [Header("Mana Settings")]
    public Transform manaContainer;
    public GameObject manaOrbPrefab;

    public void Initialize(PlayerHealth currentPlayerHealth, PlayerMana currentPlayerMana)
    {
        this.playerHealth = currentPlayerHealth;
        this.playerMana = currentPlayerMana;
        CreateHearts(playerHealth.maxHealth);

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
    private void CreateHearts(float maxHealth)
    {
        foreach (Transform child in healthContainer)
        {
            Destroy(child.gameObject);
        }
        heartFills.Clear();

        int heartCount = Mathf.CeilToInt(maxHealth / healthPerHeart);

        for (int i = 0; i < heartCount; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, healthContainer);
            Image fillImage = newHeart.transform.GetChild(1).GetComponent<Image>();
            heartFills.Add(fillImage);
        }
    }
    public void UpdateHealthUI(float oldHealth, float newHealth, float maxHealth)
    {
        int requiredHearts = Mathf.CeilToInt(maxHealth / healthPerHeart);

        if (heartFills.Count != requiredHearts)
        {
            CreateHearts(maxHealth);
        }

        for (int i = 0; i < heartFills.Count; i++)
        {
            float heartCapacity = (i + 1) * healthPerHeart;
            float heartStart = i * healthPerHeart;

            if (newHealth >= heartCapacity)
            {
                heartFills[i].fillAmount = 1.0f;
            }
            else if (newHealth <= heartStart)
            {
                heartFills[i].fillAmount = 0.0f;
            }
            else
            {
                heartFills[i].fillAmount = (newHealth - heartStart) / healthPerHeart;
            }
        }
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