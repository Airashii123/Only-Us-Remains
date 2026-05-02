using System.Collections;
using UnityEngine;

public class BotTorchLightDamage : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light torchLight;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color shotColor = Color.red;

    [Header("Shot Settings")]
    [SerializeField] private float shotFlashTime = 0.1f;
    [SerializeField] private float damageRange = 10f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Battery")]
    public BatteryBar batteryBar;
    [SerializeField] private float batteryDrainRate = 1f;
    [SerializeField] private float batteryShootDrain = 10f;

    private bool flashing = false;
    private bool lightOn = true;

    private Coroutine drainCoroutine;

    private void Start()
    {
        torchLight.enabled = lightOn;

        if (lightOn)
            drainCoroutine = StartCoroutine(DrainBatteryOverTime());
    }

    public bool IsLightOn()
    {
        return lightOn;
    }

    public void ToggleLight()
    {
        float battery = batteryBar.batterySystem.GetBatteryPercent();

        if (!lightOn && battery <= 0.01f)
            return;

        lightOn = !lightOn;
        torchLight.enabled = lightOn;

        if (lightOn)
        {
            if (drainCoroutine == null)
                drainCoroutine = StartCoroutine(DrainBatteryOverTime());
        }
        else
        {
            if (drainCoroutine != null)
            {
                StopCoroutine(drainCoroutine);
                drainCoroutine = null;
            }
        }
    }

    public bool Shoot()
    {
        if (!lightOn) return false;
        if (batteryBar.batterySystem.GetBatteryPercent() <= 0.01f) return false;

        batteryBar.batterySystem.Damage((int)batteryShootDrain);
        batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());

        StartCoroutine(FlashRed());

        Collider[] enemies = Physics.OverlapSphere(transform.position, damageRange, enemyLayer);

        bool hitSomething = false;

        foreach (Collider enemy in enemies)
        {
            Simple3StateEnemy enemyScript = enemy.GetComponent<Simple3StateEnemy>();
            if (enemyScript == null) continue;

            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);

            if (angle <= torchLight.spotAngle / 2f)
            {
                enemyScript.takedamage(1);
                hitSomething = true;
            }
        }

        return hitSomething;
    }

    private IEnumerator FlashRed()
    {
        if (flashing) yield break;

        flashing = true;
        torchLight.color = shotColor;

        yield return new WaitForSeconds(shotFlashTime);

        torchLight.color = normalColor;
        flashing = false;
    }

    private IEnumerator DrainBatteryOverTime()
    {
        while (lightOn)
        {
            float battery = batteryBar.batterySystem.GetBatteryPercent();

            if (battery <= 0.01f)
            {
                ForceTurnOff();
                yield break;
            }

            batteryBar.batterySystem.Damage((int)batteryDrainRate);
            batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());

            yield return new WaitForSeconds(1f);
        }

        drainCoroutine = null;
    }

    private void ForceTurnOff()
    {
        lightOn = false;
        torchLight.enabled = false;

        if (drainCoroutine != null)
        {
            StopCoroutine(drainCoroutine);
            drainCoroutine = null;
        }
    }
}