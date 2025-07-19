using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName = "Skill";
    public string triggerName = "Skill1"; // Animator Trigger
    public GameObject effectPrefab;
    public AudioClip soundClip;
    public Sprite skillSprite;

    [Header("Dash Settings")]
    public float dashDistance = 1f;
    public float dashPerLevel = 0.2f;

    [Header("Cooldown Settings")]
    public float cooldown = 3f;
    public float lastUsedTime = -999f;

    [Header("Level Settings")]
    public int level = 1;
    public int maxLevel = 20;

    [Header("Effect Settings")]
    public float baseRange = 0.5f;
    public float rangePerLevel = 0.2f;

    public float offset = 0.5f; // Vị trí spawn hiệu ứng
    public float scalePerLevel = 0.2f;

    [Header("Damage Settings")]
    public int baseDamage = 5;
    public int damagePerLevel = 2;

    [Header("Sound Settings")]
    public AudioClip soundClipOnHit;

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void TriggerCooldown()
    {
        lastUsedTime = Time.time;
    }

    public void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;
            Debug.Log($"{skillName} đã lên cấp {level}");
        }
        else
        {
            Debug.Log($"{skillName} đã đạt cấp tối đa!");
        }
    }

    // Tính toán các thuộc tính theo cấp
    public float GetRange()
    {
        return baseRange + (level - 1) * rangePerLevel;
    }

    public float GetDashDistance()
    {
        return dashDistance + (level - 1) * dashPerLevel;
    }

    public int GetDamage()
    {
        return baseDamage + (level - 1) * damagePerLevel;
    }

    public float GetEffectScale()
    {
        return 1f + (level - 1) * scalePerLevel;
    }
}
