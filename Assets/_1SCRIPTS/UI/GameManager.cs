using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ItemDropSpot[] dropPoints;

    private bool hasWon = false;

    void Awake()
    {
        Instance = this;
    }

    public void CheckWinCondition()
    {
        if (hasWon) return;

        foreach (var dp in dropPoints)
        {
            if (!dp.IsOccupied())
                return;
        }

        WinGame();
    }

    void WinGame()
    {
        hasWon = true;

        Debug.Log("WIN!");

        SceneManager.LoadScene(3);

        Time.timeScale = 0f; // zatrzymanie gry
    }
}