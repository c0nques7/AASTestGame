using UnityEngine;
using System.Collections;

public class NewTargetScript : MonoBehaviour {

	float randomTime;
	bool routineStarted = false;

	

	//Used to check if the target has been hit
	public bool isHit;

	public bool partyOn;

	[Header("Customizable Options")]
	//Minimum time before the target goes back up
	public float minTime;
	//Maximum time before the target goes back up
	public float maxTime;
    //Floating text prefab reference
    public GameObject FloatingHitPrefab;

	Animator anim;

    [Header("Audio")]
	public AudioClip upSound;
	public AudioClip downSound;
    public AudioClip hitSound;

	public AudioSource audioSource;

	NewTimer newTimer;

	GameObject timerConsole;


	public void Start()
	{
        partyOn = false;
        newTimer = GameObject.FindGameObjectWithTag("Controller").GetComponent<NewTimer>();
    }


    public void Update () {


		
		//Generate random time based on min and max time values
		randomTime = Random.Range (minTime, maxTime);

		if (newTimer.gameStarted == true && partyOn == true)
		{

            gameObject.GetComponent<Animation>().Play("target_up");
            //Set the upSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = upSound;
            audioSource.Play();
            partyOn = false;

        }
        if (newTimer.gameStarted == false && partyOn == false)
        {
            //Animate the target "down"
            gameObject.GetComponent<Animation>().Play("target_down");
            //anim.SetBool("down", true);

            //Set the downSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = downSound;
            audioSource.Play();
            partyOn = true;
        }

		//If the target is hit
		if (isHit == true) {
			if (routineStarted == false) {

                PointCounter.enemies += -1;
				//Animate the target "down"
				gameObject.GetComponent<Animation> ().Play("target_down");
				//anim.SetBool("down", true);

				//Set the downSound as current sound, and play it
				audioSource.GetComponent<AudioSource>().clip = downSound;
                //Also play the hitSound
                audioSource.GetComponent<AudioSource>().clip = hitSound;
				audioSource.Play();

                //Instantiate the hit text prefab
                if (FloatingHitPrefab != null)
                {
                    ShowFloatingScore();

                }

                //Start the timer
                StartCoroutine(DelayTimer());
				routineStarted = true;
			} 
		}
		if (newTimer.time > 0f)
		{
			gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}

    void ShowFloatingScore()
    {
        var go = Instantiate(FloatingHitPrefab, transform.position, transform.rotation);
        go.GetComponent<TextMesh>().text = "HIT!";

    }

    //Time before the target pops back up
    IEnumerator DelayTimer () {
		//Wait for random amount of time
		yield return new WaitForSeconds(randomTime);
		//Animate the target "up" 
		gameObject.GetComponent<Animation> ().Play ("target_up");
		//anim.SetBool("up", true);

		//Set the upSound as current sound, and play it
		audioSource.GetComponent<AudioSource>().clip = upSound;
		audioSource.Play();

        //Target is no longer hit
		isHit = false;
		routineStarted = false;
	}
}