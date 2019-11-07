using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerController))]
public class Player : NetworkBehaviour 
{
	[SyncVar]
	private bool _isDead = false;
	public bool isDead 
	{
		get { return _isDead; }
		protected set {_isDead = value; }
	}
	[SerializeField]
	private int maxHealth = 100;
	[SyncVar]
	private int currentHealth;

	private int healthAmount;

	[SerializeField]
	private Behaviour[] disabledOnDeath;
	private bool[] wasEnabled;
	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SyncVar]
	public string username = "Loading...";

	[SerializeField]
	private GameObject deathEffect;
	[SerializeField]
	private GameObject spawnEffect;

	public int kills;
	public int deaths;

	private bool firstSetup = true;

	public float GetHpAmount ()
	{
		return (float)currentHealth/maxHealth;	
	}

	public void SetupPlayer ()
	{
		if (isLocalPlayer) 
		{
			// switch cameras
			GameManager.instance.SetSceneCameraActive (false);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (true);
			GetComponent<PlayerController> ().ResetFuelAmount ();
		}

		CmdBroadcastNewPlayerSetup ();
	}

	[Command]
	private void CmdBroadcastNewPlayerSetup()
	{
		RpcSetupPlayerOnAllClients ();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		if (firstSetup) 
		{
			if (isLocalPlayer) 
			{
				deaths = 0;
				kills = 0;
			}

			wasEnabled = new bool[disabledOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {
				wasEnabled [i] = disabledOnDeath [i].enabled;	
			}
			firstSetup = false;
		}

		SetDefaults ();	
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		// little delay
		yield return new WaitForSeconds (0.1f);

		SetupPlayer ();

		SetDefaults ();
	}

	public void SetDefaults()
	{
		isDead = false;

		currentHealth = maxHealth;	

		// set components active
		for (int i = 0; i < disabledOnDeath.Length; i++) 
		{
			disabledOnDeath[i].enabled = wasEnabled[i];
		}

		// enable gameObjects
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) 
		{
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		// Enable the collider
		Collider _col = GetComponent<Collider> ();
		if (_col != null)
			_col.enabled = true;


		// create spawn effect
		Vector3 displace = new Vector3(0f, -6f, 0f);
		GameObject graphInstance = (GameObject)Instantiate(spawnEffect, transform.position + displace, Quaternion.identity);
		Destroy (graphInstance, 3f);
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount)
	{
		if (isDead)
			return;
		
		currentHealth -= _amount;

		Debug.Log (transform.name + " now has " + currentHealth + " health");

		if (currentHealth <= 0)
			Die ();
	}

	private void Die ()
	{
		deaths++;
		isDead = true;

		// Disable components of Player object
		for (int i = 0; i < disabledOnDeath.Length; i++) 
		{
			disabledOnDeath[i].enabled = false;
		}

		// Disable gameObjects of Player object
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) 
		{
			disableGameObjectsOnDeath[i].SetActive(false);
		}

		// Disable the collider
		Collider _col = GetComponent<Collider> ();
		if (_col != null)
			_col.enabled = false;

		// Spawn a death effect
		GameObject graphInstance = (GameObject)Instantiate(deathEffect, transform.position,Quaternion.identity);
		Destroy (graphInstance, 3f);

		// switch cameras
		if (isLocalPlayer) 
		{
			GameManager.instance.SetSceneCameraActive (true);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (false);
		}

		// Call respawn method
		StartCoroutine(Respawn());
	}

	// Use this for initialization
	void Start () 
	{
		username = transform.name;
	}


	// Update is called once per frame
	void Update () 
	{
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.K))
			RpcTakeDamage (9999);


	}

}
