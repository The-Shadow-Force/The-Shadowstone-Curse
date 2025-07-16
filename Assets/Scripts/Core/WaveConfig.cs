using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "Dungeon/Wave Configuration")]
public class WaveConfig : ScriptableObject
{
    [Header("Danh sách quái vật")]
    [Tooltip("Các prefab quái vật sẽ được tạo ra trong đợt này.")]
    public GameObject[] enemyPrefabs;

    [Header("Thời gian")]
    [Tooltip("Tổng thời gian diễn ra việc tạo quái (tính bằng giây).")]
    public float spawnDuration = 10f;

    [Tooltip("Thời gian nghỉ giữa mỗi lần tạo ra một con quái.")]
    public float spawnInterval = 2f;
}
