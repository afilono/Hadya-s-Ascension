using UnityEngine;

// Конкретная реализация раздвижной двери
public class SlidingDoor : Door
{
    [Header("Дополнительные параметры")]
    [SerializeField] private ParticleSystem doorParticles;
    [SerializeField] private float particleDuration = 1f;
    
    // Переопределяем методы родительского класса
    public override void OpenDoor()
    {
        base.OpenDoor();
        
        // Дополнительная логика для раздвижной двери
        if (doorParticles != null)
        {
            doorParticles.Play();
            Invoke(nameof(StopParticles), particleDuration);
        }
    }
    
    public override void CloseDoor()
    {
        base.CloseDoor();
        
        // Дополнительная логика для раздвижной двери
        if (doorParticles != null)
        {
            doorParticles.Play();
            Invoke(nameof(StopParticles), particleDuration);
        }
    }
    
    private void StopParticles()
    {
        if (doorParticles != null)
        {
            doorParticles.Stop();
        }
    }
}