using Sirenix.OdinInspector;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] SquashAndStretch squashAndStretch;

    // Button for wave activation
    [PropertySpace(25), Button]
    [ContextMenu("Play Squash and Stretch")]
    public void PlaySquashStretch()
    {
        squashAndStretch.PlaySquashAndStretch();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaySquashStretch();
        }
    }
    /// Gameplay
    // Controller triggers control height of level bar
    // Top and bottom track
    // Measures accuracy and score
    // Like da trombone game?????

    // Left mouse raise bar
    // Right mouse hold bar
    // Release to drop it



}
