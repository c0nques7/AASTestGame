using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCounter : MonoBehaviour {
    public Text score;
    public Text highScore;
    public Text enemiesLeft;
    public Text currentWave;
    public static int points;
    public static int enemies;
    public static float wave;
    private GameObject[] targets;
    Animator anim;

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
        points = 0;
        enemies = targets.Length;
        wave = 1;
        
        anim = GetComponent<Animator>();
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        

    }

    private void Update()
    {
        SetCountText();
        

    }



    

    IEnumerator WaveWait()
    {
        yield return new WaitForSeconds(6f);
    }

    public void SetCountText()
    {
        currentWave.text = "Wave: " + wave.ToString();
        score.text = "Score: " + points.ToString();
        enemiesLeft.text = "Enemies Left: " + enemies.ToString();
        if (points > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", points);
            highScore.text = "High Score: " + points.ToString();
        }

        
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey("HighScore");
    }

}
