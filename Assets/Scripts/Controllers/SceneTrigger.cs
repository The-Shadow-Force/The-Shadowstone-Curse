using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTrigger : MonoBehaviour
{
    [Header("Tên scene cần chuyển tới")]
    public string sceneName;

    [Header("Tùy chỉnh vùng trigger")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public Vector2 triggerOffset = Vector2.zero;

    private bool triggered = false;

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

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Chưa nhập tên Scene cần chuyển!");
        }
    }
}
