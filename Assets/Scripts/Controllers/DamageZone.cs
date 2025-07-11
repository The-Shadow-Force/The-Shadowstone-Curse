using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damageRadius;
    public int damageAmount;
    public LayerMask enemyLayers;

    public void Init(float radius, int damage, LayerMask layers)
    {
        damageRadius = radius;
        damageAmount = damage;
        enemyLayers = layers;

        DealDamage();
    }

    void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayers);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log($"Hit {hit.name} with VFX damage");
                // hit.GetComponent<EnemyHealth>()?.TakeDamage(damageAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
