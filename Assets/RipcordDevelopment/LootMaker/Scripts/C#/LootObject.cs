using UnityEngine;
using System.Collections;
using Ripcord.Common;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 							LootMaker 2.0, Copyright © 2017, Ripcord Development
//											LootObject.cs
//										 info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - 

public class LootObject : MonoBehaviour {

	public LootType lootType;				//The type of loot this object will affect in the LootManager

	[Header ("Loot Value")]
	public int lootValueMin;				//The minimum value the loot object can have.
	public int lootValueMax;				//The maximum value the loot object can have.
	int lootValue;							//The randomly selected value of the loot object from the maximum and minimum values.

	[Header ("Loot Highlight")]
	public bool showHighlight;				//If true, a highlight effect will be generated to draw attention to the loot object
	public GameObject highlightEffect;		//The highlight effect that will be generated

	[Header ("Loot Audio")]
	public bool playAudio;					//If true, audio effects will be used
	public AudioClip spawnClip;				//An audio file that will play when the lootObject first spawns
	public AudioClip idleClip;				//An audio file that will play until the lootObject is collected
	public AudioClip collectClip;			//An audio file that will play when the lootObject is collected

	[Header ("Collect Effects")]
	public bool showEffects;				//If true, effects will be generated when the lootObject is collected.
	public GameObject lootCollectEffect;	//The effect that will be generated when the lootObject has been collected

	[Header ("Loot Value")]
	public bool displayValue;				//If true, an object will be generated that displays the value of the lootObject
	public GameObject valueText;			//The object that will display the value of the lootObject
	public string valuePrefix;				//Any characters that will appear before the value of the loot object
	public string valueSuffix;				//Any characters that will appear after the value of the loot object

	AudioSource audioSource;				//A reference to the audio source attached to this object.  If it doesn't have one, an audio source component will be added
	Transform lootFXContainer;				//An empty object used to store all the loot effects objects, helps keeps the project hierarchy clean


	void Start () {

		if (!gameObject.GetComponent<AudioSource>() ) {										//If this object does not have an audio source...
			gameObject.AddComponent<AudioSource>();											//...add an audio source to this object
		}
		audioSource = gameObject.GetComponent<AudioSource>();								//Set a reference this object's audio source

		if (!GameObject.Find("LootFXContainer")) {											//If there is no Loot FX container in the scene...
			lootFXContainer = new GameObject("LootFXContainer").transform;					//...create a new game object called "LootFXContainer"
		}
		lootFXContainer = GameObject.Find("LootFXContainer").transform;						//Create a reference to the Loot FX Container object

		if (lootValueMax == 0) {															//If the max loot value has not been set...
			lootValue = lootValueMin;														//...set the loot value to the min loot value...
		}
		else {																				//Otherwise...
			lootValue = Random.Range(lootValueMin, lootValueMax + 1);						//...select a random value for the loot object
		}

		if (showHighlight && highlightEffect) {												//If the object is set to show a highlight and there is a highlight effect...
			GameObject newHighlight = (GameObject)Instantiate(highlightEffect, transform.position, transform.rotation);	//...generate the object...
			newHighlight.transform.parent = gameObject.transform;							//...make the new highlight object a child of this object
		}

		if (playAudio && spawnClip) {														//If the object is set to play audio and there is a spawn clip...
			audioSource.clip = spawnClip;													//...set the active audio clip to the spawn clip...
			audioSource.Play();																//...then play it

			if (idleClip) {																	//If there is an idle clip...
				StartCoroutine("PlayIdleAudio");											//...wait for the spawnClip clip to finish before playing it
			}
		}
	}

	void OnMouseOver () {

		CollectLoot();
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// PLAY IDLE AUDIO - Wait for the spawn audio clip to finish playing, then start playing the idle audio clip
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	IEnumerator PlayIdleAudio () {
		
		yield return new WaitForSeconds(spawnClip.length);
		
		audioSource.clip = idleClip;
		audioSource.loop = true;
		audioSource.Play();
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// COLLECT THE LOOT OBJECT - Remove the loot object from the scene, generate any effects it has, and add its value to the LootManager
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	public void CollectLoot () {
		
		LootManager.instance.IncreaseLoot(lootType, lootValue);

		if (playAudio && collectClip) {
			audioSource.clip = collectClip;
			audioSource.loop = false;
			audioSource.Play();
		}

		if (showEffects && lootCollectEffect) {
			GameObject newCollectEffect = (GameObject)Instantiate(lootCollectEffect, transform.position, transform.rotation);
			newCollectEffect.transform.parent = lootFXContainer;
			newCollectEffect.AddComponent<DestroyParticleSystem>();
		}

		if (displayValue) {
			GameObject newValueText = (GameObject)Instantiate(valueText, transform.position, transform.rotation);
			newValueText.transform.parent = lootFXContainer;
			newValueText.GetComponent<TextMesh>().text = valuePrefix + lootValue.ToString() + valueSuffix;
		}

		gameObject.GetComponent<Renderer>().enabled = false;							//Disable the object's renderer so that it's no longer visible
		gameObject.GetComponent<Collider>().enabled = false;							//Disable the object's collider so that it can no longer interact with the scene

		if (playAudio && collectClip) {													//If the object is set to play audio and has a collect clip assigned...
			Destroy(gameObject, collectClip.length);									//...destroy the object after the collect clip has stopped playing
		}
		else {																			//Otherwise...
			Destroy(gameObject);														//...destroy the object right away
		}
	}
}
