using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [Header("Melee Attack")]
    public int attackDamage = 20;
    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    [Header("Ranged Attack")]
    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public int arrowDamage = 40;
    public Transform shootPoint;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Attack()
    {
        Vector3 pos = transform.position + attackOffset;
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        
        if (colInfo != null)
        {
            // Ищем компонент IDamageable вместо PlayerController
            IDamageable damageable = colInfo.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
            else
            {
                // Если интерфейс не найден, можно попробовать найти HealthSystem напрямую
                HealthSystem healthSystem = colInfo.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab not assigned!");
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null) return;
        }

        if (shootPoint == null)
            shootPoint = transform;

        Vector2 direction = (player.position - shootPoint.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

        BossArrowProjectile projectile = arrow.GetComponent<BossArrowProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, arrowSpeed, arrowDamage);
        }
        else
        {
            Debug.LogError("BossArrowProjectile component missing on arrow prefab!");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Визуализация радиуса атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + attackOffset, attackRange);
    }
}
