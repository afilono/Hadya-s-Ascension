using UnityEngine;

public class Boss1PhaseAttack : StateMachineBehaviour
{
    public float speed = 2.5f; // �������� �������� �����
    public float attackRange = 3f; // ������ �����
    public float attackCooldown = 2f; // ����� ����� �������
    public LayerMask wallMask; // ���� ����

    Transform player; // ������ �� ������
    Rigidbody2D rb; // ������ �� Rigidbody2D �����
    Boss boss; // ������ �� ������ Boss
    BossWeapon bossWeapon; // ������ �� ������ �����
    float lastAttackTime = 0f; // ����� ��������� �����

    // OnStateEnter ���������� ��� ����� � ���������
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // ������� ������ �� ����
        rb = animator.GetComponent<Rigidbody2D>(); // �������� Rigidbody2D
        boss = animator.GetComponent<Boss>(); // �������� ������ Boss
        bossWeapon = animator.GetComponent<BossWeapon>(); // �������� ������ �����

        // ��������, ��� ����� �������� � Boss
        if (boss != null && boss.Player == null)
        {
            boss.Player = player;
        }
    }

    // OnStateUpdate ���������� �� ������ �����, ���� ������� ���������
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || player == null) return; // ���������, ��� boss � player ���������

        boss.LookAtPlayer(); // ������������ ����� � ������

        // ������� ����� � ������, ���� ��� ����� �� ����
        Vector2 direction = (player.position - rb.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, speed * Time.fixedDeltaTime, wallMask);
        if (hit.collider == null)
        {
            Vector2 target = new Vector2(player.position.x, player.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }

        // ���� ���� ������ � ������, ��������� �����
        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("AttackMelee"); // ��������� �������� �����
                bossWeapon.Attack(); // �������� ����� �����
                lastAttackTime = Time.time;
            }
        }
    }

    // OnStateExit ���������� ��� ������ �� ���������
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackMelee"); // ���������� ������� �����
    }
}