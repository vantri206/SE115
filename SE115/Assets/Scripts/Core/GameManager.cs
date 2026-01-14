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
        Debug.Log("Checkpoint Updated!");
    }

    public void RespawnPlayer()
    {
        if (hasCheckpoint)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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

        // yield return UIManager.Instance.FadeOut();

        yield return SceneManager.LoadSceneAsync(sceneName);

        // yield return UIManager.Instance.FadeIn();

        isTransitioning = false;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (!string.IsNullOrEmpty(nextSpawnPointID))
        {
            if (PositionPlayerAtId(player, nextSpawnPointID))
            {
                UpdateCheckpoint(player.transform.position);
            }
        }
        else if (hasCheckpoint)
        {
            player.transform.position = lastCheckpointPos;
        }
        else
        {
            UpdateCheckpoint(player.transform.position);
        }
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (player != null)
        {
            playerController.Respawn(lastCheckpointPos);
        }
    }
    private bool PositionPlayerAtId(GameObject player, string id)
    {
        SceneEntryPoint[] entries = FindObjectsByType<SceneEntryPoint>(FindObjectsSortMode.InstanceID);

        foreach (SceneEntryPoint entry in entries)
        {
            if (entry.entryId == id)
            {
                player.transform.position = entry.transform.position;
                return true;
            }
        }
        Debug.LogWarning("Can't find spawn point has id: " + id);
        return false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}