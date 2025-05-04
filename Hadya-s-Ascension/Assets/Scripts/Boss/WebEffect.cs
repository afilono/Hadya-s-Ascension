using UnityEngine;
using System.Collections;

public class WebEffect : MonoBehaviour
{
    private PlayerController playerMovement;
    private float originalSpeed;
    private Coroutine activeSlowCoroutine;
    
    void Start()
    {
        // Находим компонент управления движением игрока
        playerMovement = GetComponent<PlayerController>();
        if (playerMovement != null)
        {
            originalSpeed = playerMovement.moveSpeed;
        }
        else
        {
            Debug.LogError("PlayerMovement component not found!");
        }
    }
    
    public void ApplySlow(float slowAmount, float duration)
    {
        if (playerMovement == null) return;
        
        // Если уже есть активное замедление, остановим его
        if (activeSlowCoroutine != null)
        {
            StopCoroutine(activeSlowCoroutine);
        }
        
        // Запускаем новый эффект замедления
        activeSlowCoroutine = StartCoroutine(SlowPlayer(slowAmount, duration));
    }
    
    private IEnumerator SlowPlayer(float slowAmount, float duration)
    {
        // Запоминаем оригинальную скорость
        if (originalSpeed <= 0)
        {
            originalSpeed = playerMovement.moveSpeed;
        }
        
        // Применяем замедление
        playerMovement.moveSpeed = originalSpeed * slowAmount;
        
        // Визуальный эффект для игрока
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            renderer.color = new Color(0.5f, 0.5f, 1f); // Синеватый оттенок
            
            yield return new WaitForSeconds(duration);
            
            renderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
        
        // Восстанавливаем нормальную скорость
        playerMovement.moveSpeed = originalSpeed;
        activeSlowCoroutine = null;
    }
    
    void OnDestroy()
    {
        // Восстанавливаем скорость при уничтожении компонента
        if (playerMovement != null && originalSpeed > 0)
        {
            playerMovement.moveSpeed = originalSpeed;
        }
    }
}
