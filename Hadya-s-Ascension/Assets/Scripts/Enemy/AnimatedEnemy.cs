using UnityEngine;
using System.Collections;

// Требуем наличия компонента Animator
[RequireComponent(typeof(Animator))]
public class AnimatedEnemy : Enemy
{
    [Header("Настройки атаки")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float timeAttack = 1;
    [SerializeField] private float delaySound;
    [SerializeField] private Transform target;
    [SerializeField] private AudioSource hitSound;
    
    [Header("Настройки анимации")]
    [SerializeField] private string attackTriggerName = "Attack"; // Имя триггера атаки в аниматоре
    
    private Animator animator;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    
    protected override void Start()
    {
        base.Start();
        
        // Получаем компонент аниматора
        animator = GetComponent<Animator>();
        
        // Если цель не задана, ищем игрока
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }
    
    void Update()
    {
        if (isDead || target == null) return;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        if (distanceToTarget <= attackRange)
        {
            // Если враг в зоне атаки - атакуем
            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Если враг не в зоне атаки - двигаемся к цели
            MoveTowardsTarget();
        }
    }
    
    protected virtual void MoveTowardsTarget()
    {
        if (target == null || isAttacking) return;
        
        // Направление движения
        Vector3 direction = (target.position - transform.position).normalized;
        
        // Движение в сторону цели
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    protected virtual void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        StopMovement();
        animator.SetTrigger(attackTriggerName);
        StartCoroutine(DealDamageWithDelay(timeAttack));
        hitSound?.Play();
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(delaySound);
        hitSound?.Play();
    }
    
    protected virtual void StopMovement()
    {
        // Останавливаем врага на месте
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    protected virtual IEnumerator DealDamageWithDelay(float delay)
    {
        // Ждем указанное время перед нанесением урона
        yield return new WaitForSeconds(delay);
        
        // Наносим урон, если цель все еще в зоне досягаемости
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            
            if (distanceToTarget <= attackRange)
            {
                DealDamage();
            }
        }
        
        // Ждем окончания анимации атаки
        yield return new WaitForSeconds(attackCooldown - delay);
        isAttacking = false;
    }
    
    protected virtual void DealDamage()
    {
        // Проверяем наличие компонента IDamageable у цели
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
            Debug.Log($"Враг наносит урон: {attackDamage}");
        }
    }
    
    // Метод для вызова через Animation Event
    public virtual void OnAttackAnimationComplete()
    {
        isAttacking = false;
    }
    
    public override void Die()
    {
        if (isDead) return;
        
        // Останавливаем все корутины при смерти
        StopAllCoroutines();
        Destroy(gameObject);
        
        base.Die();
    }
    
    void OnDrawGizmosSelected()
    {
        // Отображение радиуса атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
