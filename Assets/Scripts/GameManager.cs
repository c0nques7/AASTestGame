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


	float currentAmount = 0f;
	float maxAmount = 10f;
    bool playing;

    public Text timerText;

	public GameObject player;
	public GameObject bow;
	public GameObject primary;
	Animator anim;
	// Use this for initialization
	void Start () {


        timerText = GetComponent<Text>();
		anim = GetComponent<Animator>();
		armControllerScript = player.GetComponentInChildren<ArmControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {

        anim.SetTrigger("Start");

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