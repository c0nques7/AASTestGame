using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour {

    public Text timerText;
    public Text fastestTime;
    private float startTime;
    private bool finished = false;
    public Animator anim;
    private bool mouseOver;
    private Renderer rend;
    public bool timerOn = false;

    

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        fastestTime.text = PlayerPrefs.GetFloat("FastestTime", 0).ToString();
        anim = gameObject.GetComponentInChildren<Animator>();
        mouseOver = false;
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (mouseOver == true && Input.GetKeyUp(KeyCode.E))
        {
            OnMouseOver();
            timerOn = true;
            
        }
        if (mouseOver == true && timerOn == true && Input.GetKeyUp(KeyCode.E))
        {
            Finished();
        }

         if (timerOn == true)
        {
            if (finished)
                return;

            float t = Time.time - startTime;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");

            timerText.text = minutes + ":" + seconds;


            if (t > PlayerPrefs.GetFloat("FastestTime", 0))
            {
                PlayerPrefs.SetFloat("FastestTime", t);
                fastestTime.text = "Fastest Time: " + t.ToString();
            }
        }

    }



    private void OnMouseOver()
    {
        mouseOver = true;
        anim.SetTrigger("MouseOver");
        Debug.Log("Your mouse is over the proper timer trigger.");

    }

    private void OnMouseExit()
    {
        mouseOver = false;
        anim.SetTrigger("MouseNotOver");
    }


    public void Finished()
    {
        finished = true;
        timerText.color = Color.blue;
    }

    
}
