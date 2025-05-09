using UnityEngine;

public class BossPhase2 : StateMachineBehaviour
{
    [Header("Settings")]
    public float speed = 2.5f;
    public float attackRange = 5f;
    public float retreatDistance = 3f;
    public float attackCooldown = 3f;
    public LayerMask wallMask;
    public float repositionDistance = 2f;

    private Transform player;
    private Rigidbody2D rb;
    private BossWeapon weapon;
    private float lastAttackTime;
    private Vector2 targetPosition;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        weapon = animator.GetComponent<BossWeapon>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        // �����������
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 retreatDir = (rb.position - (Vector2)player.position).normalized;
            MoveBoss(retreatDir);
        }
        else if (distanceToPlayer > attackRange)
        {
            Vector2 approachDir = ((Vector2)player.position - rb.position).normalized;
            MoveBoss(approachDir);
        }

        // �����
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("AttackRange");
            lastAttackTime = Time.time;
        }
    }

    private void MoveBoss(Vector2 direction)
    {
        // �������� ������� Raycast �� BoxCast ��� ����� ��������� ����������� ����
        Vector2 boxSize = new Vector2(1f, 1.5f); // ������ ������������� ���������� �����
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, boxSize, 0f, direction, 
            speed * Time.fixedDeltaTime, wallMask);
    
        if (hit.collider == null)
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            Vector2 alternateDirection = new Vector2(direction.y, -direction.x).normalized;
            hit = Physics2D.BoxCast(rb.position, boxSize, 0f, alternateDirection, 
                speed * Time.fixedDeltaTime, wallMask);
            if (hit.collider == null)
            {
                rb.MovePosition(rb.position + alternateDirection * speed * Time.fixedDeltaTime * 0.5f);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackRange");
    }
}