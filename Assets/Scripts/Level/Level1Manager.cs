using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level1Manager : MonoBehaviour
{
    [Header("Prefab & Spawn Config")]
    public GameObject spaceGatePrefab;
    public GameObject skeletonPrefab;
    public GameObject orcPrefab;

    public Transform gateSpawnPoint;
    public Transform enemySpawnOffset;

    private GameObject gateInstance;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private float spawnInterval = 1.5f;
    private float totalSpawnDuration = 30f;
    private float levelStartTime;

    private bool levelStarted = false;
    private bool spawningActive = false;
    private bool levelCompleted = false;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            StartCoroutine(RunLevel());
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

        yield return new WaitForSeconds(1.5f);
        levelStartTime = Time.time;
        levelStarted = true;
        spawningActive = true;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        float elapsed = 0f;
        int sequenceIndex = 0;

        GameObject[] spawnSequence = new GameObject[]
        {
            skeletonPrefab,
            orcPrefab,
            skeletonPrefab
        };

        while (elapsed < totalSpawnDuration && player != null)
        {
            GameObject prefabToSpawn = spawnSequence[sequenceIndex % spawnSequence.Length];
            Vector3 spawnPos = enemySpawnOffset.position;
            GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            sequenceIndex++;
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;

            if (player == null)
            {
                Debug.Log("Level Failed - Player Lost");
                break;
            }
        }

        spawningActive = false;
        CloseGate();
    }

    void CloseGate()
    {
        if (gateInstance != null)
        {
            Destroy(gateInstance);
            gateInstance = null;
        }
    }

    void Update()
    {
        if (levelStarted && !levelCompleted && !spawningActive && player != null)
        {
            spawnedEnemies.RemoveAll(e => e == null);

            if (spawnedEnemies.Count == 0)
            {
                Debug.Log("Level 1 Complete");
                levelCompleted = true;
                Destroy(gameObject); 
            }
        }
    }
}
