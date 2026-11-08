using System.Text;
using TMPro;
using UnityEngine;

public class DigitBackground : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] int rows = 15;
    [SerializeField] int cols = 35;

    void Update()
    {
        StringBuilder sb = new();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                sb.Append(Random.Range(0, 10));

                if (x < cols - 1)
                    sb.Append(' ');
            }

            sb.AppendLine();
        }

        text.text = sb.ToString();
    }
}