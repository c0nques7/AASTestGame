using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {

	[Header("Timer")]
	//Time before the grenade explodes
	public float grenadeTimer = 5.0f;

	[Header("Explosion Prefabs")]
	//All explosion prefabs
	public Transform explosionMetalPrefab;
	public Transform explosionConcretePrefab;
	public Transform explosionDirtPrefab;
	public Transform explosionWoodPrefab;
	
	[Header("Impact Tags")]
	//Default impact tags
	public string metalImpactStaticTag = "Metal (Static)";
	public string metalImpactTag = "Metal";
	public string woodImpactStaticTag = "Wood (Static)";
	public string woodImpactTag = "Wood";
	public string concreteImpactStaticTag = "Concrete (Static)";
	public string concreteImpactTag = "Concrete";
	public string dirtImpactStaticTag = "Dirt (Static)";
	public string dirtImpactTag = "Dirt";

	[Header("Explosion Options")]
	//Radius of the explosion
	public float radius = 25.0F;
	//Intensity of the explosion
	public float power = 350.0F;

	[Header("Throw Force")]
	public float minimumForce = 350.0f;
	public float maximumForce = 850.0f;
	float throwForce;

	[Header("Smoke Grenade")]

	//Should be checked for smoke grenade
	public bool isSmokeGrenade;
	//Time before smoke starts
	public float startSmokeTime;
	//Time until smoke stops
	public float stopSmokeTime;
	//Time until smoke grenade is destroyed
	public float destroyTimer;
	//Particle system for smoke grenade
	public ParticleSystem smokeParticles;

	[Header("Flashbang")]

	//Should be checked for flashbang 
	public bool isFlashbang;

	[Header("Audio")]
	public AudioSource impactSound;

	//Used to check what surface the 
	//grenade is exploding on
	string groundTag;

	void Awake () {
		//Create random throw force
		//based on min and max values
		throwForce = Random.Range(minimumForce, maximumForce);

		//Random rotation of the grenade
		GetComponent<Rigidbody>().AddRelativeTorque (
			Random.Range(500, 1500), //X Axis
			Random.Range(0,0), //Y Axis
			Random.Range(0,0)  //Z Axis
			* Time.deltaTime * 5000);
	}

	void Start () {
		//Launch the projectile forward by adding force to it at start
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * throwForce);

		//Start the explosion timer
		//Disable for smoke grenade
		if (!isSmokeGrenade) {
			StartCoroutine (ExplosionTimer ());
		}

		if (isSmokeGrenade == true) {
			StartCoroutine (SmokeGrenadeTimer ());
		}
	}

	void OnCollisionEnter (Collision collision) {
		//Play the impact sound on every collision
		impactSound.Play ();
	}

	IEnumerator SmokeGrenadeTimer () {

		yield return new WaitForSeconds (startSmokeTime);
		//Play smoke particles
		smokeParticles.Play ();

		yield return new WaitForSeconds (stopSmokeTime);

		//Stop smoke particles
		smokeParticles.Stop();

		yield return new WaitForSeconds (destroyTimer);

		//Destroy the smoke grenade after set amount of time
		Destroy (gameObject);
	}

	IEnumerator ExplosionTimer () {
		//Wait set amount of time
		yield return new WaitForSeconds(grenadeTimer);

		//Raycast downwards to check the ground tag
		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
			//Set the ground tag to whatever the raycast hit
			groundTag = checkGround.collider.tag;
		}

		//If ground tag is Metal or Metal(Static)
		if (groundTag == metalImpactTag || groundTag == metalImpactStaticTag)
		{
			//Instantiate metal explosion prefab
			Instantiate (explosionMetalPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Concrete or Concrete(Static)
		if (groundTag == concreteImpactTag || groundTag == concreteImpactStaticTag)
		{
			//Instantiate concrete explosion prefab
			Instantiate (explosionConcretePrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Wood or Wood(Static)
		if (groundTag == woodImpactTag || groundTag == woodImpactStaticTag)
		{
			//Instantiate wood explosion prefab
			Instantiate (explosionWoodPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Dirt or Dirt(Static)
		if (groundTag == dirtImpactTag || groundTag == dirtImpactStaticTag)
		{
			//Instantiate dirt explosion prefab
			Instantiate (explosionDirtPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		//If the ground is untagged
		if (groundTag == "Untagged") 
		{
			//Instantiate metal explosion prefab
			Instantiate (explosionMetalPrefab, checkGround.point, 
				Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		//Explosion force
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();

			//Add force to nearby rigidbodies
			if (rb != null)
				rb.AddExplosionForce (power, explosionPos, radius, 3.0F);
			
			//********** USED IN THE DEMO SCENES **********
			//If the explosion hit the tags "Target", and if "isHit" 
			//is false on the target
			if (hit.GetComponent<Collider>().tag == "Target" 
			    	&& hit.gameObject.GetComponent<TargetScript>().isHit == false) {
				
				//Animate the target 
				hit.gameObject.GetComponent<Animation> ().Play("target_down");
				//Toggle the isHit bool on the target
				hit.gameObject.GetComponent<TargetScript>().isHit = true;
			}

			//********** USED IN THE DEMO SCENES **********
			//Used for flashbang effect on the player tag
			if (isFlashbang == true) {
				if (hit.GetComponent<Collider> ().tag == "Player" 
						&& hit.gameObject.GetComponent<FlashbangEffectScript>().isBlinded == false) {
					//If the player is near the flashbang, start flashbang effect
					hit.gameObject.GetComponent<FlashbangEffectScript> ().isBlinded = true;

				}
			}
		}

		//Destroy the grenade on explosion
		Destroy (gameObject);
	}
}