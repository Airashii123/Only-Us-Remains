using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    public BatterySystem batterySystem;

    [SerializeField] private Image _batterybarSprite;
    [SerializeField] private int batteryMAX = 100;

    private void Awake()
    {
        batterySystem = new BatterySystem(batteryMAX);
        UpdateBatteryBar(1f);
    }

    public void UpdateBatteryBar(float batteryPercent)
    {
        _batterybarSprite.fillAmount = batteryPercent;
    }
}
