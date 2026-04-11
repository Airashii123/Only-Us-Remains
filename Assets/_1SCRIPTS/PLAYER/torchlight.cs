using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TorchLightDamage : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference toggleLightAction; // np. F
    [SerializeField] private InputActionReference shootAction;       // LPM

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
    [SerializeField] private float batteryDrainRate = 1f; // ile na sekundę

    private bool flashing = false;
    private bool lightOn = false;

    private void OnEnable()
    {
        toggleLightAction.action.Enable();
        shootAction.action.Enable();

        toggleLightAction.action.performed += ToggleLight;
        shootAction.action.started += Shoot;
    }

    private void OnDisable()
    {
        toggleLightAction.action.performed -= ToggleLight;
        shootAction.action.started -= Shoot;

        toggleLightAction.action.Disable();
        shootAction.action.Disable();
    }

    private void ToggleLight(InputAction.CallbackContext ctx)
    {
        lightOn = !lightOn;
        torchLight.enabled = lightOn;

        if (lightOn)
        {
            StartCoroutine(DrainBatteryOverTime());
        }
    }

    private void Shoot(InputAction.CallbackContext ctx)
    {
        if (!torchLight.enabled) return;

        // Zużycie baterii na strzał
        batteryBar.batterySystem.Damage(15);
        batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());

        StartCoroutine(FlashRed());

        Collider[] enemies = Physics.OverlapSphere(transform.position, damageRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            Simple3StateEnemy enemyScript = enemy.GetComponent<Simple3StateEnemy>();
            if (enemyScript == null) continue;

            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);

            if (angle <= torchLight.spotAngle / 2f)
            {
                enemyScript.takedamage(1);
            }
        }
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
        while (lightOn && batteryBar.batterySystem.GetBatteryPercent() > 0)
        {
            batteryBar.batterySystem.Damage((int)batteryDrainRate); // np. 1 punkt na sekundę
            batteryBar.UpdateBatteryBar(batteryBar.batterySystem.GetBatteryPercent());

            yield return new WaitForSeconds(1f);
        }

        // Wyłącz latarkę, jeśli bateria się skończy
        if (batteryBar.batterySystem.GetBatteryPercent() <= 0)
        {
            lightOn = false;
            torchLight.enabled = false;
        }
    }
}