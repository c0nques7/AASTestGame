using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 							LootMaker 2.0, Copyright © 2017, RipCord Development
//										     LootGenerator.cs
//										    info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script will spawn randomly selected loot objects on the given mount points

public class LootGenerator : MonoBehaviour {

	public Transform mountPoints;						//The parent object that contains all the mount points the new loot objects will spawn at

	List<GameObject> availableLoot;						//A list of all loot objects able to be generated based on the active toggles

	[System.Serializable]
	public class LootList {
		public string name;								//The name of the loot type, used to make things easier to find in the inspector
		public List<GameObject> lootPrefabs;			//The loot prefab objects that belong to the loot type
	}
	public List<LootList> lootTypes;


	void Start () {

		availableLoot = new List<GameObject>();
	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.G) ){
			GenerateLoot();
		}
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// GENERATE LOOT OBJECT - Rebuild the list of available loot objects then spawn a new loot object on each available mount point
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	public void GenerateLoot () {

		availableLoot.Clear();																					//Clear the existing list of available loot objects

		for (int x = 0; x < lootTypes.Count; x++) {																//Cycle through all the loot types...
			for (int y = 0; y < lootTypes[x].lootPrefabs.Count; y++) {											//...cycle through all the prefabs for that type...
				availableLoot.Add(lootTypes[x].lootPrefabs[y]);													//......and add them to the list of available loot objects
			}
		}

		if (availableLoot.Count > 0) {																			//If there are any available loot objects..

			if (mountPoints.childCount > 0) {																	//...if the mountPoints target has any child objects...
				foreach (Transform mountPoint in mountPoints) {													//......cycle through each mount point in the scene...
					SpawnLootObject(mountPoint);																//.........and spawn a loot object at its location
				}
			}
			else {																								//...otherwise...
				SpawnLootObject(mountPoints);																	//......spawn a loot object at the mountPoints target
			}
		}
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// SPAWN LOOT OBJECT - Randomly select and spawn a loot object at the given mount point
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	public void SpawnLootObject (Transform mountPoint) {

		GameObject selectedLoot = availableLoot[Random.Range(0, availableLoot.Count)];							//Select a random loot object from the available list
		GameObject newLoot = (GameObject)Instantiate(selectedLoot, mountPoint.position, mountPoint.rotation);	//Generate the new loot object at the mount point
		newLoot.transform.parent = this.transform;																//Make the new loot object a child of this object
	}
}