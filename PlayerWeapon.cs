using UnityEngine;

[System.Serializable]
public class PlayerWeapon : MonoBehaviour
{
	public string name = "Blaster";
	public int damage = 5;
	public float range = 100f;
	public float fireRate = 0f;
	 
	public GameObject graphics;
}
