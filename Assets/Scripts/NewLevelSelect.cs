using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevelSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadFirstGallery()
    {
        SceneManager.LoadScene("FirstGallery");
    }
    public void LoadFirstArena()
    {
        SceneManager.LoadScene("FirstArena");
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("TestMenu");
    }
    
}
