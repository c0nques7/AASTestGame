using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmControllerScript : MonoBehaviour {
        	
	public GameObject FloatingTextPrefab;

	public GameObject Bullet;

	public float Bullet_Velocity;

	public float Bullet_Damage;

	public GameObject Bullet_Emitter;

	Animator anim;
	
	bool isReloading;
	bool outOfAmmo;
	
	bool isShooting;
	bool isAimShooting;
	bool isAiming;
	bool isDrawing;
	bool isRunning;
	bool isJumping;

	bool isMeleeAttacking;

	bool isGrenadeReloading;
	//Used for minigun
	bool enableMinigunShooting;

	//Random number generated to choose 
	//attack animation for melee
	int randomAttackAnim;
	
	//Used for fire rate
	float lastFired;
	
	//Ammo left
	public int currentAmmo;

	//A layer mask for items that can be damaged
	public LayerMask shootableMask = -9;

	[System.Serializable]
	public class meleeSettings
	{  
		[Header("Melee Weapons")]
		//If the current weapon is a melee weapon
		public bool isMeleeWeapon;
		public int damagePerHit;

	}
	public meleeSettings MeleeSettings;
	
	[System.Serializable]
	public class shootSettings
	{  
		[Header("Ammo")]
		//Total ammo
		public int ammo;
		
		[Header("Fire Rate & Bullet Settings")]
		public bool automaticFire;
		public float fireRate;

        [Space(10)]

        //Weapon Rarity
        
		//How far the raycast will reach
		public float bulletDistance = 500.0f;
		//How much force will be applied to rigidbodies 
		//by the bullet raycast
		public float bulletForce = 500.0f;
		//How much damage the shot does to anything in the "Shootable" layer
		public int damagePerShot;
		
		[Header("Shotgun Settings")]
		public bool useShotgunSpread;
		//How big the pellet spread area will be
		public float spreadSize = 2.0f;    
		//How many pellets to shoot
		public int pellets = 30;
		
		[Header("Projectile Weapon Settings")]
		
		//If the current weapon is a projectile weapon (rpg, bazooka, etc)
		public bool projectileWeapon;
		//If the current weapon is the grenade launcher
		public bool grenadeLauncher;
		
		//The projectile spawned when shooting
		public Transform projectile;
		//The static projectile on the weapon
		//This will be hidden when shooting
		public Transform currentProjectile;
		
		//How long after shooting the reload will start
		public float reloadTime;

		[Header("Grenade Settings")]

		//If the current weapon is a grenade
		public bool grenade;

		//Delay when releasing left click to throw
		public float throwDelay = 0.15f;
		//Delay to hide grenade model
		public float hideGrenadeTimer = 0.75f;
		//Delay to show grenade model
		public float showGrenadeTimer = 0.75f;

		[Header("Flamethrower Settings")]

		//If the current weapon is the flamethrower
		public bool flamethrower;

		[Header("Minigun Settings")]

		//If the current weapon is the minigun
		public bool minigun;
	}
	public shootSettings ShootSettings;
	
	[System.Serializable]
	public class reloadSettings
	{  
		[Header("Reload Settings")]
		public bool casingOnReload;
		public float casingDelay;
		
		[Header("Bullet In Mag")]
		public bool hasBulletInMag;
		public Transform[] bulletInMag;
		public float enableBulletTimer = 1.0f;

		[Header("Bullet Or Shell Insert")]
		//If the weapon uses a bullet/shell insert style reload
		//Used for the bolt action sniper and pump shotgun for example
		public bool usesInsert;
		
	}
	public reloadSettings ReloadSettings;
	
	[System.Serializable]
	public class impactTags
	{  
		[Header("Impact Tags")]
		//Default tags for bullet impacts
		public string metalImpactStaticTag = "Metal (Static)";
		public string metalImpactTag = "Metal";
		public string woodImpactStaticTag = "Wood (Static)";
		public string woodImpactTag = "Wood";
		public string concreteImpactStaticTag = "Concrete (Static)";
		public string concreteImpactTag = "Concrete";
		public string dirtImpactStaticTag = "Dirt (Static)";
		public string dirtImpactTag = "Dirt";
	}
	public impactTags ImpactTags;
	
	//All Components
	[System.Serializable]
	public class components
	{  
		[Header("Muzzleflash Holders")]
		public bool useMuzzleflash = false;
		public GameObject sideMuzzle;
		public GameObject topMuzzle;
		public GameObject frontMuzzle;
		//Array of muzzleflash sprites
		public Sprite[] muzzleflashSideSprites;
		
		[Header("Light Front")]
		public bool useLightFlash = false;
		public Light lightFlash;
		
		[Header("Particle System")]
		public bool playSmoke = false;
		public ParticleSystem smokeParticles;
		public bool playSparks = false;
		public ParticleSystem sparkParticles;
		public bool playTracers = false;
		public ParticleSystem bulletTracerParticles;

		[Header("Melee Components")]
		public GameObject weaponTrail;
	}
	public components Components;
	
	//All weapon types
	[System.Serializable]
	public class prefabs
	{  
		[Header("Prefabs")]
		public Transform casingPrefab;
		
		[Header("Metal")]
		[Header("Bullet Impacts & Tags")]
		public Transform metalImpactStaticPrefab;
		public Transform metalImpactPrefab;
		[Header("Wood")]
		public Transform woodImpactStaticPrefab;
		public Transform woodImpactPrefab;
		[Header("Concrete")]
		public Transform concreteImpactStaticPrefab;
		public Transform concreteImpactPrefab;
		[Header("Dirt")]
		public Transform dirtImpactStaticPrefab;
		public Transform dirtImpactPrefab;
	}
	public prefabs Prefabs;
	
	[System.Serializable]
	public class spawnpoints
	{  
		[Header("Spawnpoints")]
		//Array holding casing spawn points 
		//(some weapons use more than one casing spawn)
		public Transform [] casingSpawnPoints;
		//Bullet raycast start point
		public Transform bulletSpawnPoint;
	}
	public spawnpoints Spawnpoints;
	
	[System.Serializable]
	public class audioClips
	{  
		[Header("Audio Source")]
		
		public AudioSource mainAudioSource;
		
		[Header("Audio Clips")]
		
		//All audio clips
		public AudioClip shootSound;
		public AudioClip reloadSound;
	}
	public audioClips AudioClips;

	public bool noSwitch = false;
	
	void Awake () {
		
		//Set the animator component
		anim = GetComponent<Animator>();
		
		//Set the ammo count
		RefillAmmo ();

        if(gameObject.tag == "CommonRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(20, 35);
            Debug.Log("Pew pew");
        }
        if (gameObject.tag == "UncommonRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(35, 50);
            Debug.Log("Pew pew");
        }
        if (gameObject.tag == "RareRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(50, 65);
            Debug.Log("Pew pew");
        }
        if (gameObject.tag == "UltraRareRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(65, 70);
            Debug.Log("Pew pew");
        }
        if (gameObject.tag == "OmegaRareRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(75, 90);
            Debug.Log("Pew pew");
        }
        if (gameObject.tag == "DeveloperRifle" && ShootSettings.damagePerShot == 0)
        {
            ShootSettings.damagePerShot = Random.Range(100, 135);
            Debug.Log("Pew pew");
        }

        //Hide muzzleflash and light at start, disable for projectile, grenade, melee weapons, grenade launcher and flamethrower
        if (!ShootSettings.projectileWeapon && !MeleeSettings.isMeleeWeapon && !ShootSettings.grenade && !ShootSettings.grenadeLauncher && !ShootSettings.flamethrower) {
			
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}
		
		//Disable the light flash, disable for melee weapons and grenade
		if (!MeleeSettings.isMeleeWeapon && !ShootSettings.grenade) {
			Components.lightFlash.GetComponent<Light> ().enabled = false;
		}

		//Set the "shoot" sound clip for melee weapons
		if (MeleeSettings.isMeleeWeapon == true) {
			AudioClips.mainAudioSource.clip = AudioClips.shootSound;
			//Disable the weapon trail at start
			Components.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
		}

		//Prevent from throwing grenade right at start/load
		if (ShootSettings.grenade == true) {
			isGrenadeReloading = true;
		}
	}
	
	void Update () {

		
		//Generate random number to choose which melee attack animation to play
		//If using a melee weapon
		if (MeleeSettings.isMeleeWeapon == true) {
			randomAttackAnim = Random.Range (1, 4);
		}

		//Check which animation 
		//is currently playing
		AnimationCheck ();
		
		//Left click (if automatic fire is false)
		if (Input.GetMouseButtonDown (0) && !ShootSettings.automaticFire
		    //Disable shooting while running and jumping
			&& !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping) {
			//If shotgun shoot is true
			if (ShootSettings.useShotgunSpread == true) {
				ShotgunShoot();
			}
			//If projectile weapon, grenade, melee weapons, grenade launcher and flamethrower is false
			if (!ShootSettings.projectileWeapon && !ShootSettings.useShotgunSpread && !MeleeSettings.isMeleeWeapon 
				&& !ShootSettings.grenade && !ShootSettings.grenadeLauncher & !ShootSettings.flamethrower && !ShootSettings.minigun) {
				Shoot();
				//If projectile weapon is true
			} else if (ShootSettings.projectileWeapon == true) {
				StartCoroutine(ProjectileShoot());
			}

			//If melee weapon is used, play random attack animation on left click
			if (MeleeSettings.isMeleeWeapon == true) {
				//Play attack animation 1, if not currently attacking or drawing weapon
				if (randomAttackAnim == 1 && !isMeleeAttacking && !isDrawing) {
					anim.SetTrigger("Attack 1");
					//Play weapon sound
					AudioClips.mainAudioSource.Play();
				}
				//Play attack animation 2, if not currently attacking or drawing weapon
				if (randomAttackAnim == 2 && !isMeleeAttacking && !isDrawing) {
					anim.SetTrigger("Attack 2");
					//Play weapon sound
					AudioClips.mainAudioSource.Play();
				}
				//Play attack animation 3, if not currently attacking or drawing weapon
				if (randomAttackAnim == 3 && !isMeleeAttacking && !isDrawing) {
					anim.SetTrigger("Attack 3");
					//Play weapon sound
					AudioClips.mainAudioSource.Play();
				}
			}
		}

		//Left click (used for grenade launcher)
		if (Input.GetMouseButtonDown (0) && ShootSettings.grenadeLauncher == true
			//Disable shooting while running and jumping
			&& !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping) {
			//Shoot
			GrenadeLauncherShoot();
		}

		//Left click hold (used for flamethrower)
		if (Input.GetMouseButton (0) && ShootSettings.flamethrower == true
			//Disable shooting while running, jumping and reloading
			&& !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping) {
			//If current ammo is higher than 0
			if (currentAmmo > 0) {
				//Remove ammo
				currentAmmo -= 1;
				//Play the "flame" particles
				Components.smokeParticles.Play ();
				//Enable the light flash
				Components.lightFlash.enabled = true;
			}
			//If the audio is not playing
			if (!AudioClips.mainAudioSource.isPlaying) {
				//Play shoot sound
				AudioClips.mainAudioSource.clip = AudioClips.shootSound;
				AudioClips.mainAudioSource.Play();	
			}
		//Left click release
		} else if (!isReloading && ShootSettings.flamethrower == true) {
			//Stop playing "flame" particles
			Components.smokeParticles.Stop ();
			//Stop playing shoot sound
			AudioClips.mainAudioSource.Stop();
			//Disable the light flash
			Components.lightFlash.enabled = false;
		}
		
		//Left click hold (if automatic fire is true)
		if (Input.GetMouseButton (0) && ShootSettings.automaticFire == true
		    //Disable shooting while running and jumping
			&& !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping) {
			//Shoot automatic
			if (Time.time - lastFired > 1 / ShootSettings.fireRate) {
				Shoot();
				lastFired = Time.time;
			}
		}

		//Left click hold (minigun shooting)
		if (Input.GetMouseButton (0) && ShootSettings.minigun == true
			//Disable shooting while running and jumping
			&& !outOfAmmo && !isShooting && !isRunning && !isJumping) {
			//Shoot minigun, if enable minigun shooting is true
			if (Time.time - lastFired > 1 / ShootSettings.fireRate && enableMinigunShooting == true) {
				Shoot();
				lastFired = Time.time;
			}

			anim.SetBool ("Spin Up", true);
		} else if (ShootSettings.minigun == true) {
			anim.SetBool ("Spin Up", false);
		}

		//Right click hold to aim
		//Disable aiming for melee weapons and grenade
		if (!MeleeSettings.isMeleeWeapon && !ShootSettings.grenade) {
			if (Input.GetMouseButton (1)) {
				anim.SetBool ("isAiming", true);
			} else {
				anim.SetBool ("isAiming", false);
			}
		}
			
		//Left click to throw grenade
		//Disable if currently "reloading" grenade, and if running or jumping
		if (Input.GetMouseButtonDown (0) && ShootSettings.grenade == true && 
				!isGrenadeReloading && !isRunning && !isJumping) {
			//Disable grenade throwing
			isGrenadeReloading = true;
			//Play throwing animations
			anim.SetTrigger ("Throw");

			//Start throwing grenade
			StartCoroutine(GrenadeThrow());
			//Start hide grenade timer
			StartCoroutine(HideGrenadeTimer());
		}
		
		//R key to reload
		//Not used for projectile weapons, grenade or melee weapons
		if (Input.GetKeyDown (KeyCode.R) && !isReloading && !ShootSettings.projectileWeapon 
			&& !MeleeSettings.isMeleeWeapon && !ShootSettings.grenade && !ShootSettings.minigun) {
			Reload ();
		}
		
		//Run when holding down left shift and moving
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0) {
			anim.SetFloat("Run", 0.2f);
		} else {
			//Stop running
			anim.SetFloat("Run", 0.0f);
		}
		
		//Space key to jump
		//Disable jumping while reloading
		if (Input.GetKeyDown (KeyCode.Space) && !isReloading && !isGrenadeReloading) {
			//Play jump animation
			anim.Play("Jump");
		}
		
		//If out of ammo
		if (currentAmmo == 0) {
			outOfAmmo = true;
			//if ammo is higher than 0
		} else if (currentAmmo > 0) {
			outOfAmmo = false;
		}
	}

	//Muzzleflash
	IEnumerator MuzzleFlash () {
		
		//Show muzzleflash if useMuzzleFlash is true
		if (!ShootSettings.projectileWeapon && Components.useMuzzleflash == true) {
			//Show a random muzzleflash from the array
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
				[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			Components.topMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
				[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			
			//Show the muzzleflashes
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
		}
		
		//Enable the light flash if true
		if (Components.useLightFlash == true) {
			Components.lightFlash.GetComponent<Light> ().enabled = true;
		}
		
		//Play smoke particles if true
		if (Components.playSmoke == true) {
			Components.smokeParticles.Play ();
		}
		//Play spark particles if true
		if (Components.playSparks == true) {
			Components.sparkParticles.Play ();
		}
		//Play bullet tracer particles if true
		if (Components.playTracers == true) {
			Components.bulletTracerParticles.Play();
		}
		
		//Show the muzzleflash for 0.02 seconds
		yield return new WaitForSeconds (0.02f);
		
		if (!ShootSettings.projectileWeapon && Components.useMuzzleflash == true) {
			//Hide the muzzleflashes
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}
		
		//Disable the light flash if true
		if (Components.useLightFlash == true) {
			Components.lightFlash.GetComponent<Light> ().enabled = false;
		}
	}

	//Spawn grenade projectile
	IEnumerator GrenadeThrow () {

		//Play grenade sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();

		//Wait for set amount of time to throw grenade
		yield return new WaitForSeconds (ShootSettings.throwDelay);
		//Spawn the grenade projectile
		Instantiate (ShootSettings.projectile, 
			Spawnpoints.bulletSpawnPoint.transform.position, 
			Spawnpoints.bulletSpawnPoint.transform.rotation);
	}

	//Used to hide and show the grenade mesh
	IEnumerator HideGrenadeTimer () {
		//Wait for set amount of time
		yield return new WaitForSeconds (ShootSettings.hideGrenadeTimer);
		//Hide the current grenade projectile mesh
		ShootSettings.currentProjectile.GetComponent
		<SkinnedMeshRenderer> ().enabled = false;

		//Wait for set amount of time, to show the grenade again
		yield return new WaitForSeconds (ShootSettings.showGrenadeTimer);
		//Show the current grenade projectile mesh
		ShootSettings.currentProjectile.GetComponent
		<SkinnedMeshRenderer> ().enabled = true;
	}

	//Grenade launcher shoot
	//Shoot
	void GrenadeLauncherShoot() {

		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}

		//Remove 1 bullet
		currentAmmo -= 1;

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();

		//Spawn the projectile
		Instantiate (ShootSettings.projectile, 
			Spawnpoints.bulletSpawnPoint.transform.position, 
			Spawnpoints.bulletSpawnPoint.transform.rotation);

		//Show the muzzleflash 
		StartCoroutine (MuzzleFlash ());
	}

	//Projectile shoot
	IEnumerator ProjectileShoot () {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}

		//Remove 1 bullet
		currentAmmo -= 1;

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		StartCoroutine (MuzzleFlash ());
		
		//Spawn the projectile
		Instantiate (ShootSettings.projectile, 
		             Spawnpoints.bulletSpawnPoint.transform.position, 
		             Spawnpoints.bulletSpawnPoint.transform.rotation);
		
		//Hide the current projectile mesh
		ShootSettings.currentProjectile.GetComponent
			<SkinnedMeshRenderer> ().enabled = false;
		
		yield return new WaitForSeconds (ShootSettings.reloadTime);
		
		//Play reload animation
		anim.Play ("Reload");

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
		AudioClips.mainAudioSource.Play();
		
		//Show the current projectile mesh
		ShootSettings.currentProjectile.GetComponent
			<SkinnedMeshRenderer> ().enabled = true;
		
	}
	
	//Shotgun shoot
	void ShotgunShoot() {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}
		
		//Remove 1 bullet
		currentAmmo -= 1;
		
		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		//Start casing instantiate
		if (!ReloadSettings.casingOnReload) {
			StartCoroutine (CasingDelay ());
		}
		
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash ());
		
		//Send out shotgun raycast with set amount of pellets
		for (int i = 0; i < ShootSettings.pellets; ++i) {
			
			float randomRadius = Random.Range 
				(0, ShootSettings.spreadSize);        
			float randomAngle = Random.Range 
				(0, 2 * Mathf.PI);
			
			//Raycast direction
			Vector3 direction = new Vector3 (
				randomRadius * Mathf.Cos (randomAngle),
				randomRadius * Mathf.Sin (randomAngle),
				15);
			
			direction = transform.TransformDirection (direction.normalized);
			
			RaycastHit hit;        
			if (Physics.Raycast (Spawnpoints.bulletSpawnPoint.transform.position, direction, 
			                     out hit, ShootSettings.bulletDistance)) {
				
				//If a rigibody is hit, add bullet force to it
				if (hit.rigidbody != null)
					hit.rigidbody.AddForce (direction * ShootSettings.bulletForce);
				
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "Target"
				if (hit.transform.tag == "Target") {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
					//Toggle the isHit bool on the target object
					hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
				}
				
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "ExplosiveBarrel"
				if (hit.transform.tag == "ExplosiveBarrel") {
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
					//Spawn metal impact on surface of the barrel
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "GasTank"
				if (hit.transform.tag == "GasTank") {
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
					//Spawn metal impact on surface of the gas tank
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Metal (Static)"
				if (hit.transform.tag == ImpactTags.metalImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Metal"
				if (hit.transform.tag == ImpactTags.metalImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood (Static)"
				if (hit.transform.tag == ImpactTags.woodImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.woodImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood"
				if (hit.transform.tag == ImpactTags.woodImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.woodImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete (Static)"
				if (hit.transform.tag == ImpactTags.concreteImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.concreteImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete"
				if (hit.transform.tag == ImpactTags.concreteImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.concreteImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt (Static)"
				if (hit.transform.tag == ImpactTags.dirtImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.dirtImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt"
				if (hit.transform.tag == ImpactTags.dirtImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.dirtImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
			}    
		}
	}
	
	//Shoot
	void Shoot() {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}
		
		//Remove 1 bullet
		currentAmmo -= 1;

		//Instantiate the bullet

		if (Input.GetMouseButton (0))
		{
		GameObject Temporary_Bullet_Handler;
		Temporary_Bullet_Handler = Instantiate(Bullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;
		
		Temporary_Bullet_Handler.transform.Rotate(Vector3.right * 90);

		Rigidbody Temporary_RigidBody;

		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody>();

		Temporary_RigidBody.AddForce(transform.forward * Bullet_Velocity);

		

		Destroy(Temporary_Bullet_Handler, 10f);
		
		//Time.timeScale = 0.1f;

		
		}
		
		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		//Start casing instantiate
		if (!ReloadSettings.casingOnReload) {
			StartCoroutine (CasingDelay ());
		}
		
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash ());
		
		//Raycast bullet
		RaycastHit hit;
		Ray ray = new Ray (transform.position, transform.forward);
		
		//Send out the raycast from the "bulletSpawnPoint" position
		if (Physics.Raycast (Spawnpoints.bulletSpawnPoint.transform.position, 
		                     Spawnpoints.bulletSpawnPoint.transform.forward, out hit, ShootSettings.bulletDistance)) {
			            
						if (Physics.Raycast (ray, shootableMask))
            			{
                		//Try and find a ZombieHealth Script
                		ZombieHealth zombieHealth = hit.collider.GetComponent<ZombieHealth>();

                		//If it exists...
                		if(zombieHealth != null)
                			{
                    		//Zombie Takes Damage Reflective to weapon settings
                    		zombieHealth.TakeDamage(ShootSettings.damagePerShot);
                			}

            			}

			//If a rigibody is hit, add bullet force to it
			if (hit.rigidbody != null)
				hit.rigidbody.AddForce (ray.direction * ShootSettings.bulletForce);
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "Target"
			if (hit.transform.tag == "Target") {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				//Toggle the isHit bool on the target object
				hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
			}
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "ExplosiveBarrel"
			if (hit.transform.tag == "ExplosiveBarrel") {
				//Toggle the explode bool on the explosive barrel object
				hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
				//Spawn metal impact on surface of the barrel
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "GasTank"
			if (hit.transform.tag == "GasTank") {
				//Toggle the explode bool on the explosive barrel object
				hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
				//Spawn metal impact on surface of the gas tank
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Metal (Static)"
			if (hit.transform.tag == ImpactTags.metalImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Metal"
			if (hit.transform.tag == ImpactTags.metalImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Wood (Static)"
			if (hit.transform.tag == ImpactTags.woodImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.woodImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Wood"
			if (hit.transform.tag == ImpactTags.woodImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.woodImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Concrete (Static)"
			if (hit.transform.tag == ImpactTags.concreteImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.concreteImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Concrete"
			if (hit.transform.tag == ImpactTags.concreteImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.concreteImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Dirt (Static)"
			if (hit.transform.tag == ImpactTags.dirtImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.dirtImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Dirt"
			if (hit.transform.tag == ImpactTags.dirtImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.dirtImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
		}
	}
	
	//Refill ammo
	void RefillAmmo () {
		
		currentAmmo = ShootSettings.ammo;
	}
	
	//Reload
	void Reload () {
		
		//Play reload animation
		anim.Play ("Reload");
		
		//Play reload sound
		AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
		AudioClips.mainAudioSource.Play();
		
		//Spawn casing on reload, only used on some weapons
		if (ReloadSettings.casingOnReload == true) {
			StartCoroutine(CasingDelay());
		}
		
		if (outOfAmmo == true && ReloadSettings.hasBulletInMag == true) {
			//Hide the bullet inside the mag if ammo is 0
			for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++) {
				ReloadSettings.bulletInMag[i].GetComponent
					<MeshRenderer> ().enabled = false;
			}
			//Start the "show bullet" timer
			StartCoroutine (BulletInMagTimer ());
		}
	}
	
	IEnumerator BulletInMagTimer () {
		//Wait for set amount of time 
		yield return new WaitForSeconds 
			(ReloadSettings.enableBulletTimer);
		
		//Show the bullet inside the mag
		for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++) {
			ReloadSettings.bulletInMag[i].GetComponent
				<MeshRenderer> ().enabled = true;
		}
	}
	
	IEnumerator CasingDelay () {
		//Wait set amount of time for casing to spawn
		yield return new WaitForSeconds (ReloadSettings.casingDelay);
		//Spawn a casing at every casing spawnpoint
		for (int i = 0; i < Spawnpoints.casingSpawnPoints.Length; i++) {
			Instantiate (Prefabs.casingPrefab, 
			             Spawnpoints.casingSpawnPoints [i].transform.position, 
			             Spawnpoints.casingSpawnPoints [i].transform.rotation);
		}
	}
	
	//Check current animation playing
	void AnimationCheck () {
		
		//Check if shooting
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fire")) {
			isShooting = true;
		} else {
			isShooting = false;
		}

		//Check if minigun is shooting
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Spin")) {
			enableMinigunShooting = true;
		} else {
			enableMinigunShooting = false;
		}

		//Check if shooting while aiming down sights
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Aim Fire")) {
			isAimShooting = true;
		} else {
			isAimShooting = false;
		}

		//Check if running
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//Check if jumping
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
			isJumping = true;
		} else {
			isJumping = false;
		}

		//Check if drawing weapon
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Draw")) {
			isDrawing = true;
		} else {
			isDrawing = false;
		}

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Examine"))
        {
            isDrawing = true;
        }
        else
        {
            isDrawing = false;
        }

        //Check if finsihed reloading when using "insert" style reload
        //Used for bolt action sniper and pump shotgun for example
        if (ReloadSettings.usesInsert == true && 
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			isReloading = false;
			//Used in the demo scnes
			noSwitch = false;
		}

		//Check if finsihed throwing and reloading grenade
		//Used for grenade only
		if (ShootSettings.grenade == true && 
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			isGrenadeReloading = false;
			//Used in the demo scnes
			noSwitch = false;
		}
		
		//Check if reloading
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload")) {
			// If reloading
			isReloading = true;
			//Refill ammo
			RefillAmmo();
			//Used in the demo scenes
			noSwitch = true;
		} else {
			//If not using "insert" style reload
			if (!ReloadSettings.usesInsert) {
				//If not reloading
				isReloading = false;
				//Used in the demo scenes
				noSwitch = false;
			}
		}

		//Check if melee weapon animation is playing
		//To make sure melee animations cant be played at same time
		if (MeleeSettings.isMeleeWeapon == true) {
			//Check if any melee attack animation is playing
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Attack 1") || 
				anim.GetCurrentAnimatorStateInfo (0).IsName ("Attack 2") || 
				anim.GetCurrentAnimatorStateInfo (0).IsName ("Attack 3")) {
				//If attacking
				isMeleeAttacking = true;
				//Enable the weapon trail, only shown when attacking
				Components.weaponTrail.GetComponent<TrailRenderer>().enabled = true;
			} else {
				//If not attacking
				isMeleeAttacking = false;
				//Disable the weapon trail
				Components.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
			}
		}
	}
}