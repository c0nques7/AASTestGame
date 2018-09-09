using UnityEngine;
using System.Collections;

public class ThirdPersonCharacterControllerScript : MonoBehaviour {

	Animator anim;

	//Used for fire rate
	float lastFired;

	[Header("Ammo")]
	//Total ammo
	public int ammo;
	//How much ammo is currently left
	public int currentAmmo;
	//Check if out of ammo
	bool outOfAmmo;

	[Header("Fire Rate")]
	public bool automaticFire;
	//How fast the weapon fires
	public float fireRate;

	[Header("Casing")]
	//The casing prefab that is spawned when shooting
	public Transform casingPrefab;
	//The location from where the casing spawns
	public Transform[] casingSpawnPoints;
	//How long after shooting should the casing spawn
	public float casingDelay = 0.0f;

	[Header("Mag Component")]
	//Mag prefab spawned when reloading
	public Transform magPrefab;
	//Where the mag is spawned from
	public Transform magSpawnpoint;
	//Time before current mag is shown
	public float spawnMagTimer;

	[Header("Particle Systems")]
	//Toggles smoke particle effects
	public bool playSmoke;
	public ParticleSystem smokeParticles;
	//Toggles spark particle effects
	public bool playSparks;
	public ParticleSystem sparkParticles;
	//Toggles tracer particle effects
	public bool playTracers;
	public ParticleSystem bulletTracerParticles;

	//Toggles light flash when shooting
	public bool useLightFlash;
	public Light lightFlash;

	[Header("Muzzleflash Settings")]
	//Holds all the muzzleflash sprites
	public GameObject muzzleflashHolder;
	//Minimum size of the muzzleflash sprites
	public float muzzleMinSize = 1.0f;
	//Maximum size of the muzzleflash sprites
	public float muzzleMaxSize = 2.5f;
	//Array of colors for the muzzleflash, randomly chosen
	public Color[] muzzleColor;
	Color randomMuzzleColor;

	//Minimum rotation of the muzzleflash sprites
	public float muzzleRotateMin;
	//Maximum rotation of the muzzleflash sprites
	public float muzzleRotateMax;
	float randomMuzzleRot;
	float randomMuzzleSize;
	public GameObject sideMuzzle;
	public GameObject topMuzzle;
	public GameObject frontMuzzle;
	//Array of muzzleflash sprites
	public Sprite[] muzzleflashSideSprites;

	[Header("Audio")]
	//The main audio source
	public AudioSource mainAudioSource;
	//Sound that plays when shooting
	public AudioClip[] shootSounds;
	//Sound that plays when reloading
	public AudioClip reloadSound;

	bool soundHasPlayed;

	//Check if running
	bool sprint;
	//Check if reloading
	bool isReloading;
	//Check if proned
	bool isProned;

