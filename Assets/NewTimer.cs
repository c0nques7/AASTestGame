﻿ using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;

public class NewTimer : MonoBehaviour {

    private static NewTimer _instance;

    public bool gameStarted = false;

    public bool countdownStarted = false;

    public Text timerLabel;
    public Text highScore;
    public bool pauseTimer = true;
    public bool resetTriggered;
    bool hScore;

    bool playSound1;
    bool playSound2;
    bool playSound3;
    bool playSound4;

    bool playedSound;

    public float time;
    public float bestTime;
    public float currentTime;
    

    public Canvas hudCanvas;

    public NewTargetScript newTargetScript;
    PointCounter pointCounter;

    //The Audio Source
    public AudioSource chimeSource;

    // The sound itself.
    public AudioClip clipToPlay;
    public AudioClip startClip;

    Text[] hudText;

	 Animator anim;
     Animator hudAnim;

	 	 Color m_MouseOverColor = Color.white;

		 Color m_OriginalColor;

		 MeshRenderer m_Renderer;

    public void Start()
    {
        
    }

    public void Awake()
     {
        hScore = false;
        playedSound = false;
        playSound1 = false;
        playSound2 = false;
        playSound3 = false;
        playSound4 = false;
        gameStarted = false;
        hudAnim = hudCanvas.GetComponent<Animator>();
		anim = GetComponentInChildren<Animator>();
		m_Renderer = GetComponent<MeshRenderer>();
		m_OriginalColor = m_Renderer.material.color;
        pauseTimer = false;
        resetTriggered = false;
     }
     
     
     public void Update() {


        hudText = GameObject.Find("HUDObject").GetComponentsInChildren<Text>();
        pointCounter = GameObject.Find("HUDObject").GetComponentInChildren<PointCounter>();
  
        if (hudText[9].color.a >= 0.95 && playedSound == false && playSound1 == false)
        {
            playSound1 = true;
            if (playSound1 == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
            
        }
        if (hudText[10].color.a >= 0.95 && playSound2 == false && playedSound == false)
        {
            playSound2 = true;
            playedSound = true;
            if (playSound2 == true && playedSound == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
        }
        if (hudText[11].color.a >= 0.95 && playSound3 == false && playedSound == false)
        {
            playSound3 = true;
            playedSound = true;
            if (playSound3 == true && playedSound == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
        }
        if (hudText[12].color.a >= 0.95 && playSound4 == false && playedSound == false)
        {
            playSound4 = true;
            playedSound = true;
            if (playSound4 == true && playedSound == true)
            {
                chimeSource.PlayOneShot(startClip);
                playedSound = false;
            }
        }

        if (pauseTimer == true)
         time += Time.deltaTime;
 
         
         var minutes = Mathf.Floor(time / 60);
         var seconds = time % 60;//Use the euclidean division for the seconds.
         var fraction = (time * 100) % 100;
         
         //update the label value
         timerLabel.text = string.Format ("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);

        


     }

	 void OnMouseOver()
	 {
		 m_Renderer.material.color = m_MouseOverColor;
		 anim.SetBool("MouseOver", true);

		 if (Input.GetKeyDown(KeyCode.E))
		 {
            countdownStarted = true;
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
	 }

	 void OnMouseExit()
	 {
		 anim.SetBool("MouseOver", false);
		 m_Renderer.material.color = m_OriginalColor;
	 }


     IEnumerator Countdown(int seconds)
     {
         int count = seconds;
         while (count > 0)
         {
             //Display Countdown Here
             hudAnim.SetBool("Start", true);
            yield return new WaitForSeconds(1);
             count --;
        }

        StartGame();
     }

     public void StartGame()
     {
         StartTimer();
     }
 
     //Reset Timer
     public void  ResetTimer(){
         time = 0;
         anim.SetBool("TimerStarted", false);
         Debug.Log("Timer Reset");
         resetTriggered = true;
         gameStarted = false;
         PlayerPrefs.DeleteKey("BestTime");
         //Debug.Log("The best time has been reset");
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
        
            
        if(hScore == true)
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
     public void  StartTimer(){

         //Start Timer Here
         pauseTimer = true;
         hudAnim.SetBool("Start", false);
		 anim.SetBool("TimerStarted", true);
         Debug.Log("Timer Started");
         gameStarted = true;
     }
 }
 
    