using UnityEngine;

public class PortalSender : MonoBehaviour 
{
    [Header("Settings")]
    [SerializeField] private string transitionScene; 
    [SerializeField] private string spawnPointId; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SwitchScene(transitionScene, spawnPointId);
        }
    }
}