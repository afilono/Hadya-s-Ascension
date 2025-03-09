using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; // Speed of the projectile
    public int damage; // Damage of the projectile
    public LayerMask whatIsSolid; // Layer for collisions (walls and objects)
    public LayerMask playerLayer; // Player layer (to ignore collisions with the player)

    private Vector2 moveDirection; // Direction of the projectile

    private void Start()
    {
        // Activate the projectile to make it visible
        gameObject.SetActive(true);

        // Convert the LayerMask to a layer index
        int playerLayerIndex = LayerMaskToLayerIndex(playerLayer);

        // Validate the layer indices
        if (gameObject.layer >= 0 && gameObject.layer <= 31 && playerLayerIndex >= 0 && playerLayerIndex <= 31)
        {
            // Ignore collisions with the player layer
            Physics2D.IgnoreLayerCollision(gameObject.layer, playerLayerIndex);
        }
        else
        {
            Debug.LogError("Invalid layer numbers. Layer numbers must be between 0 and 31.");
        }
    }

    // Helper method to convert a LayerMask to a layer index
    private int LayerMaskToLayerIndex(LayerMask layerMask)
    {
        // Get the layer name from the LayerMask
        string layerName = LayerMask.LayerToName(layerMask);

        // Convert the layer name to a layer index
        int layerIndex = LayerMask.NameToLayer(layerName);

        // Return the layer index
        return layerIndex;
    }

    // Method to set the direction of the projectile
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized; // Normalize the direction
    }

    private void Update()
    {
        // Move the projectile
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // Check for collisions with objects
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDirection, speed * Time.deltaTime, whatIsSolid);
        if (hitInfo.collider != null)
        {
            // Damage the enemy (EnemyController)
            if (hitInfo.collider.CompareTag("EnemyController"))
            {
                hitInfo.collider.GetComponent<EnemyController>().TakeDamage(damage);
            }
            // Damage the boss (Boss)
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                hitInfo.collider.GetComponent<Boss>().TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy the projectile after collision
        }
    }
}