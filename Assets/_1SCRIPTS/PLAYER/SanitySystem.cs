using UnityEngine;

public class SanitySystem
{
    private int sanity;
    private int sanityMAX;


    public SanitySystem(int sanityMAX)
    {
        this.sanityMAX = sanityMAX;   
        this.sanity = sanityMAX;    
    }

    public int getSanity()
    {
        return sanity;  
    }

    public void Damage(int damage)
    {
        this.sanity -= damage;
        if (this.sanity < 0)
        {
            this.sanity = 0;
        }
    }
    public void Heal(int heal)
    {
        this.sanity += heal;
        if (this.sanity > this.sanityMAX)
        {
            sanity = this.sanityMAX;
        }
    }
    public float GetSanityPercent()
    {
        return (float)this.sanity / this.sanityMAX;    
    }
}

