using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static int minDamage;
	public static int maxDamage;
	public static int weaponRarity;

	public static int zombiePoints; 
	
	public ArmControllerScript armControllerScript;

    public float speed = 1;
    private float startTime;
	float currentAmount = 0f;
	float maxAmount = 10f;
    float theTime;
    bool playing;

    public Text timerText;

	public GameObject player;
	public GameObject bow;
	public GameObject primary;
	Animator anim;
	// Use this for initialization
	void Start () {

        startTime = Time.time;
        timerText = GetComponent<Text>();
		anim = GetComponent<Animator>();
		armControllerScript = player.GetComponentInChildren<ArmControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {

        float t = Time.time - startTime;

        string minutes = ((int) t / 60).ToString();
        string seconds = (t % 60).ToString("00");

        timerText.text = minutes + ":" + seconds;

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
            anim.SetTrigger("Bow");

			if (Input.GetKeyDown(KeyCode.Z))
			{
				SlowMoPower();
			}
			

        }

		if (bow.tag == "CommonRifle")
		{
			weaponRarity = 1;
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

	void SlowMoPower()
	{
        Time.timeScale = 0.7F;
		
	}

    void timerStart()
    {
        playing = true;
    }

    void timerStop()
    {
        playing = false;
    }

}

		//xjuice