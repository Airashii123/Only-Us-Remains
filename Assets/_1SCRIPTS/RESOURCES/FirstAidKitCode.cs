using UnityEngine;

public class FirstAidKitCode : MonoBehaviour, IInteractable
{
    public HealthBar healthBar;
    [SerializeField] private int heal;

    public Points points;

    [SerializeField] private float wasteP = -2;
    [SerializeField] private float bonusP = 3;
    [SerializeField] private float wasteBorder = 0.75f;

    public string GetInteractPrompt()
    {
        return "[E] Use first aid kit";
    }

    public void Interact()
    {
        float before = healthBar.healthSystem.GetHealthPercent();
        healthBar.healthSystem.Heal(heal);
        float after = healthBar.healthSystem.GetHealthPercent();

        healthBar.UpdateHealthBar(after);

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