using UnityEngine;
using StarterAssets;

/// <summary>
/// Serializable structure to store a single line of dialogue
/// </summary>
[System.Serializable]
public struct DialogueLine
{
    public string speaker;  // The name of the character speaking
    public string message;  // The dialogue text to display
}

/// <summary>
/// Handles dialogue triggering and progression when the player is nearby
/// Displays interaction prompts and manages dialogue flow
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject dialogueUI;              // The main dialogue UI panel
    public GameObject interactionPopup;        // Popup indicator shown when player is in range
    public Transform player;                   // Reference to the player's transform
    public DialogueController dialogueController;  // Controller that displays the dialogue content

    public int milestoneIndex = -1;    // Which milestone this NPC completes
    public MilestoneManager milestoneManager;


    [Header("Dialogue Data")]
    public DialogueLine[] lines;               // Array of dialogue lines for this conversation

    [Header("Interaction Settings")]
    public float activationRange = 3f;         // Distance at which player can interact
    public KeyCode interactKey = KeyCode.M;    // Key to press to start/advance dialogue

    private int currentIndex = 0;              // Current line being displayed
    private bool dialogueActive = false;       // Whether dialogue is currently active
    private LylekGames.Tools.CharacterMovement playerMovement;  // Cached movement script for freezing
    private StarterAssetsInputs playerInputs;  // Cached Starter Assets input wrapper
    private ThirdPersonController thirdPersonController;  // Optional third-person controller
    private FirstPersonController firstPersonController;  // Optional first-person controller
    private bool starterInputsWereEnabled = true;  // Original Starter Assets input state
    private bool thirdPersonWasEnabled = true;     // Original third-person controller state
    private bool firstPersonWasEnabled = true;     // Original first-person controller state

    /// <summary>
    /// Initialize the dialogue system by hiding UI elements
    /// </summary>
    void Start()
    {
        // Hide the main dialogue UI at the start
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        // Hide the interaction prompt at the start
        if (interactionPopup != null)
            interactionPopup.SetActive(false);

        // Cache the player's movement/input scripts so we can quickly freeze/unfreeze
        EnsurePlayerComponents();
    }

    /// <summary>
    /// Check player distance and handle interaction input each frame
    /// </summary>
    void Update()
    {
        // Safety check: ensure required references exist
        if (player == null || dialogueController == null) return;

        // Calculate distance between player and this dialogue trigger
        float distance = Vector3.Distance(player.position, transform.position);

        // If player is too far away, hide the interaction popup
        if (distance > activationRange)
        {
            if (interactionPopup != null && interactionPopup.activeSelf)
                interactionPopup.SetActive(false);

            return;  // Exit early if player is out of range
        }

        // If player is within range and dialogue is not active, show interaction popup
        if (!dialogueActive && interactionPopup != null)
            interactionPopup.SetActive(true);

        // Handle interaction key press
        if (Input.GetKeyDown(interactKey))
        {
            if (!dialogueActive)
            {
                // Start new dialogue when key is pressed while inactive
                if (interactionPopup != null)
                    interactionPopup.SetActive(false);

                StartDialogue();
            }
            else
            {
                // Advance to next line when key is pressed during active dialogue
                NextLine();
            }
        }
    }

    /// <summary>
    /// Initialize and display the first line of dialogue
    /// </summary>
    void StartDialogue()
    {
        // Safety check: ensure there are dialogue lines to display
        if (lines == null || lines.Length == 0) return;

        // Set dialogue state to active
        dialogueActive = true;
        currentIndex = 0;

        // Freeze the player so they can't move during dialogue
        EnsurePlayerComponents();

        if (playerMovement != null)
            playerMovement.FreezePlayer();

        if (playerInputs != null)
        {
            starterInputsWereEnabled = playerInputs.InputEnabled;
            if (starterInputsWereEnabled)
                playerInputs.SetInputEnabled(false);
        }

        if (thirdPersonController != null)
        {
            thirdPersonWasEnabled = thirdPersonController.enabled;
            if (thirdPersonWasEnabled)
                thirdPersonController.enabled = false;
        }

        if (firstPersonController != null)
        {
            firstPersonWasEnabled = firstPersonController.enabled;
            if (firstPersonWasEnabled)
                firstPersonController.enabled = false;
        }

        // Clear any previous dialogue text
        dialogueController.SetSpeaker("");
        dialogueController.SetMessage("");

        // Show the dialogue UI and display the first line
        dialogueUI.SetActive(true);
        ShowCurrentLine();
    }

    /// <summary>
    /// Advance to the next line of dialogue or end if complete
    /// </summary>
    void NextLine()
    {
        // Move to the next line
        currentIndex++;

        // If we've reached the end, close the dialogue
        if (currentIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        // Display the next line
        ShowCurrentLine();
    }

    /// <summary>
    /// Display the current dialogue line's speaker and message
    /// </summary>
    void ShowCurrentLine()
    {
        // Get the current line from the dialogue array
        var line = lines[currentIndex];
        
        // Update the dialogue UI with the speaker name and message
        dialogueController.SetSpeaker(line.speaker);
        dialogueController.SetMessage(line.message);
    }

    /// <summary>
    /// Close the dialogue and reset all state variables
    /// </summary>
    void EndDialogue()
    {
        // Mark dialogue as no longer active
        dialogueActive = false;
        
        // Reset the line index to the beginning
        currentIndex = 0;

        // Unfreeze the player so they can move again
        EnsurePlayerComponents();

        if (playerMovement != null)
            playerMovement.UnfreezePlayer();

        if (playerInputs != null && !playerInputs.InputEnabled && starterInputsWereEnabled)
            playerInputs.SetInputEnabled(true);

        if (thirdPersonController != null && !thirdPersonController.enabled && thirdPersonWasEnabled)
            thirdPersonController.enabled = true;

        if (firstPersonController != null && !firstPersonController.enabled && firstPersonWasEnabled)
            firstPersonController.enabled = true;

        // Clear the dialogue display
        dialogueController.SetSpeaker("");
        dialogueController.SetMessage("");

        // Hide the dialogue UI
        dialogueUI.SetActive(false);

        // Complete milestone only if:
        // 1. NPC has a milestone assigned
        // 2. Manager exists
        // 3. NPC’s milestoneIndex == current active milestone
        if (milestoneIndex != -1 &&
            milestoneManager != null &&
            milestoneManager.GetCurrentMilestone() == milestoneIndex)
        {
            milestoneManager.CompleteMilestone();
        }

    }
    /// <summary>
    /// Try to locate the movement component on the player hierarchy
    /// </summary>
    void EnsurePlayerComponents()
    {
        if (player == null) return;

        // Try to find the movement component directly, or within the hierarchy
        playerMovement = player.GetComponent<LylekGames.Tools.CharacterMovement>()
                         ?? player.GetComponentInChildren<LylekGames.Tools.CharacterMovement>()
                         ?? player.GetComponentInParent<LylekGames.Tools.CharacterMovement>();

        // Cache Starter Assets input/controller scripts if they exist
        playerInputs = player.GetComponent<StarterAssetsInputs>()
                      ?? player.GetComponentInChildren<StarterAssetsInputs>()
                      ?? player.GetComponentInParent<StarterAssetsInputs>();

        thirdPersonController = player.GetComponent<ThirdPersonController>()
                              ?? player.GetComponentInChildren<ThirdPersonController>()
                              ?? player.GetComponentInParent<ThirdPersonController>();

        firstPersonController = player.GetComponent<FirstPersonController>()
                              ?? player.GetComponentInChildren<FirstPersonController>()
                              ?? player.GetComponentInParent<FirstPersonController>();
    }
}
