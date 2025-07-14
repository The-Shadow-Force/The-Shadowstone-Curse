using UnityEngine;
using System.Collections;

public class DarkFlame : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private float fireballSpeed = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(DarkFlameRoutine());
    }

    private IEnumerator DarkFlameRoutine()
    {
        animator.SetTrigger("Appear");
        yield return new WaitForSeconds(1f);

        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 3; i++)
        {
            ShootFireball();
            yield return new WaitForSeconds(shootInterval);
        }

        animator.SetTrigger("Death");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void ShootFireball()
    {
        if (player == null) return;

        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        Vector2 dir = (player.position - transform.position).normalized;
        fireball.GetComponent<Rigidbody2D>().linearVelocity = dir * fireballSpeed;
    }
}
