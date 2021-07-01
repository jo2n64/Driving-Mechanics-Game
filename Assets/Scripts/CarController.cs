using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	[SerializeField] private CarSettings carSettings;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private bool isGrounded;
	[SerializeField] private Rigidbody sphereRB;
	[SerializeField] private float moveInput, turnInput;
	[SerializeField] private float distanceFromGround = 0.5f;
	[SerializeField] private Transform groundRay;

	private void Start()
	{
		sphereRB.mass = carSettings.Weight;
	}
	//TODO disable controls while in air


	// Update is called once per frame
	private void Update()
	{
		moveInput = Input.GetAxisRaw("Vertical");
		turnInput = Input.GetAxisRaw("Horizontal");

		moveInput *= moveInput > 0 ? carSettings.MaxSpeed : carSettings.MaxReverseSpeed;

		if (isGrounded)
		{
			turnInput *= carSettings.TurnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical"); //TODO multiply with normalized acceleration from sphereRB?
			transform.Rotate(0, turnInput, 0, Space.World);
		}

		transform.position = sphereRB.transform.position;
	}

	private void FixedUpdate()
	{
		isGrounded = false;
		RaycastHit hit;
		if (Physics.Raycast(groundRay.position, -transform.up, out hit, groundLayer))
		{
			isGrounded = true;
		}

		if (isGrounded)
		{
			sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
		}
		else
		{
			sphereRB.AddForce(Vector3.up * -carSettings.Weight * 100);

		}
	}
}
