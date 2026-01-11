using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public PlayerController player;

    [Header("Respawn Settings")]
    public float respawnDelay = 1.0f;
    private Vector2 currentRespawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (player.transform != null)
        {
            currentRespawnPoint = player.transform.position;
        }
    }

    public void UpdateCheckpoint(Vector2 newPosition)
    {
        currentRespawnPoint = newPosition;

        Debug.Log("Checkpoint Updated!");

        RestorePlayerStats();
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (player != null)
        {
            player.Respawn(currentRespawnPoint);
        }
        else
        {
            player.transform.position = currentRespawnPoint;
            Debug.Log("Cant found Player Controller in Game Manager.");
        }

        RestorePlayerStats();
    }

    private void RestorePlayerStats()
    {
        if (player.health != null)
        {
            player.health.SetCurrentHeal(player.health.maxHealth);
        }

        if (player.mana != null)
        {
            player.mana.RestoreMana(player.mana.maxMana);
        }
    }
}