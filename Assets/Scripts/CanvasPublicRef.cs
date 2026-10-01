using Unity.VisualScripting;
using UnityEngine;

public class CanvasPublicRef : MonoBehaviour
{
    public static CanvasPublicRef Instance;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
}
