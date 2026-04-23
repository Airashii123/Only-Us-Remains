using System.Collections.Generic;
using UnityEngine;

public class FirstAidKitSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject firstAidKitPrefab;

    [Header("References")]
    public HealthBar healthBar;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Settings")]
    public int maxKitsOnMap = 10;

    private List<GameObject> spawnedKits = new List<GameObject>();
    public Points point;
    void Start()
    {
        SpawnMissingKits();
    }

    void Update()
    {
        spawnedKits.RemoveAll(k => k == null);

        SpawnMissingKits();
    }

    void SpawnMissingKits()
    {
        int missing = maxKitsOnMap - spawnedKits.Count;

        for (int i = 0; i < missing; i++)
        {
            SpawnKit();
        }
    }

    void SpawnKit()
    {
        if (firstAidKitPrefab == null)
        {
            Debug.LogError("FirstAidKitSpawner: brak firstAidKitPrefab!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("FirstAidKitSpawner: brak spawnPoints!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject kitGO = Instantiate(firstAidKitPrefab, spawnPoint.position, spawnPoint.rotation);

        FirstAidKitCode firstAidKitScript = kitGO.GetComponent<FirstAidKitCode>();

        if (firstAidKitScript != null)
        {
            firstAidKitScript.healthBar = healthBar;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                firstAidKitScript.points = player.GetComponent<Points>();
            }
            else
            {
                Debug.LogError("PLAYER not found!");
            }
        }
        else
        {
            Debug.LogError("Prefab apteczki nie ma skryptu FirstAidKitCode!");
        }

        spawnedKits.Add(kitGO);
    }
}