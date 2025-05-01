using System;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    [Header("Настройки босса")]
    public Transform Player;
    public float phase2HealthThreshold = 50f;
    private RoomManager roomManager;
    
    [Header("UI здоровья")]
    public GameObject bossHealthUI;

    private Animator animator;
    private bool isPhase2 = false;
    public bool isFlipped = false;
    private bool isPlayerInRoom = false;

    public float MaxHealth => maxHealth;

    protected override void Start()
    {
        base.Start();
        
        animator = GetComponent<Animator>();
        
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        roomManager = GetComponentInParent<RoomManager>();
        if (roomManager == null)
        {
            roomManager = FindObjectOfType<RoomManager>();
        }
        
        if (roomManager != null)
        {
            roomManager.OnPlayerEnterRoom += HandlePlayerEnterRoom;
            roomManager.OnPlayerExitRoom += HandlePlayerExitRoom;
            Debug.Log("Босс подписался на события комнаты");
        }
        
        SetAIActive(false);
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (roomManager != null)
        {
            roomManager.OnPlayerEnterRoom -= HandlePlayerEnterRoom;
            roomManager.OnPlayerExitRoom -= HandlePlayerExitRoom;
        }
    }

    void Update()
    {
        if (isPlayerInRoom)
        {
            // Добавляем вызов метода поворота в Update
            LookAtPlayer();
        
            if (!isPhase2 && currentHealth <= phase2HealthThreshold)
            {
                isPhase2 = true;
                animator.SetBool("IsPhase2", true);
                Debug.Log("Босс перешёл во вторую фазу!");
            }
        }
    }
    
    private void HandlePlayerEnterRoom()
    {
        isPlayerInRoom = true;
        Debug.Log("Игрок вошел в комнату босса!");
        
        SetAIActive(true);
        
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(true);
        }
    }
    
    private void HandlePlayerExitRoom()
    {
        isPlayerInRoom = false;
        Debug.Log("Игрок вышел из комнаты босса!");
        
        SetAIActive(false);
        
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
    }
    
    public void SetAIActive(bool active)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsActive", active);
        }
    
        if (GetComponent<BossWeapon>() != null)
            GetComponent<BossWeapon>().enabled = active;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Босс погиб!");
        
        OnEnemyDeath?.Invoke(this);
        
        Destroy(gameObject);
    }

    public void LookAtPlayer()
    {
        if (Player == null) return;

        Vector3 scale = transform.localScale;

        if (Player.position.x < transform.position.x && isFlipped)
        {
            scale.x = Mathf.Abs(scale.x);
            isFlipped = false;
        }
        else if (Player.position.x > transform.position.x && !isFlipped)
        {
            scale.x = -Mathf.Abs(scale.x);
            isFlipped = true;
        }

        transform.localScale = scale;
    }
}
