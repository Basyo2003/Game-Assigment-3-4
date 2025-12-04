using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    
    public TextMeshProUGUI nameText;      // Text component for speaker's name
    public TextMeshProUGUI messageText;   // Text component for the dialogue message

    private Coroutine typingCoroutine;     // Used to stop previous typing if a new one begins
    public float typingSpeed = 0.03f;      // Delay between each typed character

    // ---------------------------------------------------------------
    // Sets the speaker name instantly (used by Timeline or scripts)
    // ---------------------------------------------------------------
    public void SetSpeaker(string speaker)
    {
        nameText.text = speaker;
    }

    // ---------------------------------------------------------------
    // Starts a typewriter effect for the given message
    //
    // Timeline or scripts should call it
    // ---------------------------------------------------------------
    public void SetMessage(string message)
    {
        // If a previous typing animation is still running, stop it
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Start typing the new message letter by letter
        typingCoroutine = StartCoroutine(TypeMessage(message));
    }

    // ---------------------------------------------------------------
    // Typewriter coroutine:
    // Reveals the text gradually, one character at a time.
    // ---------------------------------------------------------------
    IEnumerator TypeMessage(string message)
    {
        messageText.text = "";  // Clear text before typing begins

        // Loop through each character in the message
        foreach (char c in message)
        {
            messageText.text += c;               // Add one character
            yield return new WaitForSeconds(typingSpeed);  // Small delay
        }
    }
}
