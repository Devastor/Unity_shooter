using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Camera cam = Camera.main;

		transform.LookAt (transform.position + 	cam.transform.rotation * Vector3.forward, 
												cam.transform.rotation * Vector3.up);

	}
}
