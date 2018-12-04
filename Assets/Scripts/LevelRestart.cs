using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour {

    public GameObject roundWonCamera;
    public GameObject roundWonCanvas;
    public GameObject hudCanvas;
    public GameObject pauseMenuCanvas;
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuCanvas.gameObject.SetActive(false);
        hudCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


        
	// Use this for initialization
	void Start () {
        
	}

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hudCanvas.gameObject.SetActive(false);
            pauseMenuCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
