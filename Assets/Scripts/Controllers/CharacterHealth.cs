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

    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        characterStats.currentHealth -= damage;

        if (healthBarUI != null)
            healthBarUI.SetHealth(characterStats.currentHealth, characterStats.maxHealth);

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
