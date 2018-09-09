using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using SmartSpawn;

/// <summary>
/// This is the custom editor script for the SmartSpawnScript.
/// 
/// This script creates a custom editor GUI to help make the SmartSpawnScript easier to use.
/// </summary>

[CustomEditor(typeof(SmartSpawnScript))]
[CanEditMultipleObjects]
public class SmartSpawnEditor : Editor {
	
	SerializedProperty spawnChance;
	SerializedProperty itemPrefab;
	SerializedObject obj;

	int _tempInt;
	Object _tempObject;

	Transform[] sPoints;
	
	void OnEnable() {

		//ob = new SerializedObject(target);
		SerializedProperty child = serializedObject.FindProperty("spawnObject");

		if(child == null || child.objectReferenceValue == null)
		{
			Debug.Log("No spawn asset found");
		} else {
			obj = new SerializedObject(child.objectReferenceValue);

			//Debug.Log(obj.GetType ().Name);

			//Error handling
			spawnChance = obj.FindProperty("spawnChance");
			if (!spawnChance.isArray) {
				// You shouldn't expect to see this.
				Debug.LogError("Property is not an array!");
			}

			itemPrefab = obj.FindProperty("itemPrefab");
			if (!itemPrefab.isArray) {
				// You shouldn't expect to see this.
				Debug.LogError("Property is not an array!");
			}
		}
		try {
			if(Selection.activeGameObject != null)
			{
				if(Selection.activeTransform)
				{
					SmartSpawnScript scr = Selection.activeGameObject.GetComponent<SmartSpawnScript>();
					if(scr != null)
					{
						scr.GetSpawnPoints();
						sPoints = scr.spawnPoints;

						if(sPoints.Length == 0)
						{
							Debug.LogWarning("No spawnpoints found, creating default spawn point");
							CreateSpawnPoint(scr);
						}
					}
				} else {
					Debug.Log("Editing SmartSpawn in project hierarchy");
				}

			}


		} catch (UnityException e)
		{
			if(e == null)
				Debug.Log("Null exception");
		}

	}

	public void CreateSpawnPoint(SmartSpawnScript _smartSpawner)
	{
		GameObject g = new GameObject();
		g.transform.position = (Selection.activeTransform.position + Random.insideUnitSphere * 5f);
		g.transform.rotation = Selection.activeTransform.rotation;
		g.transform.parent = Selection.activeTransform;
		
		g.AddComponent(typeof(SphereCollider));
		g.GetComponent<SphereCollider>().isTrigger = true;
		
		//If the smartspawner already has a spawnpoint, make sure we use the references from that spawnpoints SmartSpawnColliderCheck
		if(_smartSpawner.spawnPoints.Length > 0)
		{
			UnityEditorInternal.ComponentUtility.CopyComponent(_smartSpawner.spawnPoints[0].GetComponent<SmartSpawnColliderCheck>());
			UnityEditorInternal.ComponentUtility.PasteComponentAsNew(g);
			if(_smartSpawner.spawnPoints[0].GetComponent<SphereCollider>())
			{
				g.GetComponent<SphereCollider>().radius = _smartSpawner.spawnPoints[0].GetComponent<SphereCollider>().radius;
			}
			g.layer = _smartSpawner.spawnPoints[0].gameObject.layer;
		} else {
			//Add the smartspawncolliderCheck script to it
			g.AddComponent(typeof(SmartSpawnColliderCheck));
		}

		string n = ("SpawnPoint_" + sPoints.Length.ToString());
		g.name = n;
		Undo.RegisterCreatedObjectUndo(g, n);
		this.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		//Updates the object we are editing
		serializedObject.Update();

		//Quick references for when we want to change the size of the array
		bool enlarge = false;
		bool decrease = false;

		//Quick reference to the itemspawnscript
		SmartSpawnScript s = target as SmartSpawnScript;

		if(itemPrefab == null || spawnChance == null)
		{
			this.OnEnable();
		}

		//Start the horizontal (2 per line) GUI layour
		EditorGUILayout.BeginHorizontal();

			//Spawn timer (how long between spawns) slider and label
			EditorGUILayout.LabelField("Next spawn in: " + ((int)(s.spawnCountdown - s.timer)).ToString());

			//Button to manually call the spawn function, only if the game is in play mode
			if(GUILayout.Button("Test Spawn")){

				if(Application.isPlaying){
					s.Spawn();
				}

				if(!Application.isPlaying){
					Debug.Log("You can only call the spawn function when the game is running");
				}

			}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		//Checkbox to see if the user wants to fill any empty spawn point on each spawn
		EditorGUILayout.LabelField("Fill empty spawn points first");

		EditorGUI.BeginChangeCheck();
		bool bOne = EditorGUILayout.Toggle(s.useOpenSpawnPoints);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Fill empty sPs");
			s.useOpenSpawnPoints = bOne;
		}

