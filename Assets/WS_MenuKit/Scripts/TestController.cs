using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {

	public CustomInput InputManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (InputManager.forward)) {
			Debug.Log ("The key "+InputManager.forward.ToString()+" has been pressed!");
		}

		if (Input.GetKey (InputManager.back)) {
			Debug.Log ("The key "+InputManager.back.ToString()+" has been pressed!");
		}

		if (Input.GetKey (InputManager.left)) {
			Debug.Log ("The key "+InputManager.left.ToString()+" has been pressed!");
		}

		if (Input.GetKey (InputManager.right)) {
			Debug.Log ("The key "+InputManager.right.ToString()+" has been pressed!");
		}

		if (Input.GetKey (InputManager.crouch)) {
			Debug.Log ("The key "+InputManager.crouch.ToString()+" has been pressed!");
		}

		if (Input.GetKey (InputManager.jump)) {
			Debug.Log ("The key "+InputManager.jump.ToString()+" has been pressed!");
		}

	}
}
