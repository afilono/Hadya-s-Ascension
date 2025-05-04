using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Основные настройки")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    
    protected bool isDead = false;
    
    public float Health => currentHealth;
    
    // Событие смерти врага
    public Action<Enemy> OnEnemyDeath;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"{gameObject.name} погиб!");
        
        OnEnemyDeath?.Invoke(this);
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}