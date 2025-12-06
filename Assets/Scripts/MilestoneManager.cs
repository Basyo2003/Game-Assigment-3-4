using UnityEngine;
using TMPro;

public class MilestoneManager : MonoBehaviour
{
    [TextArea]
    public string[] milestones;       // List of steps you want to show
    public TextMeshProUGUI uiText;    // Assign your MilestoneText here
    public System.Action OnAllMilestonesCompleted; // event callback

    private int currentMilestone = 0;

    void Start()
    {
        ShowCurrentMilestone();
    }

    // Shows the current milestone in the UI
    void ShowCurrentMilestone()
    {
        if (currentMilestone < milestones.Length)
        {
            uiText.text = $"Objective:\n{milestones[currentMilestone]}";
        }
        else
        {
            uiText.text = "All objectives completed!";
            // 🔥 Trigger loading screen
            //wait 
            OnAllMilestonesCompleted?.Invoke();
        }
    }

    // Called by NPC when the player finishes a step
    public void CompleteMilestone()
    {
        currentMilestone++;
        ShowCurrentMilestone();
    }

    public int GetCurrentMilestone()
    {
        return currentMilestone;
    }
}
