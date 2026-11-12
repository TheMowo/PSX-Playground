using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Birno
{
    public class SquashAndStretch : MonoBehaviour
    {
        [Header("Notes")]
        [SerializeField, Multiline(2)] private string notes;

        [Header("Squash and Stretch Core")]
        [SerializeField, Tooltip("Defaults to current GO if not set.")]
        private List<Transform> transformToAffect = new();

        [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
        [SerializeField, Range(0, 1f), SuffixLabel("s", true)] private float animationDuration = 0.25f;
        [SerializeField] private bool canBeOverwritten;
        [SerializeField] private bool playOnStart;
        [SerializeField] private bool playsEveryTime = true;
        [SerializeField, Range(0, 100f), SuffixLabel("%", true)] private float chanceToPlay = 100f;

        [Header("Timing")]
        [SerializeField] private bool useUnscaledTime = true;

        [Flags]
        public enum SquashStretchAxis
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4
        }

        [Header("Animation Settings")]
        [SerializeField] private float initialScale = 1f;
        [SerializeField] private float maximumScale = 1.3f;
        [SerializeField] private bool resetToInitialScaleAfterAnimation = true;
        [SerializeField] private bool reverseAnimationCurveAfterPlaying;

        private bool isReversed;

        [SerializeField]
        private AnimationCurve squashAndStretchCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1f),
            new Keyframe(1f, 0f)
        );

        [Header("Looping Settings")]
        [SerializeField] private bool looping;
        [SerializeField, SuffixLabel("sec", true)] private float loopingDelay = 0.5f;

        private Coroutine squashAndStretchCoroutine;
        private Vector3 initialScaleVector;

        private bool AffectX => (axisToAffect & SquashStretchAxis.X) != 0;
        private bool AffectY => (axisToAffect & SquashStretchAxis.Y) != 0;
        private bool AffectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

        private static event Action SquashAndStretchAllObjectsLikeThisEvent;

        private void Awake()
        {
            if (transformToAffect == null || transformToAffect.Count == 0)
            {
                transformToAffect = new List<Transform>
                {
                    transform
                };
            }

            initialScaleVector = transformToAffect[0].localScale;
        }

        public static void SquashAndStretchAllObjectsLikeThis()
        {
            SquashAndStretchAllObjectsLikeThisEvent?.Invoke();
        }

        private void OnEnable()
        {
            SquashAndStretchAllObjectsLikeThisEvent += PlaySquashAndStretch;
        }

        private void OnDisable()
        {
            if (squashAndStretchCoroutine != null)
                StopCoroutine(squashAndStretchCoroutine);

            SquashAndStretchAllObjectsLikeThisEvent -= PlaySquashAndStretch;
        }

        private void Start()
        {
            if (playOnStart)
                CheckForAndStartCoroutine();
        }

        [PropertySpace(25)]
        [Button]
        [ContextMenu("Play Squash and Stretch")]
        public void PlaySquashAndStretch()
        {
            if (looping && !canBeOverwritten)
                return;

            CheckForAndStartCoroutine();
        }

        private void CheckForAndStartCoroutine()
        {
            if (axisToAffect == SquashStretchAxis.None)
            {
                Debug.Log("Axis to affect is set to None.", gameObject);
                return;
            }

            if (squashAndStretchCoroutine != null)
            {
                StopCoroutine(squashAndStretchCoroutine);

                if (playsEveryTime && resetToInitialScaleAfterAnimation)
                {
                    foreach (Transform target in transformToAffect)
                    {
                        target.localScale = initialScaleVector;
                    }
                }
            }

            squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
        }

        private IEnumerator SquashAndStretchEffect()
        {
            do
            {
                if (!playsEveryTime)
                {
                    float random = UnityEngine.Random.Range(0f, 100f);

                    if (random > chanceToPlay)
                    {
                        yield return null;
                        continue;
                    }
                }

                if (reverseAnimationCurveAfterPlaying)
                    isReversed = !isReversed;

                float elapsedTime = 0f;

                Vector3 originalScale = initialScaleVector;
                Vector3 modifiedScale = originalScale;

                while (elapsedTime < animationDuration)
                {
                    elapsedTime += useUnscaledTime
                        ? Time.unscaledDeltaTime
                        : Time.deltaTime;

                    float curvePosition;

                    if (isReversed)
                        curvePosition = 1f - (elapsedTime / animationDuration);
                    else
                        curvePosition = elapsedTime / animationDuration;

                    float curveValue = squashAndStretchCurve.Evaluate(curvePosition);

                    float remappedValue =
                        initialScale + (curveValue * (maximumScale - initialScale));

                    const float minimumThreshold = 0.0001f;

                    if (Mathf.Abs(remappedValue) < minimumThreshold)
                        remappedValue = minimumThreshold;

                    if (AffectX)
                        modifiedScale.x = originalScale.x * remappedValue;
                    else
                        modifiedScale.x = originalScale.x / remappedValue;

                    if (AffectY)
                        modifiedScale.y = originalScale.y * remappedValue;
                    else
                        modifiedScale.y = originalScale.y / remappedValue;

                    if (AffectZ)
                        modifiedScale.z = originalScale.z * remappedValue;
                    else
                        modifiedScale.z = originalScale.z / remappedValue;

                    foreach (Transform target in transformToAffect)
                    {
                        target.localScale = modifiedScale;
                    }

                    yield return null;
                }

                if (resetToInitialScaleAfterAnimation)
                {
                    foreach (Transform target in transformToAffect)
                    {
                        target.localScale = originalScale;
                    }
                }

                if (looping)
                {
                    if (useUnscaledTime)
                        yield return new WaitForSecondsRealtime(loopingDelay);
                    else
                        yield return new WaitForSeconds(loopingDelay);
                }

            } while (looping);

            squashAndStretchCoroutine = null;
        }

        public void SetLooping(bool shouldLoop)
        {
            looping = shouldLoop;
        }

        public void Setup(
            SquashStretchAxis axis,
            float time,
            float zeroMap,
            float oneMap,
            AnimationCurve curve,
            bool loop,
            float delay,
            bool playImmediately = false)
        {
            axisToAffect = axis;
            animationDuration = time;
            initialScale = zeroMap;
            maximumScale = oneMap;
            squashAndStretchCurve = curve;
            looping = loop;
            loopingDelay = delay;

            if (playImmediately)
                CheckForAndStartCoroutine();
        }
    }
}