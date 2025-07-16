using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.25f;
    public LayerMask enemyLayers;
    public int damage = 1;

    [Header("References")]
    public Animator animator;
    public float offset = 0.5f;
    public AudioSource audioSource;
    [Header("Audio")]
    public AudioClip attackSound;

    private Vector2 lastDirection = Vector2.right;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Gọi animation chém
            animator.SetTrigger("Attack");

            // Cập nhật hướng AttackPoint
            Vector2 direction = new Vector2(
                animator.GetFloat("LastMoveX"),
                animator.GetFloat("LastMoveY")
            ).normalized;

            if (direction != Vector2.zero)
                lastDirection = direction;

            attackPoint.localPosition = lastDirection * offset;

            // Gọi hàm xử lý va chạm
            Attack();
            if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("Enemy detected: " + enemy.name);
            if (enemy.CompareTag("Orc") || enemy.CompareTag("Flying_Eye") ||
    enemy.CompareTag("Skeleton") || enemy.CompareTag("Vampire") || enemy.CompareTag("Rat") || enemy.CompareTag("Boss"))
            {
                //Debug.Log("Hit " + enemy.name);

                CharacterStats stats = enemy.GetComponent<CharacterStats>();
                if (stats != null)
                {
                    stats.TakeDamage(10);
                    Animator enemyAnimator = enemy.GetComponent<Animator>();
                    if (enemyAnimator != null)
                    {
                        if (!enemy.CompareTag("Boss"))
                        {
                            enemyAnimator.SetTrigger("Hit"); // ✅ Bật animation Hit ở đúng đối tượng bị đánh
                            if (stats.currentHealth <= 0)
                            {
                                enemyAnimator.SetTrigger("Die"); // ✅ Bật animation Die ở đúng đối tượng bị đánh
                                stats.Die(); // Gọi hàm Die nếu có
                            }
                        }
                        else
                        {
                            if (stats.currentHealth <= 0)
                            {
                                enemyAnimator.SetTrigger("Die"); // ✅ Bật animation Die ở đúng đối tượng bị đánh
                                stats.BossDie(); // Gọi hàm Die nếu có
                            }
                        }
                    }
                }
            }

            // Tính vector từ player đến enemy
            Vector2 toEnemy = (enemy.transform.position - attackPoint.position).normalized;

            // Vector hướng chém (dựa trên LastMoveX/Y)
            Vector2 attackDirection = new Vector2(
                animator.GetFloat("LastMoveX"),
                animator.GetFloat("LastMoveY")
            ).normalized;

            // Tính góc giữa hướng chém và enemy
            float dot = Vector2.Dot(toEnemy, attackDirection);

            if (dot > 0) //  > 0 nghĩa là nằm trong nửa phía trước (0°–180°)
            {
                //Debug.Log("Hit " + enemy.name);

                
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
