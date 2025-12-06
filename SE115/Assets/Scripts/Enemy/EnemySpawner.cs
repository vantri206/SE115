using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private EnemyController enemyPrefab; 
    [SerializeField] private EnemyController currentEnemy;

    [SerializeField] private float respawnTime = 5.0f; 
    [SerializeField] private float fadeAppearDuration = 1.5f;

    private SpriteRenderer enemySR;
    private Collider2D[] enemyColliders;
    private Vector3 spawnPosition;

    private void Start()
    {
        if (currentEnemy == null && enemyPrefab != null)
        {
            currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
        }

        if (currentEnemy != null)
        {
            spawnPosition = transform.position;
            enemySR = currentEnemy.GetComponentInChildren<SpriteRenderer>();
            currentEnemy.onFinishDead += StartRespawnProcess;

            enemyColliders = currentEnemy.transform.GetComponentsInChildren<Collider2D>();
        }
    }

    private void StartRespawnProcess()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        currentEnemy.transform.position = spawnPosition;
        currentEnemy.ResetEnemyState();

        Color c = enemySR.color;
        c.a = 0f;
        enemySR.color = c;

        currentEnemy.gameObject.SetActive(true);
        DisableEnemyPhysicAndCollider();

        float timer = 0f;
        while (timer < fadeAppearDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeAppearDuration);

            c.a = alpha;
            enemySR.color = c;

            yield return null; 
        }

        c.a = 1f;
        enemySR.color = c;

        currentEnemy.ActivateEnemyAI();
        EnableEnemyPhysicAndCollider();
    }
    private void EnableEnemyPhysicAndCollider()
    {
        currentEnemy.EnablePhysicAndCollider();
        foreach(Collider2D collider in enemyColliders)
        {
            collider.enabled = true;
        }
    }
    private void DisableEnemyPhysicAndCollider()
    {
        currentEnemy.DisablePhysicAndCollider();
        foreach (Collider2D collider in enemyColliders)
        {
            collider.enabled = false;
        }
    }
}