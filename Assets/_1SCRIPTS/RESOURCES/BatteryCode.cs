using UnityEngine;

public class BatteryCode : MonoBehaviour, IInteractable
{
    public BatteryBar batteryBar;

    public void Interact()
    {
        batteryBar.batterySystem.Heal(10);
        batteryBar.UpdateBatteryBar(
            batteryBar.batterySystem.GetBatteryPercent()
        );

        Destroy(gameObject);
    }
}
