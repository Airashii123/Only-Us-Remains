using UnityEngine;

public class BotHand : MonoBehaviour
{
    public Transform holdPoint;
    private PickupItem heldItem;

    void Update()
    {
        if (heldItem != null)
        {
            heldItem.Follow(holdPoint);
        }
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public PickupItem GetItem()
    {
        return heldItem;
    }

    public void PickUp(PickupItem item)
    {
        if (heldItem != null) return;

        heldItem = item;
        item.SetHeld(true);
    }

    public void Drop()
    {
        if (heldItem == null) return;

        heldItem.SetHeld(false);
        heldItem.transform.position = holdPoint.position + holdPoint.forward * 0.5f;

        heldItem = null;
    }
}