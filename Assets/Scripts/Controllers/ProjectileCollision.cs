using UnityEngine;
using System;

public class ProjectileCollision : MonoBehaviour
{
    private Action onHit;

    public void Setup(Action callback)
    {
        onHit = callback;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            onHit?.Invoke();
        }
    }
}
