using UnityEngine;
using System.Collections;

public class MagScript : MonoBehaviour {

	//How long before the mag is destroyed
	public float despawnTimer = 30.0f;

	[Header("Audio")]
	public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		// Start the despawn timer
		StartCoroutine (DespawnTimer ());
	}

	void OnCollisionEnter (Collision collision) {
		//Play the random casing sound
		audioSource.Play();
	}
	
	IEnumerator DespawnTimer() {
		//Wait for set amount of time
		yield return new WaitForSeconds (despawnTimer);
		//Destroy the impact gameobject
		Destroy (gameObject);
	}
}