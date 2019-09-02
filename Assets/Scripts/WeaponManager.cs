using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	
	public Transform weaponinfotext;
	

	public static string textstatus="off";

	public string weaponName;						//The Name of the weapon
	public int maxDamage;							//Maximum damage of the weapon
	public int minDamage;							//Minimum damage of the weapon
	public static int newDamage;
	public int fireRate;							//The rate of fire of the weapon

	public float weaponRarity;  //The Rarity of the weapon

	// Use this for initialization
	void Start () {
		weaponRarity = Random.Range(0, 5);

        

    }
	
	// Update is called once per frame
	void Update () {

        
		
	}

	void OnMouseEnter()
	{
		if (textstatus == "off")
		{
            //Common Weapons
			if(weaponRarity == 0){
                gameObject.tag = "CommonRifle";
			weaponinfotext.GetComponent<TextMesh>().text = "Common Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.grey;
                Destroy(gameObject, 10f);
            }
		    
            //Uncommon Weapons
			if(weaponRarity == 1){
                gameObject.tag = "UncommonRifle";
                weaponinfotext.GetComponent<TextMesh>().text = "Uncommon Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.green;
                Destroy(gameObject, 10f);
            }

            //Rare Rifles
			if(weaponRarity == 2){
                gameObject.tag = "RareRifle";
                weaponinfotext.GetComponent<TextMesh>().text = "Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.blue;
                Destroy(gameObject, 10f);
            }

            //Ultra Rare Rifles
			if(weaponRarity == 3){
                gameObject.tag = "UltraRareRifle";
			weaponinfotext.GetComponent<TextMesh>().text = "Ultra Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.magenta;
                Destroy(gameObject, 10f);
            }
            //Omega Rare Rifle
			if(weaponRarity == 4){
                gameObject.tag = "OmegaRareRifle";
			weaponinfotext.GetComponent<TextMesh>().text = "Omega Rare Weapon";
			weaponinfotext.GetComponent<TextMesh>().color = Color.yellow;
                Destroy(gameObject, 10f);
            }
            //Weapon of the Developers
			if(weaponRarity == 5){
                gameObject.tag = "DeveloperRifle";
			weaponinfotext.GetComponent<TextMesh>().text = "Weapon of The Developers";
			weaponinfotext.GetComponent<TextMesh>().color = Color.red;
                Destroy(gameObject, 10f);
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
