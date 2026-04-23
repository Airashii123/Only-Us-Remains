using UnityEngine;

public class BatteryCode : MonoBehaviour, IInteractable
{
    public BatteryBar batteryBar;

    [SerializeField] private int heal;

    public Points points;

    [SerializeField] private float wasteP = -2;
    [SerializeField] private float bonusP = 3;
    [SerializeField] private float wasteBorder = 0.75f;

    public string GetInteractPrompt()
    {
        return "[E] Use batteries";
    }

    public void Interact()
    {
        float before = batteryBar.batterySystem.GetBatteryPercent();

        batteryBar.batterySystem.Heal(heal);

        float after = batteryBar.batterySystem.GetBatteryPercent();

        batteryBar.UpdateBatteryBar(after);


        if (before <= wasteBorder)
        {
            points.AddScore(bonusP);
        }
        
        if (after > wasteBorder)
        {
            points.AddScore(wasteP);
        }


        Destroy(gameObject);
    }
}