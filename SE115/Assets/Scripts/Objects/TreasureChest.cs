using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isOpen = false;

    [Header("Item Loot Settings")]
    [SerializeField] private GameObject[] spawnItems; 
    [SerializeField] private Transform spawnPoint;  
    [SerializeField] private Vector2 spawnForce = new Vector2(-2.0f, 7.5f); 

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D interactHitbox;

    private PlayerController playerInRange;

    private void Start()
    {
        if (animator == null) 
            animator = GetComponent<Animator>();
        if (interactHitbox == null)
            interactHitbox = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        if (playerInRange != null && !isOpen)
        {
            if (playerInRange.lastPressedInteractTime > 0.0f)
            {
                OpenChest();
            }
        }
    }

    private void OpenChest()
    {
        isOpen = true;

        if (animator != null) animator.SetTrigger("Open");
    }

    public void AE_SpawnLoot()
    {
        if (spawnItems != null && spawnItems.Length > 0)
        {
            foreach (GameObject spawnItem in spawnItems)
            {
                GameObject loot = Instantiate(spawnItem, spawnPoint.position, Quaternion.identity);

                Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float randomX = Random.Range(-1.0f, 1.0f);
                    Vector2 force = new Vector2(randomX * spawnForce.x, spawnForce.y);

                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }
    }
    public void SetPlayerInRange(PlayerController player)
    {
        playerInRange = player;
    }
    public void AE_ChestClosed()
    {
        Destroy(gameObject);
    }
}