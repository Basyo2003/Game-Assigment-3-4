using UnityEngine;
using StarterAssets;

[System.Serializable]
public struct DialogueLine
{
    public string speaker;
    public string message;
}

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject dialogueUI;                     // The UI panel showing dialogue
    public Transform player;                          // Player transform
    public DialogueController dialogueController;     // Handles UI text

    [Header("Shared Interaction Popup (One for ALL NPCs)")]
    public RectTransform interactionPopup;            // The global "Press M to talk" popup
    public Vector3 popupOffset = new Vector3(0, 150, 0);

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    [Header("Settings")]
    public float activationRange = 3f;                // Distance to detect player
    public KeyCode interactKey = KeyCode.M;           // Key to talk

    [Header("Milestones")]
    public int milestoneIndex = -1;
    public MilestoneManager milestoneManager;

    private int currentIndex = 0;
    private bool dialogueActive = false;
    private bool playerInRange = false;

    // Player systems
    private LylekGames.Tools.CharacterMovement playerMovement;
    private StarterAssetsInputs starterInputs;
    private ThirdPersonController thirdPersonController;
    private FirstPersonController firstPersonController;

    private bool starterInputsWereEnabled = true;
    private bool thirdPersonWasEnabled = true;
    private bool firstPersonWasEnabled = true;

    // Tracks which DialogueTrigger is using the popup
    private static DialogueTrigger activePopupOwner = null;


    // ============================================================
    // INITIALIZATION
    // ============================================================
    private void Start()
    {
        if (dialogueUI) dialogueUI.SetActive(false);
        if (interactionPopup) interactionPopup.gameObject.SetActive(false);

        CachePlayerComponents();
    }


    // ============================================================
    // UPDATE LOOP
    // ============================================================
    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        playerInRange = distance <= activationRange;

        HandlePopup();
        HandleInteraction();
    }


    // ============================================================
    // POPUP MANAGEMENT (ONE UI FOR ALL NPCs)
    // ============================================================
    private void HandlePopup()
    {
        if (!interactionPopup) return;

        // Player is OUT of range → hide popup if this trigger owns it
        if (!playerInRange)
        {
            if (activePopupOwner == this && interactionPopup.gameObject.activeSelf)
                interactionPopup.gameObject.SetActive(false);

            return;
        }

        // Player IN range but dialogue NOT active
        if (!dialogueActive)
        {
            // Give ownership of popup to this NPC
            activePopupOwner = this;
            interactionPopup.gameObject.SetActive(true);
        }
    }


    // ============================================================
    // PLAYER INPUT (INTERACTION)
    // ============================================================
    private void HandleInteraction()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!dialogueActive)
            {
                // Hide popup globally
                if (interactionPopup) interactionPopup.gameObject.SetActive(false);
                activePopupOwner = null;

                StartDialogue();
            }
            else
            {
                NextLine();
            }
        }
    }


    // ============================================================
    // DIALOGUE FLOW
    // ============================================================
    private void StartDialogue()
    {
        if (lines == null || lines.Length == 0) return;

        dialogueActive = true;
        currentIndex = 0;

        FreezePlayer();
        dialogueUI.SetActive(true);

        ShowCurrentLine();
    }


    private void NextLine()
    {
        currentIndex++;

        if (currentIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }


    private void ShowCurrentLine()
    {
        var line = lines[currentIndex];
        dialogueController.SetSpeaker(line.speaker);
        dialogueController.SetMessage(line.message);
    }


    private void EndDialogue()
    {
        dialogueActive = false;

        UnfreezePlayer();
        dialogueUI.SetActive(false);

        dialogueController.SetSpeaker("");
        dialogueController.SetMessage("");

        // Hide popup when dialogue ends (waits for next NPC)
        if (interactionPopup)
            interactionPopup.gameObject.SetActive(false);

        // Handle milestone
        if (milestoneIndex != -1 &&
            milestoneManager != null &&
            milestoneManager.GetCurrentMilestone() == milestoneIndex)
        {
            milestoneManager.CompleteMilestone();
        }
    }


    // ============================================================
    // PLAYER MOVEMENT CONTROL
    // ============================================================
    private void FreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.FreezePlayer();

        if (starterInputs != null)
        {
            starterInputsWereEnabled = starterInputs.InputEnabled;
            starterInputs.SetInputEnabled(false);
        }

        if (thirdPersonController != null)
        {
            thirdPersonWasEnabled = thirdPersonController.enabled;
            thirdPersonController.enabled = false;
        }

        if (firstPersonController != null)
        {
            firstPersonWasEnabled = firstPersonController.enabled;
            firstPersonController.enabled = false;
        }
    }


    private void UnfreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.UnfreezePlayer();

        if (starterInputs != null && !starterInputs.InputEnabled && starterInputsWereEnabled)
            starterInputs.SetInputEnabled(true);

        if (thirdPersonController != null && !thirdPersonController.enabled && thirdPersonWasEnabled)
            thirdPersonController.enabled = true;

        if (firstPersonController != null && !firstPersonController.enabled && firstPersonWasEnabled)
            firstPersonController.enabled = true;
    }


    // ============================================================
    // CACHE PLAYER COMPONENTS
    // ============================================================
    private void CachePlayerComponents()
    {
        if (player == null) return;

        playerMovement =
            player.GetComponentInChildren<LylekGames.Tools.CharacterMovement>();

        starterInputs =
            player.GetComponentInChildren<StarterAssetsInputs>();

        thirdPersonController =
            player.GetComponentInChildren<ThirdPersonController>();

        firstPersonController =
            player.GetComponentInChildren<FirstPersonController>();
    }
}
