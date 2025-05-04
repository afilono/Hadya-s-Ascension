using System.Collections;
using UnityEngine;

public class StatusEffectFlash : MonoBehaviour
{
    [Header("Общие настройки")]
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Эффект урона")]
    [SerializeField] [ColorUsage(true, true)] private Color damageColor = Color.red;
    [SerializeField] private float damageDuration = 0.3f;
    
    [Header("Эффект лечения")]
    [SerializeField] [ColorUsage(true, true)] private Color healColor = Color.green;
    [SerializeField] private float healDuration = 0.3f;
    
    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;
    private Coroutine activeFlashCoroutine;

    private void Awake()
    {
        // Получаем все SpriteRenderer объекта и его дочерних объектов
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        materials = new Material[spriteRenderers.Length];
        
        // Создаем экземпляры материалов для каждого спрайта
        SetupMaterials();
    }

    private void SetupMaterials()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            // Создаем экземпляр материала, чтобы изменения не влияли на другие объекты
            materials[i] = new Material(spriteRenderers[i].material);
            spriteRenderers[i].material = materials[i];
        }
    }

    /// <summary>
    /// Запускает эффект получения урона (красная вспышка)
    /// </summary>
    public void FlashDamage()
    {
        PlayFlashEffect(damageColor, damageDuration);
    }

    /// <summary>
    /// Запускает эффект лечения (зеленая вспышка)
    /// </summary>
    public void FlashHealing()
    {
        PlayFlashEffect(healColor, healDuration);
    }

    /// <summary>
    /// Запускает эффект с произвольным цветом и длительностью
    /// </summary>
    public void FlashCustom(Color color, float duration)
    {
        PlayFlashEffect(color, duration);
    }

    private void PlayFlashEffect(Color flashColor, float duration)
    {
        // Остановить текущий эффект, если он выполняется
        if (activeFlashCoroutine != null)
            StopCoroutine(activeFlashCoroutine);
            
        // Запустить новый эффект
        activeFlashCoroutine = StartCoroutine(FlashRoutine(flashColor, duration));
    }

    private IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        // Устанавливаем цвет для каждого материала
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_FlashColor", flashColor);
        }

        float elapsed = 0;
        
        while (elapsed < duration)
        {
            // Вычисляем интенсивность эффекта на основе кривой
            float flashAmount = flashCurve.Evaluate(elapsed / duration);
            
            // Устанавливаем интенсивность для каждого материала
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_FlashAmount", flashAmount);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Убеждаемся, что эффект полностью исчез
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_FlashAmount", 0);
        }
        
        activeFlashCoroutine = null;
    }

    // Для совместимости со старым кодом
    public void Flash()
    {
        FlashDamage();
    }
}
