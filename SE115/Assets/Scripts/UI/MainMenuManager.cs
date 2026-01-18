using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string firstLevelSceneName = "Scene1";

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        CheckContinueButtonStatus();
    }

    private void CheckContinueButtonStatus()
    {
        if (continueButton != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasSaveData())
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
    }

    private void OnPlayClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DeleteSaveData();
            GameManager.Instance.SwitchScene(firstLevelSceneName, "");
        }
        else
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }
    }

    private void OnContinueClicked()
    {
        if (GameManager.Instance != null)
        {
            string savedScene = GameManager.Instance.GetSavedSceneName();
            if (!string.IsNullOrEmpty(savedScene))
            {
                GameManager.Instance.SwitchScene(savedScene, "");
            }
            else
            {
                Debug.LogWarning("Save file found but scene name is invalid.");
            }
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}