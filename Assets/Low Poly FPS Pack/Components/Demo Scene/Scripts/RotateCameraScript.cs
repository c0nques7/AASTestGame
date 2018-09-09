using UnityEngine;
using System.Collections;

public class RotateCameraScript : MonoBehaviour {

	//How fast the camera rotates
	public float rotationSpeed;
	
	// Update is called once per frame
	void Update () {
		//Rotate on z based on speed
		transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime);
	}
}