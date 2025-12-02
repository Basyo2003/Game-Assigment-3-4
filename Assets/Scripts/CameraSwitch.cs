using UnityEngine;
using StarterAssets;

public class CameraToggle : MonoBehaviour
{
    [Header("Assign the full camera GameObjects")]
    public GameObject fpsCameraRoot;
    public GameObject tpsCameraRoot;
    [Tooltip("Assign up to 3 player visual roots to hide in FPS mode. Leave unused entries empty.")]
    public GameObject[] playerBodies = new GameObject[3]; // support up to 3 bodies
    [Tooltip("Delay (seconds) before hiding the playerBodies when switching to FPS")] public float hideDelay = 0.25f;
    [Tooltip("If true, only disables Renderer/SkinnedMeshRenderer components instead of SetActive(false)")]
    public bool disableRenderersOnly = false;

    Coroutine _hideCoroutine;

    void Start()
    {
        // Start in third person by default
        SetActiveCamera(tpsCameraRoot);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetActiveCamera(fpsCameraRoot);

        if (Input.GetKeyDown(KeyCode.T))
            SetActiveCamera(tpsCameraRoot);
    }

    void SetActiveCamera(GameObject activeCamRoot)
    {
        if (fpsCameraRoot == null || tpsCameraRoot == null)
        {
            Debug.LogWarning("Assign both camera roots in the Inspector!");
            return;
        }

        bool nowFPS = (activeCamRoot == fpsCameraRoot);

        // Activate/deactivate camera roots
        fpsCameraRoot.SetActive(nowFPS);
        tpsCameraRoot.SetActive(!nowFPS);

        // Notify the player controller (if present) about the camera mode change
        var playerController = GameObject.FindFirstObjectByType<ThirdPersonController>();
        if (playerController != null)
        {
            playerController.SetFirstPersonMode(nowFPS);
        }

        // Also toggle the inspector-assigned player body (if any).
        // Use a short delayed hide when entering FPS so the body doesn't disappear
        // before the camera transition completes. If we switch back before the delay
        // completes, cancel the pending hide.
        // Operate on the array of player bodies (support empty/null entries)
        if (playerBodies != null && playerBodies.Length > 0)
        {
            // Stop any pending coroutine
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            if (nowFPS)
            {
                Debug.Log($"CameraToggle: switching to FPS, will hide playerBodies after {hideDelay}s");
                if (disableRenderersOnly)
                    _hideCoroutine = StartCoroutine(DelayedDisableRenderersMultiple(playerBodies, hideDelay));
                else
                    _hideCoroutine = StartCoroutine(DelayedHideMultiple(playerBodies, hideDelay));
            }
            else
            {
                Debug.Log("CameraToggle: switching to TPS, showing playerBodies immediately");
                foreach (var pb in playerBodies)
                {
                    if (pb == null) continue;
                    if (disableRenderersOnly) SetRenderersEnabled(pb, true);
                    else pb.SetActive(true);
                }
            }
        }
    }
    System.Collections.IEnumerator DelayedHide(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (go == null)
        {
            _hideCoroutine = null;
            yield break;
        }

        Debug.Log("CameraToggle: hiding playerBody (SetActive(false))");
        go.SetActive(false);
        _hideCoroutine = null;
    }

    System.Collections.IEnumerator DelayedDisableRenderersMultiple(GameObject[] gos, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gos == null)
        {
            _hideCoroutine = null;
            yield break;
        }

        Debug.Log("CameraToggle: hiding playerBodies renderers (disable)");
        foreach (var go in gos)
        {
            if (go == null) continue;
            SetRenderersEnabled(go, false);
        }

        _hideCoroutine = null;
    }

    System.Collections.IEnumerator DelayedHideMultiple(GameObject[] gos, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gos == null)
        {
            _hideCoroutine = null;
            yield break;
        }

        Debug.Log("CameraToggle: hiding playerBodies (SetActive(false))");
        foreach (var go in gos)
        {
            if (go == null) continue;
            go.SetActive(false);
        }

        _hideCoroutine = null;
    }

    void SetRenderersEnabled(GameObject go, bool enabled)
    {
        if (go == null) return;
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = enabled;
    }
}
