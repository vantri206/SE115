using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Checkpoint System")]
    public Vector3 lastCheckpointPos;
    public bool hasCheckpoint = false;
    public GameObject playerPrefab;

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
        lastCheckpointPos = new Vector3(position.x, position.y, 0f);
        hasCheckpoint = true;
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
        SaveGame();

        isTransitioning = true;
        nextSpawnPointID = spawnPointID;
        SetPlayerInputLocked(true);

        if (SceneFader.Instance != null) 
            yield return SceneFader.Instance.FadeOut();

        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFader.Instance != null) 
            yield return SceneFader.Instance.FadeIn();

        isTransitioning = false;
        SetPlayerInputLocked(false);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "VictoryScene") return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (playerPrefab != null)
                player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            else
                return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 spawnPos = new Vector3(player.transform.position.x, player.transform.position.y, 0f);

        bool shouldLoadStats = false;
        bool foundTransitionPoint = false;

        if (!string.IsNullOrEmpty(nextSpawnPointID))
        {
            SceneEntryPoint entry = FindSpawnPoint(nextSpawnPointID);
            if (entry != null)
            {
                spawnPos = new Vector3(entry.transform.position.x, entry.transform.position.y, 0f);
                lastCheckpointPos = spawnPos;
                hasCheckpoint = true;
                foundTransitionPoint = true;
                shouldLoadStats = true; 
            }
            else
            {
                foundTransitionPoint = true;
            }
            nextSpawnPointID = "";
        }

        if (!foundTransitionPoint)
        {
            if (hasCheckpoint && lastCheckpointPos != Vector3.zero)
            {
                spawnPos = new Vector3(lastCheckpointPos.x, lastCheckpointPos.y, 0f);
                shouldLoadStats = true;
            }
            else if (File.Exists(saveFilePath))
            {
                GameSaveData data = LoadDataFromFile();
                if (data != null && data.hasCheckpoint && data.currentSceneName == scene.name)
                {
                    spawnPos = new Vector3(data.checkpointPos.x, data.checkpointPos.y, 0f);
                    lastCheckpointPos = spawnPos;
                    hasCheckpoint = true;
                    shouldLoadStats = true;
                }
                else
                {
                    SceneEntryPoint defaultEntry = FindFirstObjectByType<SceneEntryPoint>();
                    if (defaultEntry != null)
                        spawnPos = new Vector3(defaultEntry.transform.position.x, defaultEntry.transform.position.y, 0f);

                    lastCheckpointPos = spawnPos;
                    hasCheckpoint = true;
                    if (playerController != null) playerController.RestoreStats();
                }
            }
            else
            {
                SceneEntryPoint defaultEntry = FindFirstObjectByType<SceneEntryPoint>();
                if (defaultEntry != null)
                    spawnPos = new Vector3(defaultEntry.transform.position.x, defaultEntry.transform.position.y, 0f);

                lastCheckpointPos = spawnPos;
                hasCheckpoint = true;

                if (playerController != null) playerController.RestoreStats();

                SaveGame();
            }
        }
        if (playerController != null)
        {
            playerController.Respawn(spawnPos);

            if (shouldLoadStats)
            {
                GameSaveData data = LoadDataFromFile();
                if (data != null)
                {
                    if (playerController.health != null)
                    {
                        playerController.health.maxHealth = data.maxHealth;
                        playerController.health.currentHealth = data.currentHealth;
                    }
                    if (playerController.mana != null)
                    {
                        playerController.mana.maxMana = data.maxMana;
                        playerController.mana.currentMana = data.currentMana;
                    }
                    playerController.unlockSlashDash = data.unlockSlashDash;
                    playerController.unlockSwordWave = data.unlockSwordWave;
                    if (playerController.data != null)
                        playerController.data.jumpCountAmount = data.jumpCountAmount;
                }
            }
        }

        PlayerSkillManager skillManager = player.GetComponentInChildren<PlayerSkillManager>();
        if (skillManager != null) LoadSkillsForPlayer(skillManager);

        CinemachineCamera vCam = FindFirstObjectByType<CinemachineCamera>();
        if (vCam != null) { vCam.Follow = player.transform; vCam.OnTargetObjectWarped(player.transform, Vector3.zero); }
        if (GameplayHUDManager.Instance != null) GameplayHUDManager.Instance.AssignPlayer(playerController);
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
            {
                data.currentHealth = player.health.currentHealth;
                data.maxHealth = player.health.maxHealth;
            }
            if (player.mana != null)
            {
                data.currentMana = player.mana.currentMana;
                data.maxMana = player.mana.maxMana;
            }
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
                if (skill != null) data.unlockedSkillFileNames.Add(skill.name);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    private GameSaveData LoadDataFromFile()
    {
        if (!File.Exists(saveFilePath)) return null;
        try
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        catch { return null; }
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
            if (loadedSkill != null) manager.UnlockSkill(loadedSkill, false);
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
        if (player != null) player.LockInput(locked);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public bool HasSaveData()
    {
        return File.Exists(saveFilePath);
    }

    public string GetSavedSceneName()
    {
        GameSaveData data = LoadDataFromFile();
        if (data != null && !string.IsNullOrEmpty(data.currentSceneName))
        {
            return data.currentSceneName;
        }
        return null;
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        lastCheckpointPos = Vector3.zero;
        hasCheckpoint = false;
    }
}