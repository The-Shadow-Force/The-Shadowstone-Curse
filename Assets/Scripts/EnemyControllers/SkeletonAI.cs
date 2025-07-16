using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SkeletonAI : MonoBehaviour
{
    [Header("Tham Chiếu")]
    private Transform player;
    private CharacterStats playerStats;
    private CharacterStats enemyStats;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Thông Số AI")]
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private float attackRange = 0.35f;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float dealDamageAfter = 0.7f;

    [Header("Tuần Tra")]
    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolPauseTime = 2f;

    private float nextAttackTime = 0f;
    private bool isPatrolling = false;
    private bool isReturningToInitial = false;
    private bool isMoving = false;

    private Vector3 initialPosition;
    private Vector3 patrolTarget;
    private bool isAttacking = false;

    void Awake()
    {
        // GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        // if (playerObject != null)
        // {
        //     player = playerObject.transform;
        //     playerStats = playerObject.GetComponent<CharacterStats>();
        // }
        // else
        // {
        //     Debug.LogError("Không tìm thấy đối tượng Player! Hãy chắc chắn tag 'Player' được gán đúng.");
        // }

        enemyStats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
    }

    private void Start()
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
    }

    void Update()
    {
        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        if (isAttacking)
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

            if (!isReturningToInitial && !isPatrolling)
            {
                StartCoroutine(ReturnAndPatrol());
            }
        }

        animator.SetBool("isRunning", isMoving);
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
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(DealDamageIfInRange), dealDamageAfter);
            Invoke(nameof(EndAttack), dealDamageAfter + 0.3f); // ✳️ Kết thúc animation sau 0.3s
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }
    private void EndAttack()
    {
        isAttacking = false;
    }

    private void DealDamageIfInRange()
    {
        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Debug.Log(playerStats.currentHealth + "máu còn lại !");
            playerStats.TakeDamage(enemyStats.damage);
        }
        else
        {
            //Debug.Log(gameObject.name + " đánh hụt!");
        }
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;
        isMoving = true;

        // Điểm tuần tra random trong hình tròn
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = initialPosition + new Vector3(randomCircle.x, randomCircle.y, 0f);

        // Xoay mặt nếu cần
        Vector3 dir = patrolTarget - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        // Di chuyển đến mục tiêu tuần tra
        while (Vector3.Distance(transform.position, patrolTarget) > 0.1f)
        {
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;
            yield return null;
        }

        isMoving = false;
        yield return new WaitForSeconds(patrolPauseTime);

        isPatrolling = false;
    }

    private IEnumerator ReturnAndPatrol()
    {
        isReturningToInitial = true;
        isMoving = true;

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
