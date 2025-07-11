using UnityEngine;
using System.Collections;

public class FlyingEyeAI : MonoBehaviour
{
    [Header("Tham Chiếu")]
    private Transform player;
    private CharacterStats playerStats;
    private CharacterStats enemyStats;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Thông Số AI")]
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float dealDamageAfter = 0.5f;

    [Header("Hiệu Ứng Cắn")]
    [SerializeField] private float diveSpeed = 2.5f;
    [SerializeField] private float preDiveDelay = 0.2f;
    [SerializeField] private float postAttackDelay = 0.1f;

    [Header("Trinh Sát")]
    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolPauseTime = 2f;

    private float nextAttackTime = 0f;
    private bool isDiving = false;
    private bool isPatrolling = false;
    private bool isReturningToInitial = false;

    private Vector3 initialPosition;
    private Vector3 patrolTarget;

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
            Debug.LogError("Không tìm thấy Player! Gắn đúng tag 'Player'.");
        }

        enemyStats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isDiving || isReturningToInitial) return;

        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
        {
            animator.SetBool("isFlying", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isFlying", false);
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            StopAllCoroutines();
            isPatrolling = false;
            Chase();
        }
        else
        {
            animator.SetBool("isFlying", false);
            if (!isPatrolling)
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

        animator.SetBool("isFlying", true);
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            StartCoroutine(DiveThenAttackThenFlyBack());
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    private IEnumerator DiveThenAttackThenFlyBack()
    {
        isDiving = true;
        Vector3 startPos = transform.position;

        if (player != null)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (player.position.x > transform.position.x ? 1 : -1);
            transform.localScale = scale;
        }

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(preDiveDelay);

        Vector3 diveTarget = player != null ? player.position + new Vector3(0f, -0.1f, 0f) : transform.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * diveSpeed;
            transform.position = Vector3.Lerp(startPos, diveTarget, t);
            yield return null;
        }

        yield return new WaitForSeconds(dealDamageAfter);

        if (player != null && playerStats != null && playerStats.currentHealth > 0)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                playerStats.TakeDamage(enemyStats.damage);
            }
            else
            {
                //Debug.Log(gameObject.name + " cắn hụt!");
            }
        }

        yield return new WaitForSeconds(postAttackDelay);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * diveSpeed;
            transform.position = Vector3.Lerp(diveTarget, startPos, t);
            yield return null;
        }

        isDiving = false;
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;

        patrolTarget = initialPosition + (Vector3)Random.insideUnitCircle * patrolRadius;

        Vector3 dir = patrolTarget - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        animator.SetBool("isFlying", true);

        while (Vector3.Distance(transform.position, patrolTarget) > 0.1f)
        {
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isFlying", false);
        yield return new WaitForSeconds(patrolPauseTime);

        isPatrolling = false;
    }

    private IEnumerator ReturnAndPatrol()
    {
        isReturningToInitial = true;

        animator.SetBool("isFlying", true);

        while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
        {
            Vector3 direction = (initialPosition - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? 1 : -1);
            transform.localScale = scale;

            yield return null;
        }

        animator.SetBool("isFlying", false);
        yield return new WaitForSeconds(patrolPauseTime);

        isReturningToInitial = false;
        StartCoroutine(Patrol());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? initialPosition : transform.position, patrolRadius);
    }
}
