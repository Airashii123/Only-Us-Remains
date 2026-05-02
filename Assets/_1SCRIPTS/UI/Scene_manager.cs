using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_manager : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Win()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);
    }

    public void Lose()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    public void Train()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(4);
    }
}