// RoomController.cs

using System;
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
        SetBarriers(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !roomIsActive)
        {
            Debug.Log("Player entered room!");
            ActivateRoom();
        }
        Debug.Log("Player wrong tag " + other.tag);
    }

    private void ActivateRoom()
    {
        roomIsActive = true;
        SetBarriers(true); // Khóa cửa

        // Nếu có prefab level được gán, tạo nó ra
        if (levelPrefab)
        {
            Debug.Log("Activating room with level prefab: " + levelPrefab.name);
            activeLevelInstance = Instantiate(levelPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            // Nếu phòng này không có wave nào, mở cửa luôn
            Debug.LogWarning("No level prefab assigned for this room. Opening room immediately.");
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