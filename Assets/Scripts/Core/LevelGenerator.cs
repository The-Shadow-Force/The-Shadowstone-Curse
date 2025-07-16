using UnityEngine;

[System.Serializable]
public class FloorData
{
    [Header("Cấu hình Tầng")]
    public string floorName;
    public int numberOfNormalRooms = 3;

    [Header("Danh sách Prefab cho Tầng này")]
    public GameObject[] startRoomPrefabs;
    public GameObject[] normalRoomPrefabs;
    public GameObject[] bossRoomPrefabs;
    public GameObject[] corridorPrefabs;
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Thiết Lập Chung")]
    public FloorData[] floors;
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

        GenerateFloor(currentFloorIndex);
    }

    private void GenerateFloor(int floorIndex)
    {
        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }

        if (floorIndex >= floors.Length)
        {
            Debug.Log("GAME OVER - YOU WIN!");
            return;
        }

        FloorData currentFloor = floors[floorIndex];
        Transform lastConnector = null;

        GameObject startRoomInstance = Instantiate(GetRandomPrefab(currentFloor.startRoomPrefabs), levelContainer);
        startRoomInstance.transform.position = Vector3.zero; // Luôn đặt phòng đầu tiên ở gốc
        lastConnector = startRoomInstance.transform.Find("Connector_Exit");

        Transform playerSpawnPoint = startRoomInstance.transform.Find("PlayerSpawnPoint");
        if (playerSpawnPoint != null && player != null)
        {
            player.transform.position = playerSpawnPoint.position;
        }

        for (int i = 0; i < currentFloor.numberOfNormalRooms; i++)
        {
            GameObject corridorInstance = Instantiate(GetRandomPrefab(currentFloor.corridorPrefabs), levelContainer);
            AlignPieces(corridorInstance, corridorInstance.transform.Find("Connector_Entry"), lastConnector);
            lastConnector = corridorInstance.transform.Find("Connector_Exit");
            
            GameObject normalRoomInstance = Instantiate(GetRandomPrefab(currentFloor.normalRoomPrefabs), levelContainer);
            AlignPieces(normalRoomInstance, normalRoomInstance.transform.Find("Connector_Entry"), lastConnector);
            lastConnector = normalRoomInstance.transform.Find("Connector_Exit");
        }

        GameObject finalCorridorInstance = Instantiate(GetRandomPrefab(currentFloor.corridorPrefabs), levelContainer);
        AlignPieces(finalCorridorInstance, finalCorridorInstance.transform.Find("Connector_Entry"), lastConnector);
        lastConnector = finalCorridorInstance.transform.Find("Connector_Exit");

        GameObject bossRoomInstance = Instantiate(GetRandomPrefab(currentFloor.bossRoomPrefabs), levelContainer);
        AlignPieces(bossRoomInstance, bossRoomInstance.transform.Find("Connector_Entry"), lastConnector);

        Debug.Log($"Đã tạo xong {currentFloor.floorName}!");
    }

    /// <summary>
    /// Hàm AlignPieces phiên bản sửa lỗi, tính toán chính xác vị trí và hướng.
    /// </summary>
    private void AlignPieces(GameObject pieceToAlign, Transform entryConnector, Transform exitConnector)
    {
        if (entryConnector == null || exitConnector == null)
        {
            Debug.LogError($"Prefab {pieceToAlign.name} thiếu Connector_Entry hoặc Connector_Exit!", pieceToAlign);
            return;
        }

        // KHÔNG xoay gì cả
        pieceToAlign.transform.rotation = Quaternion.identity;

        // Tính vị trí offset từ tâm đến Connector_Entry
        Vector3 offset = pieceToAlign.transform.position - entryConnector.position;
        pieceToAlign.transform.position = exitConnector.position + offset;
    }
    private GameObject GetRandomPrefab(GameObject[] prefabArray)
    {
        if (prefabArray == null || prefabArray.Length == 0)
        {
            Debug.LogError("Mảng Prefab rỗng!");
            return null;
        }
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    public void GoToNextFloor()
    {
        currentFloorIndex++;
        GenerateFloor(currentFloorIndex);
    }
}