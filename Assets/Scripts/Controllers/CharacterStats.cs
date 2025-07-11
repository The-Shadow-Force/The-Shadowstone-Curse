using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // === CÁC CHỈ SỐ CƠ BẢN ===
    public float maxHealth = 100f; // Máu tối đa
    public float currentHealth;   // Máu hiện tại

    public float damage = 10f;    // Sát thương cơ bản
    public float speed = 5f;      // Tốc độ di chuyển

    [Header("Mana Stats")]
    public float maxMana = 50f; // Đặt maxMana = 0 cho những kẻ địch không dùng mana
    public float currentMana;

    public event Action OnStatsChanged;
    // === CÁC HÀM CHUNG ===

    public bool isPlayer = false; // Biến để xác định đây có phải là nhân vật người chơi hay không

    // Hàm được gọi khi đối tượng được tạo ra
    private void Awake()
    {
        currentHealth = maxHealth; // Bắt đầu game với đầy máu
        currentMana = maxMana;
    }

    // Hàm để nhận sát thương
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Đảm bảo máu không âm hoặc vượt max
        //Debug.Log(transform.name + " nhận " + damageAmount + " sát thương.");

        OnStatsChanged?.Invoke(); // Phát sự kiện khi máu thay đổi
        //// Kiểm tra nếu hết máu
        if (currentHealth <= 0 && isPlayer)
        {
            currentHealth = 0;
            Die();
        }
    }

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            OnStatsChanged?.Invoke(); // Phát sự kiện khi mana thay đổi
            return true; // Đủ mana để dùng
        }
        return false; // Không đủ mana
    }

    public void RestoreMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana); // Đảm bảo mana không vượt max
        OnStatsChanged?.Invoke(); // Phát sự kiện khi mana thay đổi
    }

    // Hàm xử lý khi chết
    public virtual void Die()
    {
        // Đây là nơi xử lý cái chết chung, ví dụ:
        Debug.Log(transform.name + " đã chết.");
        // Có thể thêm hiệu ứng nổ, âm thanh, sau đó hủy đối tượng
        StartCoroutine(DestroyAfterDelay(0.8f));
    }

    public virtual void BossDie()
    {
        // Đây là nơi xử lý cái chết chung, ví dụ:
        Debug.Log(transform.name + " đã chết.");
        // Có thể thêm hiệu ứng nổ, âm thanh, sau đó hủy đối tượng
        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}