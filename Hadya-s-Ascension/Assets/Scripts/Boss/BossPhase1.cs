using UnityEngine;

public class BossPhase1 : StateMachineBehaviour
{
    public float speed = 2.5f; // Скорость движения босса
    public float attackRange = 3f; // Радиус атаки мечом
    public float attackCooldown = 2f; // Время между атаками
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
    }

    // OnStateUpdate вызывается на каждом кадре, пока активно состояние
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        // Двигаем босса к игроку, если нет стены на пути
        Vector2 direction = (player.position - rb.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, speed * Time.fixedDeltaTime, wallMask);
        if (hit.collider == null)
        {
            Vector2 target = new Vector2(player.position.x, player.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }

        // Если босс близко к игроку, запускаем атаку
        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("AttackMelee"); // Запускаем анимацию атаки мечом
                bossWeapon.Attack(); // Вызываем метод атаки
                lastAttackTime = Time.time;
            }
        }
    }

    // OnStateExit вызывается при выходе из состояния
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackMelee"); // Сбрасываем триггер атаки
    }
}