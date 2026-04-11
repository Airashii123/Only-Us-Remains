using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthSystem healthSystem;

    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private int healthMAX = 100;

    private void Awake()
    {
        healthSystem = new HealthSystem(healthMAX);
        UpdateHealthBar(1f);
    }

    public void UpdateHealthBar(float healthPercent)
    {
        _healthbarSprite.fillAmount = healthPercent;
    }
}
