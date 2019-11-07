using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (ConfigurableJoint))]
[RequireComponent(typeof (PlayerMotor))]
public class PlayerController : NetworkBehaviour
{
	[SerializeField]
	private float speed = 20f;
	[SerializeField] 
	private float lookSensitivity = 5f;
	[SerializeField]
	private float thrusterForce = 5000f; 

	[SerializeField]
	private LayerMask environmentMask;

	[Header ("Spring settings")]
	[SerializeField]
	private float jointSpring = 40f;
	[SerializeField]
	private float jointMaxForce = 100f;

	private PlayerMotor motor;
	private ConfigurableJoint joint;

	public float maxFuel = 300f;
	[SerializeField]
	private float fuel;
	[SerializeField]
	private float fuelBurnSpeed = 5f;
	[SerializeField]
	private float fuelRegenSpeed = 2.5f;
	[SerializeField]
	private ParticleSystem engineTail;

	public float GetFuelAmount ()
	{
		return fuel;	
	}

	public void ResetFuelAmount() 
	{
		fuel = maxFuel;
	}

	void Start()
	{
		fuel = maxFuel;
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();

		SetJointSettings (jointSpring);
	}

	void Update()
	{
		

		if (PauseMenu.isOn) 
		{
			Vector3 stopVector = new Vector3 (0f, 0f, 0f);
			motor.ApplyThruster(stopVector);
			motor.Move (stopVector);
			motor.Rotate(stopVector);
			motor.RotateCamera(0);
			SetJointSettings (jointSpring);
			return;
		}
		if (Cursor.lockState != CursorLockMode.Locked) 
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Checking obstacle under Player
		RaycastHit _hit;
		if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask)) 
		{
			joint.targetPosition = new Vector3 (0f, -_hit.point.y, 0f);	
		}
		else 
		{
			joint.targetPosition = new Vector3 (0f, 0f, 0f);	
		}

		// Calculate movement velocity as 3D vector
		float _xMov = Input.GetAxisRaw ("Horizontal");
		float _zMov = Input.GetAxisRaw ("Vertical");

		Vector3 _moveHorizontal = transform.right * _xMov; // (1, 0, 0)
		Vector3 _moveVertical = transform.forward * _zMov; // (0, 0, 1)

		// final mvement vector
		Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

		//Apply movement
		motor.Move (_velocity);

		// Calculate rotation: turning around
		float _yRot = Input.GetAxisRaw("Mouse X");

		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;

		// Apply rotation
		motor.Rotate(_rotation);

		// Calculate CAMERA rotation: tilting
		float _xRot = Input.GetAxisRaw("Mouse Y");

		float _cameraRotationX = _xRot * lookSensitivity;

		// Apply rotation
		motor.RotateCamera(_cameraRotationX);

		// Calculate thruster force
		Vector3 _thrusterForce = Vector3.zero;

		if (Input.GetButton ("Jump")) 
		{
			if (fuel > 0f) 
			{
				CmdOnBurstStart ();
				//engineTail.startSpeed = 20f;
				//engineTail.startColor = Color.white;
				fuel -= fuelBurnSpeed * Time.deltaTime;
				if(transform.position.y < 40) _thrusterForce = Vector3.up * thrusterForce;
				speed = 40;
				SetJointSettings (0f);
			} 
			else 
			{
				CmdOnBurstEnd ();
				//engineTail.startSpeed = 9.16f;
				//engineTail.startColor = Color.yellow;
				SetJointSettings (jointSpring);
				speed = 20;
				fuel = 0f;
			}
		} 
		else 
		{ 
			CmdOnBurstEnd();
			//engineTail.startSpeed = 9.16f;
			//engineTail.startColor = Color.yellow;
			speed = 20;
			if (fuel < maxFuel) fuel += fuelRegenSpeed * Time.deltaTime;
			else fuel = maxFuel;
			SetJointSettings (jointSpring);
		}

		// Apply the thruster force 
		motor.ApplyThruster(_thrusterForce);
	}

	// call on the server when a player shoots
	[Command]
	void CmdOnBurstStart()
	{
		RpcDoEngineEffect ();
	}
	[Command]
	void CmdOnBurstEnd()
	{
		RpcStopEngineEffect ();
	}

	// is called on all clients we want to show shoot effect
	[ClientRpc]
	void RpcDoEngineEffect()
	{
		engineTail.startSpeed = 20f;
		engineTail.startColor = Color.white;
	}
	// is called on all clients we want to show shoot effect
	[ClientRpc]
	void RpcStopEngineEffect()
	{
		engineTail.startSpeed = 9.16f;
		engineTail.startColor = Color.yellow;
	}

	private void SetJointSettings (float _jointSpring)
	{
		joint.yDrive = new JointDrive { positionSpring = _jointSpring, 
										maximumForce = jointMaxForce
		};
	}
}
