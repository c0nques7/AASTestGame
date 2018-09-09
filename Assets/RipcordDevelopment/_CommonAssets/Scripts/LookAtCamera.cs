using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						  Ripcord Tools, Copyright © 2017, Ripcord Development
//										     LookAtCamera.cs
//												 v1.1.0
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - Attaching this script to a gameObject will cause the object to face the camera when it is instanced.  
//		- If alwaysLookAt is true, the object will always face the camera, no matter where it moves.

namespace Ripcord.Common {
	public class LookAtCamera : MonoBehaviour {

		public Camera targetCamera;		//Specify a camera for the object to look at.  If no camera is specified it will default to the one with the MainCamera tag
		public bool alwaysLookAt;		//If true, the object will continuously look at the specified camera.  If false, the object will only look on the first frame

		public Vector3 offsetRotation;

		void Start () {

			if (!targetCamera) {												//If a target camera has not been specified...
				targetCamera = Camera.main;										//...set the camera tagged "main" as the target camera
			}
		}

		void Update () {

			transform.LookAt(Camera.main.transform);							//When the object first appears, make it face towards the camera
			transform.rotation *= Quaternion.Euler(offsetRotation);				//Apply the rotation offset so the object properly aligns to the camera

			if (!alwaysLookAt) {												//If the object is not set to continuously look at the camera...
				this.enabled = false;											//...disable this script
			}
		}
	}
}