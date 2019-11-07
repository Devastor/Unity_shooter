using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour 
{
	[SerializeField]
	private Text usernameText;

	[SerializeField]
	private Player player;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		usernameText.text = player.username;
	}
}
