using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SmartSpawn {

	/// <summary>
	/// Determines how we influence a target SmartSpawnScript.
	/// Additive = Add our modifier every tick to the targets object and chance arrays
	/// Override = Override the targets object and chance arrays
	/// Single influence = Add our modifier, but only once
	/// </summary>
	public enum InfluenceModifiers
	{
		ADDITIVE = 1,
		OVERRIDE = 2,
		SINGLE_INFLUENCE = 3,
		NONE = 4
	}

	/// <summary>
	/// Smart spawn influence map. Influences (changes) values of SmartSpawners within its range, or globally.
	/// </summary>
	public class SmartSpawnInfluenceMap : MonoBehaviour {

		/// <summary>
		/// The current influence modifier.
		/// </summary>
		[Tooltip("The current influence modifier. Additive = Add our modifier every tick to the targets object and chance arrays, " +
			"Override = Override the targets object and chance arrays, " +
			"Single influence = Add our modifier, but only once. None = Don't perform influence")]
		public InfluenceModifiers currentInfluenceModifier = InfluenceModifiers.ADDITIVE;

		/// <summary>
		/// The spawn object that we use to influence smartspawn scripts
		/// </summary>
		[Tooltip("The spawn object that we use to influence smartspawn scripts")]
		public SmartSpawnScriptableObject influenceSpawnObject;

		/// <summary>
		/// How often (in seconds) the influence map checks to see if it should Update Influence
		/// eg. 1 = updates every second
		/// </summary>
		[Tooltip("How often (in seconds) the influence map checks to see if it should Update Influence. Set to -1 for wave callbacks only")]
		public float influenceUpdateRate = 1f;

		[Space(10f)]
				
		/// <summary>
		/// If true, we will attempt to perform an influence when the SmartSpawner's "OnWaveReset" action is called
		/// </summary>
		[Tooltip("If true, we will attempt to perform an influence when the SmartSpawner's OnWaveReset action is called")]
		public bool performInfluenceOnWaveReset = false;

		/// <summary>
		/// The wave we will perform influence on. -1 = all waves
		/// </summary>
		[Tooltip("The wave we will perform influence on. -1 = all waves")]
		public int waveToPerformInfluenceOn = 1;

		//Used to count what frame we're on for influenceUpdateRate
		private float nextInfluenceUpdate = 0f;

		/// <summary>
		/// We multiply the "Time until next spawn" value by this amount on each influence tick
		/// </summary>
		[Tooltip("We multiply the Time until next  wave value by this amount on each influence tick")]
		public float waveTimeMultiplier = 1f;
		
		/// <summary>
		/// We decrease the "Time until next spawn" value by this amount on each influence tick
		/// </summary>
		[Tooltip("We decrease the Time until next  wave value by this amount on each influence tick")]
		public float waveTimeAddition = -5f;

		[Space(10f)]

		/// <summary>
		/// Do we want to start influence immediately? (on Start)
		/// </summary>
		[Tooltip("Do we want to start influence immediately? (on Start)")]
		public bool startInfluenceImmediately = true;

		/// <summary>
		/// Holds a list of all the current SmartSpawnScripts in the scene.
		/// </summary>
		[HideInInspector]
		public List<SmartSpawnScript> allSpawnScripts;

		private Dictionary<SmartSpawnScript, bool> currentlyInfluencedSpawnScripts = new Dictionary<SmartSpawnScript, bool>();

		/// <summary>
		/// The influence range in units.
		/// </summary>
		[Tooltip("The influence range in units.")]
		public float influenceRange;
		
		/// <summary>
		/// If true, we don't check for range, we just influence all smartspawners
		/// </summary>
		[Tooltip("If true, we don't check for range, we just influence all smartspawners")]
		public bool influenceAllSmartSpawners = false;

		[Space(10f)]

		/// <summary>
		/// If true, we will call Spawn() on the SmartSpawners on each influence tick
		/// </summary>
		[Tooltip("If true, we will call Spawn() on the SmartSpawners on each influence tick")]
		public bool attemptSpawnOnInfluence = false;

		/// <summary>
		/// We multiply the "Time until next spawn" value by this amount on each influence tick
		/// </summary>
		[Tooltip("We multiply the Time until next spawn value by this amount on each influence tick")]
		public float spawnTimeMultiplier = 1f;

		/// <summary>
		/// We decrease the "Time until next spawn" value by this amount on each influence tick
		/// </summary>
		[Tooltip("We decrease the Time until next spawn value by this amount on each influence tick")]
		public float spawnTimeAddition = -5f;

		// Handles starting influence immediately
		void Start () 
		{
			GetSmartSpawnReferences();

			if(startInfluenceImmediately)
			{
				UpdateInfluence();
			}
		}

		//Add listener
		void OnEnable()
		{			
			if(performInfluenceOnWaveReset)
			{
				SmartSpawnScript.OnWaveReset += HandleOnWaveReset;
			}
		}

		//Remove listener for no mem. leaks!
		void OnDisable()
		{
			if(performInfluenceOnWaveReset)
			{
				SmartSpawnScript.OnWaveReset -= HandleOnWaveReset;
			}
		}

		/// <summary>
		/// Fills up our list of all the smartspawnscripts in the scene.
		/// Should be called whenever a new SmartSpawn script is added to the scene
		/// </summary>
		public void GetSmartSpawnReferences()
		{
			//System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
			//s.Start();

			allSpawnScripts.Clear();
			allSpawnScripts.AddRange(SmartSpawnScript.FindObjectsOfType(typeof(SmartSpawnScript)) as SmartSpawnScript[]);

			//s.Stop();
			//Debug.LogError("Elapsed time: " + s.Elapsed);
		}
		
		// Handles calls to UpdateInfluence()
		void Update () 
		{
			if(Time.time >= nextInfluenceUpdate && influenceUpdateRate > 0f)
			{
				UpdateInfluence();
			}
		}

		void UpdateInfluence()
		{
			nextInfluenceUpdate = Time.time + influenceUpdateRate;

			//If we're currently set to not influence anything, we can return early
			if(currentInfluenceModifier == InfluenceModifiers.NONE) return;

			if(!influenceSpawnObject)
			{
				Debug.LogError("Warning: No smartspawnscriptableobject found - make sure you've set one.");
				return;
			}

			//Then loop through all the spawn scripts in the scene to see if we should influence any, and add them
			for(int i = 0; i < allSpawnScripts.Count; i++)
			{
				//Debug.LogWarning("I:" + i.ToString() + " " + allSpawnScripts[i].name.ToString());

				if(influenceAllSmartSpawners)
				{
					//If we don't already have it, add it
					if(!currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i]))
					{
						currentlyInfluencedSpawnScripts.Add(allSpawnScripts[i], false);
					} 
					else //If we do already have it, perform influence on it
					{
						PerformInfluenceOnSmartSpawner(allSpawnScripts[i]);
					}

				} 
				else 
				{

					if(IsWithinRange(influenceRange, transform.position, allSpawnScripts[i].transform.position))
					{
						//If we don't already have it, add it
						if(!currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i]))
						{
							currentlyInfluencedSpawnScripts.Add(allSpawnScripts[i], false);
						} 
						else //If we do already have it, perform influence on it
						{
							PerformInfluenceOnSmartSpawner(allSpawnScripts[i]);
						}
					} 
					else if(currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i])) //If it's not in range, clear it and reset it 
					{
						currentlyInfluencedSpawnScripts.Remove (allSpawnScripts[i]);
						allSpawnScripts[i].ResetItemLists();
					}

				}

			}

			#if UNITY_EDITOR
			//Debug.Log("Updating influence");
			#endif
		}	

		//Like in update(), but dont perform influence
		void UpdateCurrentlyInfluencedSpawnScripts()
		{
			//Then loop through all the spawn scripts in the scene to see if we should influence any, and add them
			for(int i = 0; i < allSpawnScripts.Count; i++)
			{
				//Debug.LogWarning("I:" + i.ToString() + " " + allSpawnScripts[i].name.ToString());
				
				if(influenceAllSmartSpawners)
				{
					//If we don't already have it, add it
					if(!currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i]))
					{
						currentlyInfluencedSpawnScripts.Add(allSpawnScripts[i], false);
					} 					
				} 
				else 
				{
					
					if(IsWithinRange(influenceRange, transform.position, allSpawnScripts[i].transform.position))
					{
						//If we don't already have it, add it
						if(!currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i]))
						{
							currentlyInfluencedSpawnScripts.Add(allSpawnScripts[i], false);
						} 
					} 
					else if(currentlyInfluencedSpawnScripts.ContainsKey(allSpawnScripts[i])) //If it's not in range, clear it and reset it 
					{
						currentlyInfluencedSpawnScripts.Remove (allSpawnScripts[i]);
					}
					
				}
				
			}
		}
		
		void HandleOnWaveReset (SmartSpawnScript spawnerWithWaveReset, int waveNumber)
		{
			#if UNITY_EDITOR
			Debug.Log("Heard onWaveReset from " + spawnerWithWaveReset.name + " with wave " + waveNumber);
			#endif

			if(waveToPerformInfluenceOn == waveNumber || waveToPerformInfluenceOn == -1)
			{
				UpdateCurrentlyInfluencedSpawnScripts();

				if(currentlyInfluencedSpawnScripts.ContainsKey(spawnerWithWaveReset))
				{
					PerformInfluenceOnSmartSpawner(spawnerWithWaveReset);
				}
			}
		}

		void PerformInfluenceOnSmartSpawner(SmartSpawnScript targetSS)
		{
			//If the smartspawnscript doesn't allow us to perform influence, then don't try
			if(targetSS.allowInfluence == false) return;

			//Once we've found the scripts we want to influence, then apply the change
			switch(currentInfluenceModifier)
			{
			case InfluenceModifiers.ADDITIVE:
				targetSS.InfluenceItemLists(influenceSpawnObject);

				//Apply spawn time multiplier
				if(spawnTimeMultiplier != 1f)
				{
					targetSS.timer += (spawnTimeMultiplier * targetSS.spawnCountdown);
				}

				//Apply wave time multiplier
				if(waveTimeMultiplier != 1f)
				{
					targetSS.waveTimer += (waveTimeMultiplier * targetSS.waveResetTime);
				}

				targetSS.waveTimer += waveTimeAddition;
				
				//Try spawn if we want to
				if(attemptSpawnOnInfluence)
				{
					targetSS.Spawn();
				}
				//Debug.Log("Influencing spawner: " + targetSS.name);

				#if UNITY_EDITOR
				Debug.Log(transform.name + " Performed influence on " + targetSS.transform.name);
				#endif
				break;

			case InfluenceModifiers.OVERRIDE:
				targetSS.SetItemLists(influenceSpawnObject);

				//Apply spawn time multiplier
				if(spawnTimeMultiplier != 1f)
				{
					targetSS.timer += (spawnTimeMultiplier * targetSS.spawnCountdown);
				}

				targetSS.timer += spawnTimeAddition;

				//Apply wave time multiplier
				if(waveTimeMultiplier != 1f)
				{
					targetSS.waveTimer += (waveTimeMultiplier * targetSS.waveResetTime);
				}
				
				targetSS.waveTimer += waveTimeAddition;
				
				//Try spawn if we want to
				if(attemptSpawnOnInfluence)
				{
					targetSS.Spawn();
				}
				//Debug.Log("Overriding spawner: " + targetSS.name);

				#if UNITY_EDITOR
				Debug.Log(transform.name + " Performed influence on " + targetSS.transform.name);
				#endif
				break;

			case InfluenceModifiers.SINGLE_INFLUENCE:
				//If the key is set to false, we haven't influenced it yet, so perform influence
				if(currentlyInfluencedSpawnScripts[targetSS] == false)
				{
					targetSS.InfluenceItemLists(influenceSpawnObject);
					currentlyInfluencedSpawnScripts[targetSS] = true;

					//Apply spawn time multiplier
					if(spawnTimeMultiplier != 1f)
					{
						targetSS.timer += (spawnTimeMultiplier * targetSS.spawnCountdown);
					}

					targetSS.timer += spawnTimeAddition;

					//Apply wave time multiplier
					if(waveTimeMultiplier != 1f)
					{
						targetSS.waveTimer += (waveTimeMultiplier * targetSS.waveResetTime);
					}
					
					targetSS.waveTimer += waveTimeAddition;
					
					//Try spawn if we want to
					if(attemptSpawnOnInfluence)
					{
						targetSS.Spawn();
					}
					//Debug.Log("Single Influencing spawner: " + targetSS.name);

					#if UNITY_EDITOR
					Debug.Log(transform.name + " Performed influence on " + targetSS.transform.name);
					#endif
				}				
				break;

			default:
				Debug.LogWarning("Shouldn't have gotten here - no correct influence modifier set");
				break;
			}

		}

		/// <summary>
		/// Determines if the firstposition is within range of the secondposition
		/// </summary>
		/// <returns><c>true</c> if is within the specified range, otherwise, <c>false</c>.</returns>
		/// <param name="range">Range.</param>
		/// <param name="firstPosition">First position.</param>
		/// <param name="secondPosition">Second position.</param>
		public static bool IsWithinRange(float range, Vector3 firstPosition, Vector3 secondPosition)
		{
			float sqrDistanceToOther = 0f;
			
			sqrDistanceToOther = (firstPosition - secondPosition).sqrMagnitude;
			
			if(sqrDistanceToOther < (range * range))
			{				
				return true;
			}
			
			return false;
		}

		[ContextMenu("Check if within range of any")]
		void EditorCheckIfWithinRangeOfAny()
		{
			allSpawnScripts = new List<SmartSpawnScript>();
			allSpawnScripts.Clear();
			allSpawnScripts.AddRange(SmartSpawnScript.FindObjectsOfType(typeof(SmartSpawnScript)) as SmartSpawnScript[]);

			int count = 0;

			foreach(SmartSpawnScript s in allSpawnScripts)
			{
				if(IsWithinRange(influenceRange, transform.position, s.transform.position) || influenceAllSmartSpawners)
				{
					count ++;
				}
			}

			Debug.Log("Currently within range of: " + count + " SmartSpawners");
		}
		
		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, influenceRange);
		}
}

}