using UnityEngine;
using System.Collections;
//Used for fading out the screen
using UnityEngine.UI;
//Using for modifying motion blur on camera
using UnityStandardAssets.ImageEffects;

public class FlashbangEffectScript : MonoBehaviour {

	//Used to toggle the effect
	public bool isBlinded = false;
	bool routineStarted;

	[Header("Settings")]

	//How long before the effect can be enabled again
	public float effectTimer = 10.0f;

	//How fast the alpha fades out
	public float alphaTransitionMultiplier = 0.15f;
	//Alpha value
	float imageAlphaValue;

	//How fast the blur fades out
	public float blurTransitionMultiplier = 0.05f;
	//Motion blur value
	float motionBlurValue;

	[Header("Components")]

	//Main camera, attached to the prefab
	public Camera effectCamera;
	//Audio source
	public AudioSource flashbangAudio;
	//Flashbang image screen effect
	public CanvasGroup flashbangImage;

	void Start () {

		//Make sure flashbang image is not showing at start
		flashbangImage.alpha = 0;

		//Make sure motion blur is off at start
		effectCamera.GetComponent<MotionBlur>().blurAmount = 0;

		//Make sure audio reverb zone is off at start
		effectCamera.GetComponent<AudioReverbZone>().enabled = false;
	}
	
	void Update () {
		
		//Set the motion blur value
		effectCamera.GetComponent<MotionBlur> ().blurAmount = motionBlurValue;
		//Set the image alpha value
		flashbangImage.alpha = imageAlphaValue;

		//Decrease the motion blur value over time * multiplier
		motionBlurValue -= Time.deltaTime * blurTransitionMultiplier;
		//Decrease the image alpha value over time * multiplier
		imageAlphaValue -= Time.deltaTime * alphaTransitionMultiplier;

		//If blinded is true
		if (isBlinded == true) {
			if (routineStarted == false) {

				//Start the flashbang effect timer
				StartCoroutine(FlashbangEffect());

				routineStarted = true;
			} 
		}

		//Make sure motion blur value
		//is not negative value
		if (motionBlurValue < 0) {
			motionBlurValue = 0;
		}

		//Make sure image alpha value
		//is not a negative value
		if (imageAlphaValue < 0) {
			imageAlphaValue = 0;
		}
	}

	//Start flashbang effect
	IEnumerator FlashbangEffect () {

		//Play audio effect
		flashbangAudio.Play();
		//Enable motion blur effect
		motionBlurValue = 0.92f;
		//Make flashbang image visible
		imageAlphaValue = 1.0f;
		//Enable audio reverb zone
		effectCamera.GetComponent<AudioReverbZone>().enabled = true;
		//Wait for set amount of time
		yield return new WaitForSeconds (effectTimer);

		isBlinded = false;
		routineStarted = false;

		//Disable audio reverb zone
		//after set amount of time
		effectCamera.GetComponent<AudioReverbZone>().enabled = false;

	}
}