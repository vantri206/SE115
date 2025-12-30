using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour, IManable 
{
    [Header("Mana Settings")]
    [SerializeField] private float maxManaStart = 2f;
    public float currentMana { get; set; }
    public float maxMana { get; set; }

    public Action<float, float> onManaChanged;

    private void Start()
    {
        maxMana = maxManaStart;
        currentMana = maxMana;
        onManaChanged?.Invoke(currentMana, maxMana);
    }
    public bool ConsumeMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            onManaChanged?.Invoke(currentMana, maxMana);
            return true;
        }
        Debug.Log("Not enough Mana!");
        return false;
    }

    public void RestoreMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);
        onManaChanged?.Invoke(currentMana, maxMana);
    }
}