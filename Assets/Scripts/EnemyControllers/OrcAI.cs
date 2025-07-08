using UnityEngine;

public class OrcAI : MonoBehaviour
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

    private float nextAttackTime = 0f;

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
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            Chase();
        }
        else
        {
            animator.SetBool("isRunning", false);
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
            // Phát Animation Attack ngẫu nhiên
            int randomAttack = Random.Range(0, 2);
            if (randomAttack == 0)
            {
                animator.SetTrigger("Attack1");
            }
            else
            {
                animator.SetTrigger("Attack2");
            }

            // Gọi hàm gây sát thương sau 0.2s
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
            //Debug.Log(gameObject.name + " gây sát thương cho Player!");
            playerStats.TakeDamage(enemyStats.damage);
        }
        else
        {
            Debug.Log(gameObject.name + " đánh hụt!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
