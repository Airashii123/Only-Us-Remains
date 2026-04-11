using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public SanitySystem sanitySystem;

    [SerializeField] private Image _sanitybarSprite;
    [SerializeField] private int sanityMAX = 100;

    private void Awake()
    {
        sanitySystem = new SanitySystem(sanityMAX);
        UpdateSanityBar(1f);
    }

    public void UpdateSanityBar(float sanityPercent)
    {
        _sanitybarSprite.fillAmount = sanityPercent;
    }
}
