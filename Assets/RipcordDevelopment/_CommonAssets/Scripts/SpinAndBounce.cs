using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						  Ripcord Tools, Copyright © 2017, Ripcord Development
//											SpinAndBounce.cs
//												 v1.1.0
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script controls rotation speed and direction of any object it is applied to.  It can also make the object hover up and down.

namespace Ripcord.Common {
	public class SpinAndBounce : MonoBehaviour {

		public float spinRate;						//How fast the object will spin
		public float bounceAmount = 0.25f;			//How far the object will bounce up and down
		public float bounceSpeed = 3.0f;			//How fast the object will bounce up and down


		void Update () {
			
			transform.Rotate(0.0f, (spinRate * Time.deltaTime), 0.0f);
			transform.Translate(0.0f, Mathf.Sin(Time.time * bounceSpeed) * (bounceAmount / 100.0f), 0.0f);
		}
	}
}