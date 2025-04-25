using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Целевой объект")]
    public Transform player;
    
    [Header("Настройки следования")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Range(0.01f, 10f)]
    public float smoothTime = 0.3f;
    
    [Header("Дополнительные параметры")]
    public bool instantStartPosition = true;
    [Range(0f, 1f)]
    public float lookAheadFactor = 0.1f;

    [Header("Настройки осей")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastPlayerPosition;
    private bool isFirstFrame = true;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            lastPlayerPosition = player.position;
            
            if (instantStartPosition)
            {
                transform.position = GetTargetPosition();
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = GetTargetPosition();

        if (!followX) targetPosition.x = transform.position.x;
        if (!followY) targetPosition.y = transform.position.y;
        if (!followZ) targetPosition.z = transform.position.z;

        if (isFirstFrame && !instantStartPosition)
        {
            transform.position = targetPosition;
            isFirstFrame = false;
            return;
        }
        
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );

        lastPlayerPosition = player.position;
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition = player.position + offset;
        
        if (lookAheadFactor > 0 && !isFirstFrame)
        {
            Vector3 playerMovement = player.position - lastPlayerPosition;
            targetPosition += playerMovement * lookAheadFactor;
        }
        
        return targetPosition;
    }
}
