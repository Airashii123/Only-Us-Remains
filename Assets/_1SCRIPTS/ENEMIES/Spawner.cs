using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform player;
    public HealthBar playerHealthBar;
    public Transform[] spawnPoints;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    [Header("Wave Settings")]
    public int enemiesPerMinute = 3;

    [Header("HP Scaling")]
    public int baseHP = 3;
    public int hpIncreasePerMinute = 2;

    private int lastMinuteSpawned = -1;

    private float gameStartTime;

    void Start()
    {
        gameStartTime = Time.time;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("PLAYER");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        int minute = Mathf.FloorToInt((Time.time - gameStartTime) / 60f);

        if (minute > lastMinuteSpawned)
        {
            lastMinuteSpawned = minute;
            SpawnWave(minute);
        }
    }

    void SpawnWave(int minute)
    {
        int hpForThisWave = baseHP + (minute * hpIncreasePerMinute);

        for (int i = 0; i < enemiesPerMinute; i++)
        {
            SpawnEnemy(hpForThisWave);
        }
    }

    void SpawnEnemy(int hp)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: enemyPrefab jest NULL!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: Brak spawnPoints!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        Simple3StateEnemy enemy = enemyGO.GetComponent<Simple3StateEnemy>();

        if (enemy == null)
        {
            Debug.LogError("EnemySpawner: Prefab nie ma komponentu Simple3StateEnemy!");
            return;
        }

        enemy.player = player;
        enemy.agent = enemyGO.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemy.whatisground = groundLayer;
        enemy.whatisplayer = playerLayer;

        enemy.phealthBar = playerHealthBar;

        enemy.ehealth = hp;
    }
}