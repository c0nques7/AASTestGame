using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCounter : MonoBehaviour {
    public Text score;
    public GUISkin guiSkin = null;
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
        if (points == 50){
            Debug.Log("SuperBow Ready!");
            anim.SetTrigger("Bow");
        }
    }

    /*private void OnGUI()
    {
        GUI.skin = guiSkin;
        GUI.Label(new Rect(0.0f, 0.0f, 128f, 32.0f), "Zombies Killed:" + points.ToString());
        GUI.skin = null;
    }*/

    void SuperBow()
    {
        
    }

    public void SetCountText()
    {

        score.text = "Score: " + points.ToString();
    }

}
