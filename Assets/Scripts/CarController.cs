using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	[SerializeField] private CarSettings carSettings;
	[SerializeField] private Rigidbody sphereRB;
	[SerializeField] private float gravityForce = 10f;
	[SerializeField] private float groundRayLength = 0.5f;

	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundRay;
	[SerializeField] private Transform[] frontWheels;
	[SerializeField] private float maxWheelTurn;
	private bool isGrounded;
	private float gasInput, turnInput;
	private void Start()
	{
		sphereRB.mass = carSettings.Weight;
		sphereRB.transform.parent = null;
	}

	// Update is called once per frame
	private void Update()
	{
		gasInput = Input.GetAxisRaw("Vertical");
		gasInput *= gasInput > 0 ? carSettings.ForwardAcceleration * 1000f : carSettings.ReverseAcceleration * 1000f;

		turnInput = Input.GetAxis("Horizontal");

		//TODO steer by a unit speed somehow idk
		if (isGrounded)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * carSettings.TurnSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0f)), 1f);
		}
		if (frontWheels.Length > 0)
		{
			foreach (Transform t in frontWheels)
			{
				t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), t.localRotation.eulerAngles.z);
			}
		}
		transform.position = sphereRB.transform.position;
	}

	private void FixedUpdate()
	{
		isGrounded = false;
		RaycastHit hit;


		if (Physics.Raycast(groundRay.position, -transform.up, out hit, groundRayLength, groundLayer))
		{
			isGrounded = true;
			transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
		}

		if (isGrounded)
		{
			sphereRB.drag = carSettings.GroundDrag;
			if (gasInput != 0)
			{
				sphereRB.AddForce(transform.forward * gasInput);
			}
		}
		else
		{
			sphereRB.drag = carSettings.AirDrag;
			sphereRB.AddForce(Vector3.up * -gravityForce * 100); //TODO prolly just use physics' gravity
		}
	}
}
