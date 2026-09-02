using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Birno
{
    public class CrowdController : MonoBehaviour
    {
        // Offset object by Y value of sine wave
        // Sine wave set to always return postive (use Abs.)
        // Value += time.deltaTime --> time.time instead, since time.deltatime is the discrepancy between frames
        // resets when reaching max threshold
        // keeps wave oscillating between 1 and 0

        [SerializeField] List<GameObject> waveLayers = new List<GameObject>();

        [SerializeField] private bool toggle;

        [Range(0, 3)][SerializeField] private float maxOffset = 5;
        [SerializeField] float phaseStep = 0.5f;
        [SerializeField] float speed = 2f;

        private float originalY;

        private void Start()
        {
            originalY = waveLayers[0].transform.position.y;
            StartCoroutine(RunWave());
        }

        IEnumerator RunWave()
        {
            foreach (GameObject t in waveLayers)
            {
                StartCoroutine(MoveWaveLayer(t.transform, waveLayers.IndexOf(t)));
                yield return new WaitForSeconds(0.01f);
            }
        }

        IEnumerator MoveWaveLayer(Transform t, int index)
        {
            Vector3 basePos = t.position;
            while (toggle)
            {
                float time = Time.time * speed; // waveSpeed
                float offsetY = Mathf.Abs(Mathf.Sin(time + index * phaseStep)) * maxOffset;
                t.position = new Vector3(basePos.x, originalY + offsetY, basePos.z);
                yield return null;
            }
        }

    }
}
