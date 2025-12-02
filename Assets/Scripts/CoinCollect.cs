using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    [Header("Coin Settings")]
    public float rotationSpeed = 100f;
    public AudioClip collectSound;
    public GameObject collectEffect;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (collectSound)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        if (collectEffect)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // Add score using singleton
        if (GameManagerSingleton.Instance != null)
            GameManagerSingleton.Instance.AddScore(1);

        Destroy(gameObject);
    }
}
