// LevelManager.cs
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Cấu hình")]
    [Tooltip("Kéo 3 prefab của 3 tầng vào đây theo đúng thứ tự.")]
    public GameObject[] floorPrefabs;

    [Tooltip("Kéo đối tượng Player vào đây.")]
    public Transform player;

    // --- Biến nội bộ ---
    private int currentFloorIndex = -1;
    private GameObject currentFloorInstance;

    void Start()
    {
        // Bắt đầu game bằng cách tải tầng đầu tiên
        GoToNextFloor();
    }

    /// <summary>
    /// Hàm này được gọi để tải tầng tiếp theo.
    /// </summary>
    public void GoToNextFloor()
    {
        currentFloorIndex++;

        // Nếu đã đi hết các tầng -> Thắng game
        if (currentFloorIndex >= floorPrefabs.Length)
        {
            Debug.Log("CHÚC MỪNG! BẠN ĐÃ HOÀN THÀNH GAME!");
            if (currentFloorInstance != null)
            {
                Destroy(currentFloorInstance);
            }
            // Có thể hiện UI chiến thắng ở đây
            return;
        }

        // Xóa tầng cũ đi
        if (currentFloorInstance != null)
        {
            Destroy(currentFloorInstance);
        }

        // Tạo tầng mới từ prefab
        currentFloorInstance = Instantiate(floorPrefabs[currentFloorIndex]);
        
        // Tìm điểm spawn trong tầng mới và di chuyển người chơi đến đó
        Transform spawnPoint = currentFloorInstance.transform.Find("PlayerSpawnPoint");
        if (spawnPoint != null && player != null)
        {
            player.position = spawnPoint.position;
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy 'PlayerSpawnPoint' trong prefab của Tầng {currentFloorIndex + 1}!");
        }
    }
}