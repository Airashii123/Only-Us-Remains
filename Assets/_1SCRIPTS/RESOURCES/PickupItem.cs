using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName = "Item";

    private Rigidbody rb;
    private Collider col;

    private bool isHeld = false;
    private bool isLockedInPlace = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public string GetInteractPrompt()
    {
        if (isLockedInPlace) return "";
        return isHeld ? "" : $"[E] Pick {itemName}";
    }

    public void Interact()
    {
        if (isLockedInPlace) return;

        PlayerHand.Instance.PickUp(this);
    }

    public void SetHeld(bool held)
    {
        isHeld = held;

        rb.isKinematic = held;
        col.enabled = !held;
    }

    public void LockInPlace()
    {
        isLockedInPlace = true;

        rb.isKinematic = true;
        col.enabled = false;
    }

    public void Follow(Transform holdPoint)
    {
        transform.position = holdPoint.position;
        transform.rotation = holdPoint.rotation;
    }
}