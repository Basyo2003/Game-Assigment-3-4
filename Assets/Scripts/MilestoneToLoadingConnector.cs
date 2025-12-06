using UnityEngine;

public class MilestoneToLoadingConnector : MonoBehaviour
{
    public MilestoneManager milestoneManager;
    public LoadingScreenManager loadingScreen;
    public string sceneName = "Game_Scene";

    void Start()
    {
        milestoneManager.OnAllMilestonesCompleted += () =>
        {
            loadingScreen.LoadScene(sceneName);
        };
    }
}
