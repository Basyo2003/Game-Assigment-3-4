using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSignalHandler : MonoBehaviour
{
    public string nextSceneName = "NextScene";

    // This method is called by the Timeline Signal
    public void OnCutsceneEnd()
    {

        SceneManager.LoadScene(nextSceneName);
    }
}
