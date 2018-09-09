using UnityEngine; 
using System.Collections; 

public class CameraController : MonoBehaviour {
	public GameObject target;									//The target to follow 
	public float maxDistance = 20f;								//The amount the player can zoom out
	public float minDistance = 0.6f;							//The amount the player can zoom in 
	public float mouseRotationSpeed = 5.0f;						//The speed at which the mouse can rotate the camera 
	public float yMinLimit = -80.0f;							//How steep can the camera look up
	public float yMaxLimit = 80.0f;								//How steep can the camera look down
	public float zoomSpeed = 5.0f;								//How fast the camera can zoom in
	public float rotationSpeed = 3.0f;							//How fast can the camera rotate around in space?
	public Vector3 playerOffset = new Vector3(0, 0.5f, 0);		//The offset of the player
	public LayerMask obstacles = -1;							//The layers the camera can collide with
	
	private float yaw = 0.0f;									//The x rotation of the camera
	private float pitch = 0.0f; 								//The y rotation of the camera
	private float currentDistance; 								//The distance currently between the camera and the player
	private float desiredDistance; 								//The distance the player wants the camera to be
	private float correctedDistance; 							//If the camera is colliding with something this is the distance to be used

	void Start () {
		//Get the angles currently on the cam
		Vector3 angles = transform.eulerAngles;
		//Set those values as the start rotation
		yaw = angles.x; 
		pitch = angles.y;
		//Set the currrent distance between the camera and the player equal to half of the min + max distance
		currentDistance = desiredDistance = correctedDistance = Mathf.Lerp(minDistance, maxDistance, 0.5f);
	}
	
	void LateUpdate () {

		//Increase the x rotation of the camera when moving the mouse on the X axis
		yaw += Input.GetAxis ("Mouse X") * mouseRotationSpeed;
		//Increase the y rotation of the camera when moving the mouse on the Y axis
		pitch -= Input.GetAxis ("Mouse Y") * mouseRotationSpeed;

		//Make sure the y rotation doesn't exceed the min and max values
		pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit); 

		//Calculate the distance to the player when scrolling
		desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * 4.0f;
		//Make sure the distance doesn't exceed the min and max values
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		//Set the corrected equal to the just calculated desiredDistance
		correctedDistance = desiredDistance;
		
		//Check to see if the camera is colliding with anything
		CheckCollision();
		
		//Lerp the current distance to the correctedDistance - this gives a smooth zooming
		currentDistance = Mathf.Lerp(Mathf.Clamp(currentDistance, minDistance, maxDistance), correctedDistance, Time.deltaTime * zoomSpeed);
		
		//First convert the pitch and yaw to a quaternion instead of vector3
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);
		//Assign the rotation of the camera
		transform.rotation = rotation;
		//Assign the position of the camera
		transform.position = target.transform.position + playerOffset - (rotation * Vector3.forward * currentDistance);;		
	}
	
	private void CheckCollision() {
		RaycastHit hit; 
		//Shoot a ray from the player to the camera and check to see if it hit's anything on the way
		//If it does hit something move the camera in front of the object it hits
		if(Physics.Raycast (target.transform.position, -transform.forward, out hit,correctedDistance, obstacles)) { 
			correctedDistance = Vector3.Distance(target.transform.position, hit.point) - 0.25f;
		}
	}
}  