	// Use this for initialization
	void Start () {
		//Set the animator component
		anim = GetComponent<Animator>();

		//Set the current ammo to number set in inspector
		currentAmmo = ammo;
		//Hide all muzzleflash sprites at start
		sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
		topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		//Disable the light flash at start
		lightFlash.GetComponent<Light>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Check animations continously 
		//(which animation is currently playing)
		AnimationCheck ();

		//Check if out of ammo
		if (currentAmmo == 0) {
			outOfAmmo = true;
		}

		//Left click hold (if automatic is true, and ammo is higher than 0, and not reloading)
		if (Input.GetMouseButton (0) && automaticFire == true && !outOfAmmo && !isReloading) {
			//Shoot automatic
			if (Time.time - lastFired > 1 / fireRate) {
				Shoot ();
				//Play fire anim and restart anim
				anim.Play ("Fire", 1, 0f);
				lastFired = Time.time;
			}
		}

		//Left click to shoot semi
		if (Input.GetMouseButtonDown (0) && !automaticFire && !outOfAmmo && !isReloading) {
			Shoot ();
			//Play fire anim and restart anim
			anim.Play ("Fire", 1, 0f);
		}

		//Reload when pressing R key
		if (Input.GetKeyDown (KeyCode.R) && !isReloading) {
			//Start reloading
			Reload ();
		}

		//Character Movement -------------------- Character Movement

		//Jumping
		//Jump by pressing space bar
		if (Input.GetKeyDown (KeyCode.Space)) {
			anim.SetTrigger ("Jump");
		}

		//Walking 
		//Walk forward with W key (hold down)
		if (Input.GetKey (KeyCode.W) && !sprint) {
			anim.SetBool ("Walk", true);
		} else {
			anim.SetBool ("Walk", false);
		}
		//Walk backwards with S key (hold down)
		if (Input.GetKey (KeyCode.S) && !sprint) {
			anim.SetBool ("Walk Backwards", true);
		} else {
			anim.SetBool ("Walk Backwards", false);
		}
		//Walk strafe left with A key (hold down)
		if (Input.GetKey (KeyCode.A) && !sprint) {
			anim.SetBool ("Walk Strafe Left", true);
		} else {
			anim.SetBool ("Walk Strafe Left", false);
		}
		//Walk strafe right with D key (hold down)
		if (Input.GetKey (KeyCode.D) && !sprint) {
			anim.SetBool ("Walk Strafe Right", true);
		} else {
			anim.SetBool ("Walk Strafe Right", false);
		}

		//Running
		//Toggle sprint bool with left shift key (hold down)
		if (Input.GetKey (KeyCode.LeftShift)) {
			sprint = true;
		} else {
			sprint = false;
		}
		//Run forward with shift + W key (hold down)
		if (Input.GetKey (KeyCode.W) && sprint == true) {
			anim.SetBool ("Run", true);
		} else {
			anim.SetBool ("Run", false);
		}
		//Run backwards with shift + S key (hold down)
		if (Input.GetKey (KeyCode.S) && sprint == true) {
			anim.SetBool ("Run Backwards", true);
		} else {
			anim.SetBool ("Run Backwards", false);
		}
		//Run strafe left with shift + a key (hold down)
		if (Input.GetKey (KeyCode.A) && sprint == true) {
			anim.SetBool ("Run Strafe Left", true);
		} else {
			anim.SetBool ("Run Strafe Left", false);
		}
		//Run strafe right with shift + d key (hold down)
		if (Input.GetKey (KeyCode.D) && sprint == true) {
			anim.SetBool ("Run Strafe Right", true);
		} else {
			anim.SetBool ("Run Strafe Right", false);
		}

		//Crouching
		//Press C key to crouch down (hold down)
		if (Input.GetKey (KeyCode.C)) {
			anim.SetBool ("Crouch", true);
		} else {
			anim.SetBool ("Crouch", false);
		}
		//Crouch walk forwards with W key (hold down)
		if (Input.GetKey (KeyCode.W)) {
			anim.SetBool ("Crouch Walk", true);
		} else {
			anim.SetBool ("Crouch Walk", false);
		}
		//Crouch walk backwards with s key (hold down)
		if (Input.GetKey (KeyCode.S)) {
			anim.SetBool ("Crouch Walk Backwards", true);
		} else {
			anim.SetBool ("Crouch Walk Backwards", false);
		}
		//Crouch walk strafe left with A key (hold down)
		if (Input.GetKey (KeyCode.A)) {
			anim.SetBool ("Crouch Walk Strafe Left", true);
		} else {
			anim.SetBool ("Crouch Walk Strafe Left", false);
		}
		//Crouch walk strafe right with D key (hold down)
		if (Input.GetKey (KeyCode.D)) {
			anim.SetBool ("Crouch Walk Strafe Right", true);
		} else {
			anim.SetBool ("Crouch Walk Strafe Right", false);
		}

		//Prone
		//Press Z key to go prone if not proned already
		if (Input.GetKeyDown (KeyCode.Z) && !isProned) {
			anim.SetBool ("Prone", true);
			isProned = true;
		//Press Z key again to get up
		} else if (Input.GetKeyDown (KeyCode.Z) && isProned == true) {
			anim.SetBool ("Prone", false);
			isProned = false;
		}
		//Crawl forward with W key (hold down)
		if (Input.GetKey (KeyCode.W)) {
			anim.SetBool ("Prone Crawl", true);
		} else {
			anim.SetBool ("Prone Crawl", false);
		}
		//Crawl backwards with S key (hold down)
		if (Input.GetKey (KeyCode.S)) {
			anim.SetBool ("Prone Crawl Backwards", true);
		} else {
			anim.SetBool ("Prone Crawl Backwards", false);
		}
		//Crawl strafe left with A key (hold down)
		if (Input.GetKey (KeyCode.A)) {
			anim.SetBool ("Prone Crawl Strafe Left", true);
		} else {
			anim.SetBool ("Prone Crawl Strafe Left", false);
		}
		//Crawl strafe right with D key (hold down)
		if (Input.GetKey (KeyCode.D)) {
			anim.SetBool ("Prone Crawl Strafe Right", true);
		} else {
			anim.SetBool ("Prone Crawl Strafe Right", false);
		}

	}

