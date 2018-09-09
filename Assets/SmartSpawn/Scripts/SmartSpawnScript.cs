using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is used to spawn Objects in random spawn positions.
/// 
/// It accesses the SmartSpawnColliderCheck script to see whether 
/// there is already an item in that position.
/// 
/// Written by Alex Blaikie of Gadget Games 
/// Have a question? Send it over to support@gadget-games.com
/// Otherwise, feel free to rate this asset on the store!
/// </summary>

namespace SmartSpawn {
	
	
	public class SmartSpawnScript : MonoBehaviour {
		
		//Variables start ==========================================
		
		public delegate void SpawnAction();
		/// <summary>
		/// Occurs when an item is spawned.
		/// </summary>
		public static event SpawnAction OnSpawn;

		public delegate void WaveResetAction(SmartSpawnScript spawnerWithWaveReset, int wave);
		/// <summary>
		/// Occurs when a new wave is triggered.
		/// </summary>
		public static event WaveResetAction OnWaveReset;
		
		/// <summary>
		/// Should we check if the SmartSpawnColliderCheck script has spawns left
		/// (maxSpawnsLeft) before spawning?
		/// </summary>
		public bool useMaxPointSpawns;
		
		/// <summary>
		/// The maximum amount of spawns that this spawner can spawn, if useMaxSpawns is true
		/// </summary>
		public int maxSpawnerSpawns;
		
		/// <summary>
		/// Should we check if this spawner is under the maximum spawn amount 
		/// <param name="maxSpawnerSpawns">Max spawn amount</param> before spawning?
		/// </summary>
		public bool useMaxSpawns;
		
		/// <summary>
		/// If this is enabled, the SmartSpawner will check to see if the object it is spawning has
		/// a SmartSpawnEnemyTracker script on it, and if so, will 'track' the object, and when the object
		/// is destroyed, it will increment the Max Spawns on this spawner. Set this to true if you want to 
		/// only have 'x' max enemies spawned at once, but continue spawning enemies as they die
		/// </summary>
		public bool trackSpawnedObjects;
		
		//The array of potential spawning locations
		public Transform[] spawnPoints;
		private SmartSpawnColliderCheck[] spawnChecks;
		
		//The final spawn location
		private Transform spawnLocation;
		
		/// <summary>
		/// The scriptable spawn asset that the spawner uses to reference objects & their chances
		/// </summary>
		public SmartSpawnScriptableObject spawnObject;

		//These next two variables are taken from the spawnObject at Awake(), 
		//that's the only time we ref the spawnobject at runtime

		//The item we will spawn
		public List<Object> /*Object[]*/ itemPrefabs;
		//The spawn chance of the item.
		public List<int> /*int[]*/ spawnChances;
		
		//Spawn countdown timer
		public float timer = 0.0f;
		public float waveTimer = 0.0f;
		
		//The time between each spawn
		public float spawnCountdown;
		
		/// <summary>
		/// This determines whether we will fill up all the open spawnpoints with items first.
		/// Without this checked, a spawn is not guaranteed
		/// </summary>
		public bool useOpenSpawnPoints;

		/// <summary>
		/// Guarantees a spawn and decrements the number of that item we can spawn in each wave
		/// </summary>
		public bool useWaveSpawning = false;

		int currentWave = 1;

		/// <summary>
		/// Do we want to reset our wave values back to original values after the WaveResetTime?
		/// </summary>
		public bool resetWaveValuesAfterTime = false;

		/// <summary>
		/// After this amount of time, the SmartSpawnScriptableObject used to spawn the wave will be reset
		/// </summary>
		public float waveResetTime = 30f;
		
		//The summed up spawn ratios of the items
		private int totalRatio;
		
		//Quick reference to the ItemSpawnColliderCheck script
		private SmartSpawnColliderCheck spawnCheckScript;
		
		//For true randoming
		List<int> trueRandomSpawnPointsIndexList = new List<int>();

		public bool useRandomSpawnPointsWhenFillingEmpty = false;

		/// <summary>
		/// Determines whether we can be influenced by the SmartSpawnInfluenceMap
		/// </summary>
		public bool allowInfluence = true;
		//Variables end=============================================
		
		// Use this for initialization
		public void GetSpawnPoints () {
			
			//Fill the spawn points array with all possible spawn points
			List<Transform> sP = new List<Transform>();
			
			foreach(Transform t in transform)
			{
				sP.Add(t);
			}
			
			spawnPoints = sP.ToArray();
			
			spawnChecks = GetComponentsInChildren<SmartSpawnColliderCheck>();
			
			if(spawnChecks.Length != spawnPoints.Length)
			{
				#if UNITY_EDITOR
				Debug.LogWarning("Ignore this message if clicking on a SmartSpawner in the project hierarchy");
				#endif
				Debug.LogError("Warning - make sure all child gameobjects have one SmartSpawnColliderCheck script on them - " +
				               spawnChecks.Length + " SmartSpawnColliderChecks found, " + spawnPoints.Length + " SpawnPoints (first-level child gameobjects) found");
			}
		}

