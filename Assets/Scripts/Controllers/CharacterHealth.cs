using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;
    public CharacterStats characterStats;

    [Header("Audio Clips")]
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("UI")]
    public HealthBarUI healthBarUI;

    private AudioSource audioSource;
    private float previousHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (characterStats == null)
            characterStats = GetComponent<CharacterStats>();

        // Gán AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (healthBarUI != null)
            healthBarUI.SetHealth(characterStats.currentHealth, characterStats.maxHealth);

        previousHealth = characterStats.currentHealth;
    }

    private void Update()
    {
        // Kiểm tra nếu máu giảm (có nghĩa là bị tấn công)
        if (characterStats.currentHealth < previousHealth && !isDead)
        {
            // Kiểm tra nếu nhân vật vẫn còn sống
            if (characterStats.currentHealth > 0)
            {
                // Thêm animation và âm thanh cho player
                animator.SetTrigger("Hurt");

                // Phát âm thanh bị thương
                if (hurtClip != null)
                    audioSource.PlayOneShot(hurtClip);
            }
            else
            {
                // Nếu chết thì gọi Die() của CharacterHealth
                Die();
            }
        }

        // Cập nhật máu trước đó
        previousHealth = characterStats.currentHealth;
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        characterStats.currentHealth -= damage;

        //if (healthBarUI != null)
        //    healthBarUI.SetHealth(characterStats.currentHealth, characterStats.maxHealth);
        characterStats.TakeDamage(damage);

        if (characterStats.currentHealth > 0)
        {
            animator.SetTrigger("Hurt");

            // ✅ Phát âm thanh bị thương
            if (hurtClip != null)
                audioSource.PlayOneShot(hurtClip);
        }
        else
        {
            Die();
        }
    }

    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Dead");

        // ✅ Phát âm thanh chết
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        // Tắt chuyển động và tương tác
        GetComponent<CharacterMove>().enabled = false;
        GetComponent<CharacterAttack>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        Invoke("LoadGameOverScene", 2f);
    }
}
