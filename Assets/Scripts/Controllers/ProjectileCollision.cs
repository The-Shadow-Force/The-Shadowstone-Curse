// Thay thế ProjectileCollision.cs bằng script đơn giản này:
using System;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private Action onHit;
    private LayerMask enemyLayers;
    private bool hasHit = false;
    private float checkRadius = 1f;

    public void Setup(Action callback, LayerMask layers)
    {
        onHit = callback;
        enemyLayers = layers;
        Debug.Log($"ProjectileCollision setup with enemyLayers: {layers.value}");
    }

    void FixedUpdate()
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        if (hasHit) return;

        // Check collision mỗi FixedUpdate
        Collider2D hit = Physics2D.OverlapCircle(transform.position, checkRadius, enemyLayers);
        if (hit != null)
        {
            Debug.Log($"Projectile hit enemy {hit.name} at position {transform.position}");
            hasHit = true;
            onHit?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ collision radius để debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}