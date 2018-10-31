using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocalPoint : MonoBehaviour {

    //This is what the camera will be looking at
    public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target);
	}
}
