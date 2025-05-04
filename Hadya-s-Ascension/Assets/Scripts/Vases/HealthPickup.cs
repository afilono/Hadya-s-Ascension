using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Настройки бонуса")]
    [SerializeField] private float healthAmount = 25f; // Количество восстанавливаемого здоровья
    [SerializeField] private float playerMaxHealth = 100f; // Максимальное здоровье игрока
    
    [Header("Настройки притяжения")]
    [SerializeField] private float attractionRadius = 5f; // Радиус притяжения
    [SerializeField] private float attractionSpeed = 5f; // Скорость притяжения
    [SerializeField] private float rotationSpeed = 100f; // Скорость вращения
    
    [Header("Визуальные и звуковые эффекты")]
    [SerializeField] private AudioClip pickupSound; // Звук подбора
    [SerializeField] private GameObject pickupEffect; // Эффект подбора
    
    private Transform playerTransform;
    private bool isAttracting = false;
    
    private void Start()
    {
        // Находим игрока по тегу
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
    
    private void Update()
    {
        if (playerTransform == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Начинаем притяжение, если игрок в радиусе действия
        if (distanceToPlayer <= attractionRadius)
        {
            isAttracting = true;
        }
        
        if (isAttracting)
        {
            // Движение к игроку
            transform.position = Vector2.MoveTowards(
                transform.position, 
                playerTransform.position, 
                attractionSpeed * Time.deltaTime
            );
            
            // Эффект вращения
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Находим компонент HealthSystem
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Получаем текущее здоровье
                float currentHealth = healthSystem.Health;
                
                // Добавляем здоровье только если оно не максимальное
                if (currentHealth < playerMaxHealth)
                {
                    // Рассчитываем, сколько здоровья добавить (не превышая максимум)
                    float healthToAdd = Mathf.Min(healthAmount, playerMaxHealth - currentHealth);
                    
                    // Используем отрицательный урон для добавления здоровья
                    healthSystem.AddHp(healthToAdd);
                    
                    // Проигрываем звук подбора
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }
                    
                    // Создаем эффект подбора
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }
                }
                
                // Уничтожаем бонус
                Destroy(gameObject);
            }
        }
    }
}
