using UnityEngine;
using UnityEngine.UI;

public class GameManagerSingleton : MonoBehaviour
{
    public static GameManagerSingleton Instance { get; private set; }

    [Header("UI References")]
    public Text scoreText;      // Legacy Text (UI)
    public TMPro.TextMeshProUGUI scoreTMP; // TMP alternative (optional)

    [Header("Player Stats")]
    public int playerScore = 0;

    private void Awake()
    {
        // Make this persist between scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int value)
    {
        playerScore += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // Update both types of UI if assigned
        if (scoreText != null)
            scoreText.text = $"Score: {playerScore}";

        if (scoreTMP != null)
            scoreTMP.text = $"Score: {playerScore}";
    }
}
