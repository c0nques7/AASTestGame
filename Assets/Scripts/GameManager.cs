using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING};
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform normalEnemy;
        public int count = 10;
        public float rate;
    }

    public GameObject playerCanvas;
    public GameObject gameOverCanvas;
    public GameObject gameOverCamera;
    public GameObject player;

    PlayerHealth playerHealth;

    public int enemyCount;
    public Wave[] waves;
    private int nextWave = 0;
    public float waveNumber = 0;
    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    public Transform[] spawnPoints;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        waveNumber = 0;
        
        if (spawnPoints.Length == 0)
        {
            Debug.Log("No Spawnpoints Referenced");
        }

        waveCountdown = timeBetweenWaves;
    }
    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            //Check if enemies are still alive
            if (!EnemyIsAlive())
            {
               
                //Increase the waves
                WaveCompleted();
            }
            else
            {
                return;
            }
        }
        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                //start spawning wave
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
        if (playerHealth.isDead == true)
        {
            GameOver();
        }
        
    }


    public void GameOver()
    {
        playerCanvas.SetActive(false);
        player.SetActive(false);
        gameOverCanvas.SetActive(true);
        gameOverCamera.SetActive(true);
        

    }


    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;
        nextWave = 0;
        waveNumber++;
        if (nextWave + 1 > waves.Length - 1)
        {
            Debug.Log("Wave Restarting...");            
        }
    }

    
       
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if(searchCountdown <= 0f)
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
        state = SpawnState.SPAWNING;
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.normalEnemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }


        //During the wait state, the amount of enemies spawned increases by 10 every wave
        state = SpawnState.WAITING;
        _wave.count += 10;


        yield break;
    }
    void SpawnEnemy (Transform _enemy)
    {
        //Spawn Enemy
        
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }
}
       
//xjuice