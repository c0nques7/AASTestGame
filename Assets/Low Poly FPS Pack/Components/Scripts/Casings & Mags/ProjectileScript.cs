using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	//Used for the arrow projectile
	public bool isArrow;

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

	[Header("Customizable Options")]
	//Force added at start
	public float force = 5000f;
	//Time before the projectile is destroyed
	public float despawnTime = 30f;

	[Header("Explosion Options")]
	//Radius of the explosion
	public float radius = 50.0F;
	//Intensity of the explosion
	public float power = 250.0F;

	void Start () {
		//Launch the projectile forward by adding force to it at start
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * force);

		//Start the destroy timer
		StartCoroutine (DestroyTimer ());
	}

	//Rotates the projectile according to the direction it is going
	void FixedUpdate(){
		if(GetComponent<Rigidbody>().velocity != Vector3.zero)
			GetComponent<Rigidbody>().rotation = 
				Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);  
	}

	IEnumerator DestroyTimer () {
		//Destroy the projectile after set amount of seconds
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}

	//If the projectile collides with anything
	void OnCollisionEnter (Collision collision) {

		//********** USED IN THE DEMO SCENES **********
		//If the projectile hit the tag "Target", and if "isHit" is false
		if (collision.gameObject.tag == "Target" && 
		    	collision.gameObject.GetComponent<TargetScript>().isHit == false) {
			
			//Spawn explosion prefab on surface
			Instantiate(explosionMetalPrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));

			//Animate the target 
			collision.gameObject.transform.gameObject.GetComponent<Animation> ().Play("target_down");
			//Toggle the isHit bool on the target object
			collision.gameObject.transform.gameObject.GetComponent<TargetScript>().isHit = true;
		}

		//If the projectile collides with metal tag or metal impact static tag
		if (collision.gameObject.tag == metalImpactTag || collision.gameObject.tag == metalImpactStaticTag) {
			
			Instantiate(explosionMetalPrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));
		}

		//If the projectile collides with concrete tag or concrete impact static tag
		if (collision.gameObject.tag == concreteImpactTag || collision.gameObject.tag == concreteImpactStaticTag) {
			
			Instantiate(explosionConcretePrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));
		}

		//If the projectile collides with wood tag or wood impact static tag
		if (collision.gameObject.tag == woodImpactTag || collision.gameObject.tag == woodImpactStaticTag) {
			
			Instantiate(explosionWoodPrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));
		}

		//If the projectile collides with dirt tag or dirt impact static tag
		if (collision.gameObject.tag == dirtImpactTag || collision.gameObject.tag == dirtImpactStaticTag) {
			
			Instantiate(explosionDirtPrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));
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
			//If the explosion hit the tags "Target", and "isHit" is false
			if (hit.GetComponent<Collider>().tag == "Target" && 
			    	hit.GetComponent<TargetScript>().isHit == false) {

				//Animate the target 
				hit.gameObject.GetComponent<Animation> ().Play("target_down");
				//Toggle the isHit bool on the target object
				hit.gameObject.GetComponent<TargetScript>().isHit = true;
			}

			//If the projectile explosion hits barrels with the tag "ExplosiveBarrel"
			if (hit.transform.tag == "ExplosiveBarrel") {
				
				//Toggle the explode bool on the explosive barrel object
				hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
			}
		}

		//Destroy the projectile on collision
		if (!isArrow) {
			Destroy (gameObject);
		}

		//If arrow collides, freeze the position
		if (isArrow == true) {
			GetComponent<Rigidbody> ().isKinematic = true;
		}
	}
}