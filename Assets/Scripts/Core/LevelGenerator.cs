using UnityEngine;

[System.Serializable]
public class FloorData
{
    [Header("Cấu hình Tầng")]
    public string floorName;
    public int numberOfNormalRooms = 3; // Số phòng thường giữa phòng bắt đầu và phòng trùm

    [Header("Danh sách Prefab cho Tầng này")]
    public GameObject[] startRoomPrefabs;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] bossRoomPrefabs;
    public GameObject[] corridorPrefabs; // Các hành lang để nối phòng
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Thiết Lập Chung")]
    [Tooltip("Danh sách tất cả các tầng trong game")]
    public FloorData[] floors;

    [Tooltip("Kéo GameObject [LevelContainer] rỗng từ Hierarchy vào đây")]
    public Transform levelContainer;

    [Header("Trạng Thái Game")]
    private int currentFloorIndex = 0;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player! Hãy chắc chắn Player có tag 'Player'.");
            return;
        }

        if (levelContainer == null)
        {
            Debug.LogError("Chưa gán Level Container!");
            return;
        }

        // Bắt đầu game bằng cách tạo tầng đầu tiên
        GenerateFloor(currentFloorIndex);
    }

    /// <summary>
    /// Hàm chính để tạo ra toàn bộ một tầng.
    /// </summary>
    private void GenerateFloor(int floorIndex)
    {
        // 1. Dọn dẹp tầng cũ
        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }

        // Kiểm tra xem tầng có tồn tại không
        if (floorIndex >= floors.Length)
        {
            Debug.Log("GAME OVER - YOU WIN!");
            // Xử lý thắng game
            return;
        }

        FloorData currentFloor = floors[floorIndex];
        Transform lastConnector = null; // Điểm kết nối của mảnh ghép cuối cùng

        // 2. Tạo phòng bắt đầu (Start Room)
        GameObject startRoomPrefab = GetRandomPrefab(currentFloor.startRoomPrefabs);
        GameObject startRoomInstance = Instantiate(startRoomPrefab, levelContainer);
        lastConnector = startRoomInstance.transform.Find("Connector_Exit");

        // Đặt người chơi vào điểm bắt đầu
        Transform playerSpawnPoint = startRoomInstance.transform.Find("PlayerSpawnPoint");
        if (playerSpawnPoint != null)
        {
            player.transform.position = playerSpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy PlayerSpawnPoint trong phòng bắt đầu!");
        }

        // 3. Vòng lặp để tạo các phòng thường và hành lang
        for (int i = 0; i < currentFloor.numberOfNormalRooms; i++)
        {
            // Tạo hành lang
            GameObject corridorPrefab = GetRandomPrefab(currentFloor.corridorPrefabs);
            GameObject corridorInstance = Instantiate(corridorPrefab, levelContainer);
            AlignPieces(corridorInstance, corridorInstance.transform.Find("Connector_Entry"), lastConnector);
            lastConnector = corridorInstance.transform.Find("Connector_Exit");
            
            // Tạo phòng thường
            GameObject normalRoomPrefab = GetRandomPrefab(currentFloor.normalRoomPrefabs);
            GameObject normalRoomInstance = Instantiate(normalRoomPrefab, levelContainer);
            AlignPieces(normalRoomInstance, normalRoomInstance.transform.Find("Connector_Entry"), lastConnector);
            lastConnector = normalRoomInstance.transform.Find("Connector_Exit");
        }

        // 4. Tạo phòng trùm (Boss Room)
        // Nối phòng trùm bằng một hành lang cuối
        GameObject finalCorridorPrefab = GetRandomPrefab(currentFloor.corridorPrefabs);
        GameObject finalCorridorInstance = Instantiate(finalCorridorPrefab, levelContainer);
        AlignPieces(finalCorridorInstance, finalCorridorInstance.transform.Find("Connector_Entry"), lastConnector);
        lastConnector = finalCorridorInstance.transform.Find("Connector_Exit");

        GameObject bossRoomPrefab = GetRandomPrefab(currentFloor.bossRoomPrefabs);
        GameObject bossRoomInstance = Instantiate(bossRoomPrefab, levelContainer);
        AlignPieces(bossRoomInstance, bossRoomInstance.transform.Find("Connector_Entry"), lastConnector);

        Debug.Log($"Đã tạo xong {currentFloor.floorName}!");
    }

    /// <summary>
    /// Hàm "thần kỳ" để di chuyển và xoay một mảnh ghép mới cho khớp với mảnh cũ.
    /// </summary>
    private void AlignPieces(GameObject pieceToAlign, Transform entryConnector, Transform exitConnector)
    {
        if (entryConnector == null || exitConnector == null)
        {
            Debug.LogError($"Prefab {pieceToAlign.name} thiếu Connector_Entry hoặc Connector_Exit!", pieceToAlign);
            return;
        }

        // Tính toán độ xoay cần thiết để hai connector đối diện nhau
        Quaternion targetRotation = exitConnector.rotation * Quaternion.Euler(0, 180f, 0);
        pieceToAlign.transform.rotation = targetRotation;

        // Tính toán vị trí cần thiết để hai connector chạm vào nhau
        Vector3 entryOffset = pieceToAlign.transform.position - entryConnector.position;
        pieceToAlign.transform.position = exitConnector.position + entryOffset;
    }

    /// <summary>
    /// Hàm tiện ích để lấy một prefab ngẫu nhiên từ một mảng.
    /// </summary>
    private GameObject GetRandomPrefab(GameObject[] prefabArray)
    {
        if (prefabArray == null || prefabArray.Length == 0)
        {
            Debug.LogError("Mảng Prefab rỗng!");
            return null;
        }
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    /// <summary>
    /// Hàm công khai để các script khác (như cửa ra) gọi khi muốn chuyển tầng.
    /// </summary>
    public void GoToNextFloor()
    {
        currentFloorIndex++;
        GenerateFloor(currentFloorIndex);
    }
}