		void Awake()
		{
			ResetItemLists();
		}

		/// <summary>
		/// Resets the item lists to the initial state (before any influence was applied/at start)
		/// </summary>
		public void ResetItemLists()
		{
			SetItemLists(spawnObject);
		}

		/// <summary>
		/// Setups the item lists. Called from the influence map
		/// </summary>
		public void SetItemLists(SmartSpawnScriptableObject so)
		{
			itemPrefabs.Clear();
			spawnChances.Clear();

			itemPrefabs.AddRange(so.itemPrefab);
			spawnChances.AddRange(so.spawnChance);
		}

		/// <summary>
		/// Influences the item lists. Called from the SmartSpawnInfluenceMap.
		/// </summary>
		public void InfluenceItemLists(SmartSpawnScriptableObject so)
		{
			//Loop through all the stuff in the thing that's trying to influence us
			//If we have the same item, then change its chance, if we don't, then 
			//add it
			for(int i = 0; i < so.itemPrefab.Length; i++)
			{
				if(itemPrefabs.Contains(so.itemPrefab[i]))
				{
					//Debug.Log("Item " + itemPrefabs[itemPrefabs.IndexOf(so.itemPrefab[i])].name + " spawn chance was: " + spawnChances[itemPrefabs.IndexOf(so.itemPrefab[i])]);
					spawnChances[itemPrefabs.IndexOf(so.itemPrefab[i])] += so.spawnChance[i];
					//Debug.Log("Item " + itemPrefabs[itemPrefabs.IndexOf(so.itemPrefab[i])].name + " spawn chance changed to: " + spawnChances[itemPrefabs.IndexOf(so.itemPrefab[i])]);
				} else {
					itemPrefabs.Add(so.itemPrefab[i]);
					spawnChances.Add(so.spawnChance[i]);
					//Debug.Log("Added " + so.itemPrefab[i].name);
				}
			}
		}
		
		void Start()
		{
			GetReferences();

			GetSpawnPoints();
		}
		
		/// <summary>
		/// Called on Start()
		/// </summary>
		protected virtual void GetReferences()
		{
			
		}
		
		// Update is called once per frame
		void Update () {
			
			if(maxSpawnerSpawns <= 0 && useMaxSpawns == true) return;
			
			timer += Time.deltaTime;
			
			//When timer reaches 100, call Spawn function
			if(timer >= spawnCountdown)
			{
				Spawn();
			}			
			
			if(useWaveSpawning == true && resetWaveValuesAfterTime == true)
			{				
				waveTimer += Time.deltaTime;

				if(waveTimer > waveResetTime)
				{
					currentWave ++;
					waveTimer = 0f;

					if(OnWaveReset != null)
					{
						OnWaveReset(this, currentWave);
						#if UNITY_EDITOR
						//Debug.Log("OnWaveReset called from " + transform.name);
						#endif
					} else {
						//We only reset item lists if we don't have influence maps listening to us
						ResetItemLists();
					}
						
				}
			}
		}
		
