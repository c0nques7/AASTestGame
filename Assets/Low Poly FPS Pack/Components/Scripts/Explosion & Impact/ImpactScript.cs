using UnityEngine;
using System.Collections;

public class ImpactScript : MonoBehaviour {

	[Header("Customizable Options")]
	//How long before the impact is destroyed
	public float despawnTimer = 10.0f;

	[Header("Audio")]
	public AudioClip[] impactSounds;
	public AudioSource audioSource;

	[Header("Bullet Hole Sprite")]
	//If using the bullet hole sprite
	public bool usesBulletHole;
	//Bullet hole sprite component
	public Transform bulletHoleSprite;

	[Header("Bullet Hole Size")]
	//Minimum bullet hole size
	public float bulletHoleMinSize = 0.01f;
	//Maximum bullet hole size
	public float bulletHoleMaxSize = 0.025f;
	float randomSize;

	void Awake () {
		if (usesBulletHole == true) {
			//Get a random size for the bullet hole sprite
			randomSize = (Random.Range 
			(bulletHoleMinSize, bulletHoleMaxSize));
			//Set the random size for the bullet hole sprite
			bulletHoleSprite.transform.localScale = 
			new Vector3 (randomSize, randomSize, randomSize);
		}
	}

	void Start () {
		// Start the despawn timer
		StartCoroutine (DespawnTimer ());

		//Get a random impact sound from the array
		audioSource.clip = impactSounds
			[Random.Range(0, impactSounds.Length)];
		//Play the random impact sound
		audioSource.Play();
	}
	
	IEnumerator DespawnTimer() {
		//Wait for set amount of time
		yield return new WaitForSeconds (despawnTimer);
		//Destroy the impact gameobject
		Destroy (gameObject);
	}
}