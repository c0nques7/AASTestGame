using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Smart player spawner. 
/// 
/// This stores a dictionary of all player ids, and their spawnpoints in a scene, and when called, spawns the player at their spawnpoint
/// </summary>
/// 

namespace SmartSpawn {

	public class SmartPlayerSpawner : MonoBehaviour
	{

		
		//Variables ===============================================================================

		/// <summary>
		/// Stores all player IDs and their corresponding spawnpoints in a scene
		/// </summary>
		private Dictionary<System.Guid, SmartPlayerSpawnPoint> playerSpawnDictionary = new Dictionary<System.Guid, SmartPlayerSpawnPoint>();

			
			[Header("NOTE: You should only have 1 SmartPlayerSpawner per scene")]
			[Header("Stores player spawnpoints & spawns players when requested.")]
		/// <summary>
		/// The player prefab to spawn.
		/// </summary> 
		public GameObject playerPrefabToSpawn;

		public struct SmartPlayerSpawnPoint {
			
			/// <summary>
			/// The player spawn position.
			/// </summary>
			public Vector3 playerSpawnPosition;
			
			/// <summary>
			/// The player spawn rotation. (eulerangles)
			/// </summary>
			public Vector3 playerSpawnRotation;
			
			public SmartPlayerSpawnPoint(Vector3 pos, Vector3 rot)
			{
				this.playerSpawnPosition = pos;
				this.playerSpawnRotation = rot;
			}
			
		}

		//Prefs
		private readonly string PREFS_ID_STRING = "PLAYER_GUID_";

		private readonly string PREFS_XPOS_STRING = "SPAWNPOS_X_";
		private readonly string PREFS_YPOS_STRING = "SPAWNPOS_Y_";
		private readonly string PREFS_ZPOS_STRING = "SPAWNPOS_Z_";

		private readonly string PREFS_XROT_STRING = "SPAWNROT_X_";
		private readonly string PREFS_YROT_STRING = "SPAWNROT_Y_";
		private readonly string PREFS_ZROT_STRING = "SPAWNROT_Z_";

		private readonly string NUM_SPAWNPOINTS_STRING = "NUM_SPAWNPOINTS";

		[Tooltip("Do we want to spawn a player if there are no saved spawnpoints")]
		public bool spawnPlayerIfNoSpawnPointsFound = true;

		[Tooltip("The default player spawn point")]
		public Transform defaultPlayerSpawnPoint;

		//End variables============================================================================

		void Awake()
		{
			GetSpawnPointsFromPrefs();
		}

		//On start
		void Start()
		{
			if(spawnPlayerIfNoSpawnPointsFound)
			{
				CheckIfNoPlayerSpawnpoints();
			}
		}

		//, if we have no saved spawnpoints, spawn the player at the target position
		void CheckIfNoPlayerSpawnpoints()
		{
			if(playerSpawnDictionary.Count == 0)
			{
				SpawnPlayer(defaultPlayerSpawnPoint);
			}
		}

		/// <summary>
		/// Adds the player spawn point, using the position and rotation of the given transform as the spawnpoint position/rotation
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="targetSpawnPoint">Target spawn point transform.</param>
		public void AddPlayerSpawnPoint(System.Guid playerId, Transform targetSpawnPoint)
		{
			//If the dictionary doesn't have the key, we need to add it
			if(!playerSpawnDictionary.ContainsKey(playerId))
		 	{
				playerSpawnDictionary.Add(playerId, new SmartPlayerSpawnPoint(targetSpawnPoint.position, targetSpawnPoint.rotation.eulerAngles));
			} else {
				SmartPlayerSpawnPoint newPlayerSpawnPoint = playerSpawnDictionary[playerId];
				newPlayerSpawnPoint.playerSpawnPosition = targetSpawnPoint.position;
				newPlayerSpawnPoint.playerSpawnRotation = targetSpawnPoint.rotation.eulerAngles;
				playerSpawnDictionary[playerId] = newPlayerSpawnPoint;
			}
		}