		/// <summary>
		/// Goes through randomisation logic, and then calls <code>SpawnItem(Vector3 spawnLocation, int index)</code>
		/// </summary>
		public void Spawn() {
			
			if(useMaxSpawns == true && maxSpawnerSpawns <= 0) return;
			
			//Reset timer to 0 so process can start over
			timer = 0;
			
			//Select a random number, make sure it is a whole number with Absolute
			int randomPick = Random.Range (0,spawnChecks.Length);
			
			//Check to see whether we want to fill up empty spawn points with each spawn
			if(useOpenSpawnPoints == true){
				
				if(useRandomSpawnPointsWhenFillingEmpty)
				{
					trueRandomSpawnPointsIndexList.Clear();
					for(int i = 0; i < spawnPoints.Length; ++i)
					{
						trueRandomSpawnPointsIndexList.Add(i);
					}
					trueRandomSpawnPointsIndexList = Randomize<int>(trueRandomSpawnPointsIndexList);
					
					for(int i = 0; i < trueRandomSpawnPointsIndexList.Count; ++i)
					{
						int realRandIndex = trueRandomSpawnPointsIndexList[i];
						
						SmartSpawnColliderCheck testSpawnCheck = spawnChecks[realRandIndex];
						//Check if we can spawn on the point (is it empty)
						if(testSpawnCheck.canWeSpawn){
							spawnCheckScript = testSpawnCheck;
							randomPick = realRandIndex;
							i = trueRandomSpawnPointsIndexList.Count;
						}
					}
				} else {
					for(int i = 0; i < spawnPoints.Length; ++i)
					{
						SmartSpawnColliderCheck testSpawnCheck = spawnChecks[i];
						//Check if we can spawn on the point (is it empty)
						if(testSpawnCheck.canWeSpawn){
							spawnCheckScript = testSpawnCheck;
							randomPick = i;
						}
					}
				}
				
			} else {
				spawnCheckScript = spawnChecks[randomPick];	//If we don't want to use only open spawn points, pick a random spawn point
			}
			
			//Check whether there is already an item in the position we want to spawn
			//in, if it is clear, spawn a new item.
			if(spawnCheckScript.canWeSpawn == true)
			{	
				if(useMaxPointSpawns == false || spawnCheckScript.maxSpawnsLeft > 0)
				{
					spawnLocation = spawnPoints[randomPick];

					//So, if we are using guaranteed wave spawning, we just spawn
					//one random item, then decrease its value in the scriptableobject, then return
					if(useWaveSpawning)
					{
						for(int w = 0; w < spawnChances.Count; ++w)
						{
							if(spawnChances[w] > 0)
							{
								SpawnItem(spawnLocation.position, w);
								spawnChances[w] --;
								if(OnSpawn != null)
									OnSpawn();

								return;
							}
						}

						return;
					}
					
					for(int n = 0; n < spawnChances.Count; ++n)
					{
						totalRatio += spawnChances[n];
					}
					
					//Assign a "random chance" integer to the item we are trying to spawn
					int randomChance = Mathf.Abs (Random.Range (1, totalRatio));
					
					for(int n = 0; n < spawnChances.Count+1; ++n)
					{
						//Check the spawn ratio of the item we want to spawn
						int itemIndex = CheckItemIsBetween(n, randomChance);
						
						//If we get the correct ratio, spawn the item
						if(itemIndex != -1){
							SpawnItem(spawnLocation.position, itemIndex);
							if(OnSpawn != null)
								OnSpawn();
							//Reset totalRatio
							totalRatio = 0;
							return;
						}
					}
				} else {
					Debug.Log("Not spawning in location " + spawnCheckScript.name + ", it has no spawns left");
				}
			} else {
				if(useOpenSpawnPoints)
					Debug.Log("Not spawning - an object with the tag " + spawnCheckScript.itemTag + " is already in " + spawnCheckScript.name + "'s radius");
			}
			
			//Reset totalRatio
			totalRatio = 0;
			
		}
		
		//This function handles the spawn ratios of the items
		int CheckItemIsBetween(int n, int middle)
		{
			int lower = 0;
			int upper = 0;
			
			//Loop through, checking the items spawn ratio
			for(int i = 0; i < n; ++i)
			{
				upper += spawnChances[i];
				
				if(lower < middle && middle < upper)
				{
					//After checking the ratios, return the correct item to spawn
					return i;
				}
				
				lower += spawnChances[i];  
			}
			
			//Otherwise, return -1
			return -1;
			
		}
		
		/// <summary>
		/// Spawns the item.
		/// </summary>
		/// <param name="spawnLocation">Spawn location.</param>
		/// <param name="index">Index of item in object array.</param>
		protected virtual void SpawnItem(Vector3 spawnLocation, int index) {
			
			//Create the item at the point of the location variable after casting it
			//and make sure it is not null
			GameObject _gameObject = itemPrefabs[index] as GameObject;
			
			if(_gameObject != null){
				
				GameObject newItem = Instantiate(_gameObject, spawnLocation, _gameObject.transform.rotation) as GameObject;
				
				if(trackSpawnedObjects)
				{
					SmartSpawnEnemyTracker _enemy = newItem.GetComponent<SmartSpawnEnemyTracker>();
					if(_enemy != null)//This means it is an enemy that we want to track
					{
						_enemy.spawner = this;
						_enemy.incrementSpawnsOnDeath = true;
					} else {
						Debug.LogWarning("No SmartSpawnEnemyTracker found on " + newItem.name + " - can't track");
					}
				}
				
				//Output to debug
				//Debug.Log("Spawning " + _gameObject.name.ToString());
			}
			
			if(_gameObject == null){
				Debug.Log("The object you are trying to spawn has not been set.");
			}
			
			maxSpawnerSpawns --;//Decrease the max spawns this spawner can do
		}
		
		public void IncreaseSpawnerSpawns()
		{
			maxSpawnerSpawns ++;
		}
		
		//Randomises a list
		public static List<T> Randomize<T>(List<T> list)
		{
			List<T> randomizedList = new List<T>();
			System.Random rnd = new System.Random();
			while (list.Count > 0)
			{
				int index = rnd.Next(0, list.Count); //pick a random item from the master list
				randomizedList.Add(list[index]); //place it at the end of the randomized list
				list.RemoveAt(index);
			}
			return randomizedList;
		}
		
	}
	
}