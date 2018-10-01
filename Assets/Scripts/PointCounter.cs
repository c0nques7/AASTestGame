using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCounter : MonoBehaviour {
    public Text score;
    public static int points;
    Animator anim;

    void Start()
    {
        points = 0;
        anim = GetComponent<Animator>();
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
    }

}
