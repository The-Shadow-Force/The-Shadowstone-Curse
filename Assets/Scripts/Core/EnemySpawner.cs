using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Cấu hình")]
    [Tooltip("Kéo file cấu hình WaveConfig cho spawner này vào đây.")]
    public WaveConfig config;
    
    [Tooltip("Vị trí quái sẽ xuất hiện (nếu để trống, sẽ lấy vị trí của chính spawner).")]
    public Transform spawnPoint;

    // Sự kiện để báo cho RoomController biết khi nào wave đã hoàn thành
    public event Action OnWaveCompleted;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    void Awake()
    {
        // Nếu không có spawnPoint riêng, hãy dùng vị trí của chính spawner
        if (spawnPoint == null)
        {
            spawnPoint = this.transform;
        }
    }

    // RoomController sẽ gọi hàm này để bắt đầu
    public void StartSpawningWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        if (config == null)
        {
            Debug.LogError("Chưa gán WaveConfig cho Spawner!", this);
            yield break;
        }
        
        Debug.Log("Bắt đầu tạo quái...");
        float timer = 0f;
        while (timer < config.spawnDuration)
        {
            // Lấy một prefab quái ngẫu nhiên từ danh sách
            GameObject enemyPrefab = config.enemyPrefabs[UnityEngine.Random.Range(0, config.enemyPrefabs.Length)];
            
            // Tạo quái
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedEnemies.Add(newEnemy);

            // Đợi
            yield return new WaitForSeconds(config.spawnInterval);
            timer += config.spawnInterval;
        }
        Debug.Log("Đã tạo xong quái, chờ tiêu diệt hết...");

        // Chờ đến khi tất cả quái vật được sinh ra bị tiêu diệt
        yield return new WaitUntil(() => AllEnemiesDefeated());
        
        // Phát tín hiệu báo wave đã hoàn thành
        Debug.Log("Wave hoàn thành!");
        OnWaveCompleted?.Invoke();
    }

    private bool AllEnemiesDefeated()
    {
        // Dọn dẹp những con quái đã chết (bị null) khỏi danh sách
        spawnedEnemies.RemoveAll(e => e == null);
        // Trả về true nếu không còn con quái nào
        return spawnedEnemies.Count == 0;
    }
}
