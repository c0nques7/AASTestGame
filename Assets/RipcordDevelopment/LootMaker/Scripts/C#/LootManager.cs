using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 							LootMaker 2.0, Copyright © 2017, Ripcord Development
//											LootManager.cs
//										 info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script maintains a list of the different types of loot available and the total amount of each that has been collected

public enum LootType { 								//The different loot types the lootManager will handle
	Money, 
	Health, 
	Ammo, 
	Experience
	//Add your own (don't forget the comma)
};

public class LootManager : MonoBehaviour {

	public static LootManager instance;

	int totalMoney;
	int totalHP;
	int totalAmmo;
	int totalXP;

	public Text labelTotalMoney;
	public Text labelTotalHP;
	public Text labelTotalAmmo;
	public Text labelTotalXP;


	void Awake () {

		if (!instance) {
			instance = this;
		}
		else {
			Destroy(this);
		}
	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// INCREASE LOOT AMOUNT - Increase the specified loot type by the supplied value
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	public void IncreaseLoot (LootType type, int lootValue) {
		
		switch(type){
		case LootType.Money:
			totalMoney += lootValue;
			labelTotalMoney.text = ("$" + totalMoney);
			break;
			
		case LootType.Health:
			totalHP += lootValue;
			labelTotalHP.text = ("HP - " + totalHP);
			break;
			
		case LootType.Ammo:
			totalAmmo += lootValue;
			labelTotalAmmo.text = ("Ammo - " + totalAmmo);
			break;
			
		case LootType.Experience:
			totalXP += lootValue;
			labelTotalXP.text = ("XP - " + totalXP);
			break;
		}
	}
}