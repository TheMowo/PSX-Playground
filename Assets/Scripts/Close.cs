using UnityEngine;

public class Close : MonoBehaviour
{
    [SerializeField] GameObject frame;
    private void CloseSelf()
    {
        frame.SetActive(false);
    }
}
