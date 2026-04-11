using UnityEngine;

public class BatterySystem
{
    private int battery;
    private int batteryMAX;

    public BatterySystem(int batteryMAX)
    {
        this.batteryMAX = batteryMAX;   
        this.battery = batteryMAX;    
    }

    public int getBattery()
    {
        return battery;  
    }

    public void Damage(int damage)
    {
        this.battery -= damage;
        if (this.battery < 0)
        {
            this.battery = 0;
        }
    }
    public void Heal(int heal)
    {
        this.battery += heal;
        if (this.battery > this.batteryMAX)
        {
            battery = this.batteryMAX;
        }
    }
    public float GetBatteryPercent()
    {
        return (float)this.battery / this.batteryMAX;    
    }
}

