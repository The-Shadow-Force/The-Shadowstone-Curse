using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
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

    [Header("Triệu hồi Hắc Hỏa")]
    [SerializeField] private GameObject darkFlamePrefab;
    [SerializeField] private Transform[] summonPositions;
    [SerializeField] private float summonDuration = 2f;
    private float nextSummonTime = 0f;

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
    private bool isAttacking = false;
    private bool isSummoning = false;
    private bool isTeleporting = false;

    [Header("Dịch Chuyển")]
    [SerializeField] private float idleAfterSummonDuration = 5f; // Idle 5s sau khi triệu hồi
    [SerializeField] private float teleportCastDuration = 0.8f;
    [SerializeField] private float teleportVanishDuration = 1f;
    [SerializeField] private string disappearTrigger = "Disappear";
    [SerializeField] private string reappearTrigger = "Reappear";

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
        if (enemyStats.currentHealth <= 200)
        {
            if (!isSummoning && Time.time >= nextSummonTime)
            {
                StartCoroutine(SummonDarkFlames());
                return;
            }
        }
        
        if (player == null || playerStats == null || playerStats.currentHealth <= 0)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerStats = playerObject.GetComponent<CharacterStats>();
            }
            else
            {
                // Nếu không tìm thấy Player, không làm gì cả
                return; 
            }
            animator.SetBool("isRunning", false);
            return;
        }

        if (isAttacking || isSummoning || isTeleporting)
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

    private IEnumerator SummonDarkFlames()
    {
        isSummoning = true;
        nextSummonTime = Time.time + 20f;
        animator.SetTrigger("Summon");

        yield return new WaitForSeconds(summonDuration);

        foreach (Transform pos in summonPositions)
        {
            Instantiate(darkFlamePrefab, pos.position, Quaternion.identity);
        }

        // Idle đứng im 5 giây sau triệu hồi
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(idleAfterSummonDuration);

        // Nếu máu ≤ 100 → thực hiện teleport
        if (enemyStats.currentHealth <= 100)
        {
            StartCoroutine(TeleportNearPlayer());
        }

        isSummoning = false;
    }

    private IEnumerator TeleportNearPlayer()
    {
        if (player == null) yield break;

        isTeleporting = true;

        animator.SetTrigger(disappearTrigger);
        SetInvisible(true);

        yield return new WaitForSeconds(teleportVanishDuration);

        Vector3 directionToBoss = (transform.position - player.position).normalized;
        float teleportDistance = Random.Range(1f, 2f);
        Vector3 newPos = player.position + directionToBoss * teleportDistance;

        transform.position = newPos;

        animator.SetTrigger(reappearTrigger);
        SetInvisible(false);

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
    }

    private void SetInvisible(bool state)
    {
        spriteRenderer.enabled = !state;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = !state;
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
            isAttacking = true;
            animator.SetTrigger("Attack");

            Invoke(nameof(DealDamageIfInRange), dealDamageAfter);
            Invoke(nameof(EndAttack), dealDamageAfter + 0.5f);

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

        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = initialPosition + new Vector3(randomCircle.x, randomCircle.y, 0f);

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
