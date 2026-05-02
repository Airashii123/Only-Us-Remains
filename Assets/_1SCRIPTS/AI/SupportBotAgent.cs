using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.AI;

public class SupportBotAgent : Agent
{
    [Header("References")]
    public NavMeshAgent navAgent;
    public BotSensor botSensor;

    [Header("Target")]
    public Transform player;

    [Header("Drop Points")]
    public ItemDropSpot[] dropSpots;
    public float dropInteractRange = 2.5f;

    [Header("Systems")]
    public HealthBar playerHealthBar;
    public SanityBar playerSanityBar;
    public BotStats botStats;
    public BatteryBar batteryBar;

    [Header("Pickup Settings")]
    public float interactRange = 2.0f;
    public int healAmount = 30;
    public int SanityRestoreAmount = 30;
    public int batteryRestoreAmount = 25;

    [Header("Flashlight / Combat")]
    public bool botFlashlightOn = false;
    public float attackRange = 6f;
    public LayerMask enemyLayer;
    public float attackBatteryDrain = 2f;

    [Header("Follow distances")]
    public float followDistance = 3f;
    public float softFollowDistance = 10f;
    public float hardFollowDistance = 20f;

    [Header("Reward Settings")]
    public float rewardNearPlayer = 0.01f;
    public float penaltyTooFarSoft = -0.01f;
    public float penaltyTooFarHard = -0.05f;
    public float penaltyPerStep = -0.001f;

    public float penaltyPlayerDamage = -0.1f;
    public float rewardPlayerHeal = 0.1f;
    public float penaltyBotDamage = -0.05f;

    public float rewardRunePickup = 0.2f;
    public float rewardRuneDrop = 1.0f;

    public float rewardBatteryGain = 0.05f;
    public float penaltyBatteryWaste = -0.02f;

    [Header("State")]
    public bool hasRune = false;
    private Transform carriedRune;

    private float prevPlayerHp;
    private float prevBotHp;
    private float prevBattery;
    private float prevPlayerSanity;

    public BotHand botHand;
    public BotTorchLightDamage botTorch;
    public float pickupRange = 2f;

    public override void Initialize()
    {
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();

        if (botSensor == null)
            botSensor = GetComponentInChildren<BotSensor>();

        if (botStats == null)
            botStats = GetComponent<BotStats>();

        if (player == null && botSensor != null && botSensor.player != null)
            player = botSensor.player;

        prevPlayerHp = GetPlayerHPPercent();
        prevBotHp = GetBotHPPercent();
        prevBattery = GetBatteryPercent();
        prevPlayerSanity = GetPlayerSanityPercent();

        if (botHand == null)
            botHand = GetComponent<BotHand>();

        if (botTorch == null)
            botTorch = GetComponentInChildren<BotTorchLightDamage>();
    }

