using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public Transform Player; // Ссылка на игрока
    public int maxHealth = 100; // Максимальное здоровье босса
    public int currentHealth; // Текущее здоровье босса
    public int phase2HealthThreshold = 50; // Порог здоровья для перехода во вторую фазу

    private Animator animator; // Ссылка на аниматор
    private bool isPhase2 = false; // Флаг второй фазы
    public bool isFlipped = false;

    public delegate void BossDeathHandler();
    public static event BossDeathHandler OnBossDeath;

    public Boss boss;
    public Slider healthSlider;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Автоматически находим игрока, если он не назначен
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (boss == null)
        {
            boss = FindObjectOfType<Boss>();
            if (boss == null) Debug.LogError("Boss not found in the scene!");
        }

        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
            if (healthSlider == null) Debug.LogError("Health Slider not assigned!");
        }

        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
    }

    void Update()
    {
        // Проверяем здоровье для перехода во вторую фазу
        if (!isPhase2 && currentHealth <= phase2HealthThreshold)
        {
            isPhase2 = true;
            animator.SetBool("IsPhase2", true); // Используем булевый параметр вместо триггера
            Debug.Log("Босс перешёл во вторую фазу!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Здоровье не может быть меньше 0

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Босс погиб!");
        // Здесь можно добавить логику смерти босса (например, анимацию или завершение уровня)
        OnBossDeath?.Invoke();
        Destroy(gameObject);
    }


    public void LookAtPlayer()
    {
        if (Player == null) return; // Проверяем, что игрок назначен

        // Получаем текущий масштаб объекта
        Vector3 scale = transform.localScale;

        // Если игрок находится слева от босса и босс не отражён
        if (Player.position.x < transform.position.x && isFlipped)
        {
            scale.x = Mathf.Abs(scale.x); // Отражаем спрайт по горизонтали
            isFlipped = false;
        }
        // Если игрок находится справа от босса и босс отражён
        else if (Player.position.x > transform.position.x && !isFlipped)
        {
            scale.x = -Mathf.Abs(scale.x); // Возвращаем спрайт в исходное состояние
            isFlipped = true;
        }

        // Применяем новый масштаб
        transform.localScale = scale;
    }
}