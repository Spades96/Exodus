using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPip : MonoBehaviour
{
    [SerializeField] private string winSceneName = "Win";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(winSceneName);
        }
    }
}