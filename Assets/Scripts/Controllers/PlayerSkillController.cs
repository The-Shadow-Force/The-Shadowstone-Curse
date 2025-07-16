using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Skill Settings")]
    public Transform attackPoint;
    public Transform skillSpawnPoint;
    public LayerMask enemyLayers;

    public SkillData skill1;
    public SkillData skill2;
    public SkillData ultimate;

    private SkillData currentSkill;

    [Header("References")]
    public Animator animator;
    public AudioSource audioSource;

    private Vector2 lastDirection = Vector2.right;

    void Start()
    {
        skill1.lastUsedTime = -999f;
        skill2.lastUsedTime = -999f;
        ultimate.lastUsedTime = -999f;
    }

    void Update()
    {
        Vector2 inputDir = new Vector2(
            animator.GetFloat("LastMoveX"),
            animator.GetFloat("LastMoveY")
        ).normalized;

        if (inputDir != Vector2.zero)
            lastDirection = inputDir;

        if (Input.GetKeyDown(KeyCode.Q) && skill1.CanUse())
        {
            UseSkill(skill1);
        }
        else if (Input.GetKeyDown(KeyCode.W) && skill2.CanUse())
        {
            UseSkill(skill2);
        }
        else if (Input.GetKeyDown(KeyCode.E) && ultimate.CanUse())
        {
            UseSkill(ultimate);
        }



    }

    void UseSkill(SkillData skill)
    {
        skill.TriggerCooldown();
        currentSkill = skill;

        // Cập nhật hướng cho animation
        animator.SetFloat("LastMoveX", lastDirection.x);
        animator.SetFloat("LastMoveY", lastDirection.y);
        animator.SetTrigger(skill.triggerName);

        // Nếu là skill1 → dash + hiệu ứng từ A đến B
        if (skill == skill1)
        {
            Vector3 startPos = transform.position;
            float dashDistance = skill.GetDashDistance();
            Vector3 dashTarget = startPos + (Vector3)(lastDirection.normalized * dashDistance);

            // Dash ngay lập tức
            transform.position = dashTarget;

            if (skill.effectPrefab != null)
            {
                float scale = skill.GetEffectScale();
                int numberOfSteps = Mathf.CeilToInt(dashDistance / 0.4f);

                for (int i = 0; i <= numberOfSteps; i++)
                {
                    float t = i / (float)numberOfSteps;
                    Vector3 spawnPos = Vector3.Lerp(startPos, dashTarget, t);

                    GameObject fx = Instantiate(skill.effectPrefab, spawnPos, Quaternion.identity);
                    fx.transform.localScale = Vector3.one * scale;
                    Destroy(fx, 2f);

                    // Gây sát thương tại từng điểm
                    float radius = skill.GetRange();
                    int damage = skill.GetDamage();

                    Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, radius, enemyLayers);
                    foreach (var hit in hits)
                    {
                        if (hit.CompareTag("Orc") || hit.CompareTag("Flying_Eye") ||
                            hit.CompareTag("Skeleton") || hit.CompareTag("Vampire") ||
                            hit.CompareTag("Rat") || hit.CompareTag("Boss"))
                        {
                            CharacterStats stats = hit.GetComponent<CharacterStats>();
                            if (stats != null)
                            {
                                stats.TakeDamage(damage);

                                // Trigger animation Hit
                                Animator enemyAnimator = hit.GetComponent<Animator>();
                                if (enemyAnimator != null && !hit.CompareTag("Boss"))
                                {
                                    enemyAnimator.SetTrigger("Hit");
                                    if (stats.currentHealth <= 0)
                                    {
                                        enemyAnimator.SetTrigger("Die");
                                        stats.Die();
                                    }
                                }
                                else if (hit.CompareTag("Boss") && stats.currentHealth <= 0)
                                {
                                    enemyAnimator.SetTrigger("Die");
                                    stats.BossDie();
                                }
                            }
                        }
                    }
                    if (skill.soundClip != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(skill.soundClip);
                    }
                }
            }
        }
        else if (skill == skill2)
        {
            Vector3 spawnPos = skillSpawnPoint.position;
            GameObject fx = Instantiate(skill.effectPrefab, spawnPos, Quaternion.identity);
            fx.transform.localScale = Vector3.one * skill.GetEffectScale();

            Rigidbody2D rb = fx.GetComponent<Rigidbody2D>();
            Vector2 moveDir = GetComponent<Rigidbody2D>().linearVelocity.normalized;

            if (moveDir == Vector2.zero)
                moveDir = lastDirection;

            float speed = 2f;
            if (rb != null)
            {
                rb.linearVelocity = moveDir * speed;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            // Xoay prefab theo hướng bay
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            fx.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            //// ✅ Sử dụng Coroutine để check collision liên tục
            //StartCoroutine(CheckProjectileCollision(fx, skill, enemyLayers));
            // Thay vì callback phức tạp
            fx.AddComponent<ProjectileCollision>().Setup(skill, enemyLayers);
            Destroy(fx, 5f);
        }
        else if (skill == ultimate)
        {
            Vector3 spawnPos = skillSpawnPoint.position + Vector3.up * 0.15f;

            // Spawn hiệu ứng tại chỗ
            if (skill.effectPrefab != null)
            {
                GameObject fx = Instantiate(skill.effectPrefab, spawnPos, Quaternion.identity);
                fx.transform.localScale = Vector3.one * skill.GetEffectScale();
                Destroy(fx, 2f);
            }

            // Gây sát thương xung quanh nhân vật
            float radius = skill.GetRange();
            int damage = skill.GetDamage();

            Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, radius, enemyLayers);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Orc") || hit.CompareTag("Flying_Eye") ||
                    hit.CompareTag("Skeleton") || hit.CompareTag("Vampire") ||
                    hit.CompareTag("Rat") || hit.CompareTag("Boss"))
                {
                    CharacterStats stats = hit.GetComponent<CharacterStats>();
                    if (stats != null)
                    {
                        stats.TakeDamage(damage);

                        // Trigger animation Hit
                        Animator enemyAnimator = hit.GetComponent<Animator>();
                        if (enemyAnimator != null && !hit.CompareTag("Boss"))
                        {
                            enemyAnimator.SetTrigger("Hit");
                            if (stats.currentHealth <= 0)
                            {
                                enemyAnimator.SetTrigger("Die");
                                stats.Die();
                            }
                        }
                        else if (hit.CompareTag("Boss") && stats.currentHealth <= 0)
                        {
                            enemyAnimator.SetTrigger("Die");
                            stats.BossDie();
                        }
                    }
                }
            }
            GetComponent<CharacterMove>().DisableInputTemporarily(2f);


            // Phát âm thanh
            if (skill.soundClip != null && audioSource != null)
            {
                StartCoroutine(PlayRepeatedSound(skill.soundClip, 2, 0.8f)); // 3 lần, 
            }
        }



    }

    // Gọi từ animation event
    public void SpawnSkillEffect()
    {
        if (currentSkill == null) return;

        if (currentSkill.effectPrefab != null)
        {
            GameObject fx = Instantiate(currentSkill.effectPrefab, skillSpawnPoint.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * currentSkill.GetEffectScale();
            Destroy(fx, 2f);
        }

        // Gây sát thương tại điểm cố định
        float range = currentSkill.GetRange();
        int damage = currentSkill.GetDamage();
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, range, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log($"Hit {hit.name} with {currentSkill.skillName} at level {currentSkill.level}");
                hit.GetComponent<CharacterStats>()?.TakeDamage(damage);
            }
        }

        // Phát âm thanh
        if (currentSkill.soundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentSkill.soundClip);
        }

        currentSkill = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;

        if (skill1 != null)
            Gizmos.DrawWireSphere(attackPoint.position, skill1.GetRange());
    }

    private System.Collections.IEnumerator PlayRepeatedSound(AudioClip clip, int times, float delay)
    {
        for (int i = 0; i < times; i++)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(delay);
        }
    }

    private System.Collections.IEnumerator CheckProjectileCollision(GameObject projectile, SkillData skill, LayerMask enemyLayers)
    {
        float checkRadius = 0.5f;

        while (projectile != null)
        {
            // Check collision mỗi frame
            Collider2D hit = Physics2D.OverlapCircle(projectile.transform.position, checkRadius, enemyLayers);

            if (hit != null)
            {
                Debug.Log($"Projectile hit {hit.name}!");

                // Gây damage AoE tại vị trí va chạm
                float radius = skill.GetRange();
                int damage = skill.GetDamage();

                Collider2D[] hits = Physics2D.OverlapCircleAll(projectile.transform.position, radius, enemyLayers);
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
                Animator anim = projectile.GetComponent<Animator>();
                if (anim != null)
                    anim.SetTrigger("Hit");

                // Phát âm thanh
                if (skill.soundClipOnHit != null)
                    AudioSource.PlayClipAtPoint(skill.soundClipOnHit, projectile.transform.position);

                Destroy(projectile, 0.5f);
                yield break; // Thoát khỏi coroutine
            }

            yield return null; // Chờ frame tiếp theo
        }
    }

}

