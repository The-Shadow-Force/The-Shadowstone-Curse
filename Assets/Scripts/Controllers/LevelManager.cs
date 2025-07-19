// LevelManager.cs

using System.Linq;
using System.Reflection;
using UnityEngine;

public class LevelManager : MonoBehaviour
{[Header("Cài Đặt")]
    [Tooltip("Kéo đối tượng Player vào đây.")]
    public Transform playerTransform;

    [Tooltip("Kéo các điểm bắt đầu của mỗi tầng vào đây theo đúng thứ tự (Tầng 1, Tầng 2, Tầng 3).")]
    public Transform[] floorSpawnPoints;

    private int currentFloorIndex = 0;

    void Start()
    {
        // Khi game bắt đầu, đảm bảo người chơi luôn ở điểm bắt đầu của tầng 1
        if (playerTransform != null && floorSpawnPoints.Length > 0)
        {
            playerTransform.position = floorSpawnPoints[0].position;
        }
    }

    /// <summary>
    /// Hàm này được gọi bởi các Cửa Ra để dịch chuyển người chơi đến tầng tiếp theo.
    /// </summary>
    public void GoToNextFloor()
    {
        // Tăng chỉ số tầng
        currentFloorIndex++;

        // Nếu còn tầng tiếp theo trong danh sách
        if (currentFloorIndex < floorSpawnPoints.Length)
        {
            // Dịch chuyển người chơi đến điểm bắt đầu của tầng đó
            playerTransform.position = floorSpawnPoints[currentFloorIndex].position;
            Debug.Log($"Đã dịch chuyển đến Tầng {currentFloorIndex + 1}");
        }
        else
        {
            // Nếu đã hết tầng -> Thắng game
            Debug.Log("CHÚC MỪNG! BẠN ĐÃ HOÀN THÀNH GAME!");
            // Có thể vô hiệu hóa người chơi hoặc hiện màn hình chiến thắng
            // playerTransform.gameObject.SetActive(false);
        }
    }
}