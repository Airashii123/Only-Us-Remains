using UnityEngine;

public class CandleInteraction : MonoBehaviour
{
    public GameObject runeStone;   // Przypisz w inspectorze kamień z runą
    public GameObject candleFire;  // Opcjonalnie obiekt z efektem płomienia świeczki

    private bool isLit = false;

    void OnTriggerStay(Collider other)
    {
        // Zakładamy, że gracz ma tag "Player"
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            LightCandle();
        }
    }

    void LightCandle()
    {
        if (!isLit)
        {
            isLit = true;
            // Włącz efekt świecy
            if (candleFire != null)
                candleFire.SetActive(true);

            // Włącz kamień z runą
            if (runeStone != null)
                runeStone.SetActive(true);

            Debug.Log("Świeczka zapalona, kamień aktywowany!");
        }
    }
}