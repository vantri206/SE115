using UnityEngine;
using UnityEngine.SceneManagement;

public class AssignCanvasCamera : MonoBehaviour
{
    private Canvas canvas;
    private void Awake()
    {
        if(canvas == null)
            canvas = GetComponent<Canvas>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "UI";
        }
    }
}
