using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class Menu : MonoBehaviour {

	[Header("Menu Settings")]
	public bool active;																		// Responsible variable if the button is active or not

	public int maxFontSize = 22;															// Maximum font size
	public int minFontSize = 18;															// Minimum  font size

	public Color mouseEnter;																// MenuItem color when the mouse enters
	public Color mouseExit;																	// MenuItem color when the mouse exits
	public Color mousePressed;																// MenuItem color when the mouse pressed / click
	public Color deactivatedColor;															// MenuItem color when disabled

	[Header("Line Settings")]
	public bool enableLine;																	// Enable underline
	public bool enableLineEffect;															// Enable effect when mouse enters
	public float widthLine=200f;															// Width underline
	public float heightLineMin=1f;															// Minimum height underline
	public float heightLineMax=2f;															// Maximum height underline

	[Header("Animation Settings")]
	public bool initAnim;																	// Initial animation
	public float timerInitAnim;																// Time for animation starts

	public float menuXStart = -233f;														// Initial X-axis
	public float menuXEnd = 115f;															// End X-axis
	public float speedAnim = 400f;															// Speed animation

	[Header("Events")]
	[SerializeField]
	private UnityEvent Enter = new UnityEvent ();

	[SerializeField]
	private UnityEvent Exit = new UnityEvent ();

	[SerializeField]
	private UnityEvent Click = new UnityEvent ();

	// Variables that the user does not need to change
	private MenuControl _menuc;															// Menu Control Component
	private Image _effectSelected;														// Underline Component
	private Text _text;																	// Text Component
	private Vector3 _initPos;															// Initial position
	private RectTransform _rect;														// RectTransform component


	// Use this for initialization
	void Start () {
		getComponents ();
		basicSettings ();
			
	}// END
	
	// Update is called once per frame
	void Update () {
		if (initAnim == true) {
			updateAnimation ();
		}
	}// END

	//-----------------------------------------------------------------------------START METHODS MENUITEM--------------------------------------------------------------------\\
	// Get the components
	void getComponents(){
		_rect = this.GetComponent<RectTransform> ();										// Get the RectTransform component of this object
		_menuc = FindObjectOfType<MenuControl> ();											// Get the Control Menu (there should only be one)
		_text = this.GetComponent<Text> ();													// Get the Text component of children
		_effectSelected = this.GetComponentInChildren<Image> ();							// Get the Image component of children
	}// END

	// Basic and necessary settings
	void basicSettings(){
		_initPos = _rect.localPosition;														// Get initial position
		if (initAnim == true) {																// If the initial animation is true
			_rect.localPosition = new Vector3(menuXStart, _initPos.y, _initPos.z);			// Arrow the position of the object to the X axis of the variable "menuXStart"
		}

		//Set Default Color
		_text.color = mouseExit;
		_effectSelected.color = mouseExit;

		// If the button is not active
		if (active == false) {
			_text.color = deactivatedColor; // Set color to "deactivatedColor"
		}

		// If the underline is false it defines the effect of the underline as false too
		if (enableLine == false) {
			_effectSelected.enabled = false;
		}
			
	}// END

	// Update initial animation
	void updateAnimation(){
		// If the time to start the animation is over
		if (timerInitAnim <= 0) {
			_rect.transform.localPosition = Vector2.MoveTowards (_rect.transform.localPosition, new Vector2 (menuXEnd, _initPos.y), speedAnim * Time.deltaTime); // Starts Animation
		}
		if (timerInitAnim >= 0) {timerInitAnim -= Time.deltaTime;} // If the animation time is greater than zero (ie not started) it starts to go down 1 second
	}// END
	//-----------------------------------------------------------------------------END METHODS MENUITEM----------------------------------------------------------------------\\


	//-----------------------------------------------------------------------------START METHODS ON/OFF----------------------------------------------------------------------\\
	// Method by setting the default menuItem again (mouseExit)
	public void menuDisable(){
		if (active == true) {																			// If the button is active
			_text.color = mouseExit;																	// Sets the default color
			_effectSelected.color = mouseExit;															// Sets the default color (underline)
			if (enableLineEffect==true) {																// If the underline effect is active
				_effectSelected.rectTransform.sizeDelta = new Vector2 (widthLine, heightLineMin);		// Set a new size
			}
			_text.fontSize = minFontSize;																// Sets the default font size
		}
	}// END

	// Method by setting the active menuItem (mouseEnter)
	public void menuEnable(){
		if (active == true) {																			// If the button is active
			_text.color = mouseEnter;																	// Sets new color (mouseEnter)
			_effectSelected.color = mouseEnter;															// Sets new color for underline (mouseEnter)
			if (enableLineEffect==true) {																// If the underline effect is active
				_effectSelected.rectTransform.sizeDelta = new Vector2 (widthLine, heightLineMax);		// Set a new size
			}
			_text.fontSize = maxFontSize;																// Sets font size to max font size
		}
	}// END

	// Set underline is active or no
	public void setMenuLine(bool value){
		enableLine = value;																				// Set value
		_effectSelected.gameObject.SetActive (value);													// Set value
	}// END

	// Activate an object and mask it
	public void enableObject(GameObject obj){
		obj.SetActive (true);																			// Active the object
        if (_menuc.inGame == false)
        {
            _menuc.mask.SetActive(true);                                                                // Active the mask
            _menuc.setAlphaMask(0.5f);                                                                  // Set alpha of mask to 0.5f
        }
	}// END
	//-----------------------------------------------------------------------------END METHODS ON/OFF------------------------------------------------------------------------\\

	public void CallTheEvent(int index){
		if (index == 0) {
			Enter.Invoke ();
		} else if (index == 1) {
			Exit.Invoke ();
		} else if (index == 2) {
			Click.Invoke ();
		}
	}// END

	public void SetText(string newText){
		this.GetComponent<Text> ().text = newText;
	}

	public void showMessageInConsole(string s){
		Debug.Log (s);
	}
}
