using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	[SerializeField] private float airDrag, groundDrag;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private bool isGrounded;
	[SerializeField] private Rigidbody sphereRB;
	[SerializeField] private float moveInput, turnInput;
	[SerializeField] private float fwdSpeed, reverseSpeed, turnSpeed;
	void Start()
	{

	}
	//TODO disable controls while in air
	//TODO kill myself


	// Update is called once per frame
	void Update()
	{
		moveInput = Input.GetAxisRaw("Vertical");
		turnInput = Input.GetAxisRaw("Horizontal");

		moveInput *= moveInput > 0 ? fwdSpeed : reverseSpeed;

		transform.position = sphereRB.transform.position;

		turnInput *= turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical"); //TODO multiply with normalized acceleration from sphereRB?
		transform.Rotate(0, turnInput, 0, Space.World);

		RaycastHit hit;
		isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, groundLayer);
		transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

		if (isGrounded)
		{
			sphereRB.drag = groundDrag;
		}
		else
		{
			sphereRB.drag = airDrag;
		}
	}

	private void FixedUpdate()
	{
		if (isGrounded)
		{
			sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
		}
		else
		{
			sphereRB.AddTorque(transform.up * Physics.gravity.y);

		}
	}
}
