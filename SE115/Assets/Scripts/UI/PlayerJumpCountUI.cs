using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJumpCountUI : MonoBehaviour
{
    public PlayerController player;
    public TMP_Text jumpCountText;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        player.onJumpLeftChanged += UpdateJumpLeftCount;
    }
    private void UpdateJumpLeftCount(int newCount)
    {
        jumpCountText.text = newCount.ToString();
    }
    private void OnDisable()
    {
        player.onJumpLeftChanged -= UpdateJumpLeftCount;
    }
}
