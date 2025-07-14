using UnityEngine;

public class Fireball : MonoBehaviour
{
    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats stats = other.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.TakeDamage(10); // tuỳ chỉnh sát thương
                Debug.Log("Fireball hit the player!");
            }
            Destroy(gameObject);
        }
    }
}
