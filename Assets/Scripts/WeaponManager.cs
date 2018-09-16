using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	
	public Transform weaponinfotext;
	

	public static string textstatus="off";

	public string weaponName;								//The Name of the weapon
	public int maxDamage;							//Maximum damage of the weapon
	public int minDamage;							//Minimum damage of the weapon
	public int newDamage;
	public int fireRate;							//The rate of fire of the weapon

	public float weaponRarity;  //The Rarity of the weapon

	// Use this for initialization
	void Start () {
		weaponRarity = 0;


		
	}
	
	// Update is called once per frame
	void Update () {

		
	}

	void OnMouseEnter()
	{
		if (textstatus == "off")
		{
			if(weaponRarity == 0){
			weaponinfotext.GetComponent<TextMesh>().text = "Common Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.grey;
			Debug.Log("Common Weapon");
			minDamage = 20;
			maxDamage = 35;
			
			newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;
			}
		
			if(weaponRarity == 1){
			weaponinfotext.GetComponent<TextMesh>().text = "Uncommon Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.green;
			Debug.Log("Uncommon Weapon");

			minDamage = 35;
			maxDamage = 50;
			
			newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;

			}

			if(weaponRarity == 2){
			weaponinfotext.GetComponent<TextMesh>().text = "Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.blue;
			Debug.Log("Rare Weapon");

			minDamage = 50;
			maxDamage = 65;
			
			int newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;
			}


			if(weaponRarity == 3){
			weaponinfotext.GetComponent<TextMesh>().text = "Ultra Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.magenta;
			Debug.Log("Ultra Rare Weapon");

			minDamage = 65;
			maxDamage = 80;
			
			int newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;
			}

			if(weaponRarity == 4){
			weaponinfotext.GetComponent<TextMesh>().text = "Omega Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.magenta;
			Debug.Log("Omega Rare Weapon");

			minDamage = 80;
			maxDamage = 95;
			
			int newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;
			}

			if(weaponRarity == 5){
			weaponinfotext.GetComponent<TextMesh>().text = "Weapon of The Developers";
			weaponinfotext.GetComponent<TextMesh>().color = Color.magenta;
			Debug.Log("Weapon of The Developers");

			minDamage = 95;
			maxDamage = 110;
			
			int newDamage = Random.Range(minDamage, maxDamage);
			Debug.Log(newDamage);
			ArmControllerScript.damage = newDamage;
			}
			
			textstatus="on";
			Instantiate (weaponinfotext, new Vector3(transform.position.x, transform.position.y), weaponinfotext.rotation);

		}
	}
	void OnMouseExit()
	{
		if (textstatus == "on")
		{
			textstatus = "off";
		}
	}
}
