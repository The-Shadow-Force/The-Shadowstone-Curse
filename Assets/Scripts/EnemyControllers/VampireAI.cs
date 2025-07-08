using UnityEngine;
using System.Collections;

public class VampirenAI : MonoBehaviour
{
    [Header("Tham Chiếu")]
    private Transform player;
    private CharacterStats playerStats;
    private CharacterStats enemyStats;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Thông Số AI")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float dealDamageAfter = 0.2f;

    [Header("Tuần Tra")]
    [SerializeField] private float patrolRadius = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolPauseTime = 2f;

    private float nextAttackTime = 0f;
    private bool isPatrolling = false;
    private bool isReturningToInitial = false;
    private Vector3 initialPosition;
    private Vector3 patrolTarget;
    private bool isMoving = false;

    void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerStats = playerObject.GetComponent<CharacterStats>();
        }
        else
        {
            Debug.LogError("Không tìm thấy đối tượng Player! Hãy chắc chắn tag 'Player' được gán đúng.");
        }

        enemyStats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isRunning", false);
            StopAllCoroutines();
            isPatrolling = false;
            isReturningToInitial = false;
            isMoving = false;
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            StopAllCoroutines();
            isPatrolling = false;
            isReturningToInitial = false;
            isMoving = true;
            Chase();
        }
        else
        {
            animator.SetBool("isRunning", false);
            isMoving = false;

            if (!isReturningToInitial && !isPatrolling)
            {
                StartCoroutine(ReturnAndPatrol());
            }
        }
    }

    private void Chase()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * enemyStats.speed * Time.deltaTime;

        if (direction.x != 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (direction.x > 0 ? 1 : -1);
            transform.localScale = localScale;
        }

        animator.SetBool("isRunning", true);
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            Invoke(nameof(DealDamageIfInRange), dealDamageAfter);
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    private void DealDamageIfInRange()
    {
        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            playerStats.TakeDamage(enemyStats.damage);
            enemyStats.currentHealth += 10;
            enemyStats.currentHealth = Mathf.Min(enemyStats.currentHealth, enemyStats.maxHealth);
        }
        else
        {
            Debug.Log(gameObject.name + " đánh hụt!");
        }
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;
        isMoving = true;

        // Chọn điểm tuần tra ngẫu nhiên trong hình tròn quanh vị trí gốc
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = initialPosition + new Vector3(randomCircle.x, randomCircle.y, 0f);

        // Xoay mặt theo hướng tuần tra
        Vector3 dir = patrolTarget - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        animator.SetBool("isRunning", true);

        while (Vector3.Distance(transform.position, patrolTarget) > 0.1f)
        {
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isRunning", false);
        isMoving = false;

        yield return new WaitForSeconds(patrolPauseTime);

        isPatrolling = false;
    }

    private IEnumerator ReturnAndPatrol()
    {
        isReturningToInitial = true;
        isMoving = true;
        animator.SetBool("isRunning", true);

        while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
        {
            Vector3 direction = (initialPosition - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;

            if (direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? 1 : -1);
                transform.localScale = scale;
            }

            yield return null;
        }

        animator.SetBool("isRunning", false);
        isMoving = false;

        yield return new WaitForSeconds(patrolPauseTime);

        isReturningToInitial = false;
        StartCoroutine(Patrol());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? initialPosition : transform.position, patrolRadius);
    }
}
