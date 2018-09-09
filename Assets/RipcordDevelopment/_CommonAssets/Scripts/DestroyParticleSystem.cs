using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						  Ripcord Tools, Copyright © 2017, Ripcord Development
//										 DestroyParticleSystem.cs
//												 v1.1.0
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - Destroys the attached particle system once the last particle has disappeared

namespace Ripcord.Common {
	public class DestroyParticleSystem : MonoBehaviour {

		void Update () {
			
			if (gameObject.GetComponent<ParticleSystem>().isStopped) {			//If the particle system has stopped playing...
				Destroy(gameObject);											//...destroy the gameObject
			}
		}
	}
}