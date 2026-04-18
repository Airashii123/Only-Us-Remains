using UnityEngine;

public class BatteryCode : MonoBehaviour, IInteractable
{
    public BatteryBar batteryBar;

    public string GetInteractPrompt()
    {
        return "[E] Use batteries";
    }
    public void Interact()
    {
        batteryBar.batterySystem.Heal(10);
        batteryBar.UpdateBatteryBar(
            batteryBar.batterySystem.GetBatteryPercent()
        );

        Destroy(gameObject);
    }
}
