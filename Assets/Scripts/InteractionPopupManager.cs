using UnityEngine;
using UnityEngine.UI;

public class InteractionPopupManager : MonoBehaviour
{
    public static InteractionPopupManager Instance;

    public GameObject popupRoot;      // drag your InteractionPopup GameObject here
    public RectTransform popupRect;   // popupRoot.GetComponent<RectTransform>()
    public Vector3 screenOffset = new Vector3(0, 50, 0); // pixels above the NPC screen position
    Canvas mainCanvas;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (popupRoot != null)
            popupRoot.SetActive(false);

        mainCanvas = popupRoot != null ? popupRoot.GetComponentInParent<Canvas>() : null;
    }

    // Show the popup and position it above worldPosition (use NPC world position)
    public void ShowPopupAt(Vector3 worldPosition)
    {
        if (popupRoot == null) return;

        popupRoot.SetActive(true);

        // convert world position to screen point
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        screenPos += screenOffset;

        // If canvas is Screen Space - Overlay, directly set position:
        if (mainCanvas != null && mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            popupRect.position = screenPos;
        }
        else
        {
            // For other canvas modes (ScreenSpace - Camera or WorldSpace) convert to canvas local pos
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform, screenPos, mainCanvas.worldCamera, out localPoint);
            popupRect.localPosition = localPoint;
        }
    }

    public void HidePopup()
    {
        if (popupRoot == null) return;
        popupRoot.SetActive(false);
    }
}
