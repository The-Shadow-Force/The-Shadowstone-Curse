using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Thiết lập Phòng")]
    [Tooltip("Các rào chắn sẽ được kích hoạt khi vào phòng.")]
    public GameObject[] barriers;

    [Header("Thiết lập Level")]
    [Tooltip("Kéo prefab Level Manager (ví dụ: Level1Manager) cho phòng này vào đây.")]
    public GameObject levelPrefab;
    
    [Tooltip("Vị trí để tạo ra Level Manager.")]
    public Transform spawnPoint;

    // --- Biến nội bộ ---
    private GameObject activeLevelInstance;
    private bool roomIsActive = false;
    private bool roomCleared = false;

    void Start()
    {
        // Mở sẵn cửa khi bắt đầu
        SetBarriers(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !roomIsActive)
        {
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        roomIsActive = true;
        SetBarriers(true); // Khóa cửa

        // Nếu có prefab level được gán, tạo nó ra
        if (levelPrefab != null)
        {
            activeLevelInstance = Instantiate(levelPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            // Nếu phòng này không có wave nào, mở cửa luôn
            OpenRoom();
        }
    }
    
    void Update()
    {
        // Nếu phòng đang hoạt động và chưa được dọn dẹp
        if (roomIsActive && !roomCleared)
        {
            // Kiểm tra xem đối tượng level đã tự hủy chưa
            if (activeLevelInstance == null)
            {
                // Nếu đã tự hủy, có nghĩa là level đã hoàn thành
                roomCleared = true;
                OpenRoom();
            }
        }
    }

    private void OpenRoom()
    {
        Debug.Log("Phòng đã được dọn dẹp. Mở cửa!");
        SetBarriers(false);
    }

    private void SetBarriers(bool state)
    {
        foreach (var barrier in barriers)
        {
            barrier.SetActive(state);
        }
    }
}
