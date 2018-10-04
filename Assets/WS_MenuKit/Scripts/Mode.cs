using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum animDir{
	vertical, horizontal
}

public class Mode : MonoBehaviour {

	[Header("Basic Settings")]
	public bool isActive = true;							// This game mode is active
	public bool enableMask = true;							// If this option is off, this makes a darkening effect
	public float minSize = 1f;								// Minimum size of game mode
	public float maxSize = 1.10f;							// Maximum game mode size

	[Header("Animation Settings")]
	public bool isAnim=true;								// Initial animation
	public animDir ad;										// Direction of the animation (from where it comes)
	public float timerInitAnim;								// Time for animation to start
	public float axisStart = -700f;							// If the animation is active: Distance where the game mode (panel) appears
	public float animSpeed = 700f;							// Animation speed

	// Variables that the user does not need to change
	private RectTransform _rect;							// Variable responsible for the Rect Transform component
	private GameObject _mask;								// Variable responsible for mask object (effect)
	private float _currentTime;								// Variable responsible for the animation time
	private Vector3 _initPos;								// Variable responsible for the initial position of the object

	// Use this for initialization
	void Awake () {
		startSettings ();									// Call the method "startSettings"
		if(isAnim == true){
			resetAnim ();										// Call the method "resetAnim"
		}
	}
	
	// Update is called once per frame
	void Update () {		
		if(isAnim == true){
			updateAnimation ();									// Call the method "updateAnimation"
		}
	}

	// Start the basics settings
	void startSettings(){
		_rect = this.GetComponent<RectTransform> ();		// Get the Rect Transform component of this object
		_initPos = _rect.localPosition;						// Get the initial position of the object and store it in the variable responsible for it
		_currentTime = timerInitAnim;						// Get the time of animation

		if (enableMask==true && isActive==false) {			// See the mask effect on active and game mode is off
			_mask = this.transform.GetChild (3).gameObject;	// Get the GameObject of mask
			_mask.SetActive (true);							// Enable the mask effect
		}
	}// END

	// Updating the Animation
	void updateAnimation(){
		// If the current time is less than zero
		if (_currentTime <= 0) {
			// The animation starts
			_rect.transform.localPosition = Vector2.MoveTowards (_rect.transform.localPosition, _initPos, animSpeed * Time.deltaTime);
		} else { // ELSE
			// The timer starts
			_currentTime -= Time.deltaTime;
		}
	}// END

	// Effect of selecting when the mouse enters
	public void mouseEnter(){
		// If the game mode is active
		if(isActive)
			_rect.localScale = new Vector3 (maxSize, maxSize, maxSize); // I set the size of the game mode (panel) with the value of the maxSize variable
	}// END

	// Returns the default when the mouse exits
	public void mouseExit(){
		// If the game mode is active
		if(isActive)
			_rect.localScale = new Vector3 (minSize, minSize, minSize); // I set the size of the game mode (panel) with the value of the variable minSize
	}// END

	// When clicking / pressed
	public void mousePressed(string value){
		// If the game mode is active
		if (isActive) {
			switch (value) {
			case "playmode1":								// If I pass the string 'playmode1'
				Debug.Log ("Play in Mode 1");				// I display a message on the console saying game mode 1 was pressed
				break;
			case "playmode2":								// If I pass the string 'playmode2'
				Debug.Log ("Play in Mode 2");				// I display a message on the console saying game mode 2 was pressed
				break;
			case "playmode3":								// If I pass the string 'playmode3'
				Debug.Log ("Play in Mode 3");				// I display a message on the console saying game mode 3 was pressed
				break;
			}
		}
	}// END

	// Function responsible for calling two methods
	public void resetAnim(){
		setTimeInit ();										// Call the method
		setPosInitAnim ();									// Call the method
	}

	// Method responsible for setting the current time with the initial time
	private void setTimeInit(){
		_currentTime = timerInitAnim;						// Current time is the starting time
	}

	// Method responsible for defining the current position as the initial position
	private void setPosInitAnim(){
		switch (ad) {
		case animDir.vertical:								// If the direction is vertical
			_rect.localPosition = new Vector2 (_rect.localPosition.x, axisStart); // The position of the Y axis is defined as the value of the axisStart variable
			break;
		case animDir.horizontal:
			_rect.localPosition = new Vector2 (axisStart, _rect.localPosition.y); // The position of the X axis is defined as the value of the axisStart variable
			break;
		}
	}// END
}
