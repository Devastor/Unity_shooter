﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour 
{
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	GameObject playerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance;

	// Use this for initialization
	void Start () 
	{
		if (!isLocalPlayer) 
		{
			DisableComponents ();
			AssignRemoteLayer ();
		} 
		else 
		{	
			
			// Disable player graphics for local player
			SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			// Create player UI
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			// Configure playerUI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null)
				Debug.LogError ("No PlayerUI component on PlayerUI prefab.");
			ui.SetPlayer(GetComponent<Player>());

			GetComponent<Player> ().SetupPlayer ();
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

	public override void OnStartClient()
	{
		base.OnStartClient ();

		string _netID = GetComponent<NetworkIdentity> ().netId.ToString();
		Player _player = GetComponent<Player> ();

		GameManager.RegisterPlayer (_netID, _player);
	}

	void AssignRemoteLayer()
	{
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void DisableComponents ()
	{
		for (int i = 0; i < componentsToDisable.Length; i++) 
		{
			componentsToDisable [i].enabled = false;
		}
	}

	void OnDisable()
	{
		Destroy (playerUIInstance);

		if (isLocalPlayer)
			GameManager.instance.SetSceneCameraActive (true);

		GameManager.UnRegisterPlayer (transform.name);
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
