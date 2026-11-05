using UnityEngine;

public enum SurfaceType
{
    Concrete,
    Metal,
    Wood,
    Grass,
    Water
}

public class Surface : MonoBehaviour
{
    [SerializeField] private SurfaceType _surfaceType;
    public SurfaceType surfaceType => _surfaceType;
}
