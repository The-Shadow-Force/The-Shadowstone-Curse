// CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Kéo đối tượng Player vào đây.")]
    public Transform target;

    [Tooltip("Độ mượt khi camera di chuyển theo. Số càng nhỏ càng mượt.")]
    public float smoothSpeed = 0.125f;

    // Giữ khoảng cách Z mặc định của camera
    private Vector3 offset;

    void Start()
    {
        // Tính toán và lưu lại khoảng cách Z ban đầu
        offset = new Vector3(0, 0, transform.position.z);
    }

    // Dùng FixedUpdate để camera di chuyển đồng bộ với vật lý và mượt hơn
    void FixedUpdate()
    {
        if (target != null)
        {
            // Vị trí mong muốn của camera là vị trí của player
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, 0) + offset;
            
            // Di chuyển camera một cách mượt mà từ vị trí hiện tại đến vị trí mong muốn
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}