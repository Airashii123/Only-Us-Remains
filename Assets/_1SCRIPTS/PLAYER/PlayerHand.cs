using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance;

    public Transform holdPoint;

    private PickupItem heldItem;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (heldItem != null)
        {
            heldItem.Follow(holdPoint);

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Drop();
            }
        }
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

    public bool HasItem()
    {
        return heldItem != null;
    }

    public PickupItem GetItem()
    {
        return heldItem;
    }
}