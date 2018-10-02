using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundWon : MonoBehaviour {

    public GameObject RoundWonCamera;
    public GameObject HudCanvas;
    public GameObject roundWonCanvas;
    public Canvas hudCanvas;
    public GameObject player;
    bool roundWon = false;

	// Use this for initialization
	void Start () {
        {
          
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the entering collider is the player...
        if (other.gameObject == player)
        {
            YouWon();
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public void YouWon()
    {
        // ... the round is over with.
        roundWon = true;
        Debug.Log("You Won This Round");
        hudCanvas.GetComponent<Timer>().Finished();
        player.gameObject.SetActive(false);
        HudCanvas.gameObject.SetActive(false);
        RoundWonCamera.gameObject.SetActive(true);
        Time.timeScale = 0.5f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
