using UnityEngine;

public class BossPhase2 : StateMachineBehaviour
{
    public float speed = 2.5f; // Скорость движения босса
    public float attackRange = 5f; // Радиус атаки луком
    public float retreatDistance = 3f; // Дистанция отступления
    public float attackCooldown = 3f; // Время между атаками
    public LayerMask wallMask; // Слой стен

    private Transform player; // Ссылка на игрока
    private Rigidbody2D rb; // Ссылка на Rigidbody2D босса
    private BossWeapon bossWeapon; // Ссылка на оружие босса
    private float lastAttackTime = 0f; // Время последней атаки

    // OnStateEnter вызывается при входе в состояние
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Находим игрока по тегу
        rb = animator.GetComponent<Rigidbody2D>(); // Получаем Rigidbody2D
        bossWeapon = animator.GetComponent<BossWeapon>(); // Получаем оружие босса

        Debug.Log("Босс во второй фазе: бегает и стреляет из лука!");
    }

    // OnStateUpdate вызывается на каждом кадре, пока активно состояние
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(player.position, rb.position);

        // Если игрок слишком близко, отступаем
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 direction = (rb.position - (Vector2)player.position).normalized;
            Vector2 newPos = Vector2.MoveTowards(rb.position, rb.position + direction, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }

        // Если игрок в радиусе атаки, атакуем
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("AttackRange"); // Запускаем анимацию атаки луком
                bossWeapon.EnragedAttack(); // Вызываем метод усиленной атаки
                lastAttackTime = Time.time;
                Debug.Log("Босс атакует из лука!");
            }
        }
    }

    // OnStateExit вызывается при выходе из состояния
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackRange"); // Сбрасываем триггер атаки
    }
}