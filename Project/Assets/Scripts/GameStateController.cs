using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public static string lastCheckpointScene = "Level 1";
    public static int playerMaxHealth = 10; 

    public void Easy()
    {
        playerMaxHealth = 40;
        SceneManager.LoadScene("Intro");
    }

    public void Medium()
    {
        playerMaxHealth = 10;
        SceneManager.LoadScene("Intro");
    }

    public void Hard()
    {
        playerMaxHealth = 1;
        SceneManager.LoadScene("Intro");
    }

    public void LaunchGame() { SceneManager.LoadScene("Difficulty"); }
    public void Level1() { lastCheckpointScene = "Level 1"; SceneManager.LoadScene("Level 1"); }
    public void Boss1Lore() { SceneManager.LoadScene("Boss 1 lore"); }
    public void Level2() { lastCheckpointScene = "Level 2"; SceneManager.LoadScene("Level 2"); }
    public void Boss2Lore() { SceneManager.LoadScene("Boss 2 lore"); }
    public void Level3() { lastCheckpointScene = "Level 3"; SceneManager.LoadScene("Level 3"); }
    public void Boss3Lore() { SceneManager.LoadScene("Boss 3 lore"); }
    public void Harpe() { lastCheckpointScene = "Harpe"; SceneManager.LoadScene("Harpe"); }
    public void Ending() { SceneManager.LoadScene("Ending"); }
    public void RestartGame() { SceneManager.LoadScene(lastCheckpointScene); }
    public void MainMenu() { SceneManager.LoadScene("Title"); }
    public void Win() { SceneManager.LoadScene("Win"); }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}