using UnityEngine;

namespace Birno
{
    public class BeatController : MonoBehaviour
    {
        public AudioSource audioSource;
        public float songBpm = 120f; // Set your song's tempo here

        private float secPerBeat;
        private float songPosition;
        private float songPositionInBeats;
        private float dspSongTime;

        void Start()
        {
            secPerBeat = 60f / songBpm;
            dspSongTime = (float)AudioSettings.dspTime;
            audioSource.Play();
        }

        void Update()
        {
            // Calculate the exact position of the track in seconds
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);

            // Convert the song position into beats
            songPositionInBeats = songPosition / secPerBeat;

            // Example: Check if a full beat has passed to trigger an action
            int currentBeat = Mathf.FloorToInt(songPositionInBeats);
            // Put logic here to spawn notes or pulse visuals on 'currentBeat'
        }
    }
}
