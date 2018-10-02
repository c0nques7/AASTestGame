using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCounter : MonoBehaviour {
    public Text score;
    public Text highScore;
    public static int points;
    Animator anim;


    void Start()
    {
        points = 0;
        anim = GetComponent<Animator>();
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    private void Update()
    {
        SetCountText();
        //if (points == 50){
            //Debug.Log("SuperBow Ready!");
            //anim.SetTrigger("Bow");
        //}
    }

    void SuperBow()
    {
        
    }

    public void SetCountText()
    {

        score.text = "Score: " + points.ToString();
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
