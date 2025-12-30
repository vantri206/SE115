using UnityEngine;

public class InteractZone : MonoBehaviour
{
    [SerializeField] private TreasureChest chest; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log("SetPlayer");
            chest.SetPlayerInRange(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            chest.SetPlayerInRange(null);
        }
    }
}