    // ============================================================
    // OBSERVATIONS
    // ============================================================
    public override void CollectObservations(VectorSensor sensor)
    {
        // HP
        sensor.AddObservation(GetPlayerHPPercent());
        sensor.AddObservation(GetBotHPPercent());

        // Player stamina
        sensor.AddObservation(GetPlayerSanityPercent());

        // Battery (shared)
        sensor.AddObservation(GetBatteryPercent());

        // Flashlight states
        sensor.AddObservation(botFlashlightOn ? 1f : 0f);

        // Has rune
        sensor.AddObservation(hasRune ? 1f : 0f);

        // Distance to player
        float distToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : hardFollowDistance;
        sensor.AddObservation(distToPlayer / hardFollowDistance);

        // Nearest enemy
        Transform enemy = botSensor != null ? botSensor.GetNearestEnemy(transform) : null;

        if (enemy != null && player != null)
        {
            float distBotEnemy = Vector3.Distance(transform.position, enemy.position);
            float distPlayerEnemy = Vector3.Distance(player.position, enemy.position);

            sensor.AddObservation(distBotEnemy / 20f);
            sensor.AddObservation(distPlayerEnemy / 20f);
        }
        else
        {
            sensor.AddObservation(1f);
            sensor.AddObservation(1f);
        }

        // Enemy count
        int enemyCount = botSensor != null ? Mathf.Clamp(botSensor.enemies.Count, 0, 5) : 0;
        sensor.AddObservation(enemyCount / 5f);

        // Nearest pickup
        Transform pickup = botSensor != null ? botSensor.GetNearestPickup(transform) : null;

        if (pickup != null)
        {
            float distPickup = Vector3.Distance(transform.position, pickup.position);
            sensor.AddObservation(distPickup / 20f);

            sensor.AddObservation(pickup.CompareTag("FIRSTAID") ? 1f : 0f);
            sensor.AddObservation(pickup.CompareTag("TABS") ? 1f : 0f);
            sensor.AddObservation(pickup.CompareTag("BATTERY") ? 1f : 0f);
        }
        else
        {
            sensor.AddObservation(1f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        // Nearest rune
        Transform rune = botSensor != null ? botSensor.GetNearestRune(transform) : null;

        if (rune != null)
        {
            float distRune = Vector3.Distance(transform.position, rune.position);
            sensor.AddObservation(distRune / 20f);
        }
        else
        {
            sensor.AddObservation(1f);
        }

        // Danger level
        sensor.AddObservation(ComputeDangerLevel());
    }

    // ============================================================
    // ACTIONS
    // ============================================================
    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        Transform enemy = botSensor != null ? botSensor.GetNearestEnemy(transform) : null;
        Transform pickup = botSensor != null ? botSensor.GetNearestPickup(transform) : null;
        Transform rune = botSensor != null ? botSensor.GetNearestRune(transform) : null;

        Debug.Log("ACTION: " + action);

        switch (action)
        {
            case 0:
                // Idle
                break;

            case 1:
                // Move to player
                if (player != null)
                    MoveTo(player.position);
                break;

            case 2:
                // Move to enemy
                if (enemy != null)
                    MoveTo(enemy.position);
                break;

            case 3:
                // Move to pickup
                if (pickup != null)
                    MoveTo(pickup.position);
                break;

            case 4:
                // Move to rune
                if (rune != null)
                    MoveTo(rune.position);
                break;

            case 5:
                // Pickup nearest object
                TryPickup();
                break;

            case 6:
                // Drop rune
                TryDropRune();
                break;

            case 7:
                if (botTorch != null && !botFlashlightOn)
                {
                    botTorch.ToggleLight();
                    botFlashlightOn = true;
                }
                break;

            case 8:
                {
                    if (botTorch == null)
                    {
                        AddReward(-0.01f);
                        break;
                    }

                    Transform enemy1 = botSensor != null ? botSensor.GetNearestEnemy(transform) : null;

                    // brak celu = zła decyzja
                    if (enemy == null)
                    {
                        AddReward(-0.01f);
                        break;
                    }

                    float dist = Vector3.Distance(transform.position, enemy.position);

                    // poza zasięgiem = kara (uczy podejścia)
                    if (dist > attackRange)
                    {
                        AddReward(-0.01f);
                        break;
                    }

                    float battery = GetBatteryPercent();

                    // brak energii = kara
                    if (battery <= 0.05f)
                    {
                        AddReward(-0.02f);
                        break;
                    }

                    // właściwy strzał (już stabilny raycast w Shoot)
                    bool hit = botTorch.Shoot();

                    if (hit)
                    {
                        AddReward(0.1f);

                        // dodatkowy reward za faktyczne zadanie obrażeń
                        Simple3StateEnemy e = enemy.GetComponent<Simple3StateEnemy>();
                        if (e != null)
                        {
                            AddReward(0.05f);
                        }
                    }
                    else
                    {
                        AddReward(-0.02f);
                    }

                    // mały koszt decyzji (uczy economy użycia)
                    AddReward(-0.005f);

                    break;
                }

            case 9:
                ItemDropSpot freeSpot = GetNearestFreeDropSpot();
                if (freeSpot != null)
                    MoveTo(freeSpot.transform.position);
                break;

            case 10:
                // Retreat
                RetreatFrom(enemy);
                break;

            case 11:
                if (botTorch != null && botFlashlightOn)
                {
                    botTorch.ToggleLight();
                    botFlashlightOn = false;
                }
                break;


        }

        ApplyRewards();
    }

    // ============================================================
    // PICKUP + DROP
    // ============================================================
    private void TryPickup()
    {
        if (botHand == null) return;
        if (botHand.HasItem()) return;

        // szukamy obiektu z PickupItem w pobliżu
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider hit in hits)
        {
            PickupItem item = hit.GetComponent<PickupItem>();
            if (item == null) continue;

            botHand.PickUp(item);

            AddReward(0.2f); // reward za podniesienie runy
            return;
        }
    }

