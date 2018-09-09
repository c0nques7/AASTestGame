using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 							LootMaker 2.0, Copyright © 2017, RipCord Development
//										   DEMO_LootGenerator.cs
//										    info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - For demonstration only - This script will spawn randomly selected loot objects based on the toggles that are active in the scene

public class DEMO_LootGenerator : MonoBehaviour {

	public Transform mountPoints;						//The parent object that contains all the mount points the new loot objects will spawn at

	 List<GameObject> availableLoot;					//A list of all loot objects able to be generated based on the active toggles

	[System.Serializable]
	public class LootList {
		public string name;								//The name of the loot type, used to make things easier to find in the inspector
		public Toggle lootToggle;						//The toggle object that will determine whether this loot type will be generated or not
		public List<GameObject> lootPrefabs;			//The loot prefab objects that belong to the loot type
	}
	public List<LootList> lootTypes;


	void Start () {

		availableLoot = new List<GameObject>();
	}

	void OnMouseDown () {

		GenerateLoot();
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// GENERATE LOOT OBJECT - Randomly select and spawn a loto object on each available mount point
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	public void GenerateLoot () {

		availableLoot.Clear();																							//Clear the existing list of available loot objects

		for (int x = 0; x < lootTypes.Count; x++) {																		//Cycle through all the loot types...
			if (lootTypes[x].lootToggle.isOn) {																			//...if the loot type toggle is active...
				for (int y = 0; y < lootTypes[x].lootPrefabs.Count; y++) {												//......cycle through all the prefabs for that type...
					availableLoot.Add(lootTypes[x].lootPrefabs[y]);														//.........and add them to the list of available loot objects
				}
			}
		}

		foreach (Transform child in transform) {																		//Cycle through all child objects of the LootGenerator...
			Destroy(child.gameObject);																					//...and destroy each one
		}

		if (availableLoot.Count > 0) {																					//If there are any available loot objects..
			foreach (Transform mountPoint in mountPoints) {																//...for each mount point in the scene...
				GameObject selectedLoot = availableLoot[Random.Range(0, availableLoot.Count)];							//......select a random loot object from the available list...
				GameObject newLoot = (GameObject)Instantiate(selectedLoot, mountPoint.position, transform.rotation);	//......generate the new loot object at the mount point...
				newLoot.transform.parent = this.transform;																//......and make it a child of this object
			}
		}
	}
}