		EditorGUILayout.EndHorizontal();
		

		
		//Checkbox to see if the user wants to fill any empty spawn point on each spawn
		if(s.useOpenSpawnPoints)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Randomise filled Spawnpoints");
			
			EditorGUI.BeginChangeCheck();
			bool bTwenty = EditorGUILayout.Toggle(s.useRandomSpawnPointsWhenFillingEmpty);
			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Track spawned objects");
				s.useRandomSpawnPointsWhenFillingEmpty = bTwenty;
			}
			
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		
		//Checkbox to see if the user wants to fill any empty spawn point on each spawn
		EditorGUILayout.LabelField("Cap maximum spawns");
		
		EditorGUI.BeginChangeCheck();
		bool bTwo = EditorGUILayout.Toggle(s.useMaxSpawns);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Use max spawns");
			s.useMaxSpawns = bTwo;
		}
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		//Checkbox to see if the user wants to fill any empty spawn point on each spawn
		EditorGUILayout.LabelField("Use Wave Spawning");
		
		EditorGUI.BeginChangeCheck();
		bool bWave = EditorGUILayout.Toggle(s.useWaveSpawning);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Use Wave Spawning");
			s.useWaveSpawning = bWave;
		}
		
		EditorGUILayout.EndHorizontal();

		if(s.useWaveSpawning == true)
		{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Reset waves after time?");
		
		EditorGUI.BeginChangeCheck();
		bool bWaveT = EditorGUILayout.Toggle(s.resetWaveValuesAfterTime);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Reset waves after time?");
			s.resetWaveValuesAfterTime = bWaveT;
		}
		
		EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		if(s.resetWaveValuesAfterTime == true && s.useWaveSpawning == true)
		{
			
			EditorGUI.BeginChangeCheck();
			float rOne = EditorGUILayout.Slider("Wave reset time", s.waveResetTime, 0f, (s.waveResetTime + 10f));
			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Wave resettime");
				s.waveResetTime = rOne;
			}
			
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		//Checkbox to see if the user wants to fill any empty spawn point on each spawn
		EditorGUILayout.LabelField("Track spawned objects");
		
		EditorGUI.BeginChangeCheck();
		bool bfour = EditorGUILayout.Toggle(s.trackSpawnedObjects);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Track spawned objects");
			s.trackSpawnedObjects = bfour;
		}
		
		EditorGUILayout.EndHorizontal();

		
		EditorGUILayout.BeginHorizontal();
		if(s.useMaxSpawns)
		{

			EditorGUI.BeginChangeCheck();
			int iOne = EditorGUILayout.IntSlider("Max spawns", s.maxSpawnerSpawns, 0, 150);
			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Change max spawns");
				s.maxSpawnerSpawns = iOne;
			}

		}
		EditorGUILayout.EndHorizontal();
		
		//Start the horizontal (2 per line) GUI layour
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField("Time between spawns");

		EditorGUI.BeginChangeCheck();
		float iTwo = EditorGUILayout.Slider(s.spawnCountdown, 0f, 500f);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Change time between spawns");
			s.spawnCountdown = iTwo;
		}
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		SmartSpawnScriptableObject spOne = (SmartSpawnScriptableObject)EditorGUILayout.ObjectField("Spawner type", s.spawnObject, typeof(SmartSpawnScriptableObject), false);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Change spawner type");
			s.spawnObject = spOne;
		}
		
		SmartSpawnScriptableObject targetSpawnObject = s.spawnObject;

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Create new Spawner type"))
		{
			CreateSmartSpawnAsset();
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Add spawn point"))
		{
			CreateSpawnPoint(s);
		}
		
		EditorGUILayout.EndHorizontal();
		
		
		if(Application.isPlaying)
		{
			EditorGUILayout.LabelField("Note: Values changed in playmode are not saved");	
		}
				
		if(targetSpawnObject != null)
		{
			//Labels
			EditorGUILayout.LabelField("Spawn ratio", "Object to spawn");			
			
			//Start the horizontal (2 per line) GUI layour
			EditorGUILayout.BeginHorizontal();
			


			if(targetSpawnObject.spawnChance != null)
			{
				
				//Make a column of the spawn chance ratio sliders
				EditorGUILayout.BeginVertical();

				if(Application.isPlaying)
				{
					for(int i = 0; i < s.spawnChances.Count; ++i)
					{	
						//s.spawnObject.spawnChance[i] = EditorGUILayout.IntSlider(s.spawnObject.spawnChance[i], 0, 100);		
						s.spawnChances[i] = EditorGUILayout.IntSlider(s.spawnChances[i], 0, Mathf.Max(100, s.spawnChances[i]));						
					}
				} else {
					for(int i = 0; i < targetSpawnObject.spawnChance.Length; ++i)
					{	
						//s.spawnObject.spawnChance[i] = EditorGUILayout.IntSlider(s.spawnObject.spawnChance[i], 0, 100);					
						EditorGUI.BeginChangeCheck();
						int iThree = EditorGUILayout.IntSlider(s.spawnObject.spawnChance[i], 0, Mathf.Max(100, s.spawnObject.spawnChance[i]));
						if(EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(obj.targetObject, "Change spawn chance");
							s.spawnObject.spawnChance[i] = iThree;
						}
						
					}
				}
				EditorGUILayout.EndVertical();
				
				//Make a column of the item prefab pickers
				EditorGUILayout.BeginVertical();

				if(Application.isPlaying)
				{
					for(int n = 0; n < s.itemPrefabs.Count; ++n)
					{	
						//s.spawnObject.spawnChance[i] = EditorGUILayout.IntSlider(s.spawnObject.spawnChance[i], 0, 100);		
						s.itemPrefabs[n] = EditorGUILayout.ObjectField(s.itemPrefabs[n], typeof(GameObject), false);					
					}
				} else {
					for(int n = 0; n < targetSpawnObject.spawnChance.Length; ++n)
					{
						
						EditorGUI.BeginChangeCheck();
						Object pOne = EditorGUILayout.ObjectField(s.spawnObject.itemPrefab[n], typeof(GameObject), false);
						if(EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(obj.targetObject, "Change spawn obj");
							s.spawnObject.itemPrefab[n] = pOne;
						}
					}
				}
				EditorGUILayout.EndVertical();

			}

			EditorGUILayout.BeginVertical();
			
			//Plus button
			if(!Application.isPlaying)
			{
				if (GUILayout.Button("Add")) {
					enlarge = true;
				}

				//Minus button
				if (GUILayout.Button("Remove")) {
					decrease = true;
				}
			}
			
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeArray();
				serializedObject.ApplyModifiedProperties();
			}
			
			EditorGUILayout.EndVertical();
			
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseArray();
				serializedObject.ApplyModifiedProperties();
			}
			
			EditorGUILayout.EndHorizontal();
		}

		if (GUI.changed)
		{
			this.OnEnable();

			EditorUtility.SetDirty (target);

			if(s.spawnObject != null)
			{
				EditorUtility.SetDirty(s.spawnObject);
			}		
		}

	}
	
	//Increase the size of the arrays
	void EnlargeArray() {
		int enlarged = spawnChance.arraySize;
		int itemEnlarged = itemPrefab.arraySize;
		spawnChance.InsertArrayElementAtIndex(enlarged);
		itemPrefab.InsertArrayElementAtIndex(itemEnlarged);
		obj.ApplyModifiedProperties();

	}

	//Decrease the size of the arrays
	void DecreaseArray() {
		spawnChance.arraySize--;
		itemPrefab.arraySize = spawnChance.arraySize;
		obj.ApplyModifiedProperties();

	}

	void ModifySmartSpawnAsset(Object[] items, int[] chances, SmartSpawnScriptableObject origAsset)
	{
		origAsset.itemPrefab = items;
		origAsset.spawnChance = chances;		

		AssetDatabase.SaveAssets();
	}

	void OnSceneGUI()
	{
		if(sPoints.Length == 0) OnEnable();

		foreach(Transform t in sPoints)
		{
			t.position = Handles.PositionHandle(t.position, t.rotation);
		}


		if (GUI.changed){
		EditorUtility.SetDirty (target);
		}


	}

	[MenuItem("Assets/Create/Spawn Asset")]
	public static void CreateSmartSpawnAsset()
	{
		SmartSpawnScriptableObject asset = ScriptableObject.CreateInstance<SmartSpawnScriptableObject>();

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if(path == "")
		{
			path = "Assets";
		} else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + "SpawnerAsset.Asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		
		//EditorUtility.FocusProjectWindow();
		
		Selection.activeObject = asset;
		
	}
	
}
