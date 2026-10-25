using UnityEngine;
using UnityEngine.UI;

public class M_UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] CornerMarkers = new RectTransform[4]; // Top Left, Top Right, Bottom Left, Bottom Right
    [SerializeField] private Canvas canvas; // assign UI canvas here


    private void Awake()
    {
        ToggleCornerMarkers(false);
    }

    public void AdjustBounds(Collider collider)
    {
        var (topLeft, topRight, bottomLeft, bottomRight) = BoundsCalculator.CalculateCanvasCorners(collider, canvas);

        CornerMarkers[0].anchoredPosition = topLeft;
        CornerMarkers[1].anchoredPosition = topRight;
        CornerMarkers[2].anchoredPosition = bottomLeft;
        CornerMarkers[3].anchoredPosition = bottomRight;
    }

    public void ToggleCornerMarkers(bool active)
    {
        foreach (var cornerMarker in CornerMarkers)
        {
            cornerMarker.GetComponent<Image>().enabled = active;
        }
    }
}
