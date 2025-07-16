using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    [Header("Điểm Đến")]
    [Tooltip("Kéo GameObject là điểm đến của cổng dịch chuyển này vào đây.")]
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            // Nếu không gán điểm đến thì không làm gì cả
            if (destination == null)
            {
                Debug.LogWarning("Chưa gán điểm đến cho Teleporter!", this.gameObject);
                return;
            }

            // Dịch chuyển Player đến vị trí của điểm đến
            other.transform.position = destination.position;
        }
    }
}
