using UnityEngine;

public class HealthSystem
{
    private int health;
    private int healthMAX;

    public HealthSystem(int healthMAX)
    {
        this.healthMAX = healthMAX;   
        this.health = healthMAX;    
    }

    public int getHealth()
    {
        return health;  
    }

    public void Damage(int damage)
    {
        this.health -= damage;
        if (this.health < 0)
        {
            this.health = 0;
        }
    }
    public void Heal(int heal)
    {
        this.health += heal;
        if (this.health > this.healthMAX)
        {
            health = this.healthMAX;
        }
    }
    public float GetHealthPercent()
    {
        return (float)this.health / this.healthMAX;    
    }
}

