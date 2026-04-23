using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ItemDropSpot[] dropPoints;

    private bool hasWon = false;

    [Header("Enemy Spawn")]
    public GameObject enemyPrefab;
    public Transform enemySpawnPoint;

    private bool enemySpawned = false;
    public EnemySpawner enemySpawner;

    void Awake()
    {
        Instance = this;
    }

    void SpawnEnemy()
    {
        enemySpawned = true;

        Debug.Log("ENEMY SPAWNED!");

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
        {
            Debug.LogError("PLAYER NOT FOUND!");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);

        Simple3StateEnemy enemyScript = enemyObj.GetComponent<Simple3StateEnemy>();

        if (enemyScript == null)
        {
            Debug.LogError("Simple3StateEnemy missing on prefab!");
            return;
        }

        enemyScript.player = playerObj.transform;
        enemyScript.phealthBar = playerObj.GetComponent<HealthBar>();
    }

    public void CheckWinCondition()
    {
        if (hasWon) return;

        foreach (var dp in dropPoints)
        {
            if (!dp.IsOccupied())
                return;
        }

        enemySpawner.enabled = false; // zatrzymujesz fale
        SpawnEnemy(); // albo boss
    }

    public void EnemyDefeated()
    {
        if (hasWon) return;

        hasWon = true;
        WinGame();
    }

    void WinGame()
    {
        hasWon = true;

        Debug.Log("WIN!");

        StartCoroutine(LoadWinScene());
    }

    IEnumerator LoadWinScene()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(3);
    }
}