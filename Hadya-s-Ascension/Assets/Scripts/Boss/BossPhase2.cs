using UnityEngine;

public class BossPhase2 : StateMachineBehaviour
{
    public float speed = 2.5f; // �������� �������� �����
    public float attackRange = 5f; // ������ ����� �����
    public float retreatDistance = 3f; // ��������� �����������
    public float attackCooldown = 3f; // ����� ����� �������
    public LayerMask wallMask; // ���� ����

    private Transform player; // ������ �� ������
    private Rigidbody2D rb; // ������ �� Rigidbody2D �����
    private BossWeapon bossWeapon; // ������ �� ������ �����
    private float lastAttackTime = 0f; // ����� ��������� �����

    // OnStateEnter ���������� ��� ����� � ���������
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // ������� ������ �� ����
        rb = animator.GetComponent<Rigidbody2D>(); // �������� Rigidbody2D
        bossWeapon = animator.GetComponent<BossWeapon>(); // �������� ������ �����

        Debug.Log("���� �� ������ ����: ������ � �������� �� ����!");
    }

    // OnStateUpdate ���������� �� ������ �����, ���� ������� ���������
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(player.position, rb.position);

        // ���� ����� ������� ������, ���������
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 direction = (rb.position - (Vector2)player.position).normalized;
            Vector2 newPos = Vector2.MoveTowards(rb.position, rb.position + direction, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }

        // ���� ����� � ������� �����, �������
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("AttackRange"); // ��������� �������� ����� �����
                bossWeapon.EnragedAttack(); // �������� ����� ��������� �����
                lastAttackTime = Time.time;
                Debug.Log("���� ������� �� ����!");
            }
        }
    }

    // OnStateExit ���������� ��� ������ �� ���������
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackRange"); // ���������� ������� �����
    }
}