		/// <summary>
		/// Adds the player spawn point, using the given spawn position & rotation
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="spawnPosition">Spawn position.</param>
		/// <param name="spawnRotation">Spawn rotation.</param>
		public void AddPlayerSpawnPoint(System.Guid playerId, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			//If the dictionary doesn't have the key, we need to add it
			if(!playerSpawnDictionary.ContainsKey(playerId))
			{
				playerSpawnDictionary.Add(playerId, new SmartPlayerSpawnPoint(spawnPosition, spawnRotation.eulerAngles));
			} else {
				SmartPlayerSpawnPoint newPlayerSpawnPoint = playerSpawnDictionary[playerId];
				newPlayerSpawnPoint.playerSpawnPosition = spawnPosition;
				newPlayerSpawnPoint.playerSpawnRotation = spawnRotation.eulerAngles;
				playerSpawnDictionary[playerId] = newPlayerSpawnPoint;
			}
		}

		/// <summary>
		/// Adds the player spawn point, using the given spawn position & no rotation
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="spawnPosition">Spawn position.</param>
		/// <param name="spawnRotation">Spawn rotation.</param>
		public void AddPlayerSpawnPoint(System.Guid playerId, Vector3 spawnPosition)
		{
			//If the dictionary doesn't have the key, we need to add it
			if(!playerSpawnDictionary.ContainsKey(playerId))
			{
				playerSpawnDictionary.Add(playerId, new SmartPlayerSpawnPoint(spawnPosition, Vector3.zero));
			} else {
				SmartPlayerSpawnPoint newPlayerSpawnPoint = playerSpawnDictionary[playerId];
				newPlayerSpawnPoint.playerSpawnPosition = spawnPosition;
				newPlayerSpawnPoint.playerSpawnRotation = Vector3.zero;
				playerSpawnDictionary[playerId] = newPlayerSpawnPoint;
			}
		}

		/// <summary>
		/// Adds the player spawn point, using the given spawn position & rotation
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="spawnPosition">Spawn position.</param>
		/// <param name="spawnRotation">Spawn rotation.</param>
		public void AddPlayerSpawnPoint(System.Guid playerId, float spawnPositionX, float spawnPositionY, float spawnPositionZ, 
		                                float spawnRotationX, float spawnRotationY, float spawnRotationZ)
		{
			//If the dictionary doesn't have the key, we need to add it
			Vector3 spawnPosition = new Vector3(spawnPositionX, spawnPositionY, spawnPositionZ);
			Quaternion spawnRotation = Quaternion.Euler(new Vector3(spawnRotationX, spawnRotationY, spawnRotationZ));
			if(!playerSpawnDictionary.ContainsKey(playerId))
			{
				playerSpawnDictionary.Add(playerId, new SmartPlayerSpawnPoint(spawnPosition, spawnRotation.eulerAngles));
			} else {
				SmartPlayerSpawnPoint newPlayerSpawnPoint = playerSpawnDictionary[playerId];
				newPlayerSpawnPoint.playerSpawnPosition = spawnPosition;
				newPlayerSpawnPoint.playerSpawnRotation = spawnRotation.eulerAngles;
				playerSpawnDictionary[playerId] = newPlayerSpawnPoint;
			}
		}

		/// <summary>
		/// Checks whether we have a player spawn point saved for the given guid
		/// </summary>
		/// <returns><c>true</c> if this instance has player spawn point the specified playerGuid; otherwise, <c>false</c>.</returns>
		/// <param name="playerGuid">Player GUID.</param>
		public bool HasPlayerSpawnPoint(System.Guid playerGuid)
		{
			return playerSpawnDictionary.ContainsKey(playerGuid);
		}

		/// <summary>
		/// Clears all player spawn points.
		/// </summary>
		[ContextMenu("Clear all player spawnpoints in active scene")]
		public void ClearAllPlayerSpawnPoints()
		{
			if(!Application.isPlaying)
			{
				Debug.LogWarning("Game is not playing, can't clear live spawnpoints - clear points from prefs instead");
				return;
			}

			playerSpawnDictionary.Clear();
		}

		/// <summary>
		/// Clears all player spawn points from player prefs (local files)
		/// </summary>
		[ContextMenu("Clear all player spawnpoints from prefs")]
		public void ClearAllPlayerSpawnPointsFromPrefs()
		{
			#if UNITY_EDITOR
			var prevSpawnPoints = PlayerPrefs.GetInt(NUM_SPAWNPOINTS_STRING);
			Debug.Log("Cleared " + prevSpawnPoints.ToString() + " spawn points from prefs");
			#endif

			PlayerPrefs.SetInt(NUM_SPAWNPOINTS_STRING, 0);
			PlayerPrefs.Save();


		}

