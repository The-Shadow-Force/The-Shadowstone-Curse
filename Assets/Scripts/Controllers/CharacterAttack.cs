using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int damage = 1;

    [Header("References")]
    public Animator animator;
    public float offset = 0.5f;

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
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            //enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage); nào có thì bỏ comment
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
