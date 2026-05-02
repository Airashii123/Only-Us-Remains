using UnityEngine;

public class HealthSystem
{
    private int health;
    private int healthMAX;

    public Points points;
    public float attackedPoints;

    public HealthSystem(int healthMAX, Points points)
    {
        this.healthMAX = healthMAX;
        this.health = healthMAX;

        this.points = points;
    }

    public int getHealth()
    {
        return health;
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health < 0)
        {
            health = 0;
        }
    }

    public void Attacked(int damage)
    {
        health -= damage;

        if (health < 0)
        {
            health = 0;
        }

        if (points != null)
        {
            points.AddScore(attackedPoints);
        }
    }

    public void Heal(int heal)
    {
        health += heal;

        if (health > healthMAX)
        {
            health = healthMAX;
        }
    }

    public float GetHealthPercent()
    {
        return (float)health / healthMAX;
    }
}