		/// <summary>
		/// Removes the player spawn point.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		public void RemovePlayerSpawnPoint(System.Guid playerId)
		{
			playerSpawnDictionary.Remove(playerId);
		}

		/// <summary>
		/// Spawns the player.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		public void SpawnPlayer(System.Guid playerId)
		{
			if(HasPlayerSpawnPoint(playerId))
			{
				//Spawn the player prefab
				SmartPlayerSpawnPoint playerSpawnInfo = playerSpawnDictionary[playerId];

				Vector3 spawnPosition = playerSpawnDictionary[playerId].playerSpawnPosition;

				Quaternion spawnRotation = Quaternion.Euler(playerSpawnInfo.playerSpawnRotation);

				GameObject.Instantiate(playerPrefabToSpawn, spawnPosition, spawnRotation);

			} else {
				Debug.LogWarning("No player spawn point found for guid: " + playerId.ToString());
			}
		}

		/// <summary>
		/// Spawns the player at the target transform
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		public void SpawnPlayer(Transform targetTransform)
		{				
			GameObject.Instantiate(playerPrefabToSpawn, targetTransform.position, targetTransform.rotation);
		}

		void OnDestroy()
		{
			SaveSpawnPointsToPrefs();
		}

		//Save all spawn points to player prefs
		public void SaveSpawnPointsToPrefs()
		{
			PlayerPrefs.SetInt(NUM_SPAWNPOINTS_STRING, playerSpawnDictionary.Count);

			for(int i = 0; i < playerSpawnDictionary.Count; i++)
			{
				Debug.Log(i);
				string idPrefString = PREFS_ID_STRING + i.ToString();

				string xPosPrefString = PREFS_XPOS_STRING + i.ToString();
				string yPosPrefString = PREFS_YPOS_STRING + i.ToString();
				string zPosPrefString = PREFS_ZPOS_STRING + i.ToString();

				string xRotPrefString = PREFS_XROT_STRING + i.ToString();
				string yRotPrefString = PREFS_YROT_STRING + i.ToString();
				string zRotPrefString = PREFS_ZROT_STRING + i.ToString();

				Debug.Log(idPrefString);
				PlayerPrefs.SetString(idPrefString, playerSpawnDictionary.Keys.ElementAt(i).ToString());

				PlayerPrefs.SetFloat(xPosPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnPosition.x);
				PlayerPrefs.SetFloat(yPosPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnPosition.y);
				PlayerPrefs.SetFloat(zPosPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnPosition.z);

				PlayerPrefs.SetFloat(xRotPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnRotation.x);
				PlayerPrefs.SetFloat(yRotPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnRotation.y);
				PlayerPrefs.SetFloat(zRotPrefString, playerSpawnDictionary.Values.ElementAt(i).playerSpawnRotation.z);
			}

			PlayerPrefs.Save();
		}

		//Get all spawn points from player prefs
		void GetSpawnPointsFromPrefs()
		{
			int count = PlayerPrefs.GetInt(NUM_SPAWNPOINTS_STRING);

			if(count == 0) 
			{
				Debug.LogWarning("No saved spawnpoints found");
				return;
			}

			for(int i = 0; i < count; i++)
			{
				Debug.Log(i);

				System.Guid savedGuid = new System.Guid(PlayerPrefs.GetString((PREFS_ID_STRING + i.ToString())));

				float xPos = PlayerPrefs.GetFloat((PREFS_XPOS_STRING + i.ToString()));
				float yPos = PlayerPrefs.GetFloat((PREFS_YPOS_STRING + i.ToString()));
				float zPos = PlayerPrefs.GetFloat((PREFS_ZPOS_STRING + i.ToString()));

				float xRot = PlayerPrefs.GetFloat((PREFS_XROT_STRING + i.ToString()));
				float yRot = PlayerPrefs.GetFloat((PREFS_YROT_STRING + i.ToString()));
				float zRot = PlayerPrefs.GetFloat((PREFS_ZROT_STRING + i.ToString()));

				AddPlayerSpawnPoint(savedGuid, xPos, yPos, zPos, xRot, yRot, zRot);

				SpawnPlayer(savedGuid);
			}
		}


		//Debugging tools
		[ContextMenu("Check amount of saved spawnpoints")]
		void AmountOfUniquePlayers()
		{
			Debug.Log("Amount of unique player spawn points in player dictionary: " + playerSpawnDictionary.Count);
		}



	}
}

