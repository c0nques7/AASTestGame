 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;
 
 public class NewTimer : MonoBehaviour {
 
     private static NewTimer _instance ;
 
     public Text timerLabel;
     public bool pauseTimer = true;
     public bool resetTriggered;
 
     public float time;

     public Canvas hudCanvas;

     public TargetScript targetScript;

	 Animator anim;
     Animator hudAnim;

	 	 Color m_MouseOverColor = Color.white;

		 Color m_OriginalColor;

		 MeshRenderer m_Renderer;
 
     void Awake()
     {
        hudAnim = hudCanvas.GetComponent<Animator>();
		anim = GetComponentInChildren<Animator>();
		m_Renderer = GetComponent<MeshRenderer>();
		m_OriginalColor = m_Renderer.material.color;
        pauseTimer = false;
        resetTriggered = false;
     }
     
     
     void Update() { 
         if(pauseTimer == true)
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
         ;
     }
 
     //Reset Timer
     public void  ResetTimer(){
         time = 0;
         anim.SetBool("TimerStarted", false);
         Debug.Log("Timer Reset");
         resetTriggered = true;
     }
 
     //Stop Timer
     public void  StopTimer(){
         //Stop Timer Here
         anim.SetBool("TimerStarted", false);
         pauseTimer = false;
         Debug.Log("Timer Stopped");
     }
 
     //Start Timer
     public void  StartTimer(){
         //Start Timer Here
         pauseTimer = true;
         hudAnim.SetBool("Start", false);
		 anim.SetBool("TimerStarted", true);
         Debug.Log("Timer Started");
     }
 }
 
    