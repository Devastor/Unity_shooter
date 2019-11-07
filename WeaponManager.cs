using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
	[SerializeField]
	private string weaponLayerName = "Weapon";
	[SerializeField]
	private GameObject weaponHolder;  
	[SerializeField] 
	private PlayerWeapon primaryWeapon;
	private PlayerWeapon currentWeapon; 

	private WeaponGraphics currentGraphics;

	// Use this for initialization
	void Start () 
	{
		EquipWeapon (primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentGraphics()
	{
		return currentGraphics;
	}

	void EquipWeapon(PlayerWeapon _weapon)
	{
		currentWeapon = _weapon;

		GameObject _weaponInst = (GameObject)Instantiate (_weapon.graphics, weaponHolder.transform.position, weaponHolder.transform.rotation);
		_weaponInst.transform.SetParent (weaponHolder.transform);

		currentGraphics = _weaponInst.GetComponent<WeaponGraphics> ();
		if (currentGraphics == null)
			Debug.LogError ("No graphics component of the weapon object " + _weaponInst.name);

		if (isLocalPlayer) 
		{
			Util.SetLayerRecursively(_weaponInst, LayerMask.NameToLayer(weaponLayerName));
		}
	}

	void SetLayerRecursively (GameObject _obj, int _newLayer)
	{
		_obj.layer = _newLayer;
		foreach (Transform child in _obj.transform) 
		{			
			SetLayerRecursively (child.gameObject, _newLayer);
		}
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
