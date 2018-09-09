using UnityEngine;
using System.Collections;

namespace SmartSpawn {
	
	public class SmartSpawnEnemyTracker : MonoBehaviour {
		
		public SmartSpawnScript spawner;
		
		public bool incrementSpawnsOnDeath = false;
		
		void OnDestroy()
		{
			if(incrementSpawnsOnDeath)
			{
				spawner.IncreaseSpawnerSpawns();
			}
		}
		
	}
	
}