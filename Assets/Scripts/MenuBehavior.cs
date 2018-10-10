using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour {

	public GameObject[] planet;
	public GameObject currentPlanet;
	Animator anim; 
	



	// Use this for initialization
	void Start () {
	anim = GetComponent<Animator>();

	currentPlanet = planet[0];

	anim.SetTrigger("isViewed");
		
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetTrigger("isViewed");

	
	}
}