	void Shoot () {
		//Remove 1 ammo
		currentAmmo -= 1;
		//Spawn casing at spawnpoint
		StartCoroutine(CasingDelay());
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash());
		//Choose random shoot sound from array
		mainAudioSource.clip = shootSounds[Random.Range(0, shootSounds.Length)];
		//Play audio
		mainAudioSource.Play();
	}

	void Reload () {
		//Play reload animation
		anim.Play("Reload");
		//Play reload sound
		mainAudioSource.clip = reloadSound;
		mainAudioSource.Play ();

		//Refill ammo
		currentAmmo = ammo;
		//Uncheck out of ammo
		outOfAmmo = false;

		//Spawn mag and hide current
		StartCoroutine(MagSpawn());
	}

	//Casing spawn
	IEnumerator CasingDelay () {
		//Wait set amount of time for casing to spanw
		yield return new WaitForSeconds (casingDelay);
		//Spawn a casing at every casing spawnpoint 
		for (int i = 0; i < casingSpawnPoints.Length; i++) {
			Instantiate (casingPrefab,
				casingSpawnPoints[i].transform.position,
				casingSpawnPoints[i].transform.rotation);
		}
	}
		
	//Muzzleflash
	IEnumerator MuzzleFlash () {
		//Show muzzleflash if useMuzzleflash is true
		//Show a random muzzleflash from the array
		sideMuzzle.GetComponent<SpriteRenderer> ().sprite = muzzleflashSideSprites 
			[Random.Range (0, muzzleflashSideSprites.Length)];
		topMuzzle.GetComponent<SpriteRenderer> ().sprite = muzzleflashSideSprites 
			[Random.Range (0, muzzleflashSideSprites.Length)];
		//Show the muzzleflashes
		sideMuzzle.GetComponent<SpriteRenderer>().enabled = true;
		topMuzzle.GetComponent<SpriteRenderer>().enabled = true;
		frontMuzzle.GetComponent<SpriteRenderer>().enabled = true;
		
		//Generate random muzzleflash size
		randomMuzzleSize = Random.Range (muzzleMinSize, muzzleMaxSize);
		//Generate random muzzleflash rotation
		randomMuzzleRot = Random.Range (muzzleRotateMin, muzzleRotateMax);
		//Generate random muzzleflash color
		randomMuzzleColor = muzzleColor[Random.Range(0, muzzleColor.Length)];
				
		//Random muzzleflash color
		sideMuzzle.GetComponent<SpriteRenderer>()
			.color = randomMuzzleColor;
		topMuzzle.GetComponent<SpriteRenderer>()
			.color = randomMuzzleColor;
		frontMuzzle.GetComponent<SpriteRenderer>()
			.color = randomMuzzleColor;

		//Random muzzleflash size
		muzzleflashHolder.transform.localScale = new Vector3
			(randomMuzzleSize, randomMuzzleSize, randomMuzzleSize);
		//Random muzzleflash rotation on z axis
		muzzleflashHolder.transform.Rotate (0,0,randomMuzzleRot);

		//Enable the ligkht flash if true
		if (useLightFlash == true) {
			lightFlash.GetComponent<Light>().enabled = true;
		}
		//Play smoke particles if true
		if (playSmoke == true) {
			smokeParticles.Play ();
		}
		//Play spark particles if true
		if (playSparks == true) {
			sparkParticles.Play ();
		}
		//Play tracer particles if true
		if (playTracers == true) {
			bulletTracerParticles.Play ();
		}

		//Wait for set amount of time
		yield return new WaitForSeconds (0.02f);

		//Hide the muzzleflashes
		sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
		topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;

		//Disable the light flash if true
		if (useLightFlash == true) {
			lightFlash.GetComponent<Light>().enabled = false;
		}
	}

	//Spawn empty mag prefab
	IEnumerator MagSpawn () {
		//Wait before spawning mag
		yield return new WaitForSeconds (spawnMagTimer);
		//Spawn mag prefab
		Instantiate (magPrefab,
			magSpawnpoint.transform.position,
			magSpawnpoint.transform.rotation);
	}
		
	//Check which animation is playing
	void AnimationCheck () {
		//Check if reloading
		if (anim.GetCurrentAnimatorStateInfo (1).IsName ("Reload")) {
			isReloading = true;
		} else {
			isReloading = false;
		}
	}
}