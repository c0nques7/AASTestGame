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

    Color targetColor = Color.red;
    Renderer rend;

    [Header("Timer and Target")]
    public NewTargetScript newTargetScript;
    public NewTimer newTimer;

    public GameObject gameTarget;


    [Header("Wave Controls")]
	public Wave[] waves;
	private int nextWave = 0;

	public float timeBetweenWaves = 5f;
	private float waveCountdown;
	private SpawnState state = SpawnState.counting;

    private float searchCountdown = 1f;

    [Header("Spawnpoint Controls")]
    public Transform[] spawnPoints;


    [Header("Entry and Exit Portals")]
    public GameObject endPortal; 
	public GameObject spawnPortal;

    //Floats used to modify game speed
    float currentAmount = 0f;
    float maxAmount = 10f;

    [Header("Misc References")]
    public GameObject player;
	public GameObject bow;
	public GameObject primary;
    public GameObject celebration;
	Animator anim;

    // Use this for initialization
    void Start () {

        rend = GetComponent<Renderer>();
        rend.material.color = targetColor;
        newTimer = GameObject.FindGameObjectWithTag("Controller").GetComponent<NewTimer>();
        newTargetScript = GameObject.FindGameObjectWithTag("Target").GetComponent<NewTargetScript>();
        gameTarget = GameObject.FindGameObjectWithTag("Target");

        player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No Spawn Points Referenced");
        }
        anim = GetComponent<Animator>();
        waveCountdown = timeBetweenWaves;
        
	}

    // Update is called once per frame
    void Update () {

        if (newTimer.gameStarted == true)
        {
            //Debug.Log("Game started.");
        }
        if (newTimer.gameStarted == false)
        {
            gameObject.gameTarget.rend.material.color = targetColor;
            Debug.Log("Game has not started.");
        }

        if (state == SpawnState.waiting)
        {
            //Check if enemies are still alive
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                PointCounter.wave += 1;
            }
            else
            {
                return;
            }
        }

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

    public void GameOver()
    {
        celebration.SetActive(true);
    }

	void SpawnEnemy(Transform _enemy)
		{
        //Spawn Enemy
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No Spawn Points Referenced");
        }
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
		Instantiate(spawnPortal, _sp.position, _sp.rotation);
        Instantiate(_enemy, _sp.position, _sp.rotation);
		//Debug.Log("Spawning Enemy: " + _enemy.name);

		}

    void WaveCompleted()
    {
        state = SpawnState.counting;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
			endPortal.SetActive(true);
            Debug.Log("You win!");
        }
        nextWave++;
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                return false;
            }
        }
        
        return true;
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