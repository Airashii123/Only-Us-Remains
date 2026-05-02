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

        // PLAYER
        if (PlayerHand.Instance != null && PlayerHand.Instance.HasItem())
        {
            var item = PlayerHand.Instance.GetItem();
            PlayerHand.Instance.Drop();

            storedItem = item;
            item.transform.SetPositionAndRotation(placePoint.position, placePoint.rotation);
            item.LockInPlace();

            GameManager.Instance.CheckWinCondition();
            return;
        }

        // BOT
        BotHand botHand = FindObjectOfType<BotHand>();
        if (botHand != null && botHand.HasItem())
        {
            var item = botHand.GetItem();
            botHand.Drop();

            storedItem = item;
            item.transform.SetPositionAndRotation(placePoint.position, placePoint.rotation);
            item.LockInPlace();

            GameManager.Instance.CheckWinCondition();
            return;
        }
    }

    public bool IsOccupied()
    {
        return storedItem != null;
    }
}