 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;

public class NewTimer : MonoBehaviour {

    public bool gameStarted = false;

    public bool countdownStarted = false;

    public Text timerLabel;
    public Text highScore;
    public bool pauseTimer;
    public bool resetTriggered;
    public bool mouseOver;
    
    //Only enable this if you move something up on the HUDCanvas and stuff stops working
    //Jump to line 66
    //bool hudChecked;
    bool hScore;

    bool playSound1;
    bool playSound2;
    bool playSound3;
    bool playSound4;

    bool playedSound;

    public float time;
    public float bestTime;
    public float currentTime;

    GameManager gameManager;

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
        //After this is enabled, jump to line 90
        //hudChecked = false;
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

     }
     
     
     public void Update() {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hudText = GameObject.Find("HUDObject").GetComponentsInChildren<Text>();
        pointCounter = GameObject.Find("HUDObject").GetComponentInChildren<PointCounter>();

        //Finally, enable lines 154-326
        /*if(hudText != null && hudChecked == false)
        {
            CheckForHUDObject();
            hudChecked = true;
        }*/

        if (hudText[12].color.a >= 0.9 && playedSound == false && playSound1 == false)
        {
            playSound1 = true;
            if (playSound1 == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
            
        }

        if (hudText[13].color.a >= 0.9 && playSound2 == false && playedSound == false)
        {
            playSound2 = true;
            playedSound = true;
            if (playSound2 == true && playedSound == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
        }

        if (hudText[14].color.a >= 0.9 && playSound3 == false && playedSound == false)
        {
            playSound3 = true;
            playedSound = true;
            if (playSound3 == true && playedSound == true)
            {
                chimeSource.PlayOneShot(clipToPlay);
                playedSound = false;
            }
        }

        if (hudText[15].color.a >= 0.9 && playSound4 == false && playedSound == false)
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
    //Only enable this if you move something in the HUDCanvas
     /*void CheckForHUDObject()
     {
         if(hudText[0] != null)
        {
            Debug.Log("0 here! I'm present, and my name is: " + hudText[0]);
        }
        else
        {
            Debug.Log("HUD Text Object 0 is not present");
        }

        if(hudText[1] != null)
        {
            Debug.Log("1 here! I'm present, and my name is: " + hudText[1]);
        }
        else
        {
            Debug.Log("HUD Text Object 1 is not present");
        }

        if(hudText[2] != null)
        {
            Debug.Log("2 here! I'm present, and my name is: " + hudText[2]);
        }
        else
        {
            Debug.Log("HUD Text Object 2 is not present");
        }

        if(hudText[3] != null)
        {
            Debug.Log("3 here! I'm present, and my name is: " + hudText[3]);
        }
        else
        {
            Debug.Log("HUD Text Object 3 is not present");
        }

        if(hudText[4] != null)
        {
            Debug.Log("4 here! I'm present, and my name is: " + hudText[4]);
        }
        else
        {
            Debug.Log("HUD Text Object 4 is not present");
        }

        if(hudText[5] != null)
        {
            Debug.Log("5 here! I'm present, and my name is: " + hudText[5]);
        }
        else
        {
            Debug.Log("HUD Text Object 5 is not present");
        }

        if(hudText[6] != null)
        {
            Debug.Log("6 here! I'm present, and my name is: " + hudText[6]);
        }
        else
        {
            Debug.Log("HUD Text Object 6 is not present");
        }

        if(hudText[7] != null)
        {
            Debug.Log("7 here! I'm present, and my name is: " + hudText[7]);
        }
        else
        {
            Debug.Log("HUD Text Object 7 is not present");
        }

        if(hudText[8] != null)
        {
            Debug.Log("8 here! I'm present, and my name is: " + hudText[8]);
        }
        else
        {
            Debug.Log("HUD Text Object 8 is not present");
        }

        if(hudText[9] != null)
        {
            Debug.Log("9 here! I'm present, and my name is: " + hudText[9]);
        }
        else
        {
            Debug.Log("HUD Text Object 9 is not present");
        }

        if(hudText[10] != null)
        {
            Debug.Log("10 here! I'm present, and my name is: " + hudText[10]);
        }
        else
        {
            Debug.Log("HUD Text Object 10 is not present");
        }

        if(hudText[11] != null)
        {
            Debug.Log("11 here! I'm present, and my name is: " + hudText[11]);
        }
        else
        {
            Debug.Log("HUD Text Object 11 is not present");
        }

        if(hudText[12] != null)
        {
            Debug.Log("12 here! I'm present, and my name is: " + hudText[12]);
        }
        else
        {
            Debug.Log("HUD Text Object 12 is not present");
        }

        if(hudText[13] != null)
        {
            Debug.Log("13 here! I'm present, and my name is: " + hudText[13]);
        }
        else
        {
            Debug.Log("HUD Text Object 13 is not present");
        }

        if(hudText[14] != null)
        {
            Debug.Log("14 here! I'm present, and my name is: " + hudText[14]);
        }
        else
        {
            Debug.Log("HUD Text Object 14 is not present");
        }
        
        if(hudText[15] != null)
        {
            Debug.Log("15 here! I'm present, and my name is: " + hudText[15]);
        }
        else
        {
            Debug.Log("HUD Text Object 15 is not present");
        }
        
        if(hudText[16] != null)
        {
            Debug.Log("16 here! I'm present, and my name is: " + hudText[16]);
        }
        else
        {
            Debug.Log("HUD Text Object 16 is not present");
        }
        
        if(hudText[17] != null)
        {
            Debug.Log("17 here! I'm present, and my name is: " + hudText[17]);
        }
        else
        {
            Debug.Log("HUD Text Object 17 is not present");
        }
        
        if(hudText[18] != null)
        {
            Debug.Log("18 here! I'm present, and my name is: " + hudText[18]);
        }
        else
        {
            Debug.Log("HUD Text Object 18 is not present");
        }
     }*/

	 void OnMouseOver()
	 {
		 m_Renderer.material.color = m_MouseOverColor;
		 anim.SetBool("MouseOver", true);
         mouseOver = true;

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
        mouseOver = false;
    }


     private IEnumerator Countdown(int seconds)
     {
        countdownStarted = true;
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
 
    