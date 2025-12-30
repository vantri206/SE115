using UnityEngine;

public class GameplayHUDManager : MonoBehaviour
{
    public static GameplayHUDManager Instance;

    public PlayerHUD playerHUD;
    public BossHealthBarUI bossHealthUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.gameObject.SetActive(false);
        }
    }
    public void AssignPlayer(PlayerController player)
    {
        if (playerHUD != null)
        {
            playerHUD.Initialize(player);
        }
        else
        {
            Debug.LogError("PlayerHUD not Instance!");
        }
    }
    public void ShowBossHealth(BossEnemyHealth boss)
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.Initialize(boss);
        }
    }
    public void HideBossHealth()
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.Hide();
        }
    }
}