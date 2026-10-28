using UnityEngine;

public class InfoZone : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] private string title;

    [TextArea(3, 10)]
    [SerializeField] private string bodyText;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        UI_InfoBox.Instance.Show(title, bodyText);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        UI_InfoBox.Instance.Hide();
    }
}