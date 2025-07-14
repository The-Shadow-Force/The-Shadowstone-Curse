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
                Debug.Log("Hit " + enemy.name);
                // enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage); khi nào có thì bỏ comment dòng này
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
