using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreen;   // Assign the Canvas here

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        loadingScreen.SetActive(false);
    }

    // Call this to load a new game scene
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    private System.Collections.IEnumerator LoadAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // Wait until loading finishes
        while (op.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(1f); // artificial delay for spinner

        op.allowSceneActivation = true;

        // Hide canvas AFTER the scene is ready
        yield return new WaitForSeconds(0.2f);
        loadingScreen.SetActive(false);
    }
}
