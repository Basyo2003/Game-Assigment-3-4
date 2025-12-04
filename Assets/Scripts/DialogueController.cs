using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;

    public void SetSpeaker(string speaker)
    {
        nameText.text = speaker;
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
