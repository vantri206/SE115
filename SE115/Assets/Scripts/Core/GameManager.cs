using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Checkpoint System")]
    public Vector3 lastCheckpointPos;
    public bool hasCheckpoint = false;

    [Header("Scene Transition System")]
    public string nextSpawnPointID;
    public bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UpdateCheckpoint(Vector3 position)
    {
        lastCheckpointPos = position;
        hasCheckpoint = true;
        nextSpawnPointID = "";
    }

    public void RespawnPlayer()
    {
        if (hasCheckpoint && !isTransitioning)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    IEnumerator RespawnRoutine()
    {
        isTransitioning = true;

        SetPlayerInputLocked(true);

        if (SceneFader.Instance != null) yield return SceneFader.Instance.FadeOut();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (SceneFader.Instance != null) yield return SceneFader.Instance.FadeIn();

        isTransitioning = false;

        SetPlayerInputLocked(false);
    }

    public void SwitchScene(string sceneName, string spawnPointID)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName, spawnPointID));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnPointID)
    {
        isTransitioning = true;
        nextSpawnPointID = spawnPointID;

        SetPlayerInputLocked(true);

        if (SceneFader.Instance != null)
        {
            yield return SceneFader.Instance.FadeOut();
        }

        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFader.Instance != null)
        {
            yield return SceneFader.Instance.FadeIn();
        }

        isTransitioning = false;

        SetPlayerInputLocked(true);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 spawnPos = lastCheckpointPos;

        if (!string.IsNullOrEmpty(nextSpawnPointID))
        {
            SceneEntryPoint entry = FindSpawnPoint(nextSpawnPointID); 
            if (entry != null)
            {
                spawnPos = entry.transform.position;
                UpdateCheckpoint(spawnPos); 
            }
        }
        else if (!hasCheckpoint)
        {
            UpdateCheckpoint(player.transform.position); 
            spawnPos = player.transform.position;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Respawn(spawnPos);
        }
    }
    SceneEntryPoint FindSpawnPoint(string id)
    {
        SceneEntryPoint[] entries = FindObjectsByType<SceneEntryPoint>(FindObjectsSortMode.InstanceID);
        foreach (SceneEntryPoint entry in entries)
        {
            if (entry.entryId == id) return entry;
        }
        return null;
    }
    private void SetPlayerInputLocked(bool locked)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.LockInput(locked);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}