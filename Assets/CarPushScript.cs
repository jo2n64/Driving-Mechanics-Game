using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPushScript : MonoBehaviour
{

	[SerializeField] private Rigidbody rb;
	[SerializeField] private float speed;


	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * speed * 100f, ForceMode.Acceleration);
	}
}
