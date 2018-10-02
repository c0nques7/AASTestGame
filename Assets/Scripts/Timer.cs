using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public Text timerText;
    public Text fastestTime;
    private float startTime;
    private bool finished = false;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        fastestTime.text = PlayerPrefs.GetFloat("FastestTime", 0).ToString();
	}
	
	// Update is called once per frame
	void Update () {

        if (finished)
            return;

        float t = Time.time - startTime;
        string minutes = ((int) t / 60).ToString();
        string seconds = (t % 60).ToString("f2");

        timerText.text = minutes + ":" + seconds;

        if (t > PlayerPrefs.GetFloat("FastestTime", 0))
        {
            PlayerPrefs.SetFloat("FastestTime", t);
            fastestTime.text = "Fastest Time: " + t.ToString();
        }

    }

    public void Finished()
    {
        finished = true;
        timerText.color = Color.blue;
    }
}
