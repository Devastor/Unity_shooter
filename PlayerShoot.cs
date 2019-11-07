using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour 
{
	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam; 

	[SerializeField]
	private LayerMask mask;

	private PlayerWeapon currentWeapon;

	private WeaponManager weaponManager;

	//public AudioClip impact;
	[SerializeField]
	private AudioSource audioSource;

	public Vector3 audioPosition;

	// Use this for initialization
	void Start () 
	{
		if (cam == null)
		{
			Debug.LogError ("PlayerShoot: No camera!");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager> ();
		//weaponGFX.layer = LayerMask.NameToLayer (weaponLayerName);
		// change weapon camera layers for the local player

	}


	// Update is called once per frame
	void Update () 
	{
		currentWeapon = weaponManager.GetCurrentWeapon ();

		if (PauseMenu.isOn)
			return;
		
		if (currentWeapon.fireRate <= 0f) 
		{
			if (Input.GetButtonDown ("Fire1")) 
			{
				Shoot ();
			}
		}
		else 
		{
			if (Input.GetButtonDown ("Fire1")) 
			{
				InvokeRepeating ("Shoot", 0f, 1f/(currentWeapon.fireRate));
			}
			else if (Input.GetButtonUp("Fire1"))
			{
				CancelInvoke ("Shoot");
			}
				
		}
	}

	// call on the server when a player shoots
	[Command]
	void CmdOnShoot()
	{
		RpcDoShootEffect ();
	}

	// is called on all clients we want to show shoot effect
	[ClientRpc]
	void RpcDoShootEffect()
	{
		if (Vector3.Distance (transform.position, audioPosition) < 20f)
			audioSource.Play ();
		else if (Vector3.Distance (transform.position, audioPosition) < 50f)
		{
			audioSource.volume = 0.35f;
			audioSource.Play ();
		}
		else if (Vector3.Distance (transform.position, audioPosition) < 100f)
		{
			audioSource.volume = 0.1f;
			audioSource.Play ();
		}

		weaponManager.GetCurrentGraphics ().muzzleFlash.Play ();
	}

	// Is called on the server when we hit smth
	// taakes in the hit point and the normal to a surface
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffect (_pos, _normal);
	}

	// called on all clients
	// here we spawn cool effects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		GameObject _hitEffectReference = (GameObject)Instantiate (weaponManager.GetCurrentGraphics().impactEffect, _pos, Quaternion.LookRotation(_normal));
		Destroy (_hitEffectReference, 0.5f);
	}

	[Client]
	void Shoot()
	{
		if (!isLocalPlayer)
			return;
		
		audioPosition = audioSource.transform.position;

		// we are shooting, call the onShoot method on the server
		CmdOnShoot ();

		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)) 
		{
			// We hit smth
			if (_hit.collider.tag == PLAYER_TAG) CmdPlayerShot(_hit.collider.name, currentWeapon.damage);

			// We hit somth, call the OnHit on the server
			CmdOnHit (_hit.point, _hit.normal);
		}
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage)
	{
		Player _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage);
	}
}
