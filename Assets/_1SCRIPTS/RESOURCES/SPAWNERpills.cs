using System.Collections.Generic;
using UnityEngine;

public class SPAWNERpills : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pillsPrefab;

    [Header("References")]
    public SanityBar sanityBar;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Settings")]
    public int maxKitsOnMap = 10;

    private List<GameObject> spawnedKits = new List<GameObject>();
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

        GameObject kitGO = Instantiate(pillsPrefab, spawnPoint.position, spawnPoint.rotation);

        PillsCode PillsScript = kitGO.GetComponent<PillsCode>();

        if (PillsScript != null)
        {
            PillsScript.sanityBar = sanityBar;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                PillsScript.points = player.GetComponent<Points>();
            }
            else
            {
                Debug.LogError("PLAYER not found!");
            }
        }

        spawnedKits.Add(kitGO);
    }
}