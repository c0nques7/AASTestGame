using UnityEngine;
using System.Collections;

namespace SmartSpawn.Demos {

public class SSBasicMoveScript : MonoBehaviour {
		
		[Header("Very basic rigidbody control script")]
	private Rigidbody attachedRigidbody;

	private float speed = 3f;

	// Use this for initialization
	void Start () {
		attachedRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W))
		{
			attachedRigidbody.velocity = attachedRigidbody.transform.TransformDirection(Vector3.forward) * speed;
		}
		if(Input.GetKey(KeyCode.S))
		{
			attachedRigidbody.velocity = attachedRigidbody.transform.TransformDirection(Vector3.back) * speed;
		}
		if(Input.GetKey(KeyCode.A))
		{
			attachedRigidbody.velocity = attachedRigidbody.transform.TransformDirection(Vector3.left) * speed;
		}
		if(Input.GetKey(KeyCode.D))
		{
			attachedRigidbody.velocity = attachedRigidbody.transform.TransformDirection(Vector3.right) * speed;
		}

		if(!Input.anyKey)
		{
			attachedRigidbody.velocity = Vector3.zero;
		}


	}
}

}