using UnityEngine;
using System.Collections;

public class StairDoor : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnPointID;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isGoingUp;
    [SerializeField] private float waitDuration = 1.0f;

    private PlayerController playerInRange;
    private bool isActiviting = false;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (isGoingUp)
            animator.SetTrigger("isStairUp");
    }
    IEnumerator EnterStairSequence()
    {
        isActiviting = true;

        if (playerInRange != null)
        {
            playerInRange.LockInput(true);
            playerInRange.spriteRenderer.enabled = false;
        }
        if (animator != null)
        {
            string triggerName = isGoingUp ? "goUp" : "goDown";
            animator.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(waitDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SwitchScene(sceneToLoad, spawnPointID);
        }
    }

    public void SetPlayerInRange(PlayerController player)
    {
        playerInRange = player;

        if (player != null && !isActiviting)
        {
            StartCoroutine(EnterStairSequence());
        }
    }
}