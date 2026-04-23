using UnityEngine;

public class PillsCode : MonoBehaviour, IInteractable
{
    public SanityBar sanityBar;
    [SerializeField] private int heal;

    public Points points;

    [SerializeField] private float wasteP = -2;
    [SerializeField] private float bonusP = 3;
    [SerializeField] private float wasteBorder = 0.75f;

    public string GetInteractPrompt()
    {
        return "[E] Use pills";
    }
    public void Interact()
    {
        float before = sanityBar.sanitySystem.GetSanityPercent();
        sanityBar.sanitySystem.Heal(heal);
        float after = sanityBar.sanitySystem.GetSanityPercent();
        sanityBar.UpdateSanityBar(after);

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
