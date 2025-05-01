using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    [Header("Визуал двери")]
    [SerializeField] private GameObject doorVisual; // Изменено на GameObject
    [Header("Параметры движения")]
    [SerializeField] private Vector3 closedPositionOffset;
    [SerializeField] private Vector3 openPositionOffset;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float openedThreshold = 0.01f; // Порог для определения открытия
    
    [Header("Звуковые эффекты")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    // События двери
    public event Action OnDoorOpened;
    public event Action OnDoorClosed;
    
    // Состояние двери
    private bool isOpen = true;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 targetPosition;
    private AudioSource audioSource;
    private bool isDoorFullyOpen = false;
    
    protected virtual void Awake()
    {
        // Сохраняем начальную позицию и вращение
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Настройка аудио
        SetupAudio();
        
        // Если doorVisual не установлен, попробуем найти его среди дочерних объектов
        if (doorVisual == null && transform.childCount > 0)
        {
            doorVisual = transform.GetChild(0).gameObject;
        }
    }
    
    void Start()
    {
        // Устанавливаем начальное целевое положение
        SetTargetPositionBasedOnState();
        
        // Убедимся, что дверь видна при старте
        if (doorVisual != null)
        {
            doorVisual.SetActive(true);
        }
    }
    
    void Update()
    {
        // Плавное движение к целевой позиции
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // Проверка, полностью ли открыта дверь
        if (isOpen && !isDoorFullyOpen)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget < openedThreshold)
            {
                isDoorFullyOpen = true;
                
                // Выключаем объект двери, когда она полностью открыта
                if (doorVisual != null)
                {
                    doorVisual.SetActive(false);
                }
            }
        }
    }
    
    public virtual void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            isDoorFullyOpen = false;
            
            // Включаем объект двери при начале открытия
            if (doorVisual != null)
            {
                doorVisual.SetActive(true);
            }
            
            SetTargetPositionBasedOnState();
            PlaySound(openSound);
            OnDoorOpened?.Invoke();
        }
    }
    
    public virtual void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            isDoorFullyOpen = false;
            
            // Включаем дверь при начале закрытия
            if (doorVisual != null && !doorVisual.activeSelf)
            {
                doorVisual.SetActive(true);
            }
            
            SetTargetPositionBasedOnState();
            PlaySound(closeSound);
            OnDoorClosed?.Invoke();
        }
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
    
    private void SetTargetPositionBasedOnState()
    {
        // Выбираем смещение в зависимости от состояния двери
        Vector3 offset = isOpen ? openPositionOffset : closedPositionOffset;
        
        // Применяем смещение с учетом начального вращения
        targetPosition = TransformPointWithInitialRotation(offset);
    }
    
    private Vector3 TransformPointWithInitialRotation(Vector3 offset)
    {
        // Создаем матрицу трансформации, сохраняющую начальное вращение
        Matrix4x4 transformMatrix = Matrix4x4.TRS(initialPosition, initialRotation, Vector3.one);
        
        // Применяем смещение с учетом начального вращения
        return transformMatrix.MultiplyPoint3x4(offset);
    }
    
    private void SetupAudio()
    {
        if (openSound != null || closeSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // 3D звук
            }
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
    // Метод для сброса позиции к начальной
    public void ResetToInitialPosition()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        // Сбрасываем состояние
        isDoorFullyOpen = false;
        
        // Включаем визуал при сбросе позиции
        if (doorVisual != null)
        {
            doorVisual.SetActive(true);
        }
    }
}
