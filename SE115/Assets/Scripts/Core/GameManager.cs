using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Checkpoint System")]
    public Vector3 lastCheckpointPos;
    public bool hasCheckpoint = false;

    [Header("Scene Transition System")]
    public string nextSpawnPointID;
    public bool isTransitioning = false;

    [Header("Save System Settings")]
    private string saveFilePath;
    private const string SKILL_RESOURCE_PATH = "Skills/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, "save_data.json");

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

        SaveGame();
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
        SetPlayerInputLocked(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
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
            if (File.Exists(saveFilePath))
            {
                GameSaveData data = LoadDataFromFile();
                if (data != null && data.hasCheckpoint)
                {
                    spawnPos = data.checkpointPos;
                    lastCheckpointPos = data.checkpointPos;
                    hasCheckpoint = true;

                    if (playerController != null)
                    {
                        if (playerController.health != null)
                            playerController.health.currentHealth = data.currentHealth;

                        if (playerController.mana != null)
                            playerController.mana.currentMana = data.currentMana;

                        playerController.unlockSlashDash = data.unlockSlashDash;
                        playerController.unlockSwordWave = data.unlockSwordWave;

                        if (playerController.data != null)
                        {
                            playerController.data.jumpCountAmount = data.jumpCountAmount;
                        }
                    }
                }
                else
                {
                    UpdateCheckpoint(player.transform.position);
                    spawnPos = player.transform.position;
                }
            }
            else
            {
                UpdateCheckpoint(player.transform.position);
                spawnPos = player.transform.position;
            }
        }
        else
        {
            GameSaveData data = LoadDataFromFile();
            if (data != null && playerController != null)
            {
                if (playerController.health != null)
                    playerController.health.currentHealth = data.currentHealth;
                if (playerController.mana != null)
                    playerController.mana.currentMana = data.currentMana;
            }
        }

        if (playerController != null)
        {
            playerController.Respawn(spawnPos);
        }

        PlayerSkillManager skillManager = player.GetComponentInChildren<PlayerSkillManager>();
        if (skillManager != null)
        {
            LoadSkillsForPlayer(skillManager);
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public List<string> unlockedSkillFileNames = new List<string>();
        public Vector3 checkpointPos;
        public bool hasCheckpoint;
        public string currentSceneName;

        public float maxHealth;
        public float maxMana;

        public float currentHealth;
        public float currentMana;

        public bool unlockSlashDash;
        public bool unlockSwordWave;
        public int jumpCountAmount;
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();

        data.checkpointPos = lastCheckpointPos;
        data.hasCheckpoint = hasCheckpoint;
        data.currentSceneName = SceneManager.GetActiveScene().name;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            if (player.health != null)
                data.currentHealth = player.health.currentHealth;

            if (player.mana != null)
                data.currentMana = player.mana.currentMana;

            data.unlockSlashDash = player.unlockSlashDash;
            data.unlockSwordWave = player.unlockSwordWave;

            if (player.data != null)
            {
                data.jumpCountAmount = player.data.jumpCountAmount;
            }
        }

        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null)
        {
            foreach (SkillBase skill in skillManager.unlockedSkills)
            {
                if (skill != null)
                {
                    data.unlockedSkillFileNames.Add(skill.name);
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game Saved!");
    }

    private GameSaveData LoadDataFromFile()
    {
        if (!File.Exists(saveFilePath)) return null;
        try
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    private void LoadSkillsForPlayer(PlayerSkillManager manager)
    {
        GameSaveData data = LoadDataFromFile();
        if (data == null) return;

        manager.unlockedSkills.Clear();
        manager.skillsSlot.Clear();

        foreach (string fileName in data.unlockedSkillFileNames)
        {
            SkillBase loadedSkill = Resources.Load<SkillBase>(SKILL_RESOURCE_PATH + fileName);

            if (loadedSkill != null)
            {
                manager.UnlockSkill(loadedSkill, false);
            }
            else
            {
                Debug.LogWarning($"Not found file skill: {fileName}");
            }
        }
    }

    SceneEntryPoint FindSpawnPoint(string id)
    {
        SceneEntryPoint[] entries = FindObjectsByType<SceneEntryPoint>(FindObjectsSortMode.InstanceID);
        foreach (SceneEntryPoint entry in entries)
        {
            if (entry.entryId == id) 
                return entry;
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