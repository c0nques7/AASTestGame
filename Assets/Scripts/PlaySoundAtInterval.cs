using UnityEngine;
using System.Collections;


public class PlaySoundAtInterval : MonoBehaviour
{

    // Public variables
    // Will the sound play on startup?
    public bool playAtStartup = false;

    // The interval of time (in seconds) that the sound will be played.
    public float interval = 3.0f;

    //The Audio Source
    public AudioSource chimeSource;

    // The sound itself.
    public AudioClip clipToPlay;

    // Private variables
    // A modifier that will prevent the script from running in the event of an error
    private bool disableScript = false;

    // The amount of time that has passed since the last initial playback of the sound.
    private float trackedTime = 0.0f;

    // Tracks to see if we've played this at startup.
    private bool playedAtStartup = false;

    NewTimer newTimer;

    // Use this for initialization
    void Start()
    {
        newTimer = GameObject.FindGameObjectWithTag("Controller").GetComponent<NewTimer>();
        if (interval < 1.0f)
        { // Make sure the interval isn't 0, or we'll be constantly playing the sound!
            Debug.LogError("Interval base must be at least 1.0!");
            disableScript = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!disableScript)
        {

            // Play the sound when the scene starts
            if (playAtStartup  != playedAtStartup) {
                chimeSource.PlayOneShot(clipToPlay);
                playedAtStartup = true;
            }

            

            // Increment the timer
            trackedTime += Time.deltaTime;

            // Check to see that the proper amount of time has passed
            if (newTimer.countdownStarted == true && trackedTime >= interval)
            {
                // Play the sound, reset the timer
               chimeSource.PlayOneShot(clipToPlay);
                trackedTime = 0.0f;
                if (newTimer.time > 0)
                {
                    disableScript = true;
                }
            }
        }
    }


}
