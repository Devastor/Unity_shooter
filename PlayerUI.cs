using UnityEngine;

public class PlayerUI : MonoBehaviour 
{
	[SerializeField]
	RectTransform Fuel;
	[SerializeField] 
	RectTransform hpLevel;
	//[SerializeField]
	//GameObject pauseMenu;
	[SerializeField]
	GameObject scoreboard;
	[SerializeField]
	GameObject pauseMenu;

	private PlayerController controller;
	private Player player; 

	public void SetPlayer(Player _player)
	{
		player = _player;
		controller = player.GetComponent<PlayerController> ();
	}

	void SetFuelAmount(float _amount)
	{
		Fuel.localScale = new Vector3 (1f, _amount/300, 1f);
	}

	void SetHpAmount(float _amount)
	{
		hpLevel.localScale = new Vector3 (1f, _amount, 1f);
	}

	// Use this for initialization
	void Start () 
	{
		PauseMenu.isOn = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetFuelAmount(controller.GetFuelAmount ());
		SetHpAmount (player.GetHpAmount ());

		if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			scoreboard.SetActive (true);
		}
		else if (Input.GetKeyUp (KeyCode.Tab)) 
		{
			scoreboard.SetActive (false);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			TogglePauseMenu ();
		}
	}

	void TogglePauseMenu()
	{
		if (Cursor.lockState != CursorLockMode.Locked) 
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		pauseMenu.SetActive (!pauseMenu.activeSelf);
		PauseMenu.isOn = pauseMenu.activeSelf;
	}
}
