using UnityEngine;

public class Group1AnimationTrigger : MonoBehaviour
{
    public Animator[] animators;

    // Call this from Timeline signal
    public void PlayAnimation(string triggerName)
    {
        foreach (var anim in animators)
        {
            anim.SetTrigger(triggerName);
        }
    }
}
