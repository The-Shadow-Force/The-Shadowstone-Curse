using UnityEngine;

public class ChestController : MonoBehaviour
{
    // Kéo và thả đối tượng Canvas bạn muốn bật/tắt vào đây từ cửa sổ Hierarchy.
    [Header("Thiết lập Canvas")]
    [Tooltip("Canvas chứa phần thưởng sẽ được bật/tắt.")]
    public GameObject[] rewardCanvas;

    /// <summary>
    /// Hàm này được gọi một lần khi kịch bản được khởi tạo.
    /// </summary>
    void Start()
    {
        // Đảm bảo rằng canvas đã được gán trong Inspector.
        if (rewardCanvas == null)
        {
            // Ghi lại lỗi nếu bạn quên gán Canvas để dễ dàng sửa lỗi.
            Debug.LogError("Lỗi: Chưa gán 'Reward Canvas' cho rương có tên: " + gameObject.name);
            return; // Dừng thực thi để tránh lỗi thêm.
        }

        // Đảm bảo rằng canvas bị tắt khi bắt đầu game.
        foreach (var canva in rewardCanvas)
        {
            canva.SetActive(false);
        }
    }

    /// <summary>
    /// Hàm này của Unity sẽ tự động được gọi khi một đối tượng khác
    /// đi vào vùng trigger của đối tượng này.
    /// </summary>
    /// <param name="other">Collider2D của đối tượng đã đi vào.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có tag là "Player" hay không.
        if (other.CompareTag("Player"))
        {
            if (rewardCanvas != null)
            {
                // Bật canvas lên.
                foreach (var canva in rewardCanvas)
                {
                    canva.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Hàm này của Unity sẽ tự động được gọi khi một đối tượng khác
    /// đi ra khỏi vùng trigger của đối tượng này.
    /// </summary>
    /// <param name="other">Collider2D của đối tượng đã đi ra.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng đi ra có tag là "Player" hay không.
        if (other.CompareTag("Player"))
        {
            if (rewardCanvas != null)
            {
                // Tắt canvas đi.
                foreach (var canva in rewardCanvas)
                {
                    canva.SetActive(false);
                }
            }
        }
    }
}
