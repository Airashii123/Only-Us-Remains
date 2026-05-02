using UnityEngine;

public class BotStats : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int hpMax = 100;

    public HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = new HealthSystem(hpMax, null);
    }

    public float GetHPPercent()
    {
        return healthSystem.GetHealthPercent();
    }

    public int GetHP()
    {
        return healthSystem.getHealth();
    }

    public void Damage(int dmg)
    {
        healthSystem.Damage(dmg);
    }

    public void Heal(int heal)
    {
        healthSystem.Heal(heal);
    }

    public bool IsDead()
    {
        return GetHP() <= 0;
    }

    public void ResetStats()
    {
        healthSystem = new HealthSystem(hpMax, null);
    }
}