    private ItemDropSpot GetNearestFreeDropSpot()
    {
        ItemDropSpot best = null;
        float bestDist = Mathf.Infinity;

        foreach (var spot in dropSpots)
        {
            if (spot == null) continue;
            if (spot.IsOccupied()) continue;

            float d = Vector3.Distance(transform.position, spot.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = spot;
            }
        }

        return best;
    }
    private void TryDropRune()
    {
        if (botHand == null) return;
        if (!botHand.HasItem()) return;

        ItemDropSpot freeSpot = GetNearestFreeDropSpot();
        if (freeSpot == null) return;

        float dist = Vector3.Distance(transform.position, freeSpot.transform.position);

        if (dist <= dropInteractRange)
        {
            freeSpot.Interact();
            AddReward(1.0f);
        }
    }

    // ============================================================
    // ITEM USAGE LOGIC
    // ============================================================
    private void UseFirstAid()
    {
        float playerHp = GetPlayerHPPercent();
        float botHp = GetBotHPPercent();

        // decyzja: ratuj gracza jeśli ma mniej HP
        if (playerHp < botHp)
        {
            playerHealthBar.healthSystem.Heal(healAmount);
            playerHealthBar.UpdateHealthBar(playerHealthBar.healthSystem.GetHealthPercent());
        }
        else
        {
            botStats.Heal(healAmount);
        }
    }

    private void UsePills()
    {
        if (playerSanityBar == null) return;

        playerSanityBar.sanitySystem.Heal(SanityRestoreAmount);
        playerSanityBar.UpdateSanityBar(playerSanityBar.sanitySystem.GetSanityPercent());
    }

