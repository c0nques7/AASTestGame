using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public enum SpawnState { spawning, waiting, counting };


    //Floats used to modify game speed
    float currentAmount = 0f;
    float maxAmount = 10f;

    //Timer Variables
    [Header("HUD Elements")]
    public Text stopWatch;
    public Text fastestTime;
    public Text currentScore;
    public Text highScore;
    public Text currentWave;
    public Text enemiesLeft;
    public Text score;

    [Header("Gameplay Variables")]
    public static int points;
    //public static int enemies;
    public static float wave;
    public int enemyCount;
    private GameObject[] enemies;

    [Header("Debug")]
    public bool pauseTimer;
    public float time;
    public float bestTime;
    public float currentTime;

    [Header("Music and Sounds")]
    public AudioSource musicSource;
    public AudioClip idleMusic;
    public AudioClip actionMusic;



    [Header("Wave Controls")]
    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;
    private SpawnState state = SpawnState.counting;

    private float searchCountdown = 1f;

    [Header("Spawnpoint Controls")]
    public Transform[] spawnPoints;


    [Header("Entry and Exit Points")]
    public GameObject endPortal;
    public GameObject spawnPortal;
    public GameObject entryPoint;
    public GameObject exitPoint;



    [Header("Misc References")]
    public GameObject player;
    public GameObject bow;
    public GameObject primary;
    public GameObject celebration;
    Animator anim;


    // Use this for initialization
    void Start()
    {
        pauseTimer = false;
        musicSource.PlayOneShot(idleMusic);
        
        points = 0;
        wave = 1;
        enemyCount = 0;
        SetCountText();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No Spawn Points Referenced");
        }
        anim = GetComponent<Animator>();
        waveCountdown = timeBetweenWaves;
    }

    // Update is called once per frame
    public void Update()
    {        
        SetCountText();

        if (state == SpawnState.waiting)
        {
            //Check if enemies are still alive
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                wave += 1;
            }
            else
            {
                return;
            }
        }

        /*if (waveCountdown <= 0)
		{
			if (state != SpawnState.spawning)
			{
                StartCoroutine(SpawnWave(waves[nextWave]));
			}
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}*/





        /*if (PointCounter.points >= 50){
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
			

        }*/

        //Check to see if the timescale has been modified
        if (Time.timeScale == 0.7f)
        {
            currentAmount += Time.deltaTime;
        }
        //When currentAmount is greater than maxAmount, return the timescale to 1.0
        if (currentAmount > maxAmount)
        {
            currentAmount = 0f;
            Time.timeScale = 1.0f;
        }

        //Stopwatch Controls

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Starting countdown coroutine...");
            //countdownStarted = true;
            StartCoroutine(Countdown(5));
        }
        if (pauseTimer == false && Input.GetKeyDown(KeyCode.T))
        {
            ResetTimer();
        }
        if (pauseTimer == true && Input.GetKeyDown(KeyCode.Q))
        {
            StopTimer();
        }

        //Stopwatch
        if (pauseTimer == true)
            time += Time.deltaTime;


        var minutes = Mathf.Floor(time / 60);
        var seconds = time % 60;//Use the euclidean division for the seconds.
        var fraction = (time * 100) % 100;

        //update the label value
        stopWatch.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);

        


    }

    public void SetCountText()
    {
        currentWave.text = wave.ToString();
        score.text = points.ToString();
        enemiesLeft.text = enemyCount.ToString();

        if (points > PlayerPrefs.GetInt("HighScore", 0))
        {

            PlayerPrefs.SetInt("HighScore", points);
            highScore.text = "High Score: " + points.ToString();
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
        Debug.Log("Spawning wave");
        state = SpawnState.spawning;
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }
        state = SpawnState.waiting;
        yield break;
    }

    void SlowMoPower()
    {
        Time.timeScale = 0.7F;
        PointCounter.points = 0;

    }

    private IEnumerator Countdown(int seconds)
    {
        //countdownStarted = true;
        int count = seconds;
        while (count > 0)
        {
            //Display Countdown Here
            //hudAnim.SetBool("Start", true);
            yield return new WaitForSeconds(1);
            count--;
        }
        Debug.Log("Starting spawn coroutine...");
        yield return StartCoroutine(SpawnWave(waves[nextWave]));
        if (waveCountdown <= 0)
        {
            if (state != SpawnState.spawning)
            {
                Debug.Log("Starting spawn coroutine...");
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }


        StartGame();
    }

    public void StartGame()
    {
        StartTimer();

    }

    //Reset Timer
    public void ResetTimer()
    {
        time = 0;
        anim.SetBool("TimerStarted", false);
        Debug.Log("Timer Reset");
        //resetTriggered = true;
        //gameStarted = false;
        PlayerPrefs.DeleteKey("BestTime");
        //Debug.Log("The best time has been reset");
    }

    void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("BestTime");
    }

    //Stop Timer
    public void StopTimer()
    {
        float currentTime = time;
        //hudAnim.SetBool("EndGame", true);
        Debug.Log("The current time is: " + currentTime);

        if (currentTime <= PlayerPrefs.GetFloat("BestTime", currentTime))
        {
            //hScore = true;
            PlayerPrefs.SetFloat("BestTime", currentTime);
            //pointCounter.highScore.text = PlayerPrefs.GetFloat("BestTime").ToString("Fastest Time: " + "0:00.00");
            Debug.Log("The best time is: " + PlayerPrefs.GetFloat("BestTime"));
        }

        /*if (hScore == true)
        {
            hudAnim.SetBool("HighScore", true);
            hScore = false;
        }*/

        Debug.Log("Timer Stopped");
        //Stop Timer Here
        anim.SetBool("TimerStarted", false);
        pauseTimer = false;
    }

    //Start Timer
    public void StartTimer()
    {

        //Start Timer Here
        pauseTimer = true;
        //hudAnim.SetBool("Start", false);
        anim.SetBool("TimerStarted", true);
        Debug.Log("Timer Started");
        //gameStarted = true;
    }

}

//xjuice