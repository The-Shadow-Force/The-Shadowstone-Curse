using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.H)) // Nhấn phím H để test
    //    {
    //        TakeDamage(1);
    //    }
    //}

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Dead");

        // Tùy chọn: Disable chuyển động và các tương tác khác
        GetComponent<CharacterMove>().enabled = false;
        GetComponent<CharacterAttack>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // Ngăn bị đẩy sau khi chết (tùy chọn)
        GetComponent<Collider2D>().enabled = false; // Không va chạm nữa (nếu cần)
    }
}
