using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIController : MonoBehaviour
{
    public SkillData skillData;
    public Image iconImage;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;

    private void Update()
    {
        float cooldownRemaining = (skillData.lastUsedTime + skillData.cooldown) - Time.time;

        if (cooldownRemaining > 0f)
        {
            // Hiện overlay và text
            cooldownOverlay.gameObject.SetActive(true);
            cooldownText.gameObject.SetActive(true);

            // Cập nhật overlay dạng radial fill
            float fillAmount = cooldownRemaining / skillData.cooldown;
            cooldownOverlay.fillAmount = fillAmount;

            // Cập nhật text làm tròn
            cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
        }
        else
        {
            // Ẩn overlay và text
            cooldownOverlay.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }
    }
}
