using UnityEngine;
using System.Collections.Generic;

public class CrowdController : MonoBehaviour
{
    // Offset object by Y value of sine wave
    // Sine wave set to always return postive (use Abs.)
    // Value += time.deltaTime
    // resets when reaching max threshold
    // keeps wave oscillating between 1 and 0


    List<Transform> waveLayers = new List<Transform>();
    [SerializeField] private float maxOffset = 1;
    float timePosIncrement = 0;
    [SerializeField] private bool toggle;

    // Update is called once per frame
    void Update()
    {
        float offsetY = Mathf.Abs(Mathf.Sin(Time.deltaTime)) * maxOffset;
        if (toggle) Debug.Log(offsetY);
    }
}
