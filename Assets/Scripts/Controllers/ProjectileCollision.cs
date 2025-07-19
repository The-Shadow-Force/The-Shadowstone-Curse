// Sửa lại ProjectileCollision.cs
using System;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private SkillData skill;
    private LayerMask enemyLayers;
    private bool hasHit = false;
    private float checkRadius = 0.5f;

    public void Setup(SkillData skillData, LayerMask layers)
    {
        skill = skillData;
        enemyLayers = layers;
        Debug.Log($"ProjectileCollision setup with enemyLayers: {layers.value}");
    }

    void FixedUpdate()
    {
        if (hasHit) return;

        // Check collision mỗi FixedUpdate
        Collider2D hit = Physics2D.OverlapCircle(transform.position, checkRadius, enemyLayers);
        if (hit != null)
        {
            Debug.Log($"Projectile hit enemy {hit.name} at position {transform.position}");
            hasHit = true;
            ExplodeAndDamage();
        }
    }

    private void ExplodeAndDamage()
    {
        // Gây damage AoE tại vị trí va chạm
        float radius = skill.GetRange();
        int damage = skill.GetDamage();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayers);
        foreach (var enemy in hits)
        {
            if (enemy.CompareTag("Orc") || enemy.CompareTag("Flying_Eye") ||
                enemy.CompareTag("Skeleton") || enemy.CompareTag("Vampire") ||
                enemy.CompareTag("Rat") || enemy.CompareTag("Boss"))
            {
                Debug.Log($"Hit {enemy.name} with {skill.skillName} at level {skill.level}");

                CharacterStats stats = enemy.GetComponent<CharacterStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);

                    // Trigger animation
                    Animator enemyAnimator = enemy.GetComponent<Animator>();
                    if (enemyAnimator != null)
                    {
                        if (!enemy.CompareTag("Boss"))
                        {
                            enemyAnimator.SetTrigger("Hit");
                            if (stats.currentHealth <= 0)
                            {
                                enemyAnimator.SetTrigger("Die");
                                stats.Die();
                            }
                        }
                        else if (stats.currentHealth <= 0)
                        {
                            enemyAnimator.SetTrigger("Die");
                            stats.BossDie();
                        }
                    }
                }
            }
        }

        // Trigger animation nổ
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Hit");

        // Phát âm thanh
        if (skill.soundClipOnHit != null)
            AudioSource.PlayClipAtPoint(skill.soundClipOnHit, transform.position);

        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        if (skill != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, skill.GetRange());
        }
    }
}