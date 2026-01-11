using UnityEngine;

public class ManaPotion : MonoBehaviour, ICollectable
{
    [SerializeField] private float manaAmount = 1.0f;
    public void Collect(GameObject target)
    {
        PlayerMana playerMana = target.GetComponentInChildren<PlayerMana>();

        if (playerMana != null)
        {
            playerMana.RestoreMana(manaAmount);
            Destroy(gameObject);
        }
    }
}
