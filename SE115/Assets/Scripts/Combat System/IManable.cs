using UnityEngine;

public interface IManable
{
    float currentMana { get; }
    float maxMana { get; }
    bool ConsumeMana(float amount);
    void RestoreMana(float amount);
}
