using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetsHaveAChat : MonoBehaviour {

	public bool MouseOver;
	Animator anim;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver(){
		 anim.SetBool("MouseOver", true);
		 Debug.Log("The animator is working and is set to true");
	}
}

