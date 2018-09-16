using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfoText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().sortingOrder = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (WeaponManager.textstatus == "off")
		{
			Destroy(gameObject);
		}
		
	}
}