    private void UseBattery()
    {
        if (batteryBar == null) return;

        batteryBar.batterySystem.Heal(batteryRestoreAmount);
        batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());
    }

    // ============================================================
    // COMBAT
    // ============================================================
    private void TryAttack(Transform enemy)
    {
        if (!botFlashlightOn) return;
        if (enemy == null) return;
        if (batteryBar == null) return;

        float battery = GetBatteryPercent();
        if (battery <= 0.01f) return;

        float dist = Vector3.Distance(transform.position, enemy.position);
        if (dist > attackRange) return;

        // drain battery
        batteryBar.batterySystem.Damage((int)attackBatteryDrain);
        batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());

        // damage enemy
        Simple3StateEnemy e = enemy.GetComponent<Simple3StateEnemy>();
        if (e != null)
        {
            e.takedamage(1);
            AddReward(0.05f);
        }
    }

    private void RetreatFrom(Transform enemy)
    {
        if (enemy == null || player == null) return;

        Vector3 dir = (transform.position - enemy.position).normalized;
        Vector3 target = transform.position + dir * 6f;

        MoveTo(target);
    }

    // ============================================================
    // MOVEMENT
    // ============================================================
    private void MoveTo(Vector3 position)
    {
        if (navAgent == null) return;

        navAgent.isStopped = false;

        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            navAgent.SetDestination(hit.position);
        else
            navAgent.SetDestination(position);
    }

    // ============================================================
    // DANGER LEVEL
    // ============================================================
    private float ComputeDangerLevel()
    {
        float hp = GetPlayerHPPercent();
        float battery = GetBatteryPercent();

        float dangerHP = 1f - hp;

        int enemyCount = botSensor != null ? botSensor.enemies.Count : 0;
        float dangerEnemies = Mathf.Clamp01(enemyCount / 5f);

        float dangerBattery = Mathf.Clamp01((0.3f - battery) / 0.3f);

        float danger = (0.5f * dangerHP) + (0.35f * dangerEnemies) + (0.15f * dangerBattery);

        return Mathf.Clamp01(danger);
    }

    // ============================================================
    // REWARDS
    // ============================================================
    private void ApplyRewards()
    {
        AddReward(penaltyPerStep);

        // follow shaping
        if (player != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            if (distToPlayer <= followDistance)
                AddReward(rewardNearPlayer);

            if (distToPlayer > softFollowDistance)
                AddReward(penaltyTooFarSoft);

            if (distToPlayer > hardFollowDistance)
                AddReward(penaltyTooFarHard);
        }

        // HP shaping
        float currentPlayerHp = GetPlayerHPPercent();
        if (currentPlayerHp < prevPlayerHp)
            AddReward(penaltyPlayerDamage);
        else if (currentPlayerHp > prevPlayerHp)
            AddReward(rewardPlayerHeal);

        float currentBotHp = GetBotHPPercent();
        if (currentBotHp < prevBotHp)
            AddReward(penaltyBotDamage);

        // battery shaping
        float currentBattery = GetBatteryPercent();
        if (currentBattery > prevBattery)
            AddReward(rewardBatteryGain);

        // kara za latarkę ON bez enemy
        if (botFlashlightOn)
        {
            Transform enemy = botSensor != null ? botSensor.GetNearestEnemy(transform) : null;
            if (enemy == null)
                AddReward(penaltyBatteryWaste);
        }

        prevPlayerHp = currentPlayerHp;
        prevBotHp = currentBotHp;
        prevBattery = currentBattery;
        prevPlayerSanity = GetPlayerSanityPercent();

        // end episode
        if (currentPlayerHp <= 0.01f)
        {
            AddReward(-10f);
            EndEpisode();
        }

        if (currentBotHp <= 0.01f)
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

    // ============================================================
    // GETTERS
    // ============================================================
    private float GetPlayerHPPercent()
    {
        if (playerHealthBar == null || playerHealthBar.healthSystem == null)
            return 1f;

        return Mathf.Clamp01(playerHealthBar.healthSystem.GetHealthPercent());
    }

    private float GetPlayerSanityPercent()
    {
        if (playerSanityBar == null || playerSanityBar.sanitySystem == null)
            return 1f;

        return Mathf.Clamp01(playerSanityBar.sanitySystem.GetSanityPercent());
    }

    private float GetBotHPPercent()
    {
        if (botStats == null || botStats.healthSystem == null)
            return 1f;

        return Mathf.Clamp01(botStats.GetHPPercent());
    }

    private float GetBatteryPercent()
    {
        if (batteryBar == null || batteryBar.batterySystem == null)
            return 1f;

        return Mathf.Clamp01(batteryBar.batterySystem.GetBatteryPercent());
    }

    // ============================================================
    // EPISODE RESET
    // ============================================================
    public override void OnEpisodeBegin()
    {
        if (botStats != null)
            botStats.ResetStats();

        if (hasRune && carriedRune != null)
        {
            carriedRune.SetParent(null);
            carriedRune = null;
            hasRune = false;
        }

        botFlashlightOn = false;

        prevPlayerHp = GetPlayerHPPercent();
        prevBotHp = GetBotHPPercent();
        prevBattery = GetBatteryPercent();
        prevPlayerSanity = GetPlayerSanityPercent();
    }

    // ============================================================
    // HEURISTIC
    // ============================================================
    private int lastAction = 0;

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;

        if (Input.GetKeyDown(KeyCode.Alpha1)) lastAction = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) lastAction = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) lastAction = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) lastAction = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) lastAction = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6)) lastAction = 6;
        if (Input.GetKeyDown(KeyCode.Alpha7)) lastAction = 7;
        if (Input.GetKeyDown(KeyCode.Alpha8)) lastAction = 8;
        if (Input.GetKeyDown(KeyCode.Alpha9)) lastAction = 9;
        if (Input.GetKeyDown(KeyCode.Alpha0)) lastAction = 0;

        d[0] = lastAction;
    }
}