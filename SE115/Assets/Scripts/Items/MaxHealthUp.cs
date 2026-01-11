using UnityEngine;

public class MaxHealthUP : MonoBehaviour, ICollectable
{
    [SerializeField] private float healthUpAmount = 20.0f;
    public void Collect(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponentInChildren<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.SetMaxHeal(healthUpAmount + playerHealth.maxHealth);
            Destroy(gameObject);
        }
    }
}
