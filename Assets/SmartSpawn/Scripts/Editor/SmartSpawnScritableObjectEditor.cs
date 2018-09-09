using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartSpawn;

[CustomEditor(typeof(SmartSpawnScriptableObject))]
[CanEditMultipleObjects]
public class SmartSpawnScritableObjectEditor : Editor {

	SerializedProperty spawnChance;
	SerializedProperty itemPrefab;
	SerializedObject obj;

	void OnEnable() {

		//ob = new SerializedObject(target);
		spawnChance = serializedObject.FindProperty("spawnChance");
		itemPrefab = serializedObject.FindProperty("itemPrefab");
		
		if(spawnChance == null || itemPrefab == null)
		{
			Debug.LogWarning("No spawn asset found");
		} else {
			obj = serializedObject;
		}

	}

	public override void OnInspectorGUI(){
		//Quick references for when we want to change the size of the array
		bool enlarge = false;
		bool decrease = false;

		EditorGUILayout.LabelField("You can also drag this asset into the 'Spawner Type'");
		EditorGUILayout.LabelField("field of a Smart Spawn Script to edit");

		serializedObject.Update();

		SmartSpawnScriptableObject targetSpawnObject = target as SmartSpawnScriptableObject;

		if(targetSpawnObject != null)
		{
			//Labels

			if(Application.isPlaying)
			{
				EditorGUILayout.LabelField("Note: Values are not saved in playmode");	
			}

			EditorGUILayout.LabelField("Spawn ratio", "Object to spawn");			
			
			//Start the horizontal (2 per line) GUI layour
			EditorGUILayout.BeginHorizontal();
			
			
			
			if(targetSpawnObject.spawnChance != null)
			{
				
				//Make a column of the spawn chance ratio sliders
				EditorGUILayout.BeginVertical();
				for(int i = 0; i < targetSpawnObject.spawnChance.Length; ++i)
				{	
					//s.spawnObject.spawnChance[i] = EditorGUILayout.IntSlider(s.spawnObject.spawnChance[i], 0, 100);
					
					EditorGUI.BeginChangeCheck();
					int iThree = EditorGUILayout.IntSlider(targetSpawnObject.spawnChance[i], 0, 100);
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(obj.targetObject, "Change spawn chance");
						targetSpawnObject.spawnChance[i] = iThree;
					}
					
				}
				
				EditorGUILayout.EndVertical();
				
				//Make a column of the item prefab pickers
				EditorGUILayout.BeginVertical();
				
				for(int n = 0; n < targetSpawnObject.spawnChance.Length; ++n)
				{
					
					EditorGUI.BeginChangeCheck();
					Object pOne = EditorGUILayout.ObjectField(targetSpawnObject.itemPrefab[n], typeof(GameObject), false);
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(obj.targetObject, "Change spawn obj");
						targetSpawnObject.itemPrefab[n] = pOne;
					}
				}
				EditorGUILayout.EndVertical();
				
			}
			
			EditorGUILayout.BeginVertical();
			
			//Plus button
			if (GUILayout.Button("Add")) {
				enlarge = true;
			}
			
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeArray();
				serializedObject.ApplyModifiedProperties();
			}
			
			//Minus button
			if (GUILayout.Button("Remove")) {
				decrease = true;
			}
			
			
			EditorGUILayout.EndVertical();
			
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseArray();
				serializedObject.ApplyModifiedProperties();
			}
			
			EditorGUILayout.EndHorizontal();
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

}
