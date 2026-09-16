using UnityEngine;
using Unity.Cinemachine;

public class BoundsCalculator : MonoBehaviour
{
    public static (Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
    CalculateCanvasCorners(Collider collider, Canvas canvas)
    {
        Camera cam = Camera.main;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Get all 8 world corners of the collider
        Vector3[] corners = GetCorners(collider.bounds);

        // Project each corner into viewport space
        Vector2[] viewportCorners = new Vector2[corners.Length];
        for (int i = 0; i < corners.Length; i++)
            viewportCorners[i] = cam.WorldToViewportPoint(corners[i]);

        // Find min/max in viewport space
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (var vp in viewportCorners)
        {
            if (vp.x < minX) minX = vp.x;
            if (vp.x > maxX) maxX = vp.x;
            if (vp.y < minY) minY = vp.y;
            if (vp.y > maxY) maxY = vp.y;
        }

        // Convert viewport (0–1) into canvas-local coordinates
        Vector2 topLeft = new Vector2(minX * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                                      maxY * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
        Vector2 topRight = new Vector2(maxX * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                                       maxY * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
        Vector2 bottomLeft = new Vector2(minX * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                                         minY * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
        Vector2 bottomRight = new Vector2(maxX * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                                          minY * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);

        return (topLeft, topRight, bottomLeft, bottomRight);
    }

    private static Vector3[] GetCorners(Bounds bounds)
    {
        return new Vector3[]
        {
            bounds.min,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            bounds.max
        };
    }
}
