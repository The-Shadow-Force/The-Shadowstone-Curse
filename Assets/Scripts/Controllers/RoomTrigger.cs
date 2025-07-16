using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    private bool triggered = false;

    [Header("Tùy chỉnh vùng trigger")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public Vector2 triggerOffset = Vector2.zero;

    private void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = triggerSize;
        col.offset = triggerOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        triggered = true;

        MonoBehaviour[] parentScripts = GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour script in parentScripts)
        {
            MethodInfo method = script.GetType().GetMethod("StartLevel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null && method.GetParameters().Length == 0)
            {
                method.Invoke(script, null);
                break;
            }
        }

        gameObject.SetActive(false);
    }
}
