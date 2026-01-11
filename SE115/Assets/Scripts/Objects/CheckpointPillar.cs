using UnityEngine;

public class CheckpointPillar : MonoBehaviour
{
    [Header("Refrences")]
    public Animator animator;

    [Header("Settings")]
    public GameObject saveCompleteEffect; 
    public GameObject floatingTextPrefab; 
    public Transform textSpawnPoint;      
    public AudioClip saveCompleteSound;  
    private AudioSource audioSource;

    private void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("Save");
            GameManager.Instance.UpdateCheckpoint(transform.position);
        }
    }

    public void OnSaveFinished()
    {
        if (saveCompleteSound != null)
        {
            audioSource.PlayOneShot(saveCompleteSound);
        }

        if (saveCompleteEffect != null)
        {
            Instantiate(saveCompleteEffect, transform.position, Quaternion.identity);
        }

        if (floatingTextPrefab != null && textSpawnPoint != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, textSpawnPoint.position, Quaternion.identity);
        }

        Debug.Log("Save Completed!");
    }
}