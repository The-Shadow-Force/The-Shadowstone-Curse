using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level5Manager : MonoBehaviour
{
    [Header("Prefab & Spawn Config")]
    public GameObject spaceGatePrefab;
    public GameObject skeleton1Prefab;
    public GameObject skeleton2Prefab;
    public GameObject orcPrefab;
    public GameObject ratPrefab;
    public GameObject flyingEyePrefab;
    public GameObject bossPrefab;

    [Header("Cửa ra")]
    public GameObject exitWall;

    public Transform gateSpawnPoint;
    public Transform enemySpawnOffset;

    private GameObject gateInstance;
    private GameObject bossInstance;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private float spawnInterval = 1.5f;
    private float totalSpawnDuration = 30f;
    private float levelStartTime;

    private bool levelStarted = false;
    private bool spawningActive = false;
    private bool bossSpawned = false;
    private bool levelCompleted = false;
    private GameObject player;

    public void StartLevel()
    {
        if (!levelStarted && player != null)
        {
            StartCoroutine(RunLevel());
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            //StartLevel(); // Gọi từ RoomTrigger.cs
        }
        else
        {
            Debug.LogWarning("No Player Found. Level won't start.");
        }
    }

    IEnumerator RunLevel()
    {
        yield return new WaitForSeconds(1f);
        gateInstance = Instantiate(spaceGatePrefab, gateSpawnPoint.position, Quaternion.identity);

        SetExitWallTrigger(false);

        yield return new WaitForSeconds(1.5f);
        levelStartTime = Time.time;
        levelStarted = true;
        spawningActive = true;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        float elapsed = 0f;
        GameObject[] spawnOptions = new GameObject[]
        {
            skeleton1Prefab,
            skeleton2Prefab,
            orcPrefab,
            ratPrefab,
            flyingEyePrefab
        };

        while (elapsed < totalSpawnDuration && player != null)
        {
            GameObject prefabToSpawn = spawnOptions[Random.Range(0, spawnOptions.Length)];
            Vector3 spawnPos = enemySpawnOffset.position;
            GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;

            if (player == null)
            {
                Debug.Log("Level Failed - Player Lost");
                yield break;
            }
        }

        spawningActive = false;
        CloseGate();
    }

    void Update()
    {
        if (!levelStarted || levelCompleted || player == null) return;

        spawnedEnemies.RemoveAll(e => e == null);

        // Khi hết enemy thường -> spawn boss
        if (!bossSpawned && !spawningActive && spawnedEnemies.Count == 0)
        {
            SpawnBoss();
        }

        // Boss chết -> hoàn tất màn
        if (bossSpawned && bossInstance == null)
        {
            Debug.Log("Level 5 Complete");
            levelCompleted = true;
            OpenRoom();
        }
    }

    void SpawnBoss()
    {
        Debug.Log("Spawning Boss...");
        Vector3 spawnPos = enemySpawnOffset.position;
        bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        bossSpawned = true;
    }

    void CloseGate()
    {
        if (gateInstance != null)
        {
            Destroy(gateInstance);
            gateInstance = null;
        }
    }

    private void OpenRoom()
    {
        Debug.Log("Boss đã bị tiêu diệt. Mở cửa ra!");
        SetExitWallTrigger(true);
    }

    private void SetExitWallTrigger(bool isTrigger)
    {
        if (exitWall != null)
        {
            BoxCollider2D col = exitWall.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = isTrigger;
            }
            else
            {
                Debug.LogWarning("Exit wall does not have a BoxCollider2D.");
            }
        }
    }
}
