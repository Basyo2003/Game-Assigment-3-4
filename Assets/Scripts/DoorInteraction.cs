using UnityEngine;

public class DoorAnimatorToggle : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.M;
    public float interactDistance = 3f;

    private Animator animator;
    private Transform player;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Tag your player as 'Player'.");
    }

    void Update()
    {
        if (player == null || animator == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;
            animator.SetBool("isOpen", isOpen);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
