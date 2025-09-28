using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private float currentHealth;
    [SerializeField] private float maxHealth;

    public Action OnDeath;
    public Action<float> OnHealthChanged;

    public bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;  

    public void IncreaseHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Heal: {amount}  Health: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth); // notify listeners, such as UIController

    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Damage: {amount}  Health: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth); // notify listeners, such as UIController

        if(currentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
