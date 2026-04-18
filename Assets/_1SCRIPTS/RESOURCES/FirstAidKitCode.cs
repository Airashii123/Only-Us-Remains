using UnityEngine;

public class FirstAidKitCode : MonoBehaviour, IInteractable
{
    public HealthBar healthBar;

    public string GetInteractPrompt()
    {
        return "[E] Use first aid kit";
    }

    public void Interact()
    {
        healthBar.healthSystem.Heal(10);
        healthBar.UpdateHealthBar(
            healthBar.healthSystem.GetHealthPercent()
        );

        Destroy(gameObject);
    }
}