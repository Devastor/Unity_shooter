using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour 
{
	[SerializeField]
	private uint roomSize = 6;

	private string roomName;

	private NetworkManager networkManager;

	void Start()
	{
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null) 
		{
			networkManager.StartMatchMaker ();
		}
	}

	public void SetRoomName(string _name)
	{
		roomName = _name;
	}

	public void CreateRoom()
	{
		Debug.Log ("Trying to create room...");
		if (roomName != "" && roomName != null) 
		{
			Debug.Log ("Creating room: " + roomName + " with room for " + roomSize + " players");
			// Create room
			networkManager.matchMaker.CreateMatch(roomName, roomSize, false, "", "", "", 0, 0, networkManager.OnMatchCreate);

		}
	}
}
