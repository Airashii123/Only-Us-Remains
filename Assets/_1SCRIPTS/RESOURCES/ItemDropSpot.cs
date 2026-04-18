using UnityEngine;

public class ItemDropSpot : MonoBehaviour, IInteractable
{
    public Transform placePoint;

    private PickupItem storedItem;

    public string GetInteractPrompt()
    {
        if (storedItem == null)
            return "[E] Place";
        else
            return "";
    }

    public void Interact()
    {
        if (storedItem != null) return;

        var item = PlayerHand.Instance.GetItem();
        if (item == null) return;

        PlayerHand.Instance.Drop();

        storedItem = item;

        item.transform.SetPositionAndRotation(placePoint.position, placePoint.rotation);

        item.LockInPlace();

        GameManager.Instance.CheckWinCondition();
    }

    public bool IsOccupied()
    {
        return storedItem != null;
    }
}