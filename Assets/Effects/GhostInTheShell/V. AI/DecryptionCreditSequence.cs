using System.Collections;
using UnityEngine;

public sealed class DecryptionCreditSequence : MonoBehaviour
{
    [SerializeField] private GhostRevealText decryptionReveal;
    [SerializeField, Min(0f)] private float entryDuration = 4f;
    [SerializeField] private bool useUnscaledTime = true;

    [SerializeField, TextArea(2, 5)]
    private string[] entries =
    {
        "SECTION 9",
        "MAJOR MOTOKO KUSANAGI",
        "PUBLIC SECURITY\nNETWORK ACCESS",
        "GHOST DIVE INITIALIZED"
    };

    private Coroutine sequenceCoroutine;

    private void OnEnable()
    {
        sequenceCoroutine = StartCoroutine(SequenceRoutine());
    }

    private void OnDisable()
    {
        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);
    }

    private IEnumerator SequenceRoutine()
    {
        foreach (string entry in entries)
        {
            decryptionReveal.Play(entry);

            float elapsed = 0f;

            while (elapsed < entryDuration)
            {
                yield return null;
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        sequenceCoroutine = null;
    }
}