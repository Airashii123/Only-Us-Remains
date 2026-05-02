using System.Collections.Generic;
using UnityEngine;

public class BotSensor : MonoBehaviour
{
    [Header("Detected Objects")]
    public List<Transform> enemies = new List<Transform>();
    public List<Transform> pickups = new List<Transform>();
    public List<Transform> runes = new List<Transform>();

    public Transform player;

    private void OnTriggerEnter(Collider other)
    {
        HandleEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleExit(other);
    }

    void HandleEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
            AddUnique(enemies, other.transform);

        if (other.CompareTag("FIRSTAID") ||
            other.CompareTag("TABS") ||
            other.CompareTag("BATTERY"))
            AddUnique(pickups, other.transform);

        if (other.CompareTag("RUNE"))
            AddUnique(runes, other.transform);

        if (other.CompareTag("PLAYER"))
            player = other.transform;
    }

    void HandleExit(Collider other)
    {
        if (other.CompareTag("ENEMY"))
            enemies.Remove(other.transform);

        if (other.CompareTag("FIRSTAID") ||
            other.CompareTag("TABS") ||
            other.CompareTag("BATTERY"))
            pickups.Remove(other.transform);

        if (other.CompareTag("RUNE"))
            runes.Remove(other.transform);

        if (other.CompareTag("PLAYER"))
        {
            if (player == other.transform)
                player = null;
        }
    }

    void AddUnique(List<Transform> list, Transform obj)
    {
        if (!list.Contains(obj))
            list.Add(obj);
    }

    public Transform GetNearest(List<Transform> list, Transform from)
    {
        Transform best = null;
        float bestDist = Mathf.Infinity;

        foreach (var t in list)
        {
            if (t == null) continue;

            float d = Vector3.Distance(from.position, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        return best;
    }

    public Transform GetNearestEnemy(Transform from) => GetNearest(enemies, from);
    public Transform GetNearestPickup(Transform from) => GetNearest(pickups, from);
    public Transform GetNearestRune(Transform from) => GetNearest(runes, from);

    public int GetEnemyCount() => enemies.RemoveAll(e => e == null) == 0 ? enemies.Count : enemies.Count;
    public int GetPickupCount() => pickups.RemoveAll(p => p == null) == 0 ? pickups.Count : pickups.Count;
    public int GetRuneCount() => runes.RemoveAll(r => r == null) == 0 ? runes.Count : runes.Count;
}