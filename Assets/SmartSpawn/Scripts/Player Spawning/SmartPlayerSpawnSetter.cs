using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This *EXAMPLE* script allows the player to set their spawn point in-game.
/// 
/// 
/// </summary>
/// 

namespace SmartSpawn {

	public class SmartPlayerSpawnSetter : MonoBehaviour {


		/// <summary>
		/// The linked player transform to save position and rotation of spawnpoint
		/// </summary>
		public Transform linkedPlayerTransform;

		/// <summary>
		/// The hotkey for saving spawnpoint
		/// </summary>
		[Tooltip("The hotkey for saving spawnpoint")]
		public KeyCode saveSpawnPointKeycode;

		//The smart player spawner - there should only be one of these per scene
		private SmartPlayerSpawner smartPlayerSpawner;

		//The unique identifier for this player
		private System.Guid thisPlayerGuid;

		//The playerprefs string used to save the player GUID
		private static string PLAYER_GUID_STRING = "PlayerGUID";
		
		//Do we want to spawn a prefab (eg. sleeping bag) when setting our spawnpoint
		[Tooltip("Do we want to spawn a prefab (eg. sleeping bag) when setting our spawnpoint")]
		public bool spawnPrefabAtPlayerSpawnPoint;

		/// <summary>
		/// The prefab to spawn at the player spawn point.
		/// </summary>
		[Tooltip("The prefab to spawn at the player spawn point (eg. a sleeping bag).")]
		public GameObject prefabToSpawnAtPlayerSpawnPoint;

		//Test/demo variable.
		[Tooltip("Example variable, if this is false, then the player does not have enough resources and cannot set a spawnpoint")]
		public bool hasResourcesToSetSpawnPoint = true;

		//Should we destroy our old gameobjects/prefabs that indicated spawnpoints when we spawn
		[Tooltip("Should we destroy our old gameobjects/prefabs that indicated spawnpoints when we set a new spawnpoint")]
		public bool destroyOldSpawnPoints = true;
			
		/// <summary>
		/// Do we want to use the player transforms position as their new spawn point? If not,
		/// we use a raycast to set it
		/// </summary>
		[Tooltip("Do we want to use the player transforms position as their new spawn point? If not, we use a raycast")]
		public bool useLinkedPlayerPosition = true;

		//Do we want to use the raycast to check whether we can set our spawnpoint?
		public bool useRaycastToCheck = true;
			
		//The players camera, used for raycasting
		[Tooltip("The players camera, used for raycasting to find a position to set the players spawnpoint")]
		public Transform cameraTransform;


		private List<GameObject> oldSpawnPointList = new List<GameObject>();

		void Awake()
		{
			GetOrSetPlayerId();
		}

		void Start()
		{
			//Feel free to change this out to your preferred way of referencing game controllers.
			smartPlayerSpawner = (SmartPlayerSpawner)SmartPlayerSpawner.FindObjectOfType(typeof(SmartPlayerSpawner)) as SmartPlayerSpawner;

			if(smartPlayerSpawner == null)
			{
				Debug.LogError("Warning: No smart player spawner found in scene");
			}

			if(!linkedPlayerTransform)
			{
				linkedPlayerTransform = transform;
			}
		}

		void GetOrSetPlayerId()
		{
			//If we don't have a player GUID for this local computer, make one and save it, if we do, get it.
			if(!PlayerPrefs.HasKey(PLAYER_GUID_STRING))
			{
				thisPlayerGuid = System.Guid.NewGuid();
				PlayerPrefs.SetString(PLAYER_GUID_STRING, thisPlayerGuid.ToString());
				PlayerPrefs.Save();
			} else {
				thisPlayerGuid = new System.Guid(PlayerPrefs.GetString(PLAYER_GUID_STRING));
			}
		}

		//Very basic example of checking if the player can set a spawn point
		bool CanSetPlayerSpawnPoint()
		{
			return hasResourcesToSetSpawnPoint;
		}

		//Sets the player spawn point.
		void SetPlayerSpawnPoint(Vector3 spawnPosition)
		{
			smartPlayerSpawner.AddPlayerSpawnPoint(thisPlayerGuid, spawnPosition);

			if(spawnPrefabAtPlayerSpawnPoint)
			{
				if(destroyOldSpawnPoints)
				{
					foreach(GameObject go in oldSpawnPointList)
					{
						Destroy(go);
					}
				}

				GameObject newSpawnPointPrefab = GameObject.Instantiate(prefabToSpawnAtPlayerSpawnPoint, spawnPosition, Quaternion.identity) as GameObject;
				oldSpawnPointList.Add(newSpawnPointPrefab);
			}

			#if UNITY_EDITOR
			Debug.Log("Set player spawn point");
			#endif
		}
		
		// Checks if the player attempts to save their spawnpoint via key press,
		//then saves it. If we want to use a camera raycast (eg: save spawnpoint on a physics surface in the direction the camera is facing)
		//then we fire a ray, if hit save the spawnpoint there. Otherwise we set the spawnpoint at the linked player transforms position
		void Update () {

			#if UNITY_EDITOR
			Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 5f, Color.green); //Not required, but useful to check that the ray is correct. You can't provide the ray as an object, so do it this way. 
			#endif

			if(Input.GetKeyDown(saveSpawnPointKeycode))
			{
				if(CanSetPlayerSpawnPoint())
				{
					if(!useRaycastToCheck)
					{
						SetPlayerSpawnPoint(linkedPlayerTransform.position);
						return;
					}

					RaycastHit hitInfo; //Our "out" object for information regarding our raycast
					
					Ray cameraRay = new Ray(cameraTransform.position, cameraTransform.forward); //Create a ray object at the camera's position, in the direction that the camera is currently facing

					bool hitSomething = Physics.Raycast(cameraRay, out hitInfo, 5f); //Here is the point where the physics engine actually does the ray-cast. The parameters in order define the ray to cast, what object to store the results in, the length of the ray, and the layermasks to ignore. We will go over that last part in a bit. Physics.Raycast always returns a bool that tells you if the raycast hit anything, or if you provide a layermask, if it hit the targeted layermask(s).
					
					//If we hit something, spawn the player spawner etc
					if(hitSomething)
					{
						SetPlayerSpawnPoint(hitInfo.point);
					} else {

	#if UNITY_EDITOR
						Debug.Log("Couldn't set spawn point because the raycast hit nothing");
	#endif

					}


				}


			}

		}
	}

}


