using UnityEngine;

public class SpiderBoss : Boss
{
    [Header("Настройки паука")]
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int meleeDamage = 15;
    public float biteDamageDelay = 0.5f;
    
    [Header("Настройки паутины")]
    public GameObject webPrefab;
    public float webCooldown = 3f;
    public float webShootRange = 7f;
    public Transform webShootPoint;
    public float webSpeed = 8f;
    public int webDamage = 10;
    public float webSlowAmount = 0.5f; // 50% от нормальной скорости
    public float webSlowDuration = 3f;
    
    private Animator animator;
    private float lastMeleeAttackTime = 0f;
    private float lastWebAttackTime = 0f;
    private bool isWebPhaseActive = false;
    
    protected override void Start()
    {
        base.Start();
        
        animator = GetComponent<Animator>();
        
        if (webShootPoint == null)
            webShootPoint = transform;
    }
    
    void FixedUpdate()
    {
        if (!isPlayerInRoom) return;
        
        // Проверка перехода на фазу с паутиной
        if (!isWebPhaseActive && currentHealth <= maxHealth / 2)
        {
            isWebPhaseActive = true;
            Debug.Log("Паук-босс начинает использовать паутину!");
        }
        
        // Поворот в сторону игрока
        LookAtPlayer();
        
        // Управление поведением
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        
        // Атака ближнего боя, если игрок в пределах досягаемости
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastMeleeAttackTime + attackCooldown)
            {
                MeleeAttack();
            }
        }
        else
        {
            // Стрельба паутиной, если активна фаза паутины и игрок в пределах досягаемости
            if (isWebPhaseActive && distanceToPlayer <= webShootRange)
            {
                if (Time.time >= lastWebAttackTime + webCooldown)
                {
                    ShootWeb();
                }
            }
            
            // Двигаемся к игроку
            MoveTowardsPlayer();
        }
    }
    
    void MoveTowardsPlayer()
    {
        if (Player == null) return;
        
        // Направление движения
        Vector2 direction = (Player.position - transform.position).normalized;
        
        // Проверка на наличие стены перед боссом
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveSpeed * Time.deltaTime, LayerMask.GetMask("Wall"));
        if (hit.collider == null)
        {
            // Движение в сторону игрока
            transform.position = Vector2.MoveTowards(transform.position, Player.position, moveSpeed * Time.deltaTime);
            
            // Включаем анимацию ходьбы
            animator.SetBool("IsWalking", true);
        }
        else
        {
            // Останавливаемся, если впереди стена
            animator.SetBool("IsWalking", false);
        }
    }
    
    void MeleeAttack()
    {
        // Запускаем анимацию атаки
        animator.SetTrigger("Attack");
    
        lastMeleeAttackTime = Time.time;
        animator.SetBool("IsWalking", false);
    
        // Вызываем метод нанесения урона с задержкой
        Invoke("ApplyBiteDamage", biteDamageDelay);
    }

    void ApplyBiteDamage()
    {
        // Наносим урон игроку, если он все еще в зоне атаки
        if (Player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
            if (distanceToPlayer <= attackRange)
            {
                IDamageable damageable = Player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(meleeDamage);
                }
                else
                {
                    HealthSystem healthSystem = Player.GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        healthSystem.TakeDamage(meleeDamage);
                    }
                }
            }
        }
    }
    
    void ShootWeb()
    {
        lastWebAttackTime = Time.time;
        
        if (webPrefab == null)
        {
            Debug.LogError("Web prefab not assigned!");
            return;
        }
        
        // Направление выстрела
        Vector2 direction = (Player.position - webShootPoint.position).normalized;
        
        // Создаем снаряд паутины
        GameObject web = Instantiate(webPrefab, webShootPoint.position, Quaternion.identity);
        
        // Инициализируем снаряд
        SpiderWebProjectile projectile = web.GetComponent<SpiderWebProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, webSpeed, webDamage, webSlowAmount, webSlowDuration);
        }
    }
    
    // Переопределяем Die для корректной обработки смерти
    public override void Die()
    {
        if (isDead) return;
        
        animator.SetTrigger("Death");
        base.Die();
    }
    
    void OnDrawGizmosSelected()
    {
        // Отображение радиусов атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, webShootRange);
    }
}
