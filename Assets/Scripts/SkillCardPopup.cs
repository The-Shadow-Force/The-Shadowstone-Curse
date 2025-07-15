using System.Linq;
using UnityEngine;

public class SkillCardPopup : MonoBehaviour
{
    [Header("Cards")]
    public CardFlipButton[] cardButtons;

    [Header("Skill Pool")]
    public SkillData[] allSkillData; // Gán 3 SkillData có sẵn trong Inspector

    void OnEnable()
    {
        // Random 3 kỹ năng khác nhau từ danh sách
        var indices = Enumerable.Range(0, allSkillData.Length)
                                .OrderBy(_ => Random.value)
                                .Take(3)
                                .ToArray();

        for (int i = 0; i < cardButtons.Length; i++)
        {
            cardButtons[i].Init(this, allSkillData[indices[i]]);
        }
    }

    public void OnSkillChosen(SkillData data)
    {
        data.LevelUp();
        Debug.Log($"Bạn đã chọn: {data.skillName}");
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Resume game nếu bạn pause trước đó
    }
}

