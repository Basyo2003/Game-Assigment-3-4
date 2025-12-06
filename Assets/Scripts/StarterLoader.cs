using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterLoader : MonoBehaviour
{
    public string sceneToLoad = "Level_1";

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
