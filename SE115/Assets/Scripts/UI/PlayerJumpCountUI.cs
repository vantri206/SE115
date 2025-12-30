using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJumpCountUI : MonoBehaviour
{
    private PlayerController player;
    public TMP_Text jumpCountText;

    public void Initialize(PlayerController currentPlayer)
    {
        player = currentPlayer;

        UpdateJumpLeftCount(player.jumpLeft);

        player.onJumpLeftChanged += UpdateJumpLeftCount;
    }
    private void OnDisable()
    {
        if (player != null)
        {
            player.onJumpLeftChanged -= UpdateJumpLeftCount;
        }
    }
    private void UpdateJumpLeftCount(int newCount)
    {
        jumpCountText.text = newCount.ToString();
    }
}
