using UnityEngine;

public class LampToggle : MonoBehaviour
{
    [Header("Lamp Settings")]
    public Light lampLight;          // The light to toggle
    public float interactionDistance = 3f; // How close player must be
    public KeyCode toggleKey = KeyCode.Y;  // Key to toggle lamp

    private Transform player;
    private bool isOn = true;

    void Start()
    {
        if (lampLight == null)
        {
            lampLight = GetComponentInChildren<Light>();
        }

        // Try to find player automatically
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player is tagged 'Player'.");
        }
    }

    void Update()
    {
        if (player == null || lampLight == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // If player is near enough
        if (distance <= interactionDistance)
        {
            // Listen for toggle key
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleLamp();
            }
        }
    }

    void ToggleLamp()
    {
        isOn = !isOn;
        lampLight.enabled = isOn;

        // Optional: print to console
        Debug.Log($"Lamp '{gameObject.name}' turned {(isOn ? "ON" : "OFF")}");
    }

    // Optional: visualize the range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
