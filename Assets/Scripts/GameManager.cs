using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public enum SpawnState  { spawning, waiting, counting };

	[System.Serializable]
	public class Wave{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
	}

    [Header("Canvaes, Animators, Renderers and Colors")]
    Canvas hudCanvas;
    Animator anim;
    Animator hudAnim;
    Color m_MouseOverColor = Color.white;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;
    Renderer rend;

    [Header("Text")]
    public Text timerLabel;
    public Text timerText;
    public Text fastestTime;

    Text[] hudText;

    [Header("Audio")]
    public AudioClip clipToPlay;
    public AudioClip startClip;
    public AudioClip upSound;
    public AudioClip downSound;
    public AudioClip hitSound;
    public AudioSource audioSource;


    [Header("Timer and Target")]

    public GameObject gameTarget;
    public GameObject FloatingHitPrefab;
    GameObject timerController;
    Animator timerAnim;
    

    [Header("Variables")]

    public bool pauseTimer = true;
    public bool resetTriggered;
    bool partyOn;
    bool gameStarted;
    bool countdownStarted;
    bool isHit = false;
    public bool mouseOver = false;
    float minTime;
    float maxTime;
    float time;
    float bestTime;
    float currentTime;
    float randomTime;
    //Floats used to modify game speed
    float currentAmount = 0f;
    float maxAmount = 10f;

    [Header("Scripts")]
    NewTargetScript newTargetScript;
    PointCounter pointCounter;
    NewTimer newTimer;


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
    
	

    bool hScore;
    
    bool playSound1;
    bool playSound2;
    bool playSound3;
    bool playSound4;

    bool playedSound;

    // Use this for initialization
    void Start () {
        mouseOver = false;
        playedSound = false;
        playSound1 = false;
        playSound2 = false;
        playSound3 = false;
        playSound4 = false;
        gameStarted = false;
        hScore = false;
        hudAnim = hudCanvas.GetComponent<Animator>();
        anim = GetComponentInChildren<Animator>();
        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;
        pauseTimer = false;
        resetTriggered = false;
        
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
    public void Update () {
        newTimer = GameObject.FindGameObjectWithTag("Controller").GetComponent<NewTimer>();
        timerController = GameObject.FindGameObjectWithTag("Controller");
        timerAnim = GameObject.FindGameObjectWithTag("Controller").GetComponent<Animator>();
        hudText = GameObject.Find("HUDObject").GetComponentsInChildren<Text>();
        pointCounter = GameObject.Find("HUDObject").GetComponentInChildren<PointCounter>();
        rend = GetComponent<Renderer>();

        Debug.Log("MouseOver Bool is currently" + timerAnim.GetBool("MouseOver"));

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


        TargetControl();
		

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

        

        //Mouseover Events
        if (timerAnim.GetBool("MouseOver"))
        {
            
        }

        if (newTimer.mouseOver == false)
        {
            
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

        if (pauseTimer == true)
            time += Time.deltaTime;


        var minutes = Mathf.Floor(time / 60);
        var seconds = time % 60;//Use the euclidean division for the seconds.
        var fraction = (time * 100) % 100;

        //update the label value
        timerLabel.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);


    }

    void TargetControl()
    {
        if (newTimer.gameStarted == true && partyOn == true)
        {
            gameObject.GetComponent<Animation>().Play("target_up");
            //Set the upSound as current sound, and play it
            audioSource.pitch = 1;
            audioSource.GetComponent<AudioSource>().clip = upSound;
            audioSource.Play();
            partyOn = false;
            rend.material.color = Color.red;

        }
        if (newTimer.gameStarted == false && partyOn == false)
        {
            //Animate the target "down"
            gameObject.GetComponent<Animation>().Play("target_down");
            partyOn = true;
        }

        //Generate random time based on min and max time values
        randomTime = Random.Range(minTime, maxTime);

        //If the target is hit

        if (isHit == true && partyOn == false)
        {
            PointCounter.enemies += -1;
            //Animate the target "down"
            gameObject.GetComponent<Animation>().Play("target_down");
            //anim.SetBool("down", true);

            //Set the audiosource to .5 pitch
            audioSource.pitch = 0.5f;
            //Set the downSound as current sound, and play it
            audioSource.GetComponent<AudioSource>().clip = downSound;
            //Also play the hitSound
            audioSource.GetComponent<AudioSource>().clip = hitSound;
            audioSource.Play();

            //Instantiate the hit text prefab
            if (FloatingHitPrefab != null)
            {
                ShowFloatingScore();

            }
            rend.material.color = Color.green;
            isHit = false;
        }
    }

    void ShowFloatingScore()
    {
        var go = Instantiate(FloatingHitPrefab, transform.position, transform.rotation);
        go.GetComponent<TextMesh>().text = "HIT!";

    }


    IEnumerator Countdown(int seconds)
    {
        int count = seconds;
        while (count > 0)
        {
            //Display Countdown Here
            hudAnim.SetBool("Start", true);
            yield return new WaitForSeconds(1);
            count--;
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
        resetTriggered = true;
        gameStarted = false;
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
        hudAnim.SetBool("EndGame", true);
        Debug.Log("The current time is: " + currentTime);

        if (currentTime <= PlayerPrefs.GetFloat("BestTime", currentTime))
        {
            hScore = true;
            PlayerPrefs.SetFloat("BestTime", currentTime);
            pointCounter.highScore.text = PlayerPrefs.GetFloat("BestTime").ToString("Fastest Time: " + "0:00.00");
            Debug.Log("The best time is: " + PlayerPrefs.GetFloat("BestTime"));
        }

        if (hScore == true)
        {
            hudAnim.SetBool("HighScore", true);
            hScore = false;
        }

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
        hudAnim.SetBool("Start", false);
        anim.SetBool("TimerStarted", true);
        Debug.Log("Timer Started");
        gameStarted = true;
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