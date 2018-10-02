using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour {

    public GameObject roundWonCamera;
    public GameObject roundWonCanvas;
    public GameObject hudCanvas;
    public GameObject player;

    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
        player.gameObject.SetActive(true);
        hudCanvas.gameObject.SetActive(true);
        roundWonCamera.gameObject.SetActive(false);
        roundWonCanvas.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        
	}

	// Update is called once per frame
	void Update () {
        
    }
}
