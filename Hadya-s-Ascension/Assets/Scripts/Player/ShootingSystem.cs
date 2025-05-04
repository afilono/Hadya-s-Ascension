using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private int projectileDamage = 10;
    [SerializeField] private AudioSource fireSound;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool rotateTowardsMouse = true;
    [SerializeField] private bool drawDebugLine = true;
    [SerializeField] private Color debugLineColor = Color.red;
    
    private Camera mainCamera;
    private float lastFireTime = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
            {
                mainCamera = cameras[0];
            }
        }
    }

    private void Start()
    {
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (mainCamera == null) return;
        
        Vector3 mousePosition = GetMouseWorldPosition();
        if (mousePosition == Vector3.zero) return;
        
        Vector2 direction = ((Vector2)(mousePosition - firePoint.position)).normalized;
        
        if (drawDebugLine)
        {
            Debug.DrawLine(firePoint.position, mousePosition, debugLineColor);
            Debug.DrawRay(firePoint.position, direction * 3f, Color.green);
        }
        
        if (rotateTowardsMouse && firePoint != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Input.GetMouseButton(0) && Time.time >= lastFireTime + fireCooldown)
        {
            Vector3 currentMousePos = GetMouseWorldPosition();
            Vector2 fireDirection = ((Vector2)(currentMousePos - firePoint.position)).normalized;
            
            FireProjectile(fireDirection);
            lastFireTime = Time.time;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane;
        
        if (mainCamera.orthographic)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            worldPosition.z = 0;
            return worldPosition;
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
            float distance = -ray.origin.z / ray.direction.z;
            Vector3 worldPosition = ray.origin + ray.direction * distance;
            return worldPosition;
        }
    }

    void FireProjectile(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, rotation);
        projectile.transform.SetParent(null);
        
        fireSound?.Play();
        
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetDirection(direction);
            projectileComponent.speed = projectileSpeed;
            projectileComponent.damage = projectileDamage;
        }
        else
        {
            Destroy(projectile);
        }
    }
}
