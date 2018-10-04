using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum SpawnState  { spawning, waiting, counting };

	[System.Serializable]
	public class Wave{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
	}

	public Wave[] waves;
	private int nextWave = 0;

	public float timeBetweenWaves = 5f;
	public float waveCountdown;
	private SpawnState state = SpawnState.counting;


	public WeaponManager weaponManager; 

	public static int minDamage;
	public static int maxDamage;
	public static int zombiePoints; 
	
	public ArmControllerScript armControllerScript;


	float currentAmount = 0f;
	float maxAmount = 10f;

	public GameObject player;
	public GameObject bow;
	public GameObject primary;
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		armControllerScript = player.GetComponentInChildren<ArmControllerScript>();
		waveCountdown = timeBetweenWaves;
	}
	
	// Update is called once per frame
	void Update () {

		if (waveCountdown <= 0)
		{
			if (state != SpawnState.spawning)
			{
				StartCoroutine(SpawnWave ( waves[nextWave] ) );
			}
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}

		

		

		if (PointCounter.points >= 50){
            Debug.Log("SuperBow Ready!");
            anim.SetTrigger("Bow");

			if (Input.GetKeyDown(KeyCode.B))
			{
				primary.SetActive(false);
				bow.SetActive(true);
				PointCounter.points = 0;
			}
			

        }

		if (PointCounter.points >= 100){
            Debug.Log("SlowMo Ready!");
            //anim.SetTrigger("Bow");

			if (Input.GetKeyDown(KeyCode.Z))
			{
				SlowMoPower();
			}
			

        }

		//Check to see if the timescale has been modified
		if (Time.timeScale == 0.7f)
		{
			currentAmount += Time.deltaTime;
		}
		//When currentAmount is greater than maxAmount, return the timescale to 1.0
		if (currentAmount > maxAmount){
			currentAmount = 0f;
			Time.timeScale = 1.0f;
		}
	}

	void SpawnEnemy(Transform _enemy)
		{
			//Spawn Enemy
			Debug.Log("Spawning Enemy: " + _enemy.name);
		}

		IEnumerator SpawnWave(Wave _wave)
		{
			state = SpawnState.spawning;

			for (int i = 0; i < _wave.count; i++)
			{
				SpawnEnemy(_wave.enemy);
				yield return new WaitForSeconds( 1f/_wave.rate);
			}
			state = SpawnState.waiting;
			yield break;
		}

	void SlowMoPower()
	{
		Time.timeScale = 0.7F;
		PointCounter.points = 0;
		
	}

}

		//xjuice