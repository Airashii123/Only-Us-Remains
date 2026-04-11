using UnityEngine;

public class PillsCode : MonoBehaviour, IInteractable
{
    public SanityBar sanityBar;

    public void Interact()
    {
        sanityBar.sanitySystem.Heal(10);
        sanityBar.UpdateSanityBar(
            sanityBar.sanitySystem.GetSanityPercent()
        );

        Destroy(gameObject);
    }
}
