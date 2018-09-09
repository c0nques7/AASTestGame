using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 							LootMaker 2.0, Copyright © 2017, Ripcord Development
//											TextBehaviour.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script controls the movement, rotation, and visiblity of the generated text mesh.

public class TextBehaviour : MonoBehaviour {

	public Color textColour;			//The colour of the generated text
	float alpha = 1.0f;					//The alpha value for the text object

	public Transform targetTransform;	//This is the transform that the generated text will travel towards when it is created.  It uses both the position and the rotation.
	
	public float moveSpeed;				//The speed at which the text will move towards the target position.
	public float rotateSpeed;			//The speed at which the text will rotate to match the target rotation.
	
	bool fadeOut;						//If true, the object will gradually become more and more invisible until it has faded out completely.
	public float timeVisible;			//The time in seconds before the object will start to fade out, or be destroyed.
	public float fadeSpeed;				//Lower numbers will increase the time it takes for the object to fade out.  Higher numbers will decrease the fade time.

	Transform lootFXContainer;			//An empty object used to store all the loot effects objects, helps keeps the project hierarchy clean


	void Start () {
		
		gameObject.GetComponent<Renderer>().material.color = textColour;	//

		if (!GameObject.Find("LootFXContainer")) {							//If there is no Loot FX container in the scene...
			lootFXContainer = new GameObject("LootFXContainer").transform;	//...create a new game object called "LootFXContainer"
		}
		lootFXContainer = GameObject.Find("LootFXContainer").transform;		//Create a reference to the Loot FX Container object

		targetTransform.parent = lootFXContainer;							//Make the target transform a child of the Loot FX Container so that it no longer moves with the parent object
	}
	

	void Update () {

		if (timeVisible > 0.0f) {											//If the visibility timer is greater than zero...
			timeVisible -= Time.deltaTime;									//...decrease the timer...
		}
		else {																//Otherwise...
			fadeOut = true;													//...start fading out the object
		}

		transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * moveSpeed);			//
		transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * rotateSpeed);		//
		
		if (fadeOut) {																											//If the object is set to fade out...
			alpha -= Time.deltaTime * fadeSpeed;																				//...decrease the alpha value
			gameObject.GetComponent<Renderer>().material.color = new Color(textColour.r, textColour.g, textColour.b, alpha);	//...set the new alpha value for the material
		}

		if (alpha <= 0.0f) {												//Once the object is no longer visible...
			Destroy(targetTransform.gameObject);							//...destroy the transform target...
			Destroy(gameObject);											//...then destroy this object
		}
	}
}
