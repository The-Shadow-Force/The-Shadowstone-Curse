using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage; // Ảnh fill (type = Filled)
    public Sprite[] healthSprites; // 5 sprite theo mức máu (máu yếu → đầy)

    public void SetHealth(int currentHealth, int maxHealth)
    {
        float ratio = (float)currentHealth / maxHealth;
        fillImage.fillAmount = ratio;

        // Tính index sprite
        int index = 0;

        if (ratio >= 0.99f) index = 4;         // 100%
        else if (ratio >= 0.8f) index = 3;     // 80-99%
        else if (ratio >= 0.6f) index = 2;     // 60-79%
        else if (ratio >= 0.4f) index = 1;     // 40-59%
        else index = 0;                        // 0-39%

        if (healthSprites != null && healthSprites.Length == 5)
        {
            fillImage.sprite = healthSprites[index];
        }
    }
}
