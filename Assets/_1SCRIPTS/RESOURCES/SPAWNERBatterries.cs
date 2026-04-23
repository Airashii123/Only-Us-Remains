using System.Collections.Generic;
using UnityEngine;

public class SPAWNERbatterries : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject batterryPrefab;

    [Header("References")]
    public BatteryBar batteryBar;

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
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject kitGO = Instantiate(batterryPrefab, spawnPoint.position, spawnPoint.rotation);

        BatteryCode batteryScript = kitGO.GetComponent<BatteryCode>();

        if (batteryScript != null)
        {
            batteryScript.batteryBar = batteryBar;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                batteryScript.points = player.GetComponent<Points>(); // ✔ TU KLUCZ
            }
            else
            {
                Debug.LogError("PLAYER not found!");
            }
        }

        spawnedKits.Add(kitGO);
    }
}