using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = Vector2.right;
    
    private HealthSystem healthSystem;
    private ShootingSystem shootingSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }
        
        shootingSystem = GetComponent<ShootingSystem>();
        if (shootingSystem == null)
        {
            shootingSystem = gameObject.AddComponent<ShootingSystem>();
        }
        
        healthSystem.OnDeath += OnPlayerDeath;
    }

    void Update()
    {
        HandleMovementInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }
    
    void OnPlayerDeath()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        movement = new Vector2(horizontal, vertical);
        
        if (movement.magnitude > 0)
        {
            movement.Normalize();
            lastNonZeroDirection = movement;
        }
    }
    
    void UpdateAnimation()
    {
        if (animator == null) return;
        
        int direction = 0;
        
        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                direction = movement.x > 0 ? 4 : 3;
            }
            else
            {
                direction = movement.y > 0 ? 1 : 2;
            }
        }
        
        animator.SetInteger("MovementDirection", direction);
    }
}
