using TMPro;
using UnityEngine;

public class CheckpointPillar : MonoBehaviour, IInteractable
{
    [Header("Refrences")]
    public Animator animator;

    [Header("Settings")]
    public GameObject floatingTextPrefab; 
    public Transform textSpawnPoint;      
    public AudioClip saveCompleteSound;  
    private AudioSource audioSource;

    private PlayerController playerInRange;
    private bool isSaved = false;
    private float savedTimer = 0.0f;

    private void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }
    private void Update()
    {
        savedTimer += Time.deltaTime;

        if(savedTimer >= 5.0f)
        {
            isSaved = false;
        }

        if (playerInRange != null && playerInRange.lastPressedInteractTime > 0.0f)
        {
            playerInRange.input.ResetInteractPressed(); 
            ActivateCheckpoint();
        }
    }
    public void SetPlayerInRange(PlayerController player)
    {
        this.playerInRange = player;
        if (player != null)
        {
            Debug.Log("Player entered checkpoint zone");
        }
        else
        {
            Debug.Log("Player left check point zone");
        }
    }

    private void ActivateCheckpoint()
    {
        if (isSaved) return;

        isSaved = true;
        savedTimer = 0.0f;

        playerInRange.RestoreStats();
        animator.SetTrigger("Save");
        GameManager.Instance.UpdateCheckpoint(transform.position);
    }
    public void OnSaveFinished()
    {
        Debug.Log("Save Finished");
        if (saveCompleteSound != null)
        {
            audioSource.PlayOneShot(saveCompleteSound);
        }

        if (floatingTextPrefab != null && textSpawnPoint != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, textSpawnPoint.position, Quaternion.identity);
            TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
            if(tmp != null)
            {
                tmp.text = "CHECKPOINT \n SAVED!";
            }
        }

        Debug.Log("Save Completed!");
    }
}