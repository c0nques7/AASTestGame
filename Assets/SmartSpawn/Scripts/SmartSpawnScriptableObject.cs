using UnityEngine;
using System.Collections;

namespace SmartSpawn {


[System.Serializable]
public class SmartSpawnScriptableObject : ScriptableObject{

	[SerializeField]
	public Object[] itemPrefab;
	
	[SerializeField]
	public int[] spawnChance;		